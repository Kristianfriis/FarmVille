using FarmVille.Client.Models;

namespace FarmVille.Client.Services;

public class FarmEngine : IDisposable
{
    public List<FarmTile> FarmGrid { get; private set; } = new();
    public int XP { get; private set; }
    public int CurrentTasks { get; private set; }
    public FarmTool CurrentTool { get; set; } = FarmTool.Pointer;
    public FarmTile? SelectedTile { get; private set; }

    private readonly FarmService _farmService;
    private readonly BarnService _barnService;
    private readonly MarketService _marketService;
    private readonly GameState _gameState;
    private readonly SnackbarService _snackbarService;
    private readonly ToolbarService _toolbarService;
    private const int MaxConcurrentTasks = 3;
    private PeriodicTimer? _timer;
    private CancellationTokenSource _cts = new();

    public event Action? OnStateChanged;
    public event Action? OnToolSelected;
    private int _farmId;
    private bool _isDirty = false;

    public FarmEngine(FarmService farmService, GameState gameState, BarnService barnService, MarketService marketService, SnackbarService snackbarService, ToolbarService toolbarService)
    {
        _barnService = barnService;
        _farmService = farmService;
        _gameState = gameState;
        _marketService = marketService;
        _snackbarService = snackbarService;
        _toolbarService = toolbarService;

        _toolbarService.OnToolChange += ChangeTool;
        _marketService.OnChooseCrop += HandleCropSelection;
        _barnService.OnCropSell += () => MarkDirty();
    }

    public async Task InitializeAsync(int farmId)
    {
        _farmId = farmId;
        if (!await LoadGame())
        {
            await CreateNewFarm();
        }

        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        _ = StartGameLoop();
    }

    private async Task StartGameLoop()
    {
      try
        {
            int secondsSinceLastSave = 0;
            while (await _timer!.WaitForNextTickAsync(_cts.Token))
            {
                UpdateTick();
                secondsSinceLastSave++;

                // Save every 5 seconds, but only if something actually changed
                if (_isDirty && secondsSinceLastSave >= 5)
                {
                    FireAndForgetSave();
                    _isDirty = false;
                    secondsSinceLastSave = 0;
                    Console.WriteLine("Auto-saved farm state.");
                }
            }
        }
        catch (OperationCanceledException) { }
    }

    public void SetCurrentTool(FarmTool tool)
    {
        CurrentTool = tool;
        switch (tool)
        {
            case FarmTool.Plow:
                _marketService.ToggleMarket();
                SelectedTile = null;
                break;
            case FarmTool.Rake:
                _marketService.ToggleMarket();
                SelectedTile = null;
                break;
        }
        OnToolSelected?.Invoke();
    }

    private async Task CreateNewFarm()
    {
        FarmGrid.Clear();
        for (int y = 0; y < 20; y++)
        {
            for (int x = 0; x < 20; x++)
            {
                FarmGrid.Add(new FarmTile { X = x, Y = y });
            }
        }
        // Initialize Barn
        var barn = FarmGrid.First(t => t.X == 2 && t.Y == 0);
        barn.Type = FarmTileType.Building;
        barn.BuildingType = "Barn";

        _gameState.PlayerInventory.AddCoins(100);

        await SaveGame();
    }

    public void UpdateTick()
    {
        foreach (var tile in FarmGrid)
        {
            if (tile.CurrentAction != TileAction.None)
            {
                if (tile.UpdateActionProgress())
                {
                    tile.CurrentAction = TileAction.None;
                    CurrentTasks = Math.Max(0, CurrentTasks - 1);
                }
            }
            else
            {
                tile.UpdateState();
            }
        }
        OnStateChanged?.Invoke();
    }

    public async Task HandleTileInteraction(FarmTile tile)
    {
        if (tile.Type == FarmTileType.Building)
        {
            if (tile.BuildingType == "Barn")
            {
                _barnService.ToggleBarn();
                return;
            }
            return;
        }

        if (tile.CurrentAction != TileAction.None)
            return;

        if (CurrentTool == FarmTool.Pointer && SelectedTile == tile)
        {
            SelectedTile = null;
            return;
        }

        switch (CurrentTool)
        {
            case FarmTool.Plow:
                // Only plow if it's grass or withered
                if ((tile.Status == TileStatus.Grass || tile.Status == TileStatus.Withered))
                {
                    if (_gameState.PlayerInventory.Coins < 15)
                    {
                        _snackbarService.Show("You don't have enough coins to plow this tile! (Cost: 15)");
                        return;
                    }

                    if (CurrentTasks >= MaxConcurrentTasks)
                    {
                        _snackbarService.Show("You are already working on the maximum number of tasks!");

                        return;
                    }

                    tile.StartAction(TileAction.Plowing, 300, 15, 1);

                    CurrentTasks++;
                    _gameState.PlayerInventory.RemoveCoins(15);

                    MarkDirty();
                }
                break;

            case FarmTool.Rake:
                // Clear a tile back to grass (maybe for free?)
                tile.Status = TileStatus.Grass;
                tile.CurrentCrop = null;

                MarkDirty();
                break;

            case FarmTool.Pointer:
                // Standard interactions: Harvest or Open Market
                SelectedTile = tile;
                if (tile.Status == TileStatus.Ready)
                {
                    XP += 5;
                    tile.Status = TileStatus.Plowed;

                    _gameState.PlayerInventory.AddCrop(tile.CurrentCrop!.ProduceType, tile.CurrentCrop!.HarvestAmount);

                    tile.CurrentCrop = null;

                    MarkDirty();
                }
                else if (tile.Status == TileStatus.Plowed)
                {
                    _marketService.ToggleMarket();
                }
                else if (tile.Status == TileStatus.Grass || tile.Status == TileStatus.Withered)
                {
                    _marketService.ToggleMarket();
                }
                break;
        }
        OnStateChanged?.Invoke();
    }


private void FireAndForgetSave()
{
    Task.Run(async () =>
    {
        try 
        {
            await SaveGame();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Save failed: {ex.Message}");
        }
    });
}
    private async Task SaveGame()
    {
        var farmData = new FarmSaveData
        {
            XP = XP,
            Grid = FarmGrid,
            CurrentTasks = CurrentTasks,
            Inventory = _gameState.PlayerInventory
        };

        await _farmService.SaveFarm(_farmId, farmData);
    }

    private async Task<bool> LoadGame()
    {
        var save = await _farmService.LoadFarm(_farmId);

        if (save is not null)
        {
            FarmGrid = save.Grid.OrderBy(t => t.Y).ThenBy(t => t.X).ToList();

            XP = save.XP;
            CurrentTasks = save.CurrentTasks;

            _gameState.PlayerInventory = save.Inventory;
            _gameState.NotifyGameLoaded();

            return true;
        }

        return false;
    }

    private void ChangeTool(FarmTool tool)
    {
        CurrentTool = tool;
    }

    private void HandleCropSelection(Crop crop)
    {
        if (SelectedTile is null)
            return;

        if (SelectedTile.Status != TileStatus.Plowed)
        {
            _snackbarService.Show("You can only plant crops on plowed tiles!");
            return;
        }

        SelectedTile.CurrentCrop = crop;
        SelectedTile.PlantedAt = DateTime.Now;
        SelectedTile.Status = TileStatus.Planted;
        _gameState.PlayerInventory.RemoveCoins(crop.Cost);

        MarkDirty();

        SelectedTile = null;
    }

    private void MarkDirty()
    {
        _isDirty = true;
        OnStateChanged?.Invoke(); // Update UI immediately
    }

    public void Dispose()
    {
        _cts.Cancel();
        _timer?.Dispose();
        _toolbarService.OnToolChange -= ChangeTool;
    }
}

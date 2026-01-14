using System;
using FarmVille.Client.Components;
using FarmVille.Client.Models;

namespace FarmVille.Client.Services;

public class FarmEngine 
{
    public List<FarmTile> FarmGrid { get; private set; } = new();
    public int XP { get; private set; }
    public int CurrentTasks { get; private set; }
    public FarmTool CurrentTool { get; set; } = FarmTool.Pointer;
    public FarmTile? SelectedTile { get; private set; }
    
    private readonly FarmService _farmService;
    private readonly GameState _gameState;
    private const int MaxConcurrentTasks = 3;

    public event Action? OnStateChanged;

    public FarmEngine(FarmService farmService, GameState gameState)
    {
        _farmService = farmService;
        _gameState = gameState;
    }

  public async Task InitializeAsync(int farmId)
    {
        var save = await _farmService.LoadFarm(farmId);
        if (save != null)
        {
            FarmGrid = save.Grid.OrderBy(t => t.Y).ThenBy(t => t.X).ToList();
            XP = save.XP;
            CurrentTasks = save.CurrentTasks;
            _gameState.PlayerInventory = save.Inventory;
        }
        else
        {
            CreateNewFarm();
        }
    }

    private void CreateNewFarm()
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
    }

    public void UpdateTick()
    {
        bool changed = false;
        foreach (var tile in FarmGrid)
        {
            if (tile.CurrentAction != TileAction.None)
            {
                if (tile.UpdateActionProgress())
                {
                    tile.CurrentAction = TileAction.None;
                    CurrentTasks = Math.Max(0, CurrentTasks - 1);
                    changed = true;
                }
            }
            else
            {
                tile.UpdateState(); 
            }
        }
        if (changed) OnStateChanged?.Invoke();
    }

    public async Task HandleTileInteraction(FarmTile tile)
    {
        // Logic from your HandleTileClick goes here...
        // Example:
        if (CurrentTool == FarmTool.Plow && CanPlow(tile))
        {
            ExecutePlow(tile);
        }
        
        OnStateChanged?.Invoke();
    }
}

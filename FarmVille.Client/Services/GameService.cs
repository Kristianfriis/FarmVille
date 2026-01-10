// using System;
// using FarmVille.Client.Models;

// namespace FarmVille.Client.Services;

// public class GameService
// {
//     private int CurrentTasks = 0;
//     private const int MaxConcurrentTasks = 3;
//     private FarmTool CurrentTool = FarmTool.Pointer;
//     private FarmTile? SelectedTile;
//     private List<FarmTile> FarmGrid = new();

//     private readonly FarmService FarmService;
//     private readonly CropService CropService;
//     private readonly GameState GameState;
//     private int FarmId;

//     public event Action? StateHasChanged;

//     private GameService(FarmService farmService, CropService cropService, GameState gameState)
//     {
//         FarmService = farmService;
//         CropService = cropService;
//         GameState = gameState;
//     }

//     public async Task InitializeGame(int farmId)
//     {
//         FarmId = farmId;
//         if (!await LoadGame())
//         {
//             GameState.PlayerInventory.AddCoins(100);

//             for (int y = 0; y < 20; y++)
//             {
//                 for (int x = 0; x < 20; x++)
//                 {
//                     FarmGrid.Add(new FarmTile { X = x, Y = y });
//                 }
//             }
//             var barnTile = FarmGrid.FirstOrDefault(t => t.X == 2 && t.Y == 0);
//             if (barnTile != null)
//             {
//                 barnTile.Type = FarmTileType.Building;
//                 barnTile.BuildingType = "Barn";
//             }

//             await SaveGame();
//         }

//         StartHeartbeat();
//     }

//     private async void StartHeartbeat()
//     {
//         while (true)
//         {
//             foreach (var tile in FarmGrid)
//             {
//                 if (tile.CurrentAction != TileAction.None)
//                 {
//                     var done = tile.UpdateActionProgress();
//                     if (done)
//                     {
//                         tile.CurrentAction = TileAction.None;
//                         tile.ActionDuration = 0;
//                         CurrentTasks = Math.Max(0, CurrentTasks - 1);
//                         await SaveGame();
//                     }
//                 }
//                 else
//                 {
//                     tile.UpdateState(); // Handle growth if already planted
//                 }
//             }

//             StateHasChanged?.Invoke();
//             await Task.Delay(1000);
//         }
//     }

//     private async Task SaveGame()
//     {
//         var farmData = new FarmSaveData
//         {
//             XP = GameState.PlayerXP,
//             Grid = FarmGrid,
//             CurrentTasks = CurrentTasks,
//             Inventory = GameState.PlayerInventory
//         };

//         await FarmService.SaveFarm(FarmId, farmData);
//     }

//     private async Task<bool> LoadGame()
//     {
//         var save = await FarmService.LoadFarm(FarmId);

//         if (save is not null)
//         {
//             FarmGrid = save.Grid.OrderBy(t => t.Y).ThenBy(t => t.X).ToList();

//             GameState.PlayerXP = save.XP;
//             CurrentTasks = save.CurrentTasks;

//             GameState.PlayerInventory = save.Inventory;
//             GameState.NotifyGameLoaded();

//             return true;
//         }

//         return false;
//     }

//     public async Task<StateChange> HandleTileClick(FarmTile tile)
//     {
//         if (tile.Type == FarmTileType.Building)
//         {
//             if (tile.BuildingType == "Barn")
//             {
//                 return new StateChange { ShowBarn = true };
//             }
//             return new StateChange { DoNothing = true };
//         }

//         if (tile.CurrentAction != TileAction.None) return new StateChange { DoNothing = true };

//         if (CurrentTool == FarmTool.Pointer && SelectedTile == tile)
//         {
//             SelectedTile = null;
//             return new StateChange { DoNothing = true };
//         }

//         switch (CurrentTool)
//         {
//             case FarmTool.Plow:
//                 // Only plow if it's grass or withered
//                 if ((tile.Status == TileStatus.Grass || tile.Status == TileStatus.Withered) && GameState.PlayerInventory.Coins >= 15)
//                 {
//                     if (GameState.PlayerInventory.Coins < 15)
//                     {
//                         alertMessage = "You don't have enough coins to plow this tile! (Cost: 15)";
//                         showAlert = true;
//                         return;
//                     }

//                     if (CurrentTasks >= MaxConcurrentTasks)
//                     {
//                         showAlert = true;
//                         alertMessage = "You are already working on the maximum number of tasks!";

//                         return;
//                     }

//                     tile.StartAction(TileAction.Plowing, 300, 15, 1);

//                     CurrentTasks++;
//                     GameState.PlayerInventory.RemoveCoins(15);

//                     await SaveGame();
//                 }
//                 break;

//             case FarmTool.Rake:
//                 // Clear a tile back to grass (maybe for free?)
//                 tile.Status = TileStatus.Grass;
//                 tile.CurrentCrop = null;

//                 await SaveGame();
//                 break;

//             case FarmTool.Pointer:
//                 // Standard interactions: Harvest or Open Market
//                 SelectedTile = tile;
//                 if (tile.Status == TileStatus.Ready)
//                 {
//                     GameState.PlayerXP += 5;
//                     tile.Status = TileStatus.Plowed;

//                     GameState.PlayerInventory.AddCrop(tile.CurrentCrop!.ProduceType, tile.CurrentCrop!.HarvestAmount);

//                     tile.CurrentCrop = null;

//                     await SaveGame();
//                 }
//                 else if (tile.Status == TileStatus.Plowed)
//                 {
//                     ShowMarket = true;
//                 }
//                 else if (tile.Status == TileStatus.Grass || tile.Status == TileStatus.Withered)
//                 {
//                     ShowMarket = false;
//                 }
//                 break;
//         }
//     }
// }

// public class StateChange
// {
//     public bool ShowAlert { get; set; } = false;
//     public string AlertMessage { get; set;} = string.Empty;
//     public bool ShowMarket { get; set; } = false;
//     public bool InventoryChanged { get; set; } = false;
//     public bool TileChanged { get; set; } = false;
//     public bool DoNothing { get; set; } = false;
//     public bool ShowBarn { get; set; } = false;
// }
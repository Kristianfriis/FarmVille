using System;
using System.Text.Json;
using FarmVille.Client.Components;

namespace FarmVille.Client.Services;

public class GameState
{
    public Inventory PlayerInventory { get; set; } = new Inventory();
 
    public event Action? OnGameLoaded;

    public void NotifyGameLoaded() => OnGameLoaded?.Invoke();
}

public class Inventory
{
    public int Coins { get; private set; }
    public Dictionary<TileType, int> Crops { get; set; } = new();

    public void AddCrop(TileType cropName, int quantity)
    {
        if (Crops.ContainsKey(cropName))
        {
            Crops[cropName] += quantity;
        }
        else
        {
            Crops[cropName] = quantity;
        }

        NotifyCropStateChanged();
    }

    public bool RemoveCrop(TileType cropName, int quantity)
    {
        if (Crops.ContainsKey(cropName) && Crops[cropName] >= quantity)
        {
            Crops[cropName] -= quantity;

            NotifyCropStateChanged();

            return true;
        }

        NotifyCropStateChanged();

        return false;
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        NotifyCoinStateChanged();
    }

    public void RemoveCoins(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            NotifyCoinStateChanged();
        }
    }

    public event Action? OnCropChange;

    private void NotifyCropStateChanged() => OnCropChange?.Invoke();

    public event Action? OnCoinChange;

    private void NotifyCoinStateChanged() => OnCoinChange?.Invoke();
}

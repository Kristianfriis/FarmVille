using System;
using System.Text.Json;
using FarmVille.Client.Components;

namespace FarmVille.Client.Services;

public class GameState
{
    public Inventory PlayerInventory { get; set; } = new Inventory();
}

public class Inventory
{
    public int Coins { get; set; }
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

        Console.WriteLine(JsonSerializer.Serialize(this));
    }

    public bool RemoveCrop(TileType cropName, int quantity)
    {
        if (Crops.ContainsKey(cropName) && Crops[cropName] >= quantity)
        {
            Crops[cropName] -= quantity;

            NotifyCropStateChanged();
            Console.WriteLine(JsonSerializer.Serialize(this));

            return true;
        }

        NotifyCropStateChanged();
        Console.WriteLine(JsonSerializer.Serialize(this));

        return false;
    }

    public event Action? OnCropChange;

    private void NotifyCropStateChanged() => OnCropChange?.Invoke();
}

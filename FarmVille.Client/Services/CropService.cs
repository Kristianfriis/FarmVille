using System;
using FarmVille.Client.Components;
using FarmVille.Client.Models;

namespace FarmVille.Client.Services;

public class CropService
{
    private List<Crop> Crops = new() {
        new Crop { Name = "Strawberries", Cost = 10, SellValue = 35, GrowthTime = TimeSpan.FromSeconds(30),IconType = TileType.StrawberrySeed, SeedType = TileType.StrawberrySeed, Stage1Type = TileType.Strawberry1, ReadyType = TileType.Strawberry2 },
        new Crop { Name = "Corn", Cost = 25, SellValue = 80, GrowthTime = TimeSpan.FromMinutes(2), IconType = TileType.CornSeed, SeedType = TileType.CornSeed, Stage1Type = TileType.Corn1, ReadyType = TileType.Corn2  }
    };

    public List<Crop> GetAllCrops()
    {
        return Crops;
    }

}

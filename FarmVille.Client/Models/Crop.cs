using FarmVille.Client.Components;

namespace FarmVille.Client.Models;

public class Crop
    {
        public string Name { get; set; } = "";
        public int Cost { get; set; }
        public int SellValue { get; set; }
        public TimeSpan GrowthTime { get; set; }
        public int UnlockLevel { get; set; }
        public TileType IconType {get; set;}
        public TileType SeedType {get; set;}
        public TileType Stage1Type {get; set;}
        public TileType Stage2Type {get; set;}
        public TileType ReadyType {get; set;}
    }
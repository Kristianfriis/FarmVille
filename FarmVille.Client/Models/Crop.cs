using FarmVille.Client.Components;

namespace FarmVille.Client.Models;

public class Crop
    {
        public string Name { get; set; } = "";
        public int Cost { get; set; }
        public int SellValue { get; set; }
        public TimeSpan GrowthTime { get; set; }
        public int UnlockLevel { get; set; }
        public string Icon { get; set; } = "🌱";
        public TileType IconType {get; set;}
    }
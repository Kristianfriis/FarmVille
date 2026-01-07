namespace FarmVille.Client.Models;

public class FarmTile
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public TileStatus Status { get; set; } = TileStatus.Grass;
    public Crop? CurrentCrop { get; set; }
    public DateTime? PlantedAt { get; set; }

    public void UpdateState()
    {
        if (Status == TileStatus.Planted && PlantedAt.HasValue && CurrentCrop != null)
        {
            var elapsed = DateTime.Now - PlantedAt.Value;
            if (elapsed > CurrentCrop.GrowthTime.Add(TimeSpan.FromDays(1))) 
                Status = TileStatus.Withered;
            else if (elapsed >= CurrentCrop.GrowthTime) 
                Status = TileStatus.Ready;
        }
    }

    public string GetTimeRemaining()
    {
        if (Status != TileStatus.Planted || !PlantedAt.HasValue || CurrentCrop == null) return "";
        var remaining = PlantedAt.Value.Add(CurrentCrop.GrowthTime) - DateTime.Now;
        return remaining.TotalSeconds > 0 ? $"{(int)remaining.TotalMinutes:D2}:{remaining.Seconds:D2}" : "Ready!";
    }
}
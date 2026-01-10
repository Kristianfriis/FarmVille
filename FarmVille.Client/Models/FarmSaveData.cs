using FarmVille.Client.Services;

namespace FarmVille.Client.Models;

public class FarmSaveData
{
    public int XP { get; set; }
    public List<FarmTile> Grid { get; set; } = new();
    public int CurrentTasks { get; set; } = 0;
    public Inventory Inventory { get; set; } = new();
}
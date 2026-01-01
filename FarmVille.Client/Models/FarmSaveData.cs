namespace FarmVille.Client.Models;

public class FarmSaveData
{
    public int Coins { get; set; }
    public int XP { get; set; }
    public List<FarmTile> Grid { get; set; } = new();
}
namespace FarmVille.Client.Components;

public enum TileType
{
    Grass,
    TilledSoil,
    Water,
    Fence,
    TreeApple,
    TreeOrange,
    Chicken,
    CornSeed,
    Corn1,
    Corn2,
    CornRipe,
    Corn,
    StrawberrySeed,
    Strawberry1,
    Strawberry2,
    StrawberryRipe,
    Strawberry
}

public static class SpriteMap
{
    // Returns (Row, Col)
    public static (int Row, int Col) GetCoords(TileType type) => type switch
    {
        TileType.Grass => (7, 1),
        TileType.TilledSoil => (7, 4), // Based on your example
        TileType.Chicken => (2, 0),
        TileType.TreeApple => (13, 0), // Note: Large sprites need their top-left coord
        TileType.StrawberrySeed => (4, 1),
        TileType.Strawberry1 => (4, 0),
        TileType.Strawberry2 => (3, 12),
        TileType.StrawberryRipe => (3, 11),
        TileType.Strawberry => (3, 10),
        TileType.CornSeed => (4, 6),
        TileType.Corn1 => (4, 5),
        TileType.Corn2 => (4, 4),
        TileType.CornRipe => (4, 3),
        TileType.Corn => (4, 1),
        _ => (0, 0)
    };
}
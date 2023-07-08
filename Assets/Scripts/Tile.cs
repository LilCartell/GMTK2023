public class Tile
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public TileType Type { get; private set; }
    public TileElement CurrentElementPresent { get; private set; }

    public Tile(TileDefinition definition)
    {
        ReloadDefinition(definition);
    }

    public void ReloadDefinition(TileDefinition definition)
    {
        X = definition.X;
        Y = definition.Y;
        Type = definition.Type;
        CurrentElementPresent = definition.StartingElement;
    }
}
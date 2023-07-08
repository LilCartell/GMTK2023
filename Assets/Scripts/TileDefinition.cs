public struct TileDefinition
{
    public int X;
    public int Y;
    public TileType Type;
    public TileElement StartingElement;
    public int Variation;

    public TileDefinition(int pX, int pY, TileType pType, TileElement pStartingElement, int pVariation)
    {
        X = pX;
        Y = pY;
        Type = pType;
        StartingElement = pStartingElement;
        Variation = pVariation;
    }
}

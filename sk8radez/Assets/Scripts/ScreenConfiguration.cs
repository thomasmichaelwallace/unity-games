public static class ScreenConfiguration
{
    public const float Ppu = 4;
    public const float XOffset = 1 / Ppu * 2;
    private const float TilePixelWidth = 8;
    private const float ScreenPixelWidth = 84;
    public const float Unit = TilePixelWidth / Ppu;
    public const float TrackVerticalDistance = 3;
    public const float TrackTopY = 2;
    public const int XCount = (int) (ScreenPixelWidth / TilePixelWidth) + 4;
    public const int EmptyXCount = 3;
    public const int YCount = 3;
}
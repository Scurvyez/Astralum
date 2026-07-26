namespace Astralum.Astronomy
{
  public static class CelestialSettings
  {
    public static bool DrawBlackHoleInfo;
    public static bool DrawConstellationLines;
    public static bool ShowLocalStarInfo;
    public static bool DrawPulsarInfo;
    public static bool DrawSkyCoordGrid;

    private static bool _lastDrawSkyCoordGrid;

    public static bool SkyCoordGridDirty => DrawSkyCoordGrid != _lastDrawSkyCoordGrid;

    public static void MarkSkyCoordGridClean()
    {
      _lastDrawSkyCoordGrid = DrawSkyCoordGrid;
    }
  }
}
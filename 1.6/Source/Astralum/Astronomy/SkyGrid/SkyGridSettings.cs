namespace Astralum.Astronomy.SkyGrid;

public static class SkyGridSettings
{
  public static bool DrawGrid;

  private static bool _lastDrawGrid;

  public static bool Dirty => DrawGrid != _lastDrawGrid;

  public static void MarkClean()
  {
    _lastDrawGrid = DrawGrid;
  }
}
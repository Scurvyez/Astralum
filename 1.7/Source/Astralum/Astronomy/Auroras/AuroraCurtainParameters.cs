namespace Astralum.Astronomy.Auroras
{
  public readonly struct AuroraCurtainParameters
  {
    public float RadiusX { get; }
    public float RadiusY { get; }
    public float TopRadiusScale { get; }
    public float BaseAltitude { get; }
    public float Height { get; }
    public float WarpStrength { get; }
    public float WarpFrequency { get; }
    public float WarpPhase { get; }

    public AuroraCurtainParameters(float radiusX, float radiusY, float topRadiusScale, float baseAltitude,
      float height, float warpStrength, float warpFrequency, float warpPhase)
    {
      RadiusX = radiusX;
      RadiusY = radiusY;
      TopRadiusScale = topRadiusScale;
      BaseAltitude = baseAltitude;
      Height = height;
      WarpStrength = warpStrength;
      WarpFrequency = warpFrequency;
      WarpPhase = warpPhase;
    }
  }
}
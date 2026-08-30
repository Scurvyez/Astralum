namespace Astralum.Astronomy.LocalStars
{
  public readonly struct SpectralClassWeight
  {
    public readonly SpectralClass SpectralClass;
    public readonly float Weight;

    public SpectralClassWeight(SpectralClass spectralClass, float weight)
    {
      SpectralClass = spectralClass;
      Weight = weight;
    }
  }
}
namespace Astralum.Astronomy.BackgroundStars
{
  public readonly struct BackgroundStarsGenerationData
  {
    public readonly int StarCount;
    public readonly float NormalizedStarCount;
    public readonly bool UseNonUniformGalacticPlaneBand;
    public readonly float GalacticPlaneBandMaskOffset;
    
    public BackgroundStarsGenerationData(int starCount, float normalizedStarCount, bool useNonUniformGalacticPlaneBand,
      float galacticPlaneBandMaskOffset)
    {
      StarCount = starCount;
      NormalizedStarCount = normalizedStarCount;
      UseNonUniformGalacticPlaneBand = useNonUniformGalacticPlaneBand;
      GalacticPlaneBandMaskOffset = galacticPlaneBandMaskOffset;
    }
  }
}
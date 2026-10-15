namespace Astralum.WorldComponents
{
  public readonly struct SkyCoord
  {
    public readonly float RightAscensionHours;
    public readonly float DeclinationDegrees;
    
    public SkyCoord(float rightAscensionHours, float declinationDegrees)
    {
      RightAscensionHours = rightAscensionHours;
      DeclinationDegrees = declinationDegrees;
    }
  }
}
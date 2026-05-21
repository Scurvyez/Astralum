namespace Astralum.World
{
  public readonly struct SkyCoord
  {
    public readonly float rightAscensionHours;
    public readonly float declinationDegrees;
    
    public SkyCoord(float rightAscensionHours, float declinationDegrees)
    {
      this.rightAscensionHours = rightAscensionHours;
      this.declinationDegrees = declinationDegrees;
    }
  }
}
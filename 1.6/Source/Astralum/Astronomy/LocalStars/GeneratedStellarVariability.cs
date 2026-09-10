namespace Astralum.Astronomy.LocalStars
{
  public struct GeneratedStellarVariability
  {
    public readonly StellarVariabilityType Type;
    public readonly float Amount;
    
    public bool HasVariability => Type != StellarVariabilityType.None && Amount > 0f;
    public bool IsIntrinsic => Type == StellarVariabilityType.Intrinsic && Amount > 0f;
    public bool IsExtrinsic => Type == StellarVariabilityType.Extrinsic && Amount > 0f;
    
    public GeneratedStellarVariability(StellarVariabilityType type, float amount)
    {
      Type = type;
      Amount = amount;
    }
  }
}
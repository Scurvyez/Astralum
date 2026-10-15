using Verse;

namespace Astralum.Astronomy
{
  public abstract class SavedPlayerNameableCelestialObject : SavedCelestialObject, IPlayerNameableCelestialObject
  {
    public string generatedName;
    public string playerSetName;
    
    public string GeneratedName
    {
      get => generatedName;
      set => generatedName = value;
    }
    
    public string PlayerSetName
    {
      get => playerSetName;
      set => playerSetName = value;
    }
    
    public string DisplayName => playerSetName.NullOrEmpty()
        ? generatedName
        : playerSetName;
    
    public bool HasPlayerSetName => !playerSetName.NullOrEmpty();
    
    public override void ExposeData()
    {
      base.ExposeData();
      
      Scribe_Values.Look(ref generatedName, "generatedName");
      Scribe_Values.Look(ref playerSetName, "playerSetName");
    }
  }
}
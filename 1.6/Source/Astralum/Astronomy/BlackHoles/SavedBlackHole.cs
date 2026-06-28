using UnityEngine;
using Verse;

namespace Astralum.Astronomy.BlackHoles
{
  public class SavedBlackHole : IExposable, IPlayerNamedCelestialObject
  {
    public string generatedName;
    public string playerSetName;
    
    public int id;
    public Vector3 localSkyPos;
    public Vector3 dir;
    public float size;
    
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
    
    public void ExposeData()
    {
      Scribe_Values.Look(ref generatedName, "generatedName");
      Scribe_Values.Look(ref playerSetName, "playerSetName");
      
      Scribe_Values.Look(ref id, "id");
      Scribe_Values.Look(ref localSkyPos, "localSkyPos");
      Scribe_Values.Look(ref dir, "dir");
      Scribe_Values.Look(ref size, "size");
    }
  }
}
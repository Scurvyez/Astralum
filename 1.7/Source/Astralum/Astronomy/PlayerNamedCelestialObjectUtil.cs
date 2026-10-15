using Verse;

namespace Astralum.Astronomy
{
  public static class PlayerNamedCelestialObjectUtil
  {
    public static void TrySetPlayerName(IPlayerNameableCelestialObject obj, string name)
    {
      if (obj == null) 
        return;
      
      name = name?.Trim();
      obj.PlayerSetName = name.NullOrEmpty() ? null : name;
    }
    
    public static void ClearPlayerName(IPlayerNameableCelestialObject obj)
    {
      obj?.PlayerSetName = null;
    }
  }
}
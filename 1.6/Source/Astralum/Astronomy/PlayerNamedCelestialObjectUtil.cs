using Verse;

namespace Astralum.Astronomy
{
  public static class PlayerNamedCelestialObjectUtil
  {
    public static string DisplayNameFor(IPlayerNamedCelestialObject obj)
    {
      if (obj == null)
        return "Astra_NameGenerator_Unknown".Translate();
      
      return obj.PlayerSetName.NullOrEmpty()
        ? obj.GeneratedName
        : obj.PlayerSetName;
    }
    
    public static void TrySetPlayerName(IPlayerNamedCelestialObject obj, string name)
    {
      if (obj == null) 
        return;
      
      name = name?.Trim();
      obj.PlayerSetName = name.NullOrEmpty() ? null : name;
    }
    
    public static void ClearPlayerName(IPlayerNamedCelestialObject obj)
    {
      obj?.PlayerSetName = null;
    }
  }
}
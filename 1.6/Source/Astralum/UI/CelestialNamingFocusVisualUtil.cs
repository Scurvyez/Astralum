using Astralum.Astronomy;
using Astralum.Astronomy.Nebulae;
using Astralum.Materials;

namespace Astralum.UI
{
  public static class CelestialNamingFocusVisualUtil
  {
    private static SavedNebula _focusedNebula;

    public static void Focus(IPlayerNameableCelestialObject celestialObject)
    {
      Clear();
      
      if (celestialObject is not SavedNebula nebula)
        return;
      
      _focusedNebula = nebula;
      NebulaeMatsUtil.SetFocused(nebula, true);
    }
    
    public static void Clear()
    {
      if (_focusedNebula == null)
        return;
      
      NebulaeMatsUtil.SetFocused(_focusedNebula, false);
      _focusedNebula = null;
    }
  }
}
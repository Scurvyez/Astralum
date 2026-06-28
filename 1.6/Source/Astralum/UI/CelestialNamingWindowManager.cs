using Astralum.Astronomy;
using RimWorld.Planet;
using Verse;

namespace Astralum.UI
{
  public static class CelestialNamingWindowManager
  {
    private static Dialog_CelestialNaming _window;
    public static void Update(bool requirePlaying)
    {
      if (!ShouldShow(requirePlaying) || !CelestialNamingSettings.ShowNamingWindow)
      {
        Close();
        return;
      }
      
      if (_window != null && Find.WindowStack.IsOpen(_window)) 
        return;
      
      _window = new Dialog_CelestialNaming();
      Find.WindowStack.Add(_window);
    }
    
    private static void Close()
    {
      if (_window == null)
        return;
      
      _window.Close(false);
      _window = null;
    }
    
    private static bool ShouldShow(bool requirePlaying)
    {
      if (requirePlaying && Current.ProgramState != ProgramState.Playing)
        return false;
      
      if (Find.World == null)
        return false;
      
      if (Find.WorldCamera == null || !Find.WorldCamera.gameObject.activeInHierarchy)
        return false;
      
      if (!WorldRendererUtility.WorldSelected)
        return false;
      
      if (Find.UIRoot?.screenshotMode?.FiltersCurrentEvent == true)
        return false;
      
      return true;
    }
  }
}
using Astralum.World;
using RimWorld.Planet;
using Verse;

namespace Astralum.UI
{
    public static class AstralumWorldInfoWindowManager
    {
        private static AstralumWorldInfoWindow _window;
        
        public static void Update(bool requirePlaying)
        {
            if (!ShouldShow(requirePlaying))
            {
                Close();
                return;
            }
            
            if (_window == null)
            {
                _window = new AstralumWorldInfoWindow();
                Find.WindowStack.Add(_window);
            }
            
            _window.RefreshSize();
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
            
            if (WorldUtils.CurrentStar == null)
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
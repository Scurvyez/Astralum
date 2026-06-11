using Verse;

namespace Astralum.Settings
{
  public class AstraSettings : ModSettings
  {
    private static AstraSettings _instance;
        
    public AstraSettings()
    {
      _instance = this;
    }
    
    #region Background star settings
    
    public static bool RenderAdditionalBackgroundStars => _instance._renderAdditionalBackgroundsStars;
    public bool _renderAdditionalBackgroundsStars = true;
    
    #endregion
    
    #region Nebulae settings
    
    public static bool RenderNebulae => _instance._renderNebulae;
    public bool _renderNebulae = true;
    
    #endregion

    #region Constellation settings
    
    public static bool RenderConstellations => _instance._renderConstellations;
    public bool _renderConstellations = true;
    
    #endregion

    #region Blackhole settings
    
    public static bool RenderBlackholes => _instance._renderBlackholes;
    public bool _renderBlackholes = true;
    
    #endregion

    #region Pulsar settings
    
    public static bool RenderPulsars => _instance._renderPulsars;
    public bool _renderPulsars = true;
    
    #endregion

    #region Shooting star settings
    
    public static bool RenderShootingStars => _instance._renderShootingStars;
    public bool _renderShootingStars = true;
    
    #endregion
    
    #region Local star settings
    
    public static bool OverrideVanillaSun => _instance._overrideVanillaSun;
    public bool _overrideVanillaSun = true;
    
    #endregion
    
    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Values.Look(ref _renderAdditionalBackgroundsStars, "_renderAdditionalBackgroundsStars", true);
      Scribe_Values.Look(ref _renderNebulae, "_renderNebulae", true);
      Scribe_Values.Look(ref _renderBlackholes, "_renderBlackholes", true);
      Scribe_Values.Look(ref _renderConstellations, "_renderConstellations", true);
      Scribe_Values.Look(ref _renderPulsars, "_renderPulsars", true);
      Scribe_Values.Look(ref _overrideVanillaSun, "_overrideVanillaSun", true);
    }
  }
}
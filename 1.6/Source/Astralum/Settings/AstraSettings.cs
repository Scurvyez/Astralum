using UnityEngine;
using Verse;

namespace Astralum.Settings
{
  public class AstraSettings : ModSettings
  {
    private float starInfoWindowXPos = -1f;
    private float starInfoWindowYPos = -1f;
    private float celestialCatalogueWindowX = 32f;
    private float celestialCatalogueWindowY = 120f;
    private float celestialCatalogueWindowWidth = 420f;
    private float celestialCatalogueWindowHeight = 560f;
    
    public bool HasCelestialCatalogueWindowRect;
    
    public Rect CelestialCatalogueWindowRect
    {
      get => new(
        celestialCatalogueWindowX,
        celestialCatalogueWindowY,
        Mathf.Max(celestialCatalogueWindowWidth, 420f),
        Mathf.Max(celestialCatalogueWindowHeight, 300f)
      );
      
      set
      {
        celestialCatalogueWindowX = value.x;
        celestialCatalogueWindowY = value.y;
        celestialCatalogueWindowWidth = value.width;
        celestialCatalogueWindowHeight = value.height;
      }
    }
    
    private static AstraSettings _instance;
        
    public AstraSettings()
    {
      _instance = this;
    }
    
    #region Background star settings
    
    public static bool RenderBackgroundStars => _instance._renderBackgroundsStars;
    public bool _renderBackgroundsStars = true;
    
    #endregion
    
    #region Nebulae settings
    
    public static bool RenderNebulae => _instance._renderNebulae;
    public bool _renderNebulae = true;
    
    #endregion
    
    #region Dustlane settings
    
    public static bool RenderDustlanes => _instance._renderDustlanes;
    public bool _renderDustlanes = true;
    
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
      Scribe_Values.Look(ref HasCelestialCatalogueWindowRect, "HasCelestialCatalogueWindowRect");
      
      Scribe_Values.Look(ref starInfoWindowXPos, "starInfoWindowXPos", -1f);
      Scribe_Values.Look(ref starInfoWindowYPos, "starInfoWindowYPos", -1f);
      Scribe_Values.Look(ref celestialCatalogueWindowX, "celestialCatalogueWindowX", 32f);
      Scribe_Values.Look(ref celestialCatalogueWindowY, "celestialCatalogueWindowY", 120f);
      Scribe_Values.Look(ref celestialCatalogueWindowWidth, "celestialCatalogueWindowWidth", 420f);
      Scribe_Values.Look(ref celestialCatalogueWindowHeight, "celestialCatalogueWindowHeight", 560f);
      
      Scribe_Values.Look(ref _renderBackgroundsStars, "_renderBackgroundsStars", true);
      Scribe_Values.Look(ref _renderNebulae, "_renderNebulae", true);
      Scribe_Values.Look(ref _renderDustlanes, "_renderDustlanes", true);
      Scribe_Values.Look(ref _renderBlackholes, "_renderBlackholes", true);
      Scribe_Values.Look(ref _renderConstellations, "_renderConstellations", true);
      Scribe_Values.Look(ref _renderPulsars, "_renderPulsars", true);
      Scribe_Values.Look(ref _overrideVanillaSun, "_overrideVanillaSun", true);
    }
  }
}
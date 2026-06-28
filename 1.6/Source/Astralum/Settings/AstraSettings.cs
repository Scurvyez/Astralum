using UnityEngine;
using Verse;

namespace Astralum.Settings
{
  public class AstraSettings : ModSettings
  {
    public float starInfoWindowXPos = -1f;
    public float starInfoWindowYPos = -1f;
    public float celestialNamingWindowX = 32f;
    public float celestialNamingWindowY = 120f;
    public float celestialNamingWindowWidth = 420f;
    public float celestialNamingWindowHeight = 560f;
    
    public bool HasCelestialNamingWindowRect;
    public bool HasStarInfoWindowPos => starInfoWindowXPos >= 0f && starInfoWindowYPos >= 0f;
    
    public Vector2 StarInfoWindowPos
    {
      get => new(starInfoWindowXPos, starInfoWindowYPos);
      set
      {
        starInfoWindowXPos = value.x;
        starInfoWindowYPos = value.y;
      }
    }
    
    public Rect CelestialNamingWindowRect
    {
      get => new(
        celestialNamingWindowX,
        celestialNamingWindowY,
        Mathf.Max(celestialNamingWindowWidth, 420f),
        Mathf.Max(celestialNamingWindowHeight, 300f)
      );
      
      set
      {
        celestialNamingWindowX = value.x;
        celestialNamingWindowY = value.y;
        celestialNamingWindowWidth = value.width;
        celestialNamingWindowHeight = value.height;
      }
    }
    
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
      Scribe_Values.Look(ref HasCelestialNamingWindowRect, "HasCelestialNamingWindowRect");
      
      Scribe_Values.Look(ref starInfoWindowXPos, "starInfoWindowXPos", -1f);
      Scribe_Values.Look(ref starInfoWindowYPos, "starInfoWindowYPos", -1f);
      Scribe_Values.Look(ref celestialNamingWindowX, "celestialNamingWindowX", 32f);
      Scribe_Values.Look(ref celestialNamingWindowY, "celestialNamingWindowY", 120f);
      Scribe_Values.Look(ref celestialNamingWindowWidth, "celestialNamingWindowWidth", 420f);
      Scribe_Values.Look(ref celestialNamingWindowHeight, "celestialNamingWindowHeight", 560f);
      
      Scribe_Values.Look(ref _renderAdditionalBackgroundsStars, "_renderAdditionalBackgroundsStars", true);
      Scribe_Values.Look(ref _renderNebulae, "_renderNebulae", true);
      Scribe_Values.Look(ref _renderBlackholes, "_renderBlackholes", true);
      Scribe_Values.Look(ref _renderConstellations, "_renderConstellations", true);
      Scribe_Values.Look(ref _renderPulsars, "_renderPulsars", true);
      Scribe_Values.Look(ref _overrideVanillaSun, "_overrideVanillaSun", true);
    }
  }
}
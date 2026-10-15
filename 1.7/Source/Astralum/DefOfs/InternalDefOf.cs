using RimWorld;
using Verse;

namespace Astralum.DefOfs
{
  [DefOf]
  public class InternalDefOf
  {
    static InternalDefOf()
    {
      DefOfHelper.EnsureInitializedInCtor(typeof(InternalDefOf));
    }
    
    #region Jobs

    public static JobDef UseTelescope;
    
    #endregion
    
    #region Game Conditions
    
    public static GameConditionDef Astra_LocalStarLight;
    public static GameConditionDef Astra_LocalStarEclipse;
    
    #endregion

    #region RulePacks
    
    public static RulePackDef Astra_NebulaName_Catalog;
    public static RulePackDef Astra_NebulaName_Coordinates;
    public static RulePackDef Astra_NebulaName_Descriptive;
    public static RulePackDef Astra_NebulaName_Discoverer;
    public static RulePackDef Astra_NebulaName_Formal;
    
    #endregion

    #region ShaderDefs
    
    public static ShaderDef Astra_GalacticDustLane01;
    public static ShaderDef Astra_Nebulae01;
    public static ShaderDef Astra_BackgroundStar01;
    public static ShaderDef Astra_Constellation01;
    public static ShaderDef Astra_ConstellationStar01;
    public static ShaderDef Astra_BlackHole01;
    public static ShaderDef Astra_Pulsar01;
    public static ShaderDef Astra_LocalStar01;
    public static ShaderDef Astra_ShootingStar01;
    public static ShaderDef Astra_SkyCoordinateGrid01;
    public static ShaderDef Astra_Aurora01;

    #endregion

    #region GlobalWorldDrawLayerDefs

    public static GlobalWorldDrawLayerDef Astra_GalacticDustLanes;
    public static GlobalWorldDrawLayerDef Astra_BackgroundStars;
    public static GlobalWorldDrawLayerDef Astra_Nebulae;
    public static GlobalWorldDrawLayerDef Astra_Constellations;
    public static GlobalWorldDrawLayerDef Astra_ConstellationStars;
    public static GlobalWorldDrawLayerDef Astra_BlackHoles;
    public static GlobalWorldDrawLayerDef Astra_Pulsars;
    public static GlobalWorldDrawLayerDef Astra_ShootingStars;
    public static GlobalWorldDrawLayerDef Astra_SkyCoordinateGrid;

    #endregion
  }
}
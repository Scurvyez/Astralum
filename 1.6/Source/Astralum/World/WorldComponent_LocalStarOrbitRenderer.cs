using Astralum.Astronomy.LocalStars;
using RimWorld.Planet;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_LocalStarOrbitRenderer : WorldComponent
  {
    public const int OrbitRenderUpdateInterval = 250;
    
    private int _nextUpdateTick;
    private GlobalDrawLayer_LocalStars _localStarLayer;

    public WorldComponent_LocalStarOrbitRenderer(RimWorld.Planet.World world) : base(world)
    {
      
    }
    
    public override void WorldComponentTick()
    {
      base.WorldComponentTick();
      
      WorldComponent_CelestialObjectDataCache data = LocalStarDataUtil.Data;
      
      if (data?.HasGeneratedLocalStars != true)
        return;
      
      int currentTick = Find.TickManager.TicksGame;
      
      if (currentTick < _nextUpdateTick)
        return;
      
      _nextUpdateTick = currentTick + OrbitRenderUpdateInterval;

      GetLocalStarLayer()?.SetDirty();
    }
    
    private GlobalDrawLayer_LocalStars GetLocalStarLayer()
    {
      if (_localStarLayer != null)
        return _localStarLayer;
      
      foreach (WorldDrawLayerBase layer in Find.WorldGrid.GlobalLayers)
      {
        if (layer is not GlobalDrawLayer_LocalStars localStarLayer)
          continue;
        
        _localStarLayer = localStarLayer;
        return _localStarLayer;
      }
      
      return null;
    }
  }
}
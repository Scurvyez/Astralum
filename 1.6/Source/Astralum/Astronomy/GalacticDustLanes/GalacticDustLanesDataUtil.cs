using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.GalacticDustLanes
{
  public static class GalacticDustLanesDataUtil
  {
    public static WorldComponent_CelestialObjectDataCache Data => Find.World.GetComponent<WorldComponent_CelestialObjectDataCache>();
    
    public static SavedGalacticDustLane Create(string id, Vector3 dir, float size, float rotation)
    {
      return CelestialObjectDataUtil.Create<SavedGalacticDustLane>(id, dir, size, rotation,
        dustlane =>
        {
          dustlane.alphaRange = new FloatRange(0.08f, 0.18f);
          dustlane.intensityRange = new FloatRange(0.35f, 0.75f);
          dustlane.noiseScaleRange = new FloatRange(2.5f, 6.5f);
          dustlane.detailScaleRange = new FloatRange(12f, 30f);
          dustlane.stretchXRange = new FloatRange(1.6f, 3.2f);
          dustlane.stretchYRange = new FloatRange(0.35f, 0.75f);
        });
    }
    
    public static SavedGalacticDustLane GetById(string id)
    {
      return Data?.DustLanes.GetById(id);
    }
  }
}
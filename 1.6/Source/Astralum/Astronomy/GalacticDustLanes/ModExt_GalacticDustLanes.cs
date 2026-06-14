using Verse;

namespace Astralum.Astronomy.GalacticDustLanes
{
  public class ModExt_GalacticDustLanes : DefModExtension
  {
    public IntRange dustLaneCount = new(4, 7);
    public FloatRange dustLaneSizeRange = new(18f, 36f);
    public FloatRange galacticPlaneBounds = new(-0.10f, 0.10f);
    
    public FloatRange alphaRange = new(0.08f, 0.18f);
    public FloatRange intensityRange = new(0.35f, 0.75f);
    
    public FloatRange noiseScaleRange = new(2.5f, 6.5f);
    public FloatRange detailScaleRange = new(12f, 30f);
    
    public FloatRange stretchXRange = new(1.6f, 3.2f);
    public FloatRange stretchYRange = new(0.35f, 0.75f);
  }
}
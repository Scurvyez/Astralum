using Verse;

namespace Astralum.Astronomy.GalacticDustLanes
{
  public class SavedGalacticDustLane : SavedCelestialObject
  {
    public FloatRange alphaRange;
    public FloatRange intensityRange;
    public FloatRange noiseScaleRange;
    public FloatRange detailScaleRange;
    public FloatRange stretchXRange;
    public FloatRange stretchYRange;
    
    public override void ExposeData()
    {
      base.ExposeData();
      
      Scribe_Values.Look(ref alphaRange, "alphaRange");
      Scribe_Values.Look(ref intensityRange, "intensityRange");
      Scribe_Values.Look(ref noiseScaleRange, "noiseScaleRange");
      Scribe_Values.Look(ref detailScaleRange, "detailScaleRange");
      Scribe_Values.Look(ref stretchXRange, "stretchXRange");
      Scribe_Values.Look(ref stretchYRange, "stretchYRange");
    }
  }
}
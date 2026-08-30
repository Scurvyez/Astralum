using Verse;

namespace Astralum.Astronomy.LocalStars
{
  public class SavedLocalStarSystem : IExposable
  {
    public string systemName;
    
    // orientation of the orbital plane as seen from the planet
    public float inclinationRadians;
    public float positionAngleRadians;
    
    // inner orbit, used for A/B in binaries and triples
    public int innerOrbitalPeriodTicks;
    public float innerInitialPhaseRadians;
    public float innerSeparation;

    // outer orbit, used only for hierarchical triples:
    // AB barycenter <-> star C
    public int outerOrbitalPeriodTicks;
    public float outerInitialPhaseRadians;
    public float outerSeparation;
    
    public void ExposeData()
    {
      Scribe_Values.Look(ref systemName, "systemName");
      Scribe_Values.Look(ref inclinationRadians, "inclinationRadians");
      Scribe_Values.Look(ref positionAngleRadians, "positionAngleRadians");
      Scribe_Values.Look(ref innerOrbitalPeriodTicks, "innerOrbitalPeriodTicks");
      Scribe_Values.Look(ref innerInitialPhaseRadians, "innerInitialPhaseRadians");
      Scribe_Values.Look(ref innerSeparation, "innerSeparation");
      Scribe_Values.Look(ref outerOrbitalPeriodTicks, "outerOrbitalPeriodTicks");
      Scribe_Values.Look(ref outerInitialPhaseRadians, "outerInitialPhaseRadians");
      Scribe_Values.Look(ref outerSeparation, "outerSeparation");
    }
  }
}
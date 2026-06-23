using Astralum.Debugging;
using Verse;

namespace Astralum
{
  [StaticConstructorOnStartup]
  public static class AstralumEntry
  {
    public const bool EnableDebugTweaks = false;
    
    static AstralumEntry()
    {
      AstraLog.Message("Astralum loaded.");
      
      // TODO: actual solar flares visible on the local star if one is active for the map?
      // TODO: look into planetary atmospheric composition (new world layer(?), maybe update existing world layer?
      // TODO: look into planet sky color change
      // TODO: look into pawn skin color change
      // TODO: update stargazing and/or telescope watching jobs to note which constellations they're watching
      // TODO: let players name stars as pawns find them via the telescope job
      // TODO: let players learn the names of certain stars as named by other factions?
      // TODO: add worker classes (easy hooks) for other modders for when a pawn discovers or observes a star
      //       like, Discovery_Worker and Observation_Worker, etc. 
    }
  }
}
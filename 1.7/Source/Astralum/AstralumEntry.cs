using System;
using Astralum.Debugging;
using Verse;

namespace Astralum
{
  [StaticConstructorOnStartup]
  public static class AstralumEntry
  {
    static AstralumEntry()
    {
      AstraLog.Message($"{DateTime.Now.Date.ToShortDateString()} " + "[Nothing to report...]");
      
      // initial release?
      // TODO: move nebula colors to XML so folks can easily add their own with patches
      // TODO: more varied/frequent stone/mineral chunk falling events?
      // TODO: new weathers?
      // TODO: actual solar flares visible on the local star if one is active for the map?
      // TODO: look into planetary atmospheric composition (new world layer(?), maybe update existing world layer?
      // TODO: add comets
      // TODO: add more constellation categories! :) like a loon, sparrow, etc.
      
      // [NFIR] = not for initial release
      // TODO: [NFIR] let players name stars as pawns find them via the telescope job
      // TODO: [NFIR] let players learn the names of certain stars as named by other factions?
      // TODO: [NFIR] non-circumbinary/trinary and circumbinary/trinary planetary systems??
    }
  }
}
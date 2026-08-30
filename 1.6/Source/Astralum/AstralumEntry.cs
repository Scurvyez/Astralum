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
      AstraLog.Message($"{DateTime.Now.Date.ToShortDateString()} "
                       + "[1.6 Alpha-Build | Nothing to report.]");
      
      // TODO: RENDER THE LOCAL STAR(S) IN FRONT OF EVERYTHING ELSE!!!
      // TODO: actual solar flares visible on the local star if one is active for the map?
      // TODO: look into planetary atmospheric composition (new world layer(?), maybe update existing world layer?
      // TODO: look into planet sky color change
      // TODO: look into pawn skin color change
      // TODO: let players name stars as pawns find them via the telescope job
      // TODO: let players learn the names of certain stars as named by other factions?
      // TODO: add comets
      // TODO: add more constellation categories! :) like a loon, sparrow, etc.
    }
  }
}
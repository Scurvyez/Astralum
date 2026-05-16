using Astralum.Debugging;
using Verse;

namespace Astralum
{
  [StaticConstructorOnStartup]
  public static class AstralumEntry
  {
    public const bool EnableDebugTweaks = true;
    public const bool EnableDebugGalacticPlane = false;

    static AstralumEntry()
    {
      AstraLog.Message("Astralum loaded.");

      // TODO: make mod settings
      // TODO: look into planetary atmospheric composition (new world layer(?), maybe update existing world layer?
      // TODO: look into planet sky color change
      // TODO: look into pawn skin color change
    }
  }
}
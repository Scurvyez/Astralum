using Astralum.Debugging;
using Verse;

namespace Astralum
{
    [StaticConstructorOnStartup]
    public static class AstralumEntry
    {
        static AstralumEntry()
        {
            AstraLog.Message("Astralum loaded.");
            
            // TODO: make mod settings
            // TODO: make data window draggable
            // TODO: look into planetary atmospheric composition (new world layer(?), maybe update existing world layer?
            // TODO: look into planet sky color change
            // TODO: look into pawn skin color change
            // TODO: CLEAN UP CODE, FOR THE LOVE OF EVERYTHING UNHOLY...
        }
    }
}
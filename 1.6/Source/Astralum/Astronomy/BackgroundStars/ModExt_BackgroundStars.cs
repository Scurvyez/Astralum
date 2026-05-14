using Verse;

namespace Astralum.Astronomy.BackgroundStars
{
    public class ModExt_BackgroundStars : DefModExtension
    {
        public int starCount = 50000;
        public FloatRange starSizeRange = new(0.085f, 0.85f);
    }
}
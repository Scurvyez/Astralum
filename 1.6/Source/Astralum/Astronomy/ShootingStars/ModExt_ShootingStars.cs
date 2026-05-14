using Verse;

namespace Astralum.Astronomy.ShootingStars
{
    public class ModExt_ShootingStars : DefModExtension
    {
        public float mTBIntervalSeconds = 20f;
        public FloatRange lifetime = new(0.45f, 2.875f);
        public FloatRange travelDistance = new(5.0f, 65.0f);
        public FloatRange length = new(0.5f, 2.1f);
        public FloatRange width = new(0.035f, 0.085f);
        public FloatRange galacticPlaneBounds = new(-0.16f, 0.16f);
    }
}
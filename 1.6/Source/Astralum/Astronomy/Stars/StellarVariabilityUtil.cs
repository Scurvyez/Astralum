using Verse;

namespace Astralum.Astronomy.Stars
{
    public class StellarVariabilityUtil
    {
        public enum StellarVariabilityType
        {
            None,
            Intrinsic,
            Extrinsic
        }
        
        public readonly struct GeneratedStellarVariability
        {
            public readonly StellarVariabilityType Type;
            public readonly float Amount;
            
            public bool HasVariability => Type != StellarVariabilityType.None && Amount > 0f;
            public bool IsIntrinsic => Type == StellarVariabilityType.Intrinsic && Amount > 0f;
            public bool IsExtrinsic => Type == StellarVariabilityType.Extrinsic && Amount > 0f;
            
            public GeneratedStellarVariability(StellarVariabilityType type, float amount)
            {
                Type = type;
                Amount = amount;
            }
        }
        
        private static float GetVariabilityAmount(SpectralClass spectralClass)
        {
            return spectralClass switch
            {
                SpectralClass.O => Rand.Range(0.05f, 0.15f),
                SpectralClass.B => Rand.Range(0.03f, 0.10f),
                SpectralClass.A => Rand.Range(0.02f, 0.08f),
                SpectralClass.F => Rand.Range(0.01f, 0.06f),
                SpectralClass.G => Rand.Range(0.01f, 0.05f),
                SpectralClass.K => Rand.Range(0.01f, 0.03f),
                SpectralClass.M => Rand.Range(0.01f, 0.02f),
                _ => Rand.Range(0.01f, 0.05f)
            };
        }
        
        public static float GenerateVariabilitySpeed()
        {
            return 0f;
        }
        
        public static GeneratedStellarVariability GenerateVariability(SpectralClass spectralClass)
        {
            // about 40% have no visible variability
            if (Rand.Range(0f, 1f) < 0.4f)
                return new GeneratedStellarVariability(StellarVariabilityType.None, 0f);

            StellarVariabilityType type = Rand.Range(0, 2) == 0
                ? StellarVariabilityType.Extrinsic
                : StellarVariabilityType.Intrinsic;

            float amount = GetVariabilityAmount(spectralClass);

            return new GeneratedStellarVariability(type, amount);
        }
        
        public static string FormatVariability(StellarVariabilityType type, float amount)
        {
            if (type == StellarVariabilityType.None || amount <= 0f)
                return "None";

            return $"{type} ({amount * 100f:0.#}%)";
        }
    }
}
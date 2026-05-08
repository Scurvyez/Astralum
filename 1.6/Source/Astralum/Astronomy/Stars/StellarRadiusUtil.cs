using UnityEngine;

namespace Astralum.Astronomy.Stars
{
    public static class StellarRadiusUtil
    {
        private static float GetRadius(SpectralClass spectralClass)
        {
            float randomNumber = Random.Range(0f, 250f);
            
            return spectralClass switch
            {
                SpectralClass.O when Mathf.Approximately(randomNumber, 250) => Random.Range(1250f, 1500f),
                SpectralClass.O when randomNumber is <= 249 and >= 240 => Random.Range(1000f, 1250f),
                SpectralClass.O when randomNumber is <= 239 and >= 230 => Random.Range(800f, 1000f),
                SpectralClass.O when randomNumber is <= 229 and >= 220 => Random.Range(500f, 800f),
                SpectralClass.O when randomNumber is <= 219 and >= 200 => Random.Range(100f, 500f),
                SpectralClass.O when randomNumber is <= 199 and >= 175 => Random.Range(30f, 100f),
                SpectralClass.O when randomNumber is <= 174 and >= 125 => Random.Range(10f, 30f),
                SpectralClass.O => Random.Range(6.6f, 10f),
                SpectralClass.B => Random.Range(1.8f, 6.6f),
                SpectralClass.A => Random.Range(1.4f, 1.8f),
                SpectralClass.F => Random.Range(1.15f, 1.4f),
                SpectralClass.G => Random.Range(0.96f, 1.15f),
                SpectralClass.K => Random.Range(0.7f, 0.96f),
                SpectralClass.M => Random.Range(0.08f, 0.7f),
                _ => Random.Range(0.96f, 1.15f)
            };
        }
        
        public static float GenerateRadius(SpectralClass spectralClass)
        {
            return GetRadius(spectralClass);
        }
        
        public static string FormatRadius(float solarRadii)
        {
            return solarRadii switch
            {
                >= 1000f => $"{solarRadii:N0} R☉",
                >= 10f => $"{solarRadii:N1} R☉",
                >= 1f => $"{solarRadii:0.00} R☉",
                _ => $"{solarRadii:0.000} R☉"
            };
        }
    }
}
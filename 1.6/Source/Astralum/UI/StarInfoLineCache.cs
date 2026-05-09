using System.Collections.Generic;
using Astralum.Astronomy.Stars;

namespace Astralum.UI
{
    public static class StarInfoLineCache
    {
        private static SavedStar _cachedStar;
        private static List<StarInfoLine> _cachedLines;

        public static List<StarInfoLine> GetLines(SavedStar star)
        {
            if (star == null)
                return [];
            
            if (_cachedStar == star && _cachedLines != null)
                return _cachedLines;
            
            _cachedStar = star;
            _cachedLines = BuildLines(star);
            
            return _cachedLines;
        }
        
        public static void Clear()
        {
            _cachedStar = null;
            _cachedLines = null;
        }
        
        public static void Rebuild(SavedStar star)
        {
            _cachedStar = star;
            _cachedLines = BuildLines(star);
        }
        
        private static List<StarInfoLine> BuildLines(SavedStar star)
        {
            return
            [
                new StarInfoLine(StellarNamingUtil.SafeName(star.starName, "Unknown Star")),
                new StarInfoLine($"System: {StellarNamingUtil.SafeName(
                    star.systemName, "Unknown System")}"),
                
                new StarInfoLine($"Class: {star.spectralClass}"),
                new StarInfoLine($"Age: {StellarAgeUtil.FormatAge(star.age)}"),
                new StarInfoLine($"Temperature: {StellarTemperatureUtil.FormatTemperature(
                    star.temperatureKelvin)}"),
                new StarInfoLine($"Rotation: {StellarRotationUtil.FormatRotation(star.rotation)}"),
                new StarInfoLine($"Magnetic Field: {StellarMagneticFieldUtil.FormatMagneticField(
                    star.magneticField)}"),
                new StarInfoLine($"Variability: {StellarVariabilityUtil.FormatVariability(
                    star.variabilityType, star.variabilityAmount)}"),
                new StarInfoLine($"Radius: {StellarRadiusUtil.FormatRadius(star.radius)}"),
                new StarInfoLine($"Luminosity: {StellarLuminosityUtil.FormatLuminosity(star.luminosity)}"),
                new StarInfoLine($"Mass: {StellarMassUtil.FormatMass(star.mass)}"),
                
                new StarInfoLine("Chromaticity:", star.chromaticity),
                new StarInfoLine("Corona Glow:", star.corona),
                new StarInfoLine($"Corona Intensity: {StellarCoronaUtil.FormatCoronaIntensity(
                    star.coronaIntensity)}")
            ];
        }
    }
}
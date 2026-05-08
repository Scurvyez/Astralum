using System.Collections.Generic;
using Astralum.Astronomy.Stars;
using Astralum.World;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.UI
{
    public class AstralumWorldInfoWindow
    {
        private const float WindowWidth = 260f;
        private const float Padding = 12f;
        private const float TopPadding = 10f;
        private const float BottomPadding = 10f;
        
        public static void DrawOnGUI(bool requirePlaying = true)
        {
            if (!ShouldDraw(requirePlaying))
                return;
            
            SavedStar star = WorldUtils.CurrentStar;
            
            if (star == null)
                return;
            
            Rect rect = GetWindowRect(star);
            Find.WindowStack.ImmediateWindow(982604171, rect, WindowLayer.GameUI, () => DrawWindowContents(star));
        }
        
        private static bool ShouldDraw(bool requirePlaying)
        {
            if (requirePlaying && Current.ProgramState != ProgramState.Playing)
                return false;
            
            if (Find.World == null)
                return false;
            
            if (Find.WorldCamera == null || !Find.WorldCamera.gameObject.activeInHierarchy)
                return false;
            
            if (!WorldRendererUtility.WorldSelected)
                return false;
            
            if (Find.UIRoot?.screenshotMode?.FiltersCurrentEvent == true)
                return false;
            
            return true;
        }
        
        private static List<StarInfoLine> GetStarInfoLines(SavedStar star)
        {
            return
            [
                new StarInfoLine(StellarNamingUtil.SafeName(star.starName, "Unknown Star")),
                new StarInfoLine($"System: {StellarNamingUtil.SafeName(star.systemName, "Unknown System")}"),
                
                new StarInfoLine($"Class: {star.spectralClass}"),
                new StarInfoLine($"Age: {StellarAgeUtil.FormatAge(star.age)}"),
                new StarInfoLine($"Temperature: {StellarTemperatureUtil.FormatTemperature(star.temperatureKelvin)}"),
                new StarInfoLine($"Rotation: {StellarRotationUtil.FormatRotation(star.rotation)}"),
                new StarInfoLine($"Magnetic Field: {StellarMagneticFieldUtil.FormatMagneticField(star.magneticField)}"),
                new StarInfoLine($"Variability: {StellarVariabilityUtil.FormatVariability(star.variabilityType, 
                    star.variabilityAmount)}"),
                new StarInfoLine($"Radius: {StellarRadiusUtil.FormatRadius(star.radius)}"),
                new StarInfoLine($"Luminosity: {StellarLuminosityUtil.FormatLuminosity(star.luminosity)}"),
                new StarInfoLine($"Mass: {StellarMassUtil.FormatMass(star.mass)}"),

                new StarInfoLine("Chromaticity:", star.chromaticity),
                new StarInfoLine("Corona Glow:", star.corona),
                new StarInfoLine($"Corona Intensity: {StellarCoronaUtil.FormatCoronaIntensity(star.coronaIntensity)}")
            ];
        }
        
        private static float GetLineHeight()
        {
            Text.Font = GameFont.Small;
            return Text.LineHeight;
        }
        
        private static Rect GetWindowRect(SavedStar star)
        {
            List<StarInfoLine> lines = GetStarInfoLines(star);
            float height = TopPadding + BottomPadding + lines.Count * GetLineHeight();
            
            return new Rect(Screen.width - WindowWidth - 16f, 120f, WindowWidth, height);
        }
        
        private static void DrawInfoLine(Rect rect, StarInfoLine line)
        {
            if (line.SwatchColor == null)
            {
                Widgets.Label(rect, line.Text);
                return;
            }
            
            const float swatchSize = 14f;
            const float swatchGap = 6f;
            
            Rect labelRect = new(rect.x, rect.y, rect.width - swatchSize - swatchGap, rect.height);
            Rect swatchRect = new(rect.xMax - swatchSize, rect.y + (rect.height - swatchSize) / 2f, 
                swatchSize, swatchSize);
            
            Widgets.Label(labelRect, line.Text);
            
            Color oldColor = GUI.color;
            GUI.color = line.SwatchColor.Value;
            Widgets.DrawBoxSolid(swatchRect, line.SwatchColor.Value);
            GUI.color = oldColor;
            
            Widgets.DrawBox(swatchRect);
        }
        
        private static void DrawWindowContents(SavedStar star)
        {
            List<StarInfoLine> lines = GetStarInfoLines(star);
            
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            
            float lineHeight = GetLineHeight();
            float y = TopPadding;
            
            foreach (StarInfoLine line in lines)
            {
                Rect lineRect = new(Padding, y, WindowWidth - Padding * 2f, lineHeight);
                DrawInfoLine(lineRect, line);
                
                y += lineHeight;
            }
            
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }
    }
}
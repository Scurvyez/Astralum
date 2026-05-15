using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.Astronomy.SkyGrid;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.World
{
    public static class WorldUtils
    {
        public const float NorthernSkyThreshold = 0.15f;
        public const float SouthernSkyThreshold = -0.15f;
        
        public static Vector3 GalacticPole => Quaternion.Euler(
            GenCelestial.CurSunPositionInWorldSpace()) * Vector3.up;
        
        public readonly struct SkyCoord
        {
            public readonly float rightAscensionHours;
            public readonly float declinationDegrees;
            
            public SkyCoord(float rightAscensionHours, float declinationDegrees)
            {
                this.rightAscensionHours = rightAscensionHours;
                this.declinationDegrees = declinationDegrees;
            }
        }
        
        public static SkyCoord DirectionToSkyCoord(Vector3 direction)
        {
            direction.Normalize();
            
            float raDegrees = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            
            if (raDegrees < 0f)
                raDegrees += 360f;
            
            float declinationDegrees = Mathf.Asin(direction.y) * Mathf.Rad2Deg;
            float raHours = raDegrees / 15f;
            
            return new SkyCoord(raHours, declinationDegrees);
        }
        
        public static string FormatRightAscension(float hours)
        {
            hours = Mathf.Repeat(hours, 24f);

            int h = Mathf.FloorToInt(hours);
            int m = Mathf.FloorToInt((hours - h) * 60f);

            return $"{h:00}h {m:00}m";
        }
        
        public static string FormatDeclination(float degrees)
        {
            string sign = degrees >= 0f ? "+" : "-";
            degrees = Mathf.Abs(degrees);
            
            int d = Mathf.FloorToInt(degrees);
            int m = Mathf.FloorToInt((degrees - d) * 60f);
            
            return $"{sign}{d:00}° {m:00}'";
        }
        
        public static string SkyHemisphere(Vector3 direction)
        {
            direction.Normalize();

            return direction.y switch
            {
                > NorthernSkyThreshold => "Northern Sky",
                < SouthernSkyThreshold => "Southern Sky",
                _ => "Equatorial Sky"
            };
        }
        
        public static SavedStar CurrentStar
        {
            get
            {
                RimWorld.Planet.World world = Find.World;
                WorldComponent_LocalStar comp = world?.GetComponent<WorldComponent_LocalStar>();
                
                return comp?.Star;
            }
        }
        
        public static bool ShouldDrawGUI()
        {
            if (Current.ProgramState != ProgramState.Playing)
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
    }
}
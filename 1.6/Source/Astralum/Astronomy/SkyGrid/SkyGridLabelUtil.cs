using System.Collections.Generic;
using Astralum.World;
using UnityEngine;

namespace Astralum.Astronomy.SkyGrid
{
    public static class SkyGridLabelUtil
    {
        private const float DistanceToLabels = 20f;
        
        private static readonly Vector2 DefaultOffset = new(24f, -24f);
        private static readonly Vector2 EquatorDeclinationOffset = new(24f, 24f);
        private static readonly Vector2 NorthSkyOffset = new(24f, 24f);
        
        public static List<SkyGridLabel> BuildLabels()
        {
            List<SkyGridLabel> labels = [];
            
            Vector3 pole = WorldUtils.GalacticPole.normalized;
            GetCircleBasis(pole, out Vector3 a, out Vector3 b);
            
            // RA hour labels around the equator
            for (int hour = 0; hour < 24; hour++)
            {
                float angle = hour / 24f * Mathf.PI * 2f;
                Vector3 dir = (Mathf.Cos(angle) * a + Mathf.Sin(angle) * b).normalized;
                
                labels.Add(new SkyGridLabel($"{hour:00}h", 
                    dir * DistanceToLabels, DefaultOffset, 1.5f));
            }
            
            // declination labels
            AddDeclinationLabels(labels, pole, a, b, 60f, DefaultOffset);
            AddDeclinationLabels(labels, pole, a, b, 30f, DefaultOffset);
            AddDeclinationLabels(labels, pole, a, b, 0f, EquatorDeclinationOffset);
            AddDeclinationLabels(labels, pole, a, b, -30f, DefaultOffset);
            AddDeclinationLabels(labels, pole, a, b, -60f, DefaultOffset);
            
            labels.Add(new SkyGridLabel("North Sky", pole * DistanceToLabels, NorthSkyOffset, 1.85f));
            labels.Add(new SkyGridLabel("South Sky", -pole * DistanceToLabels, DefaultOffset, 1.85f));
            
            return labels;
        }
        
        private static Vector3 LatitudeDirection(Vector3 pole, Vector3 equatorDir, float degrees)
        {
            float height = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float radius = Mathf.Cos(degrees * Mathf.Deg2Rad);
            
            return (pole.normalized * height + equatorDir.normalized * radius).normalized;
        }
        
        private static void AddDeclinationLabels(List<SkyGridLabel> labels, Vector3 pole, Vector3 a, Vector3 b,
            float degrees, Vector2 offset)
        {
            string text = FormatDeclinationLabel(degrees);
            
            for (int i = 0; i < 4; i++)
            {
                float angle = i / 4f * Mathf.PI * 2f;
                Vector3 equatorDir = DirectionOnCircle(a, b, angle);
                Vector3 dir = LatitudeDirection(pole, equatorDir, degrees);
                
                labels.Add(new SkyGridLabel(text, dir * DistanceToLabels, offset, 1.5f));
            }
        }
        
        private static string FormatDeclinationLabel(float degrees)
        {
            if (Mathf.Approximately(degrees, 0f))
                return "0°";
            
            return degrees > 0f
                ? $"+{Mathf.RoundToInt(degrees)}°"
                : $"{Mathf.RoundToInt(degrees)}°";
        }
        
        private static Vector3 DirectionOnCircle(Vector3 a, Vector3 b, float angle)
        {
            return (Mathf.Cos(angle) * a + Mathf.Sin(angle) * b).normalized;
        }
        
        private static void GetCircleBasis(Vector3 normal, out Vector3 a, out Vector3 b)
        {
            normal.Normalize();
            
            a = Vector3.Cross(normal, Vector3.up);
            
            if (a.sqrMagnitude < 0.001f)
                a = Vector3.Cross(normal, Vector3.right);
            
            a.Normalize();
            b = Vector3.Cross(normal, a).normalized;
        }
    }
}
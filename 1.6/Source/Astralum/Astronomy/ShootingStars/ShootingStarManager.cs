using System.Collections.Generic;
using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.ShootingStars
{
    public static class ShootingStarManager
    {
        private const float DistanceToShootingStars = 20f;
        
        private static readonly List<ShootingStar> Active = [];
        
        public static IReadOnlyList<ShootingStar> ActiveStars => Active;
        
        public static bool Dirty { get; private set; }

        static ShootingStarManager()
        {
            
        }
        
        public static void Tick()
        {
            bool changed = false;
            
            for (int i = Active.Count - 1; i >= 0; i--)
            {
                Active[i].age += Time.deltaTime;
                changed = true;
                
                if (Active[i].Expired)
                    Active.RemoveAt(i);
            }
            
            if (Rand.MTBEventOccurs(20f, 1f, Time.deltaTime))
            {
                SpawnRandom();
                changed = true;
            }
            
            if (changed)
                Dirty = true;
        }
        
        public static void Clear()
        {
            Active.Clear();
            Dirty = true;
        }
        
        public static void ClearDirty()
        {
            Dirty = false;
        }
        
        private static void SpawnRandom()
        {
            Vector3 dir = RandomGalacticPlaneDirection();
            Vector3 origin = dir * DistanceToShootingStars;
            
            Vector3 tangent = Vector3.Cross(dir, Rand.UnitVector3);
            
            if (tangent.sqrMagnitude < 0.001f)
                tangent = Vector3.Cross(dir, Vector3.up);
            
            tangent.Normalize();
            
            if (Rand.Value < 0.5f)
                tangent = -tangent;
            
            ShootingStar star = new()
            {
                origin = origin,
                travelDir = tangent,
                
                age = 0f,
                lifetime = Rand.Range(0.45f, 2.875f),
                travelDistance = Rand.Range(5.0f, 65.0f),
                length = Rand.Range(0.5f, 2.1f),
                width = Rand.Range(0.035f, 0.085f)
            };
            
            Active.Add(star);
        }
        
        private static Vector3 RandomGalacticPlaneDirection()
        {
            float angle = Rand.Range(0f, Mathf.PI * 2f);
            float localY = Rand.Range(-0.16f, 0.16f);
            
            float radius = Mathf.Sqrt(1f - localY * localY);
            
            Vector3 localDir = new(
                Mathf.Cos(angle) * radius,
                localY,
                Mathf.Sin(angle) * radius
            );
            
            Quaternion planeRotation = Quaternion.FromToRotation(
                Vector3.up,
                WorldUtils.GalacticPole.normalized
            );
            
            return (planeRotation * localDir).normalized;
        }
    }
}
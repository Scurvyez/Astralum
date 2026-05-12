using System.Collections;
using Astralum.Materials;
using Astralum.World;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Nebulae
{
    public class GlobalDrawLayer_Nebulae : WorldDrawLayerBase
    {
        private const float DistanceToNebulae = 20f;
        
        private const int NebulaCount = 13;
        
        private static readonly FloatRange NebulaSizeRange = new(6f, 18f);
        
        private bool calculatedForStaticRotation;
        private PlanetTile calculatedForStartingTile = PlanetTile.Invalid;
        
        protected override int RenderLayer => WorldCameraManager.WorldSkyboxLayer;
        
        private bool UseStaticRotation => Current.ProgramState == ProgramState.Entry;
        
        protected override Quaternion Rotation
        {
            get
            {
                if (UseStaticRotation)
                    return Quaternion.identity;
                
                return Quaternion.LookRotation(GenCelestial.CurSunPositionInWorldSpace());
            }
        }
        
        public override bool ShouldRegenerate
        {
            get
            {
                if (base.ShouldRegenerate)
                    return true;
                
                if (Find.GameInitData != null && Find.GameInitData.startingTile != calculatedForStartingTile)
                    return true;
                
                return UseStaticRotation != calculatedForStaticRotation;
            }
        }
        
        public override IEnumerable Regenerate()
        {
            foreach (object item in base.Regenerate())
                yield return item;
            
            Rand.PushState();
            Rand.Seed = Find.World.info.Seed ^ 0x4E384C41;
            
            for (int i = 0; i < NebulaCount; i++)
            {
                Material material = NebulaeMatsUtil.For(i);
                NebulaeMatsUtil.ApplyRandomNebulaProperties(material);
                
                LayerSubMesh subMesh = GetSubMesh(material);
                PrintNebula(subMesh);
            }
            
            calculatedForStartingTile = Find.GameInitData != null
                ? Find.GameInitData.startingTile
                : PlanetTile.Invalid;
            
            calculatedForStaticRotation = UseStaticRotation;
            
            Rand.PopState();
            
            FinalizeMesh(MeshParts.All);
        }
        
        private static void PrintNebula(LayerSubMesh subMesh)
        {
            Vector3 dir = RandomNebulaDirection();
            Vector3 pos = dir * DistanceToNebulae;
            
            float size = NebulaSizeRange.RandomInRange;
            
            WorldRendererUtility.PrintQuadTangentialToPlanet(pos, size, 0f, subMesh, 
                counterClockwise: true, Rand.Range(0f, 360f)
            );
        }
        
        private static Vector3 RandomNebulaDirection()
        {
            float angle = Rand.Range(0f, Mathf.PI * 2f);
            
            // smaller range = tighter to galactic plane
            // 0.0 = exactly on the plane
            float localY = Rand.Range(-0.18f, 0.18f);
            
            float radius = Mathf.Sqrt(1f - localY * localY);
            
            Vector3 localDir = new Vector3(
                Mathf.Cos(angle) * radius,
                localY,
                Mathf.Sin(angle) * radius).normalized;

            Quaternion planeRotation = Quaternion.FromToRotation(Vector3.up, WorldUtils.GalacticPole.normalized);
            
            return (planeRotation * localDir).normalized;
        }
    }
}
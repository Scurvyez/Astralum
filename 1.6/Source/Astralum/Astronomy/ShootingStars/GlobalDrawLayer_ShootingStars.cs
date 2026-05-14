using System.Collections;
using Astralum.Materials;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.ShootingStars
{
    public class GlobalDrawLayer_ShootingStars : WorldDrawLayerBase
    {
        private bool _calculatedForStaticRotation;
        private PlanetTile _calculatedForStartingTile = PlanetTile.Invalid;
        
        protected override int RenderLayer => WorldCameraManager.WorldSkyboxLayer;
        
        private bool UseStaticRotation => Current.ProgramState == ProgramState.Entry;
        
        protected override Quaternion Rotation =>
            UseStaticRotation
                ? Quaternion.identity
                : Quaternion.LookRotation(GenCelestial.CurSunPositionInWorldSpace());
        
        public override bool ShouldRegenerate
        {
            get
            {
                if (base.ShouldRegenerate)
                    return true;
                
                if (Find.GameInitData != null && Find.GameInitData.startingTile != _calculatedForStartingTile)
                    return true;
                
                if (UseStaticRotation != _calculatedForStaticRotation)
                    return true;
                
                return ShootingStarManager.Dirty;
            }
        }
        
        public override IEnumerable Regenerate()
        {
            foreach (object item in base.Regenerate())
                yield return item;
            
            LayerSubMesh subMesh = GetSubMesh(ShootingStarMatsUtil.ShootingStar);
            
            foreach (ShootingStar star in ShootingStarManager.ActiveStars)
                PrintShootingStar(star, subMesh);
            
            ShootingStarManager.ClearDirty();
            
            _calculatedForStartingTile = Find.GameInitData != null
                ? Find.GameInitData.startingTile
                : PlanetTile.Invalid;
            
            _calculatedForStaticRotation = UseStaticRotation;
            
            FinalizeMesh(MeshParts.All);
        }
        
        private static void PrintShootingStar(ShootingStar star, LayerSubMesh subMesh)
        {
            float progress = star.Progress;
            
            Vector3 head =
                star.origin +
                star.travelDir * star.travelDistance * progress;
            
            Vector3 tail =
                head -
                star.travelDir * star.length;
            
            Vector3 normal = head.normalized;
            Vector3 side = Vector3.Cross(normal, star.travelDir).normalized;
            
            float halfWidth = star.width * 0.5f;
            
            Vector3 v0 = tail - side * halfWidth;
            Vector3 v1 = tail + side * halfWidth;
            Vector3 v2 = head + side * halfWidth;
            Vector3 v3 = head - side * halfWidth;
            
            int baseIndex = subMesh.verts.Count;
            
            subMesh.verts.Add(v0);
            subMesh.verts.Add(v1);
            subMesh.verts.Add(v2);
            subMesh.verts.Add(v3);
            
            subMesh.uvs.Add(new Vector2(0f, 0f));
            subMesh.uvs.Add(new Vector2(0f, 1f));
            subMesh.uvs.Add(new Vector2(1f, 1f));
            subMesh.uvs.Add(new Vector2(1f, 0f));
            
            subMesh.tris.Add(baseIndex + 0);
            subMesh.tris.Add(baseIndex + 1);
            subMesh.tris.Add(baseIndex + 2);
            
            subMesh.tris.Add(baseIndex + 0);
            subMesh.tris.Add(baseIndex + 2);
            subMesh.tris.Add(baseIndex + 3);
        }
    }
}
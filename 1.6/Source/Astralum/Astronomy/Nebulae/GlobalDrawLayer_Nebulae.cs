using System.Collections;
using System.Collections.Generic;
using Astralum.Debugging;
using Astralum.DefOfs;
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
        
        private int _nebulaCount = 13;
        private FloatRange _nebulaSizeRange = new(6f, 18f);
        
        private bool _calculatedForStaticRotation;
        private PlanetTile _calculatedForStartingTile = PlanetTile.Invalid;
        private GlobalWorldDrawLayerDef _def;
        private ModExt_Nebulae _ext;
        
        private bool UseStaticRotation => Current.ProgramState == ProgramState.Entry;
        
        protected override int RenderLayer => WorldCameraManager.WorldSkyboxLayer;
        
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
                
                return UseStaticRotation != _calculatedForStaticRotation;
            }
        }
        
        public GlobalDrawLayer_Nebulae()
        {
            _def = InternalDefOf.Astra_Nebulae;
            _ext = _def?.GetModExtension<ModExt_Nebulae>();
            
            if (_ext == null)
            {
                AstraLog.Warning("Astra_Nebulae is missing ModExt_Nebulae. Using fallback values.");
                return;
            }
            
            _nebulaCount = Mathf.Max(0, _ext.nebulaCount);
            _nebulaSizeRange = _ext.nebulaSizeRange;
        }
        
        public override IEnumerable Regenerate()
        {
            foreach (object item in base.Regenerate())
                yield return item;
            
            WorldComponent_NebulaeData data = NebulaDataUtil.Data;
            
            if (!data.HasGeneratedNebulae)
                GenerateAndSaveNebulae(data);
            
            PrintSavedNebulae(data.nebulae);
            
            _calculatedForStartingTile = Find.GameInitData != null
                ? Find.GameInitData.startingTile
                : PlanetTile.Invalid;
            
            _calculatedForStaticRotation = UseStaticRotation;
            
            FinalizeMesh(MeshParts.All);
        }
        
        private void GenerateAndSaveNebulae(WorldComponent_NebulaeData data)
        {
            data.Clear();
            
            Rand.PushState();
            Rand.Seed = Find.World.info.Seed ^ 0x4E384C41;
            
            for (int i = 0; i < _nebulaCount; i++)
            {
                Vector3 localSkyPos = RandomNebulaDirection() * DistanceToNebulae;
                float size = _nebulaSizeRange.RandomInRange;
                float rotationDegrees = Rand.Range(0f, 360f);
                
                data.nebulae.Add(NebulaDataUtil.CreateRandom(i, localSkyPos, size, rotationDegrees));
            }
            
            Rand.PopState();
        }
        
        private void PrintSavedNebulae(List<SavedNebula> nebulae)
        {
            if (nebulae.NullOrEmpty())
                return;
            
            for (int i = 0; i < nebulae.Count; i++)
            {
                SavedNebula nebula = nebulae[i];
                
                Material material = NebulaeMatsUtil.For(nebula.nebulaId);
                NebulaDataUtil.ApplyToMaterial(material, nebula);
                LayerSubMesh subMesh = GetSubMesh(material);
                
                WorldRendererUtility.PrintQuadTangentialToPlanet(nebula.localSkyPos, nebula.size, 0f,
                    subMesh, counterClockwise: true, nebula.rotationDegrees);
            }
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
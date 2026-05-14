using System.Collections;
using Astralum.Materials;
using Astralum.World;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.SkyGrid
{
    public class GlobalDrawLayer_SkyCoordinateGrid : WorldDrawLayerBase
    {
        const float MajorWidth = 0.05f;
        const float MediumWidth = 0.025f;
        const float MinorWidth = 0.0125f;
        
        private bool _calculatedForDrawGrid;
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
                
                return SkyGridSettings.DrawGrid != _calculatedForDrawGrid;
            }
        }
        
        public override IEnumerable Regenerate()
        {
            foreach (object item in base.Regenerate())
                yield return item;
            
            if (!SkyGridSettings.DrawGrid)
            {
                _calculatedForStartingTile = Find.GameInitData != null
                    ? Find.GameInitData.startingTile
                    : PlanetTile.Invalid;
                
                _calculatedForStaticRotation = UseStaticRotation;
                _calculatedForDrawGrid = SkyGridSettings.DrawGrid;
                
                FinalizeMesh(MeshParts.All);
                yield break;
            }
            
            LayerSubMesh subMesh = GetSubMesh(SkyCoordinateGridMatsUtil.Line);
            
            Vector3 pole = WorldUtils.GalacticPole.normalized;
            
            // galactic plane
            SkyGridDrawUtil.PrintGreatCircle(subMesh, pole, MajorWidth);
            
            // hemisphere boundaries
            SkyGridDrawUtil.PrintLatitudeCircle(subMesh, pole, WorldUtils.NorthernSkyThreshold, MediumWidth);
            SkyGridDrawUtil.PrintLatitudeCircle(subMesh, pole, WorldUtils.SouthernSkyThreshold, MediumWidth);
            
            // declination lines
            SkyGridDrawUtil.PrintLatitudeCircle(subMesh, pole, SkyLatitudeHeight(30f), MinorWidth);
            SkyGridDrawUtil.PrintLatitudeCircle(subMesh, pole, SkyLatitudeHeight(-30f), MinorWidth);
            SkyGridDrawUtil.PrintLatitudeCircle(subMesh, pole, SkyLatitudeHeight(60f), MinorWidth);
            SkyGridDrawUtil.PrintLatitudeCircle(subMesh, pole, SkyLatitudeHeight(-60f), MinorWidth);
            
            // RA / longitude meridians
            SkyGridDrawUtil.PrintMeridian(subMesh, pole, Vector3.forward, MediumWidth);
            SkyGridDrawUtil.PrintMeridian(subMesh, pole, Vector3.right, MediumWidth);
            
            // tick marks along the equator
            SkyGridDrawUtil.PrintEquatorTicks(subMesh, pole, tickCount: 24, tickLength: 0.045f, width: MajorWidth);
            
            _calculatedForStartingTile = Find.GameInitData != null
                ? Find.GameInitData.startingTile
                : PlanetTile.Invalid;
            
            _calculatedForStaticRotation = UseStaticRotation;
            _calculatedForDrawGrid = SkyGridSettings.DrawGrid;
            
            FinalizeMesh(MeshParts.All);
        }
        
        private static float SkyLatitudeHeight(float degrees)
        {
            return Mathf.Sin(degrees * Mathf.Deg2Rad);
        }
    }
}
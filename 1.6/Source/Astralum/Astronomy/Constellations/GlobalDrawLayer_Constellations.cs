using System.Collections;
using System.Collections.Generic;
using Astralum.Astronomy.BackgroundStars;
using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.Debugging;
using Astralum.DefOfs;
using Astralum.Materials;
using Astralum.World;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
    public class GlobalDrawLayer_Constellations : WorldDrawLayerBase
    {
        private const float DistanceToConstellations = 20f;
        private const float MinCenterAngularDistance = 0.75f;
        
        private int _constellationCount = 13;
        private int _maxPlacementAttempts = 80;
        private float _baseStarSize = 0.25f;
        private float _brightStarSize = 0.85f;
        private float _constellationSizeMin = 3.0f;
        private float _constellationSizeMax = 3.5f;
        private float _minViewRotationAngle = 160f;
        private float _maxViewRotationAngle = 200f;
        
        private bool _calculatedForStaticRotation;
        private PlanetTile _calculatedForStartingTile = PlanetTile.Invalid;
        private GlobalWorldDrawLayerDef _def;
        private ModExt_Constellations _ext;
        
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

        public GlobalDrawLayer_Constellations()
        {
            _def = InternalDefOf.Astra_Constellations;
            _ext = _def?.GetModExtension<ModExt_Constellations>();
            
            if (_ext == null)
            {
                AstraLog.Warning("Astra_Constellations is missing ModExt_Constellations. Using fallback values.");
                return;
            }
            
            _constellationCount = Mathf.Max(0, _ext.constellationCount);
            _maxPlacementAttempts = Mathf.Max(0, _ext.maxPlacementAttempts);
            _baseStarSize = Mathf.Max(0, _ext.baseStarSize);
            _brightStarSize = Mathf.Max(0, _ext.brightStarSize);
            _constellationSizeMin = Mathf.Max(0, _ext.constellationSizeMin);
            _constellationSizeMax = Mathf.Max(0, _ext.constellationSizeMax);
            _minViewRotationAngle = Mathf.Max(0, _ext.minViewRotationAngle);
            _maxViewRotationAngle = Mathf.Max(0, _ext.maxViewRotationAngle);
        }
        
        public override IEnumerable Regenerate()
        {
            foreach (object item in base.Regenerate())
                yield return item;
            
            if (!ConstellationMaskUtil.HasMasks)
            {
                AstraLog.Warning("No constellation masks found.");
                yield break;
            }
            
            ConstellationInteractionRegistry.Clear();
            
            WorldComponent_ConstellationData data = ConstellationDataUtil.Data;
            
            if (!data.HasGeneratedConstellations)
                GenerateAndSaveConstellations(data);
            
            // TODO:
            // add a play setting so players can easily disable constellation lines(?)
            PrintSavedConstellations(data.constellations);
            
            _calculatedForStartingTile = Find.GameInitData != null
                ? Find.GameInitData.startingTile
                : PlanetTile.Invalid;
            
            _calculatedForStaticRotation = UseStaticRotation;
            
            FinalizeMesh(MeshParts.All);
        }
        
        private void GenerateAndSaveConstellations(WorldComponent_ConstellationData data)
        {
            data.Clear();

            Rand.PushState();
            Rand.Seed = Find.World.info.Seed ^ 0x5A17A11;

            List<Vector3> usedCenters = [];
            List<Texture2D> unusedMasks = ConstellationMaskUtil.CreateShuffledMaskPool();

            int count = Mathf.Min(_constellationCount, unusedMasks.Count);

            for (int i = 0; i < count; i++)
                TryCreateSavedConstellation(data.constellations, usedCenters, unusedMasks);

            Rand.PopState();
        }
        
        private bool TryCreateSavedConstellation(List<SavedConstellation> savedConstellations,
            List<Vector3> usedCenters, List<Texture2D> unusedMasks)
        {
            if (unusedMasks.NullOrEmpty())
                return false;
            
            for (int attempt = 0; attempt < _maxPlacementAttempts; attempt++)
            {
                Vector3 centerDir = RandomDensityWeightedSkyDirection();
                
                if (OverlapsExistingConstellation(centerDir, usedCenters))
                    continue;
                
                Texture2D mask = unusedMasks[unusedMasks.Count - 1];
                unusedMasks.RemoveAt(unusedMasks.Count - 1);
                
                float size = Rand.Range(_constellationSizeMin, _constellationSizeMax);
                float rotation = Rand.Range(_minViewRotationAngle, _maxViewRotationAngle);
                
                HashSet<string> usedNames = ConstellationDataUtil.Data.GetUsedNames();
                
                SavedConstellation saved = new()
                {
                    name = ConstellationNameGenerator.Generate(mask.name, usedNames),
                    maskName = mask.name,
                    centerDir = centerDir,
                    size = size,
                    rotationDegrees = rotation,
                    stars = []
                };
                
                int starCount = ConstellationMaskUtil.RandomStarPointCount();
                Vector2[] starPoints = ConstellationMaskUtil.GetStarPoints(mask, starCount);
                
                BuildSavedStars(saved, starPoints);
                
                savedConstellations.Add(saved);
                usedCenters.Add(centerDir);
                
                return true;
            }
            return false;
        }
        
        private void BuildSavedStars(SavedConstellation constellation, Vector2[] uvPoints)
        {
            if (uvPoints.NullOrEmpty())
                return;
            
            Vector3 center = constellation.centerDir.normalized * DistanceToConstellations;
            
            GetConstellationBasis(
                constellation.centerDir,
                constellation.rotationDegrees,
                out Vector3 tangentA,
                out Vector3 tangentB
            );
            
            HashSet<string> usedNames = ConstellationDataUtil.Data.GetUsedNames();
            
            for (int i = 0; i < uvPoints.Length; i++)
            {
                Vector2 uv = uvPoints[i];
                Vector2 local = new(uv.x * 2f - 1f, uv.y * 2f - 1f);
                
                Vector3 starPos =
                    center +
                    tangentA * local.x * constellation.size * 0.5f +
                    tangentB * local.y * constellation.size * 0.5f;
                
                float brightness = RandomMagnitudeBrightness();
                float visualSize = Mathf.Lerp(_baseStarSize, _brightStarSize, brightness);
                
                // very dim stars = 15% chance for unique name
                // medium dim stars = 50% chance for unique name
                // bright stars = 90% chance for unique name
                float uniqueNameChance = Mathf.Lerp(0.15f, 0.90f, brightness);
                
                string starName = StellarNamingUtil.GenerateUniqueName(
                    usedNames,
                    () => StellarNamingUtil.GenerateConstellationStarName(uniqueNameChance)
                );
                
                constellation.stars.Add(new SavedConstellationStar
                {
                    name = starName,
                    uv = uv,
                    localSkyPos = starPos,
                    spectralClass = StarClassUtil.RandomConstellationStarClass(),
                    visualSize = visualSize,
                    rotationDegrees = Rand.Range(0f, 360f)
                });
            }
        }
        
        private void PrintSavedConstellations(List<SavedConstellation> savedConstellations)
        {
            if (savedConstellations.NullOrEmpty())
                return;
            
            for (int i = 0; i < savedConstellations.Count; i++)
            {
                SavedConstellation saved = savedConstellations[i];
                Texture2D mask = ConstellationMaskUtil.GetMaskByName(saved.maskName);

                if (mask == null)
                    continue;
                
                Material material = ConstellationsMatsUtil.For(mask);
                
                if (material == null)
                    continue;
                
                LayerSubMesh lineSubMesh = GetSubMesh(material);
                
                PrintConstellationQuad(
                    saved.centerDir,
                    saved.size,
                    saved.rotationDegrees,
                    lineSubMesh
                );
                
                PrintSavedStars(saved);
            }
        }
        
        private void PrintSavedStars(SavedConstellation constellation)
        {
            if (constellation.stars.NullOrEmpty())
                return;
            
            GetConstellationSkyInfo(
                constellation,
                out string hemisphere,
                out string rightAscension,
                out string declination
            );
            
            for (int i = 0; i < constellation.stars.Count; i++)
            {
                SavedConstellationStar star = constellation.stars[i];
                
                Material material = BackgroundStarMatsUtil.For(star.spectralClass);
                LayerSubMesh subMesh = GetSubMesh(material);
                
                ConstellationInteractionRegistry.RegisterStar(
                    star.name,
                    star.localSkyPos,
                    star.spectralClass,
                    star.visualSize * 0.5f,
                    constellation.name,
                    hemisphere,
                    rightAscension,
                    declination
                );
                
                WorldRendererUtility.PrintQuadTangentialToPlanet(
                    star.localSkyPos,
                    star.visualSize,
                    0f,
                    subMesh,
                    counterClockwise: true,
                    star.rotationDegrees
                );
            }
        }
        
        private static void GetConstellationBasis(Vector3 centerDir, float rotationDegrees,
            out Vector3 tangentA, out Vector3 tangentB)
        {
            Vector3 normal = centerDir.normalized;
            
            tangentA = Vector3.Cross(normal, Vector3.up);
            
            if (tangentA.sqrMagnitude < 0.001f)
                tangentA = Vector3.Cross(normal, Vector3.right);
            
            tangentA.Normalize();
            tangentB = Vector3.Cross(normal, tangentA).normalized;
            
            Quaternion rotation = Quaternion.AngleAxis(rotationDegrees, normal);
            
            tangentA = rotation * tangentA;
            tangentB = rotation * tangentB;
        }
        
        private static float RandomMagnitudeBrightness()
        {
            float t = Rand.Value;
            return Mathf.Pow(1f - t, 2.8f);
        }
        
        private static Vector3 RandomDensityWeightedSkyDirection()
        {
            for (int i = 0; i < 32; i++)
            {
                Vector3 dir = Rand.UnitVector3.normalized;
                float density = StarDensity(dir);
                
                if (Rand.Value <= density)
                    return dir;
            }
            
            return Rand.UnitVector3.normalized;
        }
        
        private static float StarDensity(Vector3 dir)
        {
            float galacticBand = 1f - Mathf.Abs(dir.y);
            galacticBand = Mathf.Pow(Mathf.Clamp01(galacticBand), 1.8f);
            
            float noiseA = Mathf.PerlinNoise(dir.x * 2.5f + 43.17f, dir.z * 2.5f + 91.73f);
            float noiseB = Mathf.PerlinNoise(dir.y * 4.0f + 12.91f, dir.x * 4.0f + 66.34f);
            
            float clusteredNoise = Mathf.Lerp(noiseA, noiseB, 0.35f);
            clusteredNoise = Mathf.Pow(clusteredNoise, 1.4f);
            
            return Mathf.Clamp01(0.18f + galacticBand * 0.45f + clusteredNoise * 0.55f);
        }
        
        private static bool OverlapsExistingConstellation(Vector3 centerDir, List<Vector3> usedCenters)
        {
            for (int i = 0; i < usedCenters.Count; i++)
            {
                float angularDistance = Vector3.Angle(centerDir, usedCenters[i]) * Mathf.Deg2Rad;

                if (angularDistance < MinCenterAngularDistance)
                    return true;
            }
            return false;
        }
        
        private static void GetConstellationSkyInfo(SavedConstellation constellation, out string hemisphere,
            out string rightAscension, out string declination)
        {
            hemisphere = WorldUtils.SkyHemisphere(constellation.centerDir);
            
            float minRa = float.MaxValue;
            float maxRa = float.MinValue;
            float minDec = float.MaxValue;
            float maxDec = float.MinValue;
            
            for (int i = 0; i < constellation.stars.Count; i++)
            {
                Vector3 dir = constellation.stars[i].localSkyPos.normalized;
                WorldUtils.SkyCoord coord = WorldUtils.DirectionToSkyCoord(dir);
                
                minRa = Mathf.Min(minRa, coord.rightAscensionHours);
                maxRa = Mathf.Max(maxRa, coord.rightAscensionHours);
                minDec = Mathf.Min(minDec, coord.declinationDegrees);
                maxDec = Mathf.Max(maxDec, coord.declinationDegrees);
            }
            
            rightAscension =
                $"{WorldUtils.FormatRightAscension(minRa)} – {WorldUtils.FormatRightAscension(maxRa)}";
            
            declination =
                $"{WorldUtils.FormatDeclination(minDec)} – {WorldUtils.FormatDeclination(maxDec)}";
        }
        
        private static void PrintConstellationQuad(Vector3 centerDir, float size,
            float rotationDegrees, LayerSubMesh subMesh)
        {
            Vector3 center = centerDir.normalized * DistanceToConstellations;
            
            GetConstellationBasis(centerDir, rotationDegrees, out Vector3 tangentA, out Vector3 tangentB);
            
            float halfSize = size * 0.5f;
            
            Vector3 v0 = center - tangentA * halfSize - tangentB * halfSize;
            Vector3 v1 = center - tangentA * halfSize + tangentB * halfSize;
            Vector3 v2 = center + tangentA * halfSize + tangentB * halfSize;
            Vector3 v3 = center + tangentA * halfSize - tangentB * halfSize;
            
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
using System.Collections;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.Materials;
using Astralum.World;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.BackgroundStars
{
    public class GlobalDrawLayer_ColoredStars : WorldDrawLayerBase
    {
        private const float DistanceToStars = 20f;
        private const int StarCount = 50000;
        
        private static FloatRange StarSizeRange = new(0.175f, 0.85f);
        
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
            Rand.Seed = Find.World.info.Seed ^ 0x71C04ED;

            for (int i = 0; i < StarCount; i++)
            {
                PrintColoredStar();
            }
            
            calculatedForStartingTile = Find.GameInitData != null
                ? Find.GameInitData.startingTile
                : PlanetTile.Invalid;
            
            calculatedForStaticRotation = UseStaticRotation;
            
            Rand.PopState();

            if (AstralumEntry.EnableDebugGalacticPlane)
            {
                DrawDebugGalacticPlane();
            }
            
            FinalizeMesh(MeshParts.All);
        }
        
        private void DrawDebugGalacticPlane()
        {
            const int segments = 256;
            const float width = 0.02f;

            LayerSubMesh lineSubMesh = GetSubMesh(ConstellationsMatsUtil.ConstellationLine);

            Quaternion planeRotation =
                Quaternion.FromToRotation(Vector3.up, WorldUtils.GalacticPole.normalized);

            Vector3 previousPoint = Vector3.zero;

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                float angle = t * Mathf.PI * 2f;

                Vector3 localPoint = new Vector3(
                    Mathf.Cos(angle),
                    0f,
                    Mathf.Sin(angle)
                );

                Vector3 worldPoint =
                    (planeRotation * localPoint).normalized * DistanceToStars;

                if (i > 0)
                {
                    GlobalDrawLayer_Constellations.PrintSkyLine(previousPoint, worldPoint, width, lineSubMesh);
                }

                previousPoint = worldPoint;
            }
        }

        private void PrintColoredStar()
        {
            SpectralClass spectralClass = StarClassUtil.RandomBackgroundStarClass();
            
            Vector3 dir = RandomDensityWeightedDirection(spectralClass);
            Vector3 pos = dir * DistanceToStars;
            
            Material material = BackgroundStarMatsUtil.For(spectralClass);
            LayerSubMesh subMesh = GetSubMesh(material);
            
            float size = RandomStarSize(spectralClass);
            
            WorldRendererUtility.PrintQuadTangentialToPlanet(
                pos,
                size,
                0f,
                subMesh,
                counterClockwise: true,
                Rand.Range(0f, 360f)
            );
        }
        
        private static Vector3 RandomDensityWeightedDirection(SpectralClass spectralClass)
        {
            for (int i = 0; i < 32; i++)
            {
                Vector3 dir = Rand.UnitVector3.normalized;
                
                if (Rand.Value <= StarDensity(dir, spectralClass))
                    return dir;
            }
            
            return RandomGalacticPlaneDirection();
        }
        
        private static Vector3 RandomGalacticPlaneDirection()
        {
            float angle = Rand.Range(0f, Mathf.PI * 2f);
            float localY = Rand.Range(-0.12f, 0.12f);
            
            float radius = Mathf.Sqrt(1f - localY * localY);
            
            Vector3 localDir = new Vector3(
                Mathf.Cos(angle) * radius,
                localY,
                Mathf.Sin(angle) * radius
            ).normalized;
            
            Quaternion planeRotation = Quaternion.FromToRotation(Vector3.up, WorldUtils.GalacticPole.normalized);
            
            return (planeRotation * localDir).normalized;
        }
        
        private static float StarDensity(Vector3 dir, SpectralClass spectralClass)
        {
            float galacticLatitude = Mathf.Abs(Vector3.Dot(dir.normalized,
                WorldUtils.GalacticPole.normalized));
            float galacticPlane = 1f - galacticLatitude;
            
            // higher = sharper plane concentration
            float planeBias = spectralClass switch
            {
                SpectralClass.O => Mathf.Pow(galacticPlane, 18.0f),
                SpectralClass.B => Mathf.Pow(galacticPlane, 14.0f),
                SpectralClass.A => Mathf.Pow(galacticPlane, 10.0f),
                SpectralClass.F => Mathf.Pow(galacticPlane, 7.0f),
                SpectralClass.G => Mathf.Pow(galacticPlane, 4.5f),
                SpectralClass.K => Mathf.Pow(galacticPlane, 3.0f),
                SpectralClass.M => Mathf.Pow(galacticPlane, 2.0f),
                _ => galacticPlane
            };
            
            float clusterNoise = GalacticClusterNoise(dir, spectralClass);
            
            // multiply by plane bias so off-plane areas actually thin out
            float density = planeBias * Mathf.Lerp(0.35f, 1.0f, clusterNoise);
            
            // small background population so poles are not totally empty
            float backgroundFloor = spectralClass switch
            {
                SpectralClass.O => 0.0001f,
                SpectralClass.B => 0.0001f,
                SpectralClass.A => 0.0002f,
                SpectralClass.F => 0.0006f,
                SpectralClass.G => 0.0012f,
                SpectralClass.K => 0.0020f,
                SpectralClass.M => 0.0030f,
                _ => 0.002f
            };
            
            return Mathf.Clamp01(backgroundFloor + density);
        }
        
        private static float GalacticClusterNoise(Vector3 dir, SpectralClass spectralClass)
        {
            float scale = spectralClass switch
            {
                SpectralClass.O => 7.5f,
                SpectralClass.B => 6.5f,
                SpectralClass.A => 5.5f,
                SpectralClass.F => 4.5f,
                SpectralClass.G => 3.5f,
                SpectralClass.K => 3.0f,
                SpectralClass.M => 2.5f,
                _ => 3.5f
            };
            
            float noiseA = Mathf.PerlinNoise(
                dir.x * scale + 17.31f,
                dir.z * scale + 88.72f
            );
            
            float noiseB = Mathf.PerlinNoise(
                dir.y * scale + 41.93f,
                dir.x * scale + 12.44f
            );
            
            float noise = Mathf.Lerp(noiseA, noiseB, 0.35f);
            
            return Mathf.Pow(noise, 1.4f);
        }
        
        private static float RandomStarSize(SpectralClass spectralClass)
        {
            float apparentMagnitude = spectralClass switch
            {
                SpectralClass.O => Rand.Range(-1.0f, 5.0f),
                SpectralClass.B => Rand.Range(0.0f, 6.0f),
                SpectralClass.A => Rand.Range(1.0f, 7.0f),
                SpectralClass.F => Rand.Range(2.0f, 8.0f),
                SpectralClass.G => Rand.Range(3.0f, 8.5f),
                SpectralClass.K => Rand.Range(4.0f, 9.0f),
                SpectralClass.M => Rand.Range(5.0f, 10.0f),
                _ => Rand.Range(3.0f, 9.0f)
            };
            
            float brightness = Mathf.Pow(2.512f, -apparentMagnitude);
            
            float minBrightness = Mathf.Pow(2.512f, -10.0f);
            float maxBrightness = Mathf.Pow(2.512f, 1.0f);
            
            float visual = Mathf.InverseLerp(minBrightness, maxBrightness, brightness);
            
            // makes most stars tiny while preserving rare bright ones
            // increase power to "flatten" the curve and bring size variance closer to a single limit
            // decrease power to exaggerate size variance and make brighter stars even larger
            visual = Mathf.Pow(visual, 0.75f);
            
            return StarSizeRange.LerpThroughRange(visual);
        }
    }
}
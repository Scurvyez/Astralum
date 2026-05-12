using System.Collections;
using System.Collections.Generic;
using Astralum.Astronomy.BackgroundStars;
using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.Debugging;
using Astralum.Materials;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
    public class GlobalDrawLayer_Constellations : WorldDrawLayerBase
    {
        private readonly struct BuiltConstellation
        {
            public readonly Vector3[] points;
            public readonly int[] connections;
            
            public bool Valid => points is { Length: >= 2 } && connections is { Length: >= 2 };
            
            public BuiltConstellation(Vector3[] points, int[] connections)
            {
                this.points = points;
                this.connections = connections;
            }
        }
        
        private const float DistanceToConstellations = 20f;
        
        private const int ConstellationCount = 10;
        private const int MaxPlacementAttempts = 80;
        
        private const float MinCenterAngularDistance = 0.75f;
        
        private const float BaseStarSize = 0.35f;
        private const float BrightStarSize = 0.95f;
        private const float PrintedStarChance = 0.25f;
        private const float ConstellationLineWidth = 0.05f;
        
        private const float TemplateScaleMin = 0.075f;
        private const float TemplateScaleMax = 0.0925f;
        
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
            Rand.Seed = Find.World.info.Seed ^ 0x5A17A11;
            
            LayerSubMesh lineSubMesh = GetSubMesh(ConstellationsMatsUtil.ConstellationLine);
            
            List<Vector3> usedCenters = [];
            
            for (int i = 0; i < ConstellationCount; i++)
            {
                if (TryGenerateConstellation(lineSubMesh, usedCenters))
                    continue;
            }
            
            calculatedForStartingTile = Find.GameInitData != null
                ? Find.GameInitData.startingTile
                : PlanetTile.Invalid;
            
            calculatedForStaticRotation = UseStaticRotation;
            
            Rand.PopState();
            FinalizeMesh(MeshParts.All);
        }
        
        private bool TryGenerateConstellation(
            LayerSubMesh lineSubMesh,
            List<Vector3> usedCenters)
        {
            for (int attempt = 0; attempt < MaxPlacementAttempts; attempt++)
            {
                Vector3 centerDir = RandomDensityWeightedSkyDirection();
                
                if (OverlapsExistingConstellation(centerDir, usedCenters))
                    continue;
                
                ConstellationTemplate template = RandomTemplate();
                float scale = Rand.Range(TemplateScaleMin, TemplateScaleMax);
                
                BuiltConstellation constellation = BuildConstellation(centerDir, template, scale);

                if (!constellation.Valid)
                    continue;

                Vector3[] points = constellation.points;
                int[] connections = constellation.connections;
                
                usedCenters.Add(centerDir);
                
                PrintConstellationStars(points);
                PrintConstellationLines(points, connections, lineSubMesh);
                
                return true;
            }
            return false;
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
            // simple procedural “galactic band” plus noise pockets
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
        
        private static BuiltConstellation BuildConstellation(
            Vector3 centerDir,
            ConstellationTemplate template,
            float scale)
        {
            GetTangentBasis(centerDir, out Vector3 tangentA, out Vector3 tangentB);

            Vector2[] localPoints;
            int[] connections;

            if (ModsConfig.IdeologyActive)
            {
                Texture2D mask = ConstellationMaskUtil.RandomMask();

                if (mask == null)
                {
                    AstraLog.Message("Could not find any constellation masks.");
                    return default;
                }

                ConstellationMaskUtil.MaskConstellationData maskData =
                    ConstellationMaskUtil.GenerateShapeFromMask(mask);

                if (maskData.Valid)
                {
                    localPoints = maskData.points;
                    connections = maskData.connections;
                }
                else
                {
                    localPoints = GetTemplateLocalPoints(template);
                    connections = BuildTemplatePath(localPoints.Length, closed: false);
                }
            }
            else
            {
                localPoints = GetTemplateLocalPoints(template);
                connections = BuildTemplatePath(localPoints.Length, closed: false);
            }

            Vector3[] result = new Vector3[localPoints.Length];

            for (int i = 0; i < localPoints.Length; i++)
            {
                Vector2 local = localPoints[i];

                Vector3 dir = centerDir +
                              tangentA * local.x * scale +
                              tangentB * local.y * scale;

                result[i] = dir.normalized * DistanceToConstellations;
            }

            return new BuiltConstellation(result, connections);
        }
        
        private static int[] BuildTemplatePath(int pointCount, bool closed)
        {
            if (pointCount < 2)
                return [];
            
            List<int> connections = [];
            
            for (int i = 0; i < pointCount - 1; i++)
            {
                connections.Add(i);
                connections.Add(i + 1);
            }
            
            if (closed && pointCount > 2)
            {
                connections.Add(pointCount - 1);
                connections.Add(0);
            }
            
            return connections.ToArray();
        }
        
        private static void GetTangentBasis(Vector3 centerDir, out Vector3 tangentA, out Vector3 tangentB)
        {
            tangentA = Vector3.Cross(centerDir, Vector3.up);
            
            if (tangentA.sqrMagnitude < 0.001f)
                tangentA = Vector3.Cross(centerDir, Vector3.right);
            
            tangentA.Normalize();
            tangentB = Vector3.Cross(centerDir, tangentA).normalized;
            
            float rotation = Rand.Range(0f, 360f);
            Quaternion rot = Quaternion.AngleAxis(rotation, centerDir);
            
            tangentA = rot * tangentA;
            tangentB = rot * tangentB;
        }
        
        private void PrintConstellationStars(Vector3[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (Rand.Value > PrintedStarChance)
                    continue;
                
                SpectralClass spectralClass = StarClassUtil.RandomConstellationStarClass();
                
                Material material = BackgroundStarMatsUtil.For(spectralClass);
                LayerSubMesh subMesh = GetSubMesh(material);
                
                float brightness = RandomMagnitudeBrightness();
                float size = Mathf.Lerp(BaseStarSize, BrightStarSize, brightness);
                
                WorldRendererUtility.PrintQuadTangentialToPlanet(
                    points[i],
                    size,
                    0f,
                    subMesh,
                    counterClockwise: true,
                    Rand.Range(0f, 360f)
                );
            }
        }
        
        private static void PrintConstellationLines(
            Vector3[] points,
            int[] connections,
            LayerSubMesh lineSubMesh)
        {
            for (int i = 0; i < connections.Length - 1; i += 2)
            {
                int aIndex = connections[i];
                int bIndex = connections[i + 1];
                
                if (aIndex < 0 || bIndex < 0 || aIndex >= points.Length || bIndex >= points.Length)
                    continue;
                
                Vector3 a = points[aIndex];
                Vector3 b = points[bIndex];
                
                PrintSkyLine(a, b, ConstellationLineWidth, lineSubMesh);
            }
        }
        
        private static float RandomMagnitudeBrightness()
        {
            // bright stars are rare, dim stars common
            // 0 = dim, 1 = very bright
            float t = Rand.Value;
            return Mathf.Pow(1f - t, 2.8f);
        }
        
        private static ConstellationTemplate RandomTemplate()
        {
            return (ConstellationTemplate)Rand.RangeInclusive(
                0,
                (int)ConstellationTemplate.Curve
            );
        }
        
        private static Vector2[] GetTemplateLocalPoints(ConstellationTemplate template)
        {
            switch (template)
            {
                case ConstellationTemplate.Line:
                    return new[]
                    {
                        new Vector2(-1.0f, -0.15f),
                        new Vector2(-0.45f, 0.05f),
                        new Vector2(0.1f, -0.05f),
                        new Vector2(0.55f, 0.1f),
                        new Vector2(1.0f, -0.05f)
                    };

                case ConstellationTemplate.Triangle:
                    return new[]
                    {
                        new Vector2(0f, 1f),
                        new Vector2(-0.9f, -0.65f),
                        new Vector2(0.9f, -0.65f),
                        new Vector2(0.15f, -0.05f)
                    };

                case ConstellationTemplate.Cross:
                    return new[]
                    {
                        new Vector2(0f, 1f),
                        new Vector2(0f, 0.25f),
                        new Vector2(0f, -0.85f),
                        new Vector2(-0.75f, 0f),
                        new Vector2(0.75f, 0f)
                    };

                case ConstellationTemplate.Curve:
                    return new[]
                    {
                        new Vector2(-1f, -0.55f),
                        new Vector2(-0.55f, 0.1f),
                        new Vector2(0f, 0.35f),
                        new Vector2(0.55f, 0.1f),
                        new Vector2(1f, -0.45f)
                    };

                default:
                    return new[]
                    {
                        new Vector2(-1f, 0f),
                        new Vector2(-0.33f, 0f),
                        new Vector2(0.33f, 0f),
                        new Vector2(1f, 0f)
                    };
            }
        }
        
        public static void PrintSkyLine(Vector3 a, Vector3 b, float width, LayerSubMesh subMesh)
        {
            Vector3 center = ((a + b) * 0.5f).normalized * DistanceToConstellations;
            
            Vector3 normal = center.normalized;
            Vector3 lineDir = b - a;
            
            lineDir = Vector3.ProjectOnPlane(lineDir, normal);
            
            if (lineDir.sqrMagnitude < 0.001f)
                return;
            
            lineDir.Normalize();
            
            Vector3 sideDir = Vector3.Cross(normal, lineDir).normalized;
            
            float length = Vector3.Distance(a, b);
            
            Vector3 v0 = center - lineDir * length * 0.5f - sideDir * width;
            Vector3 v1 = center - lineDir * length * 0.5f + sideDir * width;
            Vector3 v2 = center + lineDir * length * 0.5f + sideDir * width;
            Vector3 v3 = center + lineDir * length * 0.5f - sideDir * width;
            
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
        
        private enum ConstellationTemplate
        {
            Line,
            Triangle,
            Cross,
            Curve
        }
    }
}
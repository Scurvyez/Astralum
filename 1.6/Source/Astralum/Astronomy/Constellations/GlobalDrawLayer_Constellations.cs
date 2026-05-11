using System.Collections;
using System.Collections.Generic;
using Astralum.Astronomy.BackgroundStars;
using Astralum.Astronomy.Stars;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
    public class GlobalDrawLayer_Constellations : WorldDrawLayerBase
    {
        private const float DistanceToConstellations = 20f;
        
        private const int ConstellationCount = 12;
        private const int MaxPlacementAttempts = 80;
        
        private const float MinCenterAngularDistance = 0.32f;
        private const float MaxLineAngularDistance = 0.26f;
        
        private const float BaseStarSize = 0.35f;
        private const float BrightStarSize = 0.95f;
        private const float ConstellationLineWidth = 0.01f;
        
        private const float TemplateScaleMin = 0.08f;
        private const float TemplateScaleMax = 0.18f;
        
        private PlanetTile calculatedForStartingTile = PlanetTile.Invalid;
        
        protected override int RenderLayer => WorldCameraManager.WorldSkyboxLayer;
        protected override Quaternion Rotation => Quaternion.identity;
        
        public override bool ShouldRegenerate
        {
            get
            {
                if (base.ShouldRegenerate)
                    return true;
                
                return Find.GameInitData != null &&
                       Find.GameInitData.startingTile != calculatedForStartingTile;
            }
        }
        
        public override IEnumerable Regenerate()
        {
            foreach (object item in base.Regenerate())
                yield return item;
            
            Rand.PushState();
            Rand.Seed = Find.World.info.Seed ^ 0x5A17A11;
            
            LayerSubMesh lineSubMesh = GetSubMesh(ConstellationsMaterialsUtil.ConstellationLine);
            
            List<Vector3> usedCenters = [];
            
            for (int i = 0; i < ConstellationCount; i++)
            {
                if (TryGenerateConstellation(lineSubMesh, usedCenters))
                    continue;
            }
            
            calculatedForStartingTile = Find.GameInitData != null
                ? Find.GameInitData.startingTile
                : PlanetTile.Invalid;
            
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
                
                Vector3[] points = BuildTemplatePoints(centerDir, template, scale);
                int[] connections = BuildConnectionPath(points);
                
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
        
        private static Vector3[] BuildTemplatePoints(
            Vector3 centerDir,
            ConstellationTemplate template,
            float scale)
        {
            GetTangentBasis(centerDir, out Vector3 tangentA, out Vector3 tangentB);
            
            Vector2[] localPoints = GetTemplateLocalPoints(template);
            Vector3[] result = new Vector3[localPoints.Length];
            
            for (int i = 0; i < localPoints.Length; i++)
            {
                Vector2 local = localPoints[i];
                
                // small imperfection so templates do not look copy/pasted
                local += Rand.InsideUnitCircle * 0.025f;
                
                Vector3 dir = centerDir +
                              tangentA * local.x * scale +
                              tangentB * local.y * scale;
                
                result[i] = dir.normalized * DistanceToConstellations;
            }
            return result;
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
        
        private static int[] BuildConnectionPath(Vector3[] points)
        {
            // lightweight stand-in for Delaunay:
            // connect each point to its nearest unvisited neighbor,
            // rejecting overly long angular spans
            List<int> path = new List<int>();
            
            bool[] used = new bool[points.Length];
            int current = 0;
            
            path.Add(current);
            used[current] = true;
            
            for (int step = 1; step < points.Length; step++)
            {
                int next = FindNearestUnusedPoint(current, points, used);
                
                if (next < 0)
                    break;
                
                float angularDistance = Vector3.Angle(
                    points[current].normalized,
                    points[next].normalized
                ) * Mathf.Deg2Rad;
                
                if (angularDistance > MaxLineAngularDistance)
                    break;
                
                path.Add(next);
                used[next] = true;
                current = next;
            }
            return path.ToArray();
        }
        
        private static int FindNearestUnusedPoint(int current, Vector3[] points, bool[] used)
        {
            int bestIndex = -1;
            float bestDistance = float.MaxValue;
            
            Vector3 currentDir = points[current].normalized;
            
            for (int i = 0; i < points.Length; i++)
            {
                if (used[i])
                    continue;
                
                float distance = Vector3.Angle(currentDir, points[i].normalized);
                
                if (distance >= bestDistance)
                    continue;
                
                bestDistance = distance;
                bestIndex = i;
            }
            return bestIndex;
        }
        
        private void PrintConstellationStars(Vector3[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                SpectralClass spectralClass = StarClassUtil.RandomConstellationStarClass();
                
                Material material = BackgroundStarMaterialsUtil.For(spectralClass);
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
            int[] path,
            LayerSubMesh lineSubMesh)
        {
            for (int i = 0; i < path.Length - 1; i++)
            {
                Vector3 a = points[path[i]];
                Vector3 b = points[path[i + 1]];
                
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
        
        private static void PrintSkyLine(Vector3 a, Vector3 b, float width, LayerSubMesh subMesh)
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
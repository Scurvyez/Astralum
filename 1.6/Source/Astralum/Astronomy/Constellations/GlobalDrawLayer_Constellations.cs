using System.Collections;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
    public class GlobalDrawLayer_Constellations : WorldDrawLayerBase
    {
        private const float DistanceToConstellations = 20f;

        private const int ConstellationCount = 12;
        private const int MinStarsPerConstellation = 4;
        private const int MaxStarsPerConstellation = 9;
        
        private const float ConstellationStarSize = 1.25f;
        private const float ConstellationLineWidth = 0.0075f;
        
        private bool calculatedForStaticRotation;
        private PlanetTile calculatedForStartingTile = PlanetTile.Invalid;
        
        protected override int RenderLayer => WorldCameraManager.WorldSkyboxLayer;
        protected override Quaternion Rotation => Quaternion.identity;
        
        private bool UseStaticRotation => Current.ProgramState == ProgramState.Entry;
        
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
            
            LayerSubMesh starSubMesh = GetSubMesh(ConstellationsMaterialsUtil.ConstellationStar);
            LayerSubMesh lineSubMesh = GetSubMesh(ConstellationsMaterialsUtil.ConstellationLine);
            
            for (int i = 0; i < ConstellationCount; i++)
            {
                GenerateConstellation(starSubMesh, lineSubMesh);
            }
            
            calculatedForStartingTile = Find.GameInitData != null
                ? Find.GameInitData.startingTile
                : PlanetTile.Invalid;
            
            calculatedForStaticRotation = UseStaticRotation;
            
            Rand.PopState();
            
            FinalizeMesh(MeshParts.All);
        }
        
        private static Vector3 RandomVisibleSkyDirection()
        {
            return new Vector3(
                Rand.Range(-1.8f, 1.8f),
                Rand.Range(-1.2f, 1.2f),
                Rand.Range(0.6f, 1.4f)
            ).normalized;
        }
        
        private static void GenerateConstellation(LayerSubMesh starSubMesh, LayerSubMesh lineSubMesh)
        {
            int starCount = Rand.RangeInclusive(MinStarsPerConstellation, MaxStarsPerConstellation);

            Vector3 centerDir = RandomVisibleSkyDirection();
            Vector3 tangentA = Vector3.Cross(centerDir, Vector3.up);
            
            if (tangentA.sqrMagnitude < 0.001f)
                tangentA = Vector3.Cross(centerDir, Vector3.right);
            
            tangentA.Normalize();
            
            Vector3 tangentB = Vector3.Cross(centerDir, tangentA).normalized;
            
            Vector3[] points = new Vector3[starCount];
            
            for (int i = 0; i < starCount; i++)
            {
                // per-constellation spread
                float x = Rand.Range(-0.16f, 0.16f);
                float y = Rand.Range(-0.16f, 0.16f);
                
                Vector3 dir = (centerDir + tangentA * x + tangentB * y).normalized;
                Vector3 pos = dir * DistanceToConstellations;
                
                points[i] = pos;
                
                WorldRendererUtility.PrintQuadTangentialToPlanet(
                    pos,
                    ConstellationStarSize,
                    0f,
                    starSubMesh,
                    counterClockwise: true,
                    Rand.Range(0f, 360f)
                );
            }
            
            for (int i = 0; i < points.Length - 1; i++)
            {
                PrintSkyLine(points[i], points[i + 1], ConstellationLineWidth, lineSubMesh);
            }
        }
        
        private static void PrintSkyLine(Vector3 a, Vector3 b, float width, LayerSubMesh subMesh)
        {
            Vector3 center = ((a + b) * 0.5f).normalized * DistanceToConstellations;
            
            Vector3 normal = center.normalized;
            Vector3 lineDir = (b - a);
            
            lineDir = Vector3.ProjectOnPlane(lineDir, normal).normalized;
            
            if (lineDir.sqrMagnitude < 0.001f)
                return;
            
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
    }
}
using Astralum.Materials;
using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Nebulae
{
    public static class NebulaDataUtil
    {
        public static WorldComponent_NebulaeData Data =>
            Find.World.GetComponent<WorldComponent_NebulaeData>();
        
        public static SavedNebula CreateRandom(int index, Vector3 localSkyPos, float size, float rotationDegrees)
        {
            Color[] palette = NebulaeColorUtil.RandomNebulaPalette();
            
            float colorStopB = Rand.Range(0.18f, 0.48f);
            float randStopC_A = colorStopB + 0.15f;
            float colorStopC = Rand.Range(colorStopB + 0.15f, Mathf.Max(randStopC_A + 0.2f, 1f));
            
            return new SavedNebula
            {
                name = $"Nebula {index + 1}",
                nebulaId = index,
                localSkyPos = localSkyPos,
                size = size,
                rotationDegrees = rotationDegrees,
                
                colorA = palette[0],
                colorB = palette[1],
                colorC = palette[2],
                colorD = palette[3],
                
                colorStopB = colorStopB,
                colorStopC = colorStopC,
                colorBandSharpness = Rand.Range(0.25f, 8f),
                
                seedOffset = new Vector4(
                    Rand.Range(-1000f, 1000f),
                    Rand.Range(-1000f, 1000f),
                    Rand.Range(-1000f, 1000f),
                    Rand.Range(-1000f, 1000f)
                ),
                
                seed = Rand.Value * 1000f,
                
                intensity = Rand.Range(1f, 2.9f),
                alpha = Rand.Range(0.25f, 1f),
                
                noiseScale = Rand.Range(3.25f, 7.5f),
                noiseStrength = Rand.Range(0.8f, 1.35f),
                
                cloudThreshold = Rand.Range(0.34f, 0.52f),
                edgeSoftness = Rand.Range(0.32f, 0.62f),
                
                warpScale = Rand.Range(1.5f, 4.5f),
                warpStrength = Rand.Range(0.18f, 0.65f),
                
                shapePower = Rand.Range(1.2f, 2.4f),
                
                coreOffset = new Vector4(
                    Rand.Range(-0.12f, 0.12f),
                    Rand.Range(-0.12f, 0.12f),
                    0f,
                    0f
                ),
                
                stretchX = 1f,
                stretchY = 1f,
                
                shaderRotation = Rand.Range(0f, Mathf.PI * 2f)
            };
        }
        
        public static void ApplyToMaterial(Material mat, SavedNebula nebula)
        {
            if (mat == null || nebula == null)
                return;
            
            mat.SetColor(InternalShaderPropertyIds.ColorA, nebula.colorA);
            mat.SetColor(InternalShaderPropertyIds.ColorB, nebula.colorB);
            mat.SetColor(InternalShaderPropertyIds.ColorC, nebula.colorC);
            mat.SetColor(InternalShaderPropertyIds.ColorD, nebula.colorD);
            
            mat.SetFloat(InternalShaderPropertyIds.ColorStopB, nebula.colorStopB);
            mat.SetFloat(InternalShaderPropertyIds.ColorStopC, nebula.colorStopC);
            mat.SetFloat(InternalShaderPropertyIds.ColorBandSharpness, nebula.colorBandSharpness);
            
            mat.SetVector(InternalShaderPropertyIds.SeedOffset, nebula.seedOffset);
            mat.SetFloat(InternalShaderPropertyIds.Seed, nebula.seed);
            
            mat.SetFloat(InternalShaderPropertyIds.Intensity, nebula.intensity);
            mat.SetFloat(InternalShaderPropertyIds.Alpha, nebula.alpha);
            
            mat.SetFloat(InternalShaderPropertyIds.NoiseScale, nebula.noiseScale);
            mat.SetFloat(InternalShaderPropertyIds.NoiseStrength, nebula.noiseStrength);
            
            mat.SetFloat(InternalShaderPropertyIds.CloudThreshold, nebula.cloudThreshold);
            mat.SetFloat(InternalShaderPropertyIds.EdgeSoftness, nebula.edgeSoftness);
            
            mat.SetFloat(InternalShaderPropertyIds.WarpScale, nebula.warpScale);
            mat.SetFloat(InternalShaderPropertyIds.WarpStrength, nebula.warpStrength);
            mat.SetFloat(InternalShaderPropertyIds.ShapePower, nebula.shapePower);
            mat.SetVector(InternalShaderPropertyIds.CoreOffset, nebula.coreOffset);
            
            mat.SetFloat(InternalShaderPropertyIds.StretchX, nebula.stretchX);
            mat.SetFloat(InternalShaderPropertyIds.StretchY, nebula.stretchY);
            mat.SetFloat(InternalShaderPropertyIds.Rotation, nebula.shaderRotation);
        }
    }
}
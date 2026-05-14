using Astralum.Astronomy.Nebulae;
using Astralum.DefOfs;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
    [StaticConstructorOnStartup]
    public static class NebulaeMatsUtil
    {
        private static readonly Material[] Nebulae = new Material[10];
        
        public static Material For(int index)
        {
            index = Mathf.Clamp(index, 0, Nebulae.Length - 1);
            
            if (Nebulae[index] == null)
                Nebulae[index] = CreateNebulaMaterial(index);
            
            return Nebulae[index];
        }
        
        private static Material CreateNebulaMaterial(int index)
        {
            Shader shader = InternalDefOf.Astra_Nebulae01.Shader;
            
            Material mat = new(shader)
            {
                name = $"Astralum_Astra_Nebulae01_{index}"
            };
            
            ApplyRandomNebulaProperties(mat);
            
            Object.DontDestroyOnLoad(mat);
            return mat;
        }
        
        public static void ApplyRandomNebulaProperties(Material mat)
        {
            Color[] palette = NebulaeColorUtil.RandomNebulaPalette();
            
            mat.SetColor(InternalShaderPropertyIds.ColorA, palette[0]);
            mat.SetColor(InternalShaderPropertyIds.ColorB, palette[1]);
            mat.SetColor(InternalShaderPropertyIds.ColorC, palette[2]);
            mat.SetColor(InternalShaderPropertyIds.ColorD, palette[3]);
            
            float stopB = Rand.Range(0.18f, 0.48f);
            float stopC = Rand.Range(stopB + 0.15f, 0.3f);
            
            mat.SetFloat(InternalShaderPropertyIds.ColorStopB, stopB);
            mat.SetFloat(InternalShaderPropertyIds.ColorStopC, stopC);
            mat.SetFloat(InternalShaderPropertyIds.ColorBandSharpness, Rand.Range(0.45f, 2.25f));
            
            mat.SetFloat(InternalShaderPropertyIds.Seed, Rand.Value * 1000f);
            mat.SetVector(InternalShaderPropertyIds.SeedOffset, new Vector4(
                Rand.Range(-1000f, 1000f),
                Rand.Range(-1000f, 1000f),
                Rand.Range(-1000f, 1000f),
                Rand.Range(-1000f, 1000f)
            ));
            
            mat.SetFloat(InternalShaderPropertyIds.Intensity, Rand.Range(0.75f, 2.75f));
            mat.SetFloat(InternalShaderPropertyIds.Alpha, Rand.Range(0.25f, 1f));
            
            mat.SetFloat(InternalShaderPropertyIds.NoiseScale, Rand.Range(3.25f, 7.5f));
            mat.SetFloat(InternalShaderPropertyIds.NoiseStrength, Rand.Range(0.8f, 1.35f));
            
            mat.SetFloat(InternalShaderPropertyIds.CloudThreshold, Rand.Range(0.34f, 0.52f));
            mat.SetFloat(InternalShaderPropertyIds.EdgeSoftness, Rand.Range(0.32f, 0.62f));
            
            mat.SetFloat(InternalShaderPropertyIds.WarpScale, Rand.Range(1.5f, 4.5f));
            mat.SetFloat(InternalShaderPropertyIds.WarpStrength, Rand.Range(0.18f, 0.65f));
            
            mat.SetFloat(InternalShaderPropertyIds.ShapePower, Rand.Range(1.2f, 2.4f));
            
            mat.SetVector(InternalShaderPropertyIds.CoreOffset, new Vector4(
                Rand.Range(-0.12f, 0.12f),
                Rand.Range(-0.12f, 0.12f),
                0f,
                0f
            ));
            
            mat.SetFloat(InternalShaderPropertyIds.StretchX, 1f);
            mat.SetFloat(InternalShaderPropertyIds.StretchY, 1f);
            
            mat.SetFloat(InternalShaderPropertyIds.Rotation, Rand.Range(0f, Mathf.PI * 2f));
        }
    }
}
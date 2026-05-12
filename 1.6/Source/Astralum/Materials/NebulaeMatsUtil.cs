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
            
            mat.SetColor("_ColorA", palette[0]);
            mat.SetColor("_ColorB", palette[1]);
            mat.SetColor("_ColorC", palette[2]);
            mat.SetColor("_ColorD", palette[3]);
            
            float stopB = Rand.Range(0.18f, 0.48f);
            float stopC = Rand.Range(stopB + 0.15f, 0.3f);
            
            mat.SetFloat("_ColorStopB", stopB);
            mat.SetFloat("_ColorStopC", stopC);
            mat.SetFloat("_ColorBandSharpness", Rand.Range(0.45f, 2.25f));
            
            mat.SetVector("_SeedOffset", new Vector4(
                Rand.Range(-1000f, 1000f),
                Rand.Range(-1000f, 1000f),
                Rand.Range(-1000f, 1000f),
                Rand.Range(-1000f, 1000f)
            ));
            
            mat.SetFloat("_Seed", Rand.Value * 1000f);
            
            mat.SetFloat("_Intensity", Rand.Range(0.75f, 2.75f));
            mat.SetFloat("_Alpha", Rand.Range(0.25f, 1f));
            
            mat.SetFloat("_NoiseScale", Rand.Range(3.25f, 7.5f));
            mat.SetFloat("_NoiseStrength", Rand.Range(0.8f, 1.35f));
            
            mat.SetFloat("_CloudThreshold", Rand.Range(0.34f, 0.52f));
            mat.SetFloat("_EdgeSoftness", Rand.Range(0.32f, 0.62f));
            
            mat.SetFloat("_WarpScale", Rand.Range(1.5f, 4.5f));
            mat.SetFloat("_WarpStrength", Rand.Range(0.18f, 0.65f));
            
            mat.SetFloat("_ShapePower", Rand.Range(1.2f, 2.4f));
            
            mat.SetVector("_CoreOffset", new Vector4(
                Rand.Range(-0.12f, 0.12f),
                Rand.Range(-0.12f, 0.12f),
                0f,
                0f
            ));
            
            mat.SetFloat("_StretchX", 1f);
            mat.SetFloat("_StretchY", 1f);
            
            mat.SetFloat("_Rotation", Rand.Range(0f, Mathf.PI * 2f));
        }
    }
}
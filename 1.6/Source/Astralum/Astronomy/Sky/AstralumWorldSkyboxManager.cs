using Astralum.Debugging;
using Astralum.DefOfs;
using Astralum.Materials;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Sky
{
    [StaticConstructorOnStartup]
    public static class AstralumWorldSkyboxManager
    {
        private static bool _applied;
        private static Material _skyboxMaterial;
        
        public static Material StarfieldMaterial => SkyboxMaterial;
        
        public static void Apply()
        {
            if (_applied)
                return;
            
            Camera camera = WorldCameraManager.WorldSkyboxCamera;
            
            if (camera == null)
                return;
            
            Material material = SkyboxMaterial;
            
            if (material == null)
                return;
            
            camera.clearFlags = CameraClearFlags.Skybox;
            
            Skybox skybox = camera.gameObject.GetComponent<Skybox>();
            
            if (skybox == null)
                skybox = camera.gameObject.AddComponent<Skybox>();
            
            skybox.material = material;
            
            _applied = true;
            
            AstraLog.Message("Applied Astralum world skybox.");
        }
        
        private static Material SkyboxMaterial
        {
            get
            {
                if (_skyboxMaterial != null)
                    return _skyboxMaterial;
                
                _skyboxMaterial = CreateSkyboxMaterial();
                return _skyboxMaterial;
            }
        }
        
        private static Material CreateSkyboxMaterial()
        {
            Shader shader = InternalDefOf.Astra_Starfield01.Shader;
            
            if (shader == null)
            {
                AstraLog.Warning("Could not find shader Astra_Starfield01.");
                return null;
            }
            
            Texture2D texture = ContentFinder<Texture2D>.Get("Astralum/Starfield/Starfield01");
            
            if (texture == null)
            {
                return null;
            }
            
            Material material = new(shader)
            {
                name = "Astralum_WorldSkybox"
            };
            
            material.SetTexture(InternalShaderPropertyIds.MainTex, texture);
            material.SetColor(InternalShaderPropertyIds.Tint, Color.white);
            material.SetFloat(InternalShaderPropertyIds.Intensity, 1f);
            material.SetFloat(InternalShaderPropertyIds.Rotation, 0f);
            
            Object.DontDestroyOnLoad(material);
            
            return material;
        }
    }
}
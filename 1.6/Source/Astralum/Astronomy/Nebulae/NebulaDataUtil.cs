using System.Collections.Generic;
using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Nebulae
{
  public static class NebulaDataUtil
  {
    public static WorldComponent_CelestialObjectDataCache Data => Find.World.GetComponent<WorldComponent_CelestialObjectDataCache>();

    public static SavedNebula Create(string id, Vector3 dir, float size, float rotationDegrees, 
      HashSet<string> usedNames)
    {
      Color[] palette = NebulaeColorUtil.RandomNebulaPalette();
      float colorStopB = Rand.Range(0.18f, 0.48f);
      float colorStopC = Rand.Range(colorStopB + 0.15f, 1f);
      string generatedName = NebulaNamingUtil.GenerateUniqueName(usedNames, id, dir);
      
      return CelestialObjectDataUtil.CreateNameable<SavedNebula>(id, dir, size, generatedName, rotationDegrees,
        nebula =>
        {
          nebula.rotationDegrees = rotationDegrees;
          nebula.colorA = palette[0];
          nebula.colorB = palette[1];
          nebula.colorC = palette[2];
          nebula.colorD = palette[3];
          nebula.colorStopB = colorStopB;
          nebula.colorStopC = colorStopC;
          nebula.colorBandSharpness = Rand.Range(0.25f, 8f);
          
          nebula.seedOffset = new Vector4(
            Rand.Range(-1000f, 1000f),
            Rand.Range(-1000f, 1000f),
            Rand.Range(-1000f, 1000f),
            Rand.Range(-1000f, 1000f));
          
          nebula.seed = Rand.Value * 1000f;
          nebula.intensity = Rand.Range(1f, 3f);
          nebula.alpha = Rand.Range(0.7f, 1f);
          nebula.noiseScale = Rand.Range(3.25f, 7.5f);
          nebula.noiseStrength = Rand.Range(0.8f, 1.35f);
          nebula.cloudThreshold = Rand.Range(0.34f, 0.52f);
          nebula.edgeSoftness = Rand.Range(0.32f, 0.62f);
          nebula.warpScale = Rand.Range(1.5f, 4.5f);
          nebula.warpStrength = Rand.Range(0.18f, 0.65f);
          nebula.shapePower = Rand.Range(1.2f, 2.4f);
          
          nebula.coreOffset = new Vector4(
            Rand.Range(-0.12f, 0.12f),
            Rand.Range(-0.12f, 0.12f),
            0f, 0f);
          
          nebula.stretchX = 1f;
          nebula.stretchY = 1f;
          nebula.shaderRotation = Rand.Range(0f, Mathf.PI * 2f);
        }
      );
    }
    
    public static SavedNebula GetById(string id)
    {
      return Data?.Nebulas.GetById(id);
    }
  }
}
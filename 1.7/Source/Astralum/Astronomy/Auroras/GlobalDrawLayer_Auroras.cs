using System.Collections;
using Astralum.Materials;
using Astralum.Settings;
using Astralum.WorldComponents;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Auroras
{
  public class GlobalDrawLayer_Auroras : WorldDrawLayerBase
  {
    public override IEnumerable Regenerate()
    {
      foreach (object item in base.Regenerate())
        yield return item;
      
      try
      {
        if (!AstraSettings.RenderWorldAuroras)
          yield break;
        
        World world = Find.World;
        
        if (world == null)
          yield break;
        
        int worldSeed = Find.World.info.Seed.GetHashCode();
        
        Material mat = AuroraMatsUtil.AuroraMaterial;
        TryGetAndPassWorldSeed(world, mat);
        
        LayerSubMesh subMesh = GetSubMesh(mat);
        float planetRadius = WorldUtils.PlanetRadius;
        
        AuroraMeshGenerator.PrintAurora(subMesh, planetRadius, Vector3.up, worldSeed ^ 0x4A17B31);
        AuroraMeshGenerator.PrintAurora(subMesh, planetRadius, Vector3.down, worldSeed ^ 0x72C91D5);
      }
      finally
      {
        FinalizeMesh(MeshParts.All);
      }
    }
    
    private static void TryGetAndPassWorldSeed(World world, Material material)
    {
      if (world == null)
        return;
      
      int seed = Find.World?.info?.Seed.GetHashCode() ?? 0;
      uint hash = (uint)seed;
      
      hash ^= hash >> 16;
      hash *= 0x7FEB352Du;
      hash ^= hash >> 15;
      hash *= 0x846CA68Bu;
      hash ^= hash >> 16;
      
      float shaderSeed = (hash & 0x00FFFFFFu) / 16777215f;
      
      material.SetFloat(InternalShaderPropertyIds.Seed, shaderSeed);
    }
  }
}
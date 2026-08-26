using System.Collections;
using System.Collections.Generic;
using Astralum.Materials;
using Astralum.Settings;
using Astralum.World;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public class GlobalDrawLayer_Constellations : WorldDrawLayerBase
  {
    private bool _calculatedForDrawConstellationLines;
    private bool _calculatedForStaticRotation;

    private bool UseStaticRotation => Current.ProgramState == ProgramState.Entry;
    protected override int RenderLayer => WorldCameraManager.WorldSkyboxLayer;

    protected override Quaternion Rotation => UseStaticRotation
        ? Quaternion.identity
        : Quaternion.LookRotation(GenCelestial.CurSunPositionInWorldSpace());
    
    public override bool ShouldRegenerate
    {
      get
      {
        if (base.ShouldRegenerate)
          return true;

        if (UseStaticRotation != _calculatedForStaticRotation)
          return true;

        return CelestialSettings.DrawConstellationLines != _calculatedForDrawConstellationLines;
      }
    }

    public override IEnumerable Regenerate()
    {
      foreach (object item in base.Regenerate())
      {
        yield return item;
      }
      
      try
      {
        if (!AstraSettings.RenderConstellations)
          yield break;
        
        ConstellationGenerationUtil.EnsureGenerated();
        WorldComponent_ConstellationDataCache data = ConstellationDataUtil.Data;
        
        if (data?.Constellations.NullOrEmpty() != false)
        {
          yield break;
        }
        
        if (!CelestialSettings.DrawConstellationLines)
        {
          yield break;
        }
        
        PrintSavedConstellationLines(data.Constellations);
      }
      finally
      {
        _calculatedForStaticRotation = UseStaticRotation;
        _calculatedForDrawConstellationLines = CelestialSettings.DrawConstellationLines;
        
        FinalizeMesh(MeshParts.All);
      }
    }
    
    private void PrintSavedConstellationLines(List<SavedConstellation> constellations)
    {
      for (int i = 0; i < constellations.Count; i++)
      {
        PrintSavedConstellationLines(constellations[i]);
      }
    }
    
    private void PrintSavedConstellationLines(SavedConstellation constellation)
    {
      Texture2D mask = ConstellationMaskUtil.GetMaskByName(constellation.maskName);
      
      if (mask == null)
        return;
      
      Material material = ConstellationsMatsUtil.For(mask);
      
      if (material == null)
        return;
      
      LayerSubMesh subMesh = GetSubMesh(material);
      
      PrintConstellationQuad(constellation.centerDir, constellation.RenderSize, constellation.Rotation, subMesh);
    }
    
    private static void PrintConstellationQuad(Vector3 centerDir, float size, float rotationDegrees, 
      LayerSubMesh subMesh)
    {
      Vector3 center = centerDir.normalized * ConstellationGenerationUtil.DistanceToConstellations;
      
      ConstellationGenerationUtil.GetConstellationBasis(centerDir, rotationDegrees,
        out Vector3 tangentA, out Vector3 tangentB);
      
      float halfSize = size * 0.5f;
      
      Vector3 v0 = center - tangentA * halfSize - tangentB * halfSize;
      Vector3 v1 = center - tangentA * halfSize + tangentB * halfSize;
      Vector3 v2 = center + tangentA * halfSize + tangentB * halfSize;
      Vector3 v3 = center + tangentA * halfSize - tangentB * halfSize;
      
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
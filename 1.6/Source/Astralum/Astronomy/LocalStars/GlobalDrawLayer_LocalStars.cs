using System.Collections;
using System.Collections.Generic;
using Astralum.API;
using Astralum.Materials;
using Astralum.World;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.LocalStars
{
  public class GlobalDrawLayer_LocalStars : WorldDrawLayerBase
  {
    protected override int RenderLayer => WorldCameraManager.WorldSkyboxLayer;
    protected override Quaternion Rotation => Quaternion.LookRotation(GenCelestial.CurSunPositionInWorldSpace());
    
    public override IEnumerable Regenerate()
    {
      foreach (object item in base.Regenerate())
        yield return item;
      
      LocalStarGenerationUtil.EnsureGenerated();
      WorldComponent_CelestialObjectDataCache data = LocalStarDataUtil.Data;
      
      if (data?.LocalStars.NullOrEmpty() != false)
      {
        FinalizeMesh(MeshParts.All);
        yield break;
      }
      
      CelestialObjectInteractionRegistry.Clear(CelestialObjectType.LocalStar);
      PrintSavedLocalStars(data.LocalStars);
      FinalizeMesh(MeshParts.All);
    }
    
    private void PrintSavedLocalStars(List<SavedLocalStar> stars)
    {
      List<SavedLocalStar> orderedStars = new(stars);
      orderedStars.Sort(CompareStarsByDepth);
      
      for (int i = 0; i < orderedStars.Count; i++)
      {
        SavedLocalStar star = orderedStars[i];
        Material material = LocalStarsMatsUtil.For(star);
        
        if (material == null)
          continue;
        
        Vector3 position = LocalStarOrbitUtil.PositionFor(star);
        float renderSize = LocalStarRenderUtil.RenderSizeFor(star);
        LayerSubMesh subMesh = GetSubMesh(material);
        
        RegisterLocalStarForInteraction(star, position, renderSize);
        PrintLocalStar(star, subMesh, position, renderSize);
      }
    }
    
    private static int CompareStarsByDepth(SavedLocalStar a, SavedLocalStar b)
    {
      float depthA = LocalStarOrbitUtil.DepthFor(a);
      float depthB = LocalStarOrbitUtil.DepthFor(b);
      
      return depthA.CompareTo(depthB);
    }
    
    private static void PrintLocalStar(SavedLocalStar star, LayerSubMesh subMesh, Vector3 position, float renderSize)
    {
      WorldRendererUtility.PrintQuadTangentialToPlanet(position, renderSize, 0f, subMesh,
        true, star.Rotation);
    }
    
    private static void RegisterLocalStarForInteraction(SavedLocalStar localStar, Vector3 position, float renderSize)
    {
      Vector3 dir = position.normalized;
      SkyCoord coord = WorldUtils.DirectionToSkyCoord(dir);
      
      CelestialObjectInteractionRegistry.Register(
        CelestialObjectType.LocalStar,
        localStar.Id,
        localStar.DisplayName,
        position,
        renderSize,
        WorldUtils.SkyHemisphere(dir),
        WorldUtils.FormatRightAscension(coord.rightAscensionHours),
        WorldUtils.FormatDeclination(coord.declinationDegrees));
    }
  }
}
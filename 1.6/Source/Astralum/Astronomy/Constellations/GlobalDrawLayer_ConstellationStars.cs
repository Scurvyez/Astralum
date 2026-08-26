using System.Collections;
using System.Collections.Generic;
using Astralum.API;
using Astralum.Materials;
using Astralum.Settings;
using Astralum.World;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public class GlobalDrawLayer_ConstellationStars : WorldDrawLayerBase
  {
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
        
        return UseStaticRotation != _calculatedForStaticRotation;
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
        
        CelestialObjectInteractionRegistry.Clear(CelestialObjectType.ConstellationStar);
        PrintSavedStars(data.Constellations);
      }
      finally
      {
        _calculatedForStaticRotation = UseStaticRotation;
        FinalizeMesh(MeshParts.All);
      }
    }
    
    private void PrintSavedStars(List<SavedConstellation> constellations)
    {
      for (int i = 0; i < constellations.Count; i++)
      {
        PrintSavedStars(constellations[i]);
      }
    }
    
    private void PrintSavedStars(SavedConstellation constellation)
    {
      if (constellation.stars.NullOrEmpty())
        return;
      
      for (int i = 0; i < constellation.stars.Count; i++)
      {
        SavedConstellationStar star = constellation.stars[i];
        RegisterConstellationStarForInteraction(star, constellation);
        Material material = BackgroundStarMatsUtil.For(star.spectralClass);
        LayerSubMesh subMesh = GetSubMesh(material);

        WorldRendererUtility.PrintQuadTangentialToPlanet(star.LocalSkyPosition, star.RenderSize, 0f,
            subMesh, true, star.Rotation);
      }
    }
    
    private static void RegisterConstellationStarForInteraction(SavedConstellationStar star, 
      SavedConstellation constellation)
    {
      Vector3 dir = star.LocalSkyPosition;
      SkyCoord coord = WorldUtils.DirectionToSkyCoord(dir);
      
      CelestialObjectInteractionRegistry.Register(
        CelestialObjectType.ConstellationStar,
        star.Id,
        star.DisplayName,
        star.LocalSkyPosition,
        star.RenderSize,
        WorldUtils.SkyHemisphere(dir),
        WorldUtils.FormatRightAscension(coord.rightAscensionHours),
        WorldUtils.FormatDeclination(coord.declinationDegrees),
        star.spectralClass, 
        constellation.DisplayName);
    }
  }
}
using System.Collections;
using Astralum.API;
using Astralum.DefOfs;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.SkyGrid
{
  public class GlobalDrawLayer_SkyCoordinateGrid : WorldDrawLayerBase
  {
    private const float MajorWidth = 0.05f;
    private const float MediumWidth = 0.025f;
    private const float MinorWidth = 0.0125f;

    private bool _calculatedForDrawGrid;
    private bool _calculatedForStaticRotation;
    
    private static bool UseStaticRotation => Current.ProgramState == ProgramState.Entry;
    
    protected override int RenderLayer => WorldCameraManager.WorldSkyboxLayer;
    protected override Quaternion Rotation => Quaternion.LookRotation(GenCelestial.CurSunPositionInWorldSpace());
    
    public override bool ShouldRegenerate
    {
      get
      {
        if (base.ShouldRegenerate)
          return true;
        
        if (UseStaticRotation != _calculatedForStaticRotation)
          return true;
        
        return CelestialDisplaySettings.DrawSkyCoordGrid != _calculatedForDrawGrid;
      }
    }

    public override IEnumerable Regenerate()
    {
      foreach (object item in base.Regenerate())
        yield return item;

      if (!CelestialDisplaySettings.DrawSkyCoordGrid)
      {
        _calculatedForStaticRotation = UseStaticRotation;
        _calculatedForDrawGrid = CelestialDisplaySettings.DrawSkyCoordGrid;

        FinalizeMesh(MeshParts.All);
        yield break;
      }

      Material mat = MaterialPool.MatFrom(InternalDefOf.Astra_SkyCoordinateGrid01.Shader);
      LayerSubMesh subMesh = GetSubMesh(mat);

      Vector3 pole = CelestialTransformUtil.GalacticPole.normalized;

      // galactic plane
      SkyGridDrawUtil.PrintGreatCircle(subMesh, pole, MajorWidth);

      // hemisphere boundaries
      SkyGridDrawUtil.PrintLatitudeCircle(subMesh, pole, SkyCoordUtil.NorthernSkyThreshold, MediumWidth);
      SkyGridDrawUtil.PrintLatitudeCircle(subMesh, pole, SkyCoordUtil.SouthernSkyThreshold, MediumWidth);

      // declination lines
      SkyGridDrawUtil.PrintLatitudeCircle(subMesh, pole, SkyCoordUtil.SkyLatitudeHeight(30f), MinorWidth);
      SkyGridDrawUtil.PrintLatitudeCircle(subMesh, pole, SkyCoordUtil.SkyLatitudeHeight(-30f), MinorWidth);
      SkyGridDrawUtil.PrintLatitudeCircle(subMesh, pole, SkyCoordUtil.SkyLatitudeHeight(60f), MinorWidth);
      SkyGridDrawUtil.PrintLatitudeCircle(subMesh, pole, SkyCoordUtil.SkyLatitudeHeight(-60f), MinorWidth);

      // RA / longitude meridians
      SkyGridDrawUtil.PrintMeridian(subMesh, pole, Vector3.forward, MediumWidth);
      SkyGridDrawUtil.PrintMeridian(subMesh, pole, Vector3.right, MediumWidth);

      // tick marks along the equator
      SkyGridDrawUtil.PrintEquatorTicks(subMesh, pole, 24, 0.045f, MajorWidth,
        Vector3.forward, Vector3.right);

      _calculatedForStaticRotation = UseStaticRotation;
      _calculatedForDrawGrid = CelestialDisplaySettings.DrawSkyCoordGrid;

      FinalizeMesh(MeshParts.All);
    }
  }
}
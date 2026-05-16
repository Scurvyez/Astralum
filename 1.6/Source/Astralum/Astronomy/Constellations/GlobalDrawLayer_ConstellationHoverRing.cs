using System.Collections;
using Astralum.Materials;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public class GlobalDrawLayer_ConstellationHoverRing : WorldDrawLayerBase
  {
    private PlanetTile _calculatedForStartingTile = PlanetTile.Invalid;
    private bool _calculatedForStaticRotation;

    protected override int RenderLayer => WorldCameraManager.WorldSkyboxLayer;

    private bool UseStaticRotation => Current.ProgramState == ProgramState.Entry;

    protected override Quaternion Rotation =>
      UseStaticRotation
        ? Quaternion.identity
        : Quaternion.LookRotation(GenCelestial.CurSunPositionInWorldSpace());

    public override bool ShouldRegenerate
    {
      get
      {
        if (base.ShouldRegenerate)
          return true;

        if (Find.GameInitData != null && Find.GameInitData.startingTile != _calculatedForStartingTile)
          return true;

        if (UseStaticRotation != _calculatedForStaticRotation)
          return true;

        return ConstellationHoverState.Dirty;
      }
    }

    public override IEnumerable Regenerate()
    {
      foreach (object item in base.Regenerate())
        yield return item;

      ConstellationHoverState.Dirty = false;

      if (ConstellationHoverState.CurrentStar.HasValue)
      {
        ConstellationInteractionRegistry.HoverStar star = ConstellationHoverState.CurrentStar.Value;

        Material material = ConstellationHoverMatsUtil.Ring;
        material.SetFloat(InternalShaderPropertyIds.PulseTime, ConstellationHoverState.PulseTime);

        LayerSubMesh subMesh = GetSubMesh(ConstellationHoverMatsUtil.Ring);
        ConstellationHoverRingRenderer.PrintRing(star.localSkyPos, subMesh);
      }

      _calculatedForStartingTile = Find.GameInitData != null
        ? Find.GameInitData.startingTile
        : PlanetTile.Invalid;

      _calculatedForStaticRotation = UseStaticRotation;

      FinalizeMesh(MeshParts.All);
    }
  }
}
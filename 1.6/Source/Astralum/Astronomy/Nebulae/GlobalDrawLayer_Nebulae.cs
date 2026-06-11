using System.Collections;
using System.Collections.Generic;
using Astralum.Astronomy.BackgroundStars;
using Astralum.Debugging;
using Astralum.DefOfs;
using Astralum.Materials;
using Astralum.Settings;
using Astralum.World;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Nebulae
{
  public class GlobalDrawLayer_Nebulae : WorldDrawLayerBase
  {
    private const float DistanceToNebulae = 20f;
    private readonly GlobalWorldDrawLayerDef _def;
    private readonly ModExt_Nebulae _ext;
    
    private IntRange _nebulaCount = new(10, 13);
    private bool _calculatedForStaticRotation;
    private FloatRange _galacticPlaneBounds = new(-0.18f, 0.18f);
    private FloatRange _nebulaSizeRange = new(6f, 18f);
    
    public GlobalDrawLayer_Nebulae()
    {
      if (!AstraSettings.RenderNebulae)
        return;
      
      _def = InternalDefOf.Astra_Nebulae;
      _ext = _def?.GetModExtension<ModExt_Nebulae>();
      
      if (_ext == null)
      {
        AstraLog.Warning("Astra_Nebulae is missing ModExt_Nebulae. Using fallback values.");
        return;
      }
      
      _nebulaCount = _ext.nebulaCount;
      _nebulaSizeRange = _ext.nebulaSizeRange;
      _galacticPlaneBounds = _ext.galacticPlaneBounds;
    }

    private bool UseStaticRotation => Current.ProgramState == ProgramState.Entry;

    protected override int RenderLayer => WorldCameraManager.WorldSkyboxLayer;

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
        
        return UseStaticRotation != _calculatedForStaticRotation;
      }
    }

    public override IEnumerable Regenerate()
    {
      foreach (object item in base.Regenerate())
        yield return item;
      
      if (!AstraSettings.RenderNebulae)
        yield break;

      try
      {
        WorldComponent_NebulaeData data = NebulaDataUtil.Data;

        if (data == null)
          yield break;

        if (!data.HasGeneratedNebulae)
          GenerateAndSaveNebulae(data);

        PrintSavedNebulae(data.nebulae);
      }
      finally
      {
        _calculatedForStaticRotation = UseStaticRotation;

        FinalizeMesh(MeshParts.All);
      }
    }

    private void GenerateAndSaveNebulae(WorldComponent_NebulaeData data)
    {
      data.Clear();

      Rand.PushState();
      Rand.Seed = Find.World.info.Seed ^ 0x4E384C41;
      
      GlobalWorldDrawLayerDef backgroundStarsDef = InternalDefOf.Astra_BackgroundStars;
      ModExt_BackgroundStars backgroundStarsExt = backgroundStarsDef?.GetModExtension<ModExt_BackgroundStars>();
      
      IntRange backgroundStarCountRange = BackgroundStarsUtil.ResolvedStarCountRange(backgroundStarsExt);
      
      BackgroundStarsGenerationData backgroundStarsGenerationData =
        BackgroundStarsUtil.GetGenerationData(Find.World.info.Seed, backgroundStarCountRange);
      
      int nebulaCount = Mathf.RoundToInt(
        Mathf.Lerp(_nebulaCount.min, _nebulaCount.max, backgroundStarsGenerationData.NormalizedStarCount));
      
      AstraLog.Message($"Generating {_nebulaCount.min} to {_nebulaCount.max} nebulae, " +
                       $"based on {backgroundStarCountRange.min} to {backgroundStarCountRange.max} background stars.");
      
      AstraLog.Message($"Nebula count: {nebulaCount}");
      
      try
      {
        for (int i = 0; i < nebulaCount; i++)
        {
          Vector3 localSkyPos =
            WorldUtils.RandomGalacticPlaneDirection(_galacticPlaneBounds) * DistanceToNebulae;

          float size = _nebulaSizeRange.RandomInRange;
          float rotationDegrees = Rand.Range(0f, 360f);

          data.nebulae.Add(
            NebulaDataUtil.CreateRandomNebula(i, localSkyPos, size, rotationDegrees)
          );
        }
      }
      finally
      {
        Rand.PopState();
      }
    }
    
    private void PrintSavedNebulae(List<SavedNebula> nebulae)
    {
      if (nebulae.NullOrEmpty())
        return;

      for (int i = 0; i < nebulae.Count; i++)
      {
        SavedNebula nebula = nebulae[i];

        Material material = NebulaeMatsUtil.For(nebula.nebulaId);
        NebulaDataUtil.ApplyToMaterial(material, nebula);
        LayerSubMesh subMesh = GetSubMesh(material);

        WorldRendererUtility.PrintQuadTangentialToPlanet(nebula.localSkyPos, nebula.size, 0f,
          subMesh, true, nebula.rotationDegrees);
      }
    }
  }
}
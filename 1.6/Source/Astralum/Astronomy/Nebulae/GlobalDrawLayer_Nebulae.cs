using System.Collections;
using System.Collections.Generic;
using Astralum.API;
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
    private readonly GlobalWorldDrawLayerDef _def;
    private readonly ModExt_Nebulae _ext;
    
    private bool _calculatedForStaticRotation = true;
    
    private IntRange _nebulaCount = new(10, 13);
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
        yield return item;
      
      Rand.PushState();
      Rand.Seed = Find.World.info.Seed ^ 0x4E384C41;

      try
      {
        if (!AstraSettings.RenderNebulae)
          yield break;
        
        WorldComponent_CelestialObjectDataCache data = NebulaDataUtil.Data;

        if (data == null)
          yield break;

        if (!data.HasGeneratedNebulae)
          GenerateAndSaveNebulae(data);
        
        if (data.Nebulas.NullOrEmpty())
          yield break;

        CelestialObjectInteractionRegistry.Clear(CelestialObjectType.Nebulae);
        PrintSavedNebulae(data.Nebulas);
      }
      finally
      {
        Rand.PopState();
        _calculatedForStaticRotation = UseStaticRotation;
        FinalizeMesh(MeshParts.All);
      }
    }

    private void GenerateAndSaveNebulae(WorldComponent_CelestialObjectDataCache data)
    {
      data.ClearNebulas();
      
      GlobalWorldDrawLayerDef backgroundStarsDef = InternalDefOf.Astra_BackgroundStars;
      ModExt_BackgroundStars backgroundStarsExt = backgroundStarsDef?.GetModExtension<ModExt_BackgroundStars>();
      
      IntRange backgroundStarCountRange = BackgroundStarsUtil.ResolvedStarCountRange(backgroundStarsExt);
      
      BackgroundStarsGenerationData backgroundStarsGenerationData =
        BackgroundStarsUtil.GetGenerationData(Find.World.info.Seed, backgroundStarCountRange);
      
      int nebulaCount = Mathf.RoundToInt(
        Mathf.Lerp(_nebulaCount.min, _nebulaCount.max, backgroundStarsGenerationData.NormalizedStarCount));
      
      HashSet<string> usedNames = [];
      
      for (int i = 0; i < nebulaCount; i++)
      {
        Vector3 dir = WorldUtils.RandomGalacticPlaneDirection(_galacticPlaneBounds);
        float size = _nebulaSizeRange.RandomInRange;
        float rotation = Rand.Range(0f, 360f);
        string id = $"nebulae_{Find.World.info.seedString}_{i}";
          
        data.Nebulas.Add(NebulaDataUtil.Create(id, dir, size, rotation, usedNames));
      }
    }
    
    private void PrintSavedNebulae(List<SavedNebula> nebulae)
    {
      if (nebulae.NullOrEmpty())
        return;
      
      for (int i = 0; i < nebulae.Count; i++)
      {
        SavedNebula nebula = nebulae[i];
        RegisterNebulaForInteraction(nebula);
        Material material = NebulaeMatsUtil.For(nebula.Id);
        NebulaeMatsUtil.ApplyToMaterial(material, nebula);
        LayerSubMesh subMesh = GetSubMesh(material);
        
        WorldRendererUtility.PrintQuadTangentialToPlanet(nebula.LocalSkyPosition, nebula.RenderSize, 0f,
          subMesh, true, nebula.Rotation);
      }
    }
    
    private static void RegisterNebulaForInteraction(SavedNebula nebula)
    {
      Vector3 dir = nebula.LocalSkyPosition.normalized;
      SkyCoord coord = WorldUtils.DirectionToSkyCoord(dir);
      
      CelestialObjectInteractionRegistry.Register(
        CelestialObjectType.Nebulae,
        nebula.Id,
        nebula.DisplayName,
        nebula.LocalSkyPosition,
        nebula.RenderSize,
        WorldUtils.SkyHemisphere(dir),
        WorldUtils.FormatRightAscension(coord.rightAscensionHours),
        WorldUtils.FormatDeclination(coord.declinationDegrees));
    }
  }
}
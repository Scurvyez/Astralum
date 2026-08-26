using System.Collections;
using System.Collections.Generic;
using Astralum.Debugging;
using Astralum.DefOfs;
using Astralum.Materials;
using Astralum.Settings;
using Astralum.World;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.GalacticDustLanes
{
  public class GlobalDrawLayer_GalacticDustLanes : WorldDrawLayerBase
  {
    private const float DistanceToDustLanes = 20f;
    
    private readonly GlobalWorldDrawLayerDef _def;
    private readonly ModExt_GalacticDustLanes _ext;
    
    private IntRange _dustLaneCount = new(4, 7);
    private FloatRange _dustLaneSizeRange = new(18f, 36f);
    private FloatRange _galacticPlaneBounds = new(-0.10f, 0.10f);
    private bool _calculatedForStaticRotation;
    
    public GlobalDrawLayer_GalacticDustLanes()
    {
      _def = InternalDefOf.Astra_GalacticDustLanes;
      _ext = _def?.GetModExtension<ModExt_GalacticDustLanes>();
      
      if (_ext == null)
      {
        AstraLog.Warning("Astra_GalacticDustLanes is missing ModExt_GalacticDustLanes. Using fallback values.");
        return;
      }
      
      _dustLaneCount = _ext.dustLaneCount;
      _dustLaneSizeRange = _ext.dustLaneSizeRange;
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
      
      try
      {
        if (!AstraSettings.RenderDustlanes)
          yield break;
        
        WorldComponent_CelestialObjectDataCache data = GalacticDustLanesDataUtil.Data;
        
        if (data == null)
          yield break;
        
        if (!data.HasGeneratedDustLanes)
          GenerateAndSaveDustLanes(data);
        
        PrintSavedDustLanes(data.DustLanes);
      }
      finally
      {
        _calculatedForStaticRotation = UseStaticRotation;
        FinalizeMesh(MeshParts.All);
      }
    }

    private void GenerateAndSaveDustLanes(WorldComponent_CelestialObjectDataCache data)
    {
      data.ClearDustLanes();
      
      Rand.PushState();
      Rand.Seed = (int)(Find.World.info.Seed ^ 0xD0571A4E);

      try
      {
        int dustLaneCount = Mathf.Clamp(_dustLaneCount.RandomInRange, 0, 10);
        
        for (int i = 0; i < dustLaneCount; i++)
        {
          Vector3 dir = WorldUtils.RandomGalacticPlaneDirection(_galacticPlaneBounds);
          float size = _dustLaneSizeRange.RandomInRange;
          float rotation = GalacticDustLanesUtil.DustLaneRotationDegrees(dir);
          string id = $"dustlane_{Find.World.info.seedString}_{i}";
          
          data.DustLanes.Add(GalacticDustLanesDataUtil.Create(id, dir, size, rotation));
        }
      }
      finally
      {
        Rand.PopState();
      }
    }
    
    private void PrintSavedDustLanes(List<SavedGalacticDustLane> dustLanes)
    {
      if (dustLanes.NullOrEmpty())
        return;

      for (int i = 0; i < dustLanes.Count; i++)
      {
        SavedGalacticDustLane dustlane = dustLanes[i];
        Material material = GalacticDustLaneMatsUtil.For(dustlane.Id);
        GalacticDustLaneMatsUtil.ApplyToMaterial(material, dustlane);
        LayerSubMesh subMesh = GetSubMesh(material);
        
        WorldRendererUtility.PrintQuadTangentialToPlanet(dustlane.LocalSkyPosition, dustlane.RenderSize, 0f,
          subMesh, true, dustlane.Rotation);
      }
    }
  }
}
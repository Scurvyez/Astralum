using System.Collections;
using Astralum.Debugging;
using Astralum.DefOfs;
using Astralum.Materials;
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
    
    private FloatRange _alphaRange = new(0.08f, 0.18f);
    private FloatRange _intensityRange = new(0.35f, 0.75f);
    
    private FloatRange _noiseScaleRange = new(2.5f, 6.5f);
    private FloatRange _detailScaleRange = new(12f, 30f);
    
    private FloatRange _stretchXRange = new(1.6f, 3.2f);
    private FloatRange _stretchYRange = new(0.35f, 0.75f);
    
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
      
      _alphaRange = _ext.alphaRange;
      _intensityRange = _ext.intensityRange;
      
      _noiseScaleRange = _ext.noiseScaleRange;
      _detailScaleRange = _ext.detailScaleRange;
      
      _stretchXRange = _ext.stretchXRange;
      _stretchYRange = _ext.stretchYRange;
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
      
      Rand.PushState();
      Rand.Seed = (int)(Find.World.info.Seed ^ 0xD0571A4E);
      
      try
      {
        int count = Mathf.Clamp(_dustLaneCount.RandomInRange, 0, 20);

        for (int i = 0; i < count; i++)
          PrintDustLane(i);
      }
      finally
      {
        Rand.PopState();
        
        _calculatedForStaticRotation = UseStaticRotation;
        
        FinalizeMesh(MeshParts.All);
      }
    }
    
    private void PrintDustLane(int index)
    {
      Material material = GalacticDustLaneMatsUtil.For(index);
      
      if (material == null)
        return;
      
      ApplyRandomMaterialProperties(material);
      
      LayerSubMesh subMesh = GetSubMesh(material);
      
      Vector3 localSkyPos = WorldUtils.RandomGalacticPlaneDirection(_galacticPlaneBounds) * DistanceToDustLanes;
      
      float size = _dustLaneSizeRange.RandomInRange;
      float rotationDegrees = Rand.Range(0f, 360f);
      
      WorldRendererUtility.PrintQuadTangentialToPlanet(localSkyPos, size, 0f, subMesh,
        counterClockwise: true, rotationDegrees);
    }
    
    private void ApplyRandomMaterialProperties(Material material)
    {
      GalacticDustLaneMatsUtil.RandomDustPalette(out var colorA, out var colorB);
      
      material.SetColor(InternalShaderPropertyIds.ColorA, colorA);
      material.SetColor(InternalShaderPropertyIds.ColorB, colorB);
      material.SetFloat(InternalShaderPropertyIds.Alpha, _alphaRange.RandomInRange);
      material.SetFloat(InternalShaderPropertyIds.Intensity, _intensityRange.RandomInRange);
      material.SetFloat(InternalShaderPropertyIds.CanvasScale, 1f);
      material.SetFloat(InternalShaderPropertyIds.NoiseScale, _noiseScaleRange.RandomInRange);
      material.SetFloat(InternalShaderPropertyIds.NoiseStrength, Rand.Range(0.55f, 0.9f));
      material.SetFloat(InternalShaderPropertyIds.DetailScale, _detailScaleRange.RandomInRange);
      material.SetFloat(InternalShaderPropertyIds.DetailStrength, Rand.Range(0.15f, 0.45f));
      material.SetFloat(InternalShaderPropertyIds.CloudThreshold, Rand.Range(0.34f, 0.58f));
      material.SetFloat(InternalShaderPropertyIds.EdgeSoftness, Rand.Range(0.24f, 0.48f));
      material.SetFloat(InternalShaderPropertyIds.EdgeFadeStart, 0.02f);
      material.SetFloat(InternalShaderPropertyIds.EdgeFadeEnd, 0.18f);
      material.SetFloat(InternalShaderPropertyIds.StretchX, _stretchXRange.RandomInRange);
      material.SetFloat(InternalShaderPropertyIds.StretchY, _stretchYRange.RandomInRange);
      material.SetFloat(InternalShaderPropertyIds.Rotation, Rand.Range(0f, Mathf.PI * 2f));
      
      material.SetVector(InternalShaderPropertyIds.SeedOffset, new Vector4(
        Rand.Range(-1000f, 1000f),
        Rand.Range(-1000f, 1000f),
        Rand.Range(-1000f, 1000f),
        Rand.Range(-1000f, 1000f)
      ));
    }
  }
}
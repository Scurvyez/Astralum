using System.Collections;
using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.Debugging;
using Astralum.DefOfs;
using Astralum.Materials;
using Astralum.Settings;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.BackgroundStars
{
  public class GlobalDrawLayer_BackgroundStars : WorldDrawLayerBase
  {
    private const float DistanceToStars = 20f;
    private readonly GlobalWorldDrawLayerDef _def;
    private readonly ModExt_BackgroundStars _ext;
    
    private bool _calculatedForStaticRotation;
    private bool _useNonUniformGalacticPlaneBand;
    private float _galacticPlaneBandMaskOffset;
    private FloatRange _galacticPlaneBounds = new(-0.16f, 0.16f);
    private FloatRange _starSizeRange = new(0.085f, 0.85f);
    private IntRange _starCount = new(10000, 50000);
    
    public GlobalDrawLayer_BackgroundStars()
    {
      if (!AstraSettings.RenderAdditionalBackgroundStars)
        return;
      
      _def = InternalDefOf.Astra_BackgroundStars;
      _ext = _def?.GetModExtension<ModExt_BackgroundStars>();
      
      if (_ext == null)
      {
        AstraLog.Warning("Astra_BackgroundStars is missing ModExt_BackgroundStars. Using fallback values.");
        return;
      }
      
      _starSizeRange = _ext.starSizeRange;
      _galacticPlaneBounds = _ext.galacticPlaneBounds;
      _starCount = BackgroundStarsUtil.ResolvedStarCountRange(_ext);
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
      
      if (!AstraSettings.RenderAdditionalBackgroundStars)
        yield break;
      
      BackgroundStarsGenerationData generationData =
        BackgroundStarsUtil.GetGenerationData(Find.World.info.Seed, _starCount);
      
      Rand.PushState();
      Rand.Seed = Find.World.info.Seed ^ 0x71C04ED ^ 0x51A75123;
      
      _useNonUniformGalacticPlaneBand = generationData.UseNonUniformGalacticPlaneBand;
      _galacticPlaneBandMaskOffset = generationData.GalacticPlaneBandMaskOffset;
      
      for (int i = 0; i < generationData.StarCount; i++) 
        PrintColoredStar();
      
      _calculatedForStaticRotation = UseStaticRotation;
      
      Rand.PopState();
      
      FinalizeMesh(MeshParts.All);
    }
    
    private void PrintColoredStar()
    {
      SpectralClass spectralClass = BackgroundStarsUtil.RandomBackgroundStarClass();
      
      Vector3 dir = _useNonUniformGalacticPlaneBand
        ? BackgroundStarsUtil.RandomDensityWeightedDirection(
          spectralClass, _galacticPlaneBounds, _galacticPlaneBandMaskOffset)
        : BackgroundStarsUtil.RandomDensityWeightedDirection(
          spectralClass, _galacticPlaneBounds);
      
      Vector3 pos = dir * DistanceToStars;
      
      Material material = BackgroundStarMatsUtil.For(spectralClass);
      LayerSubMesh subMesh = GetSubMesh(material);
      
      float size = BackgroundStarsUtil.RandomStarSize(spectralClass, _starSizeRange);
      
      WorldRendererUtility.PrintQuadTangentialToPlanet(pos, size, 0f,
        subMesh, true, Rand.Range(0f, 360f));
    }
  }
}
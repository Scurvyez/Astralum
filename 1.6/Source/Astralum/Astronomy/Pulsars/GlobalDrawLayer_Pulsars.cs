using System.Collections;
using System.Collections.Generic;
using Astralum.API;
using Astralum.Debugging;
using Astralum.DefOfs;
using Astralum.Materials;
using Astralum.Settings;
using Astralum.World;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Pulsars
{
  public class GlobalDrawLayer_Pulsars : WorldDrawLayerBase
  {
    private readonly GlobalWorldDrawLayerDef _def;
    private readonly ModExt_Pulsars _ext;

    private bool _calculatedForStaticRotation;

    private readonly float _pulsarCanvasScale = 1f;
    private readonly float _pulsarChance = 0.05f;
    private IntRange _pulsarCount = new(0, 1);
    private FloatRange _pulsarSize = new(0.3f, 2f);

    public GlobalDrawLayer_Pulsars()
    {
      if (!AstraSettings.RenderPulsars)
        return;
      
      _def = InternalDefOf.Astra_Pulsars;
      _ext = _def?.GetModExtension<ModExt_Pulsars>();

      if (_ext == null)
      {
        AstraLog.Warning("Astra_Pulsars is missing ModExt_Pulsars. Using fallback values.");
        return;
      }
      
      _pulsarChance = Mathf.Clamp01(_ext.pulsarChance);
      _pulsarCount = _ext.pulsarCount;
      _pulsarSize = new FloatRange(
        Mathf.Clamp(_ext.pulsarSize.min, 0.5f, 2f),
        Mathf.Clamp(_ext.pulsarSize.max, 0.5f, 2f)
      );
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
      Rand.Seed = Find.World.info.Seed ^ 0x7115A2;
      
      try
      {
        if (!AstraSettings.RenderPulsars)
          yield break;
        
        WorldComponent_CelestialObjectDataCache data = PulsarDataUtil.Data;
        
        if (data == null)
          yield break;
        
        if (!data.HasGeneratedPulsars)
          GenerateAndSavePulsars(data);
        
        if (data.Pulsars.NullOrEmpty())
          yield break;
        
        CelestialObjectInteractionRegistry.Clear(CelestialObjectType.Pulsar);
        PrintSavedPulsars(data.Pulsars);
      }
      finally
      {
        Rand.PopState();
        _calculatedForStaticRotation = UseStaticRotation;
        FinalizeMesh(MeshParts.All);
      }
    }
    
    private void GenerateAndSavePulsars(WorldComponent_CelestialObjectDataCache data)
    {
      data.ClearPulsars();

      if (Rand.Value > _pulsarChance)
        return;

      int pulsarCount = Mathf.Clamp(_pulsarCount.RandomInRange, 0, 10);

      for (int i = 0; i < pulsarCount; i++)
      {
        Vector3 dir = RandomPulsarDirection();
        float size = _pulsarSize.RandomInRange * _pulsarCanvasScale * 3f;
        float rotation = 0f;
        string id = $"pulsar_{Find.World.info.seedString}_{i}";
        
        data.Pulsars.Add(PulsarDataUtil.Create(id, dir, size, rotation));
      }
    }

    private void PrintSavedPulsars(List<SavedPulsar> pulsars)
    {
      if (pulsars.NullOrEmpty())
        return;
      
      for (int i = 0; i < pulsars.Count; i++)
      {
        SavedPulsar pulsar = pulsars[i];
        RegisterPulsarForInteraction(pulsar);
        LayerSubMesh subMesh = GetSubMesh(PulsarMatsUtil.Pulsar);
        
        WorldRendererUtility.PrintQuadTangentialToPlanet(pulsar.LocalSkyPosition, pulsar.RenderSize, 0f, 
          subMesh, true, Rand.Range(0f, 360f));
      }
    }

    private static void RegisterPulsarForInteraction(SavedPulsar pulsar)
    {
      Vector3 dir = pulsar.LocalSkyPosition.normalized;
      SkyCoord coord = WorldUtils.DirectionToSkyCoord(dir);
      
      CelestialObjectInteractionRegistry.Register(
        CelestialObjectType.Pulsar,
        pulsar.Id,
        pulsar.DisplayName,
        pulsar.LocalSkyPosition,
        pulsar.RenderSize,
        WorldUtils.SkyHemisphere(dir),
        WorldUtils.FormatRightAscension(coord.rightAscensionHours),
        WorldUtils.FormatDeclination(coord.declinationDegrees));
    }
    
    private static Vector3 RandomPulsarDirection()
    {
      var angle = Rand.Range(0f, Mathf.PI * 2f);
      var localY = Rand.Range(-0.22f, 0.22f);
      var radius = Mathf.Sqrt(1f - localY * localY);
      
      Vector3 localDir = new(Mathf.Cos(angle) * radius, localY, Mathf.Sin(angle) * radius);
      var planeRotation = Quaternion.FromToRotation(Vector3.up, WorldUtils.GalacticPole.normalized);

      return (planeRotation * localDir).normalized;
    }
  }
}
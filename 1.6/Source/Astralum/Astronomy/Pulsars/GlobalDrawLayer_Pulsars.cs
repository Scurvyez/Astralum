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

namespace Astralum.Astronomy.Pulsars;

public class GlobalDrawLayer_Pulsars : WorldDrawLayerBase
{
  private const float DistanceToPulsars = 20f;
  private readonly GlobalWorldDrawLayerDef _def;
  private readonly ModExt_Pulsars _ext;

  private bool _calculatedForStaticRotation;

  private readonly float _pulsarCanvasScale = 1f;
  private readonly float _pulsarChance = 0.05f;
  private IntRange _pulsarCount = new(0, 1);
  private readonly float _pulsarSize = 0.8f;

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
    _pulsarSize = Mathf.Clamp(_ext.pulsarSize, 0.1f, 10f);
    _pulsarCanvasScale = _pulsarSize * 2f;
    _pulsarCount = _ext.pulsarCount;
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
    Rand.Seed = Find.World.info.Seed ^ 0x7115A2;
    
    try
    {
      PulsarInteractionRegistry.Clear();
      
      if (!AstraSettings.RenderPulsars)
        yield break;
      
      WorldComponent_PulsarData data = PulsarDataUtil.Data;
      
      if (data == null)
        yield break;
      
      if (!data.HasGeneratedPulsars)
        GenerateAndSavePulsars(data);
      
      if (data.pulsars.NullOrEmpty())
        yield break;
      
      LayerSubMesh subMesh = GetSubMesh(PulsarMatsUtil.Pulsar);
      
      PrintSavedPulsars(data.pulsars, subMesh);
    }
    finally
    {
      Rand.PopState();
      
      _calculatedForStaticRotation = UseStaticRotation;
      
      FinalizeMesh(MeshParts.All);
    }
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
  
  private static void PrintPulsar(Vector3 localSkyPos, float size, float rotationDegrees, LayerSubMesh subMesh)
  {
    WorldRendererUtility.PrintQuadTangentialToPlanet(localSkyPos, size, 0f, subMesh,
      true, rotationDegrees);
  }
  
  private void GenerateAndSavePulsars(WorldComponent_PulsarData data)
  {
    data.Clear();

    if (Rand.Value > _pulsarChance)
      return;

    int pulsarCount = Mathf.Clamp(_pulsarCount.RandomInRange, 0, 10);

    for (int i = 0; i < pulsarCount; i++)
    {
      Vector3 dir = RandomPulsarDirection();

      float size = _pulsarSize * _pulsarCanvasScale;
      float rotation = Rand.Range(0f, 360f);
      Vector3 localSkyPos = dir * DistanceToPulsars;

      data.pulsars.Add(
        PulsarDataUtil.Create(i, localSkyPos, size, rotation)
      );
    }
  }

  private void PrintSavedPulsars(List<SavedPulsar> pulsars, LayerSubMesh subMesh)
  {
    if (pulsars.NullOrEmpty())
      return;

    PulsarInteractionRegistry.Clear();

    for (int i = 0; i < pulsars.Count; i++)
    {
      SavedPulsar pulsar = pulsars[i];

      RegisterPulsarForInteraction(pulsar);

      PrintPulsar(
        pulsar.localSkyPos,
        pulsar.size,
        pulsar.rotationDegrees,
        subMesh
      );
    }
  }

  private static void RegisterPulsarForInteraction(SavedPulsar pulsar)
  {
    Vector3 dir = pulsar.localSkyPos.normalized;

    SkyCoord coord = WorldUtils.DirectionToSkyCoord(dir);

    PulsarInteractionRegistry.Register(
      pulsar.name,
      pulsar.localSkyPos,
      pulsar.size,
      WorldUtils.SkyHemisphere(dir),
      WorldUtils.FormatRightAscension(coord.rightAscensionHours),
      WorldUtils.FormatDeclination(coord.declinationDegrees)
    );
  }
}
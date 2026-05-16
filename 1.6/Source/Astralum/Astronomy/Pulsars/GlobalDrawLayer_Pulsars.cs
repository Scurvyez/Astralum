using System.Collections;
using Astralum.Materials;
using Astralum.World;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Pulsars
{
  public class GlobalDrawLayer_Pulsars : WorldDrawLayerBase
  {
    private const float DistanceToPulsars = 20f;
    private const int TestPulsarCount = 1;
    const float PulsarCanvasScale = 2f;
    const float PulsarBaseSize = 0.8f;
    private PlanetTile _calculatedForStartingTile = PlanetTile.Invalid;

    private bool _calculatedForStaticRotation;

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

        if (Find.GameInitData != null && Find.GameInitData.startingTile != _calculatedForStartingTile)
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
      
      LayerSubMesh subMesh = GetSubMesh(PulsarMatsUtil.Pulsar);
      
      for (int i = 0; i < TestPulsarCount; i++)
      {
        Vector3 dir = RandomPulsarDirection();
        
        float size = PulsarBaseSize * PulsarCanvasScale;
        float rotation = Rand.Range(0f, 360f);

        PrintPulsar(dir * DistanceToPulsars, size, rotation, subMesh);
      }
      
      Rand.PopState();
      
      _calculatedForStartingTile = Find.GameInitData != null
        ? Find.GameInitData.startingTile
        : PlanetTile.Invalid;
      
      _calculatedForStaticRotation = UseStaticRotation;
      
      FinalizeMesh(MeshParts.All);
    }
    
    private static Vector3 RandomPulsarDirection()
    {
      float angle = Rand.Range(0f, Mathf.PI * 2f);
      float localY = Rand.Range(-0.22f, 0.22f);
      float radius = Mathf.Sqrt(1f - localY * localY);
      
      Vector3 localDir = new(
        Mathf.Cos(angle) * radius,
        localY,
        Mathf.Sin(angle) * radius
      );
      
      Quaternion planeRotation = Quaternion.FromToRotation(Vector3.up, WorldUtils.GalacticPole.normalized);
      
      return (planeRotation * localDir).normalized;
    }
    
    private static void PrintPulsar(Vector3 localSkyPos, float size, float rotationDegrees, LayerSubMesh subMesh)
    {
      WorldRendererUtility.PrintQuadTangentialToPlanet(localSkyPos, size, 0f, subMesh,
        true, rotationDegrees);
    }
  }
}
using System.Collections;
using Astralum.Debugging;
using Astralum.DefOfs;
using Astralum.Materials;
using Astralum.World;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.BlackHoles
{
  public class GlobalDrawLayer_BlackHoles : WorldDrawLayerBase
  {
    private const float CameraRotationRegenerateThreshold = 0.25f;
    private const float DistanceToBlackHoles = 20f;
    private readonly GlobalWorldDrawLayerDef _def;
    private readonly ModExt_BlackHoles _ext;
    
    private float _blackHoleCanvasScale = 1f;
    private float _blackHoleChance = 0.05f;
    private float _blackHoleSize = 0.8f;
    private int _blackHoleCount = 1;
    private PlanetTile _calculatedForStartingTile = PlanetTile.Invalid;
    private bool _calculatedForStaticRotation = true;
    private Quaternion _calculatedForCameraRotation = Quaternion.identity;
    private bool _hasCalculatedCameraRotation;
    
    public GlobalDrawLayer_BlackHoles()
    {
      _def = InternalDefOf.Astra_BlackHoles;
      _ext = _def?.GetModExtension<ModExt_BlackHoles>();
      
      if (_ext == null)
      {
        AstraLog.Warning("Astra_BlackHoles is missing ModExt_BlackHoles. Using fallback values.");
        return;
      }
      
      _blackHoleChance = Mathf.Clamp01(_ext.blackHoleChance);
      _blackHoleSize = Mathf.Clamp(_ext.blackHoleSize.RandomInRange, 0.5f, 2f);
      _blackHoleCanvasScale = _blackHoleSize * 2f;
      _blackHoleCount = Mathf.Clamp(_ext.blackHoleCount, 0, 3);
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
        
        if (Find.GameInitData != null &&
            Find.GameInitData.startingTile != _calculatedForStartingTile)
          return true;
        
        if (UseStaticRotation != _calculatedForStaticRotation)
          return true;
        
        Camera camera = WorldCameraManager.WorldSkyboxCamera ?? Find.WorldCamera;
        
        if (camera == null)
          return false;
        
        if (!_hasCalculatedCameraRotation)
          return true;
        
        float angle = Quaternion.Angle(_calculatedForCameraRotation, camera.transform.rotation);
        return angle > CameraRotationRegenerateThreshold;
      }
    }
    
    public override IEnumerable Regenerate()
    {
      foreach (object item in base.Regenerate())
        yield return item;
      
      Rand.PushState();
      Rand.Seed = Find.World.info.Seed ^ 0xB1A64C;
      
      try
      {
        if (Rand.Value > _blackHoleChance)
          yield break;
        
        LayerSubMesh subMesh = GetSubMesh(BlackHoleMatsUtil.BlackHole);
        
        for (int i = 0; i < _blackHoleCount; i++)
        {
          Vector3 dir = RandomBlackHoleDirection();
          float size = _blackHoleSize * _blackHoleCanvasScale;
          
          PrintBlackHoleBillboard(
            dir * DistanceToBlackHoles,
            size,
            subMesh,
            Rotation
          );
        }
      }
      finally
      {
        Rand.PopState();
        
        Camera camera = WorldCameraManager.WorldSkyboxCamera ?? Find.WorldCamera;
        
        if (camera != null)
        {
          _calculatedForCameraRotation = camera.transform.rotation;
          _hasCalculatedCameraRotation = true;
        }
        else
        {
          _hasCalculatedCameraRotation = false;
        }
        
        _calculatedForStartingTile = Find.GameInitData != null
          ? Find.GameInitData.startingTile
          : PlanetTile.Invalid;
        
        _calculatedForStaticRotation = UseStaticRotation;
        
        FinalizeMesh(MeshParts.All);
      }
    }
    
    private static Vector3 RandomBlackHoleDirection()
    {
      float angle = Rand.Range(0f, Mathf.PI * 2f);
      float localY = Rand.Range(-0.18f, 0.18f);
      float radius = Mathf.Sqrt(1f - localY * localY);
      
      Vector3 localDir = new(
        Mathf.Cos(angle) * radius,
        localY,
        Mathf.Sin(angle) * radius
      );
      
      Quaternion planeRotation = Quaternion.FromToRotation(Vector3.up, WorldUtils.GalacticPole.normalized);
      
      return (planeRotation * localDir).normalized;
    }
    
    private static void PrintBlackHoleBillboard(Vector3 localSkyPos, float size, LayerSubMesh subMesh, 
      Quaternion layerRotation)
    {
      Camera camera = WorldCameraManager.WorldSkyboxCamera ?? Find.WorldCamera;
      
      if (camera == null)
        return;
      
      Quaternion inverseLayerRotation = Quaternion.Inverse(layerRotation);
      
      Vector3 right = inverseLayerRotation * camera.transform.right;
      Vector3 up = inverseLayerRotation * camera.transform.up;
      
      float halfSize = size * 0.5f;
      
      Vector3 center = localSkyPos;
      
      Vector3 v0 = center - right * halfSize - up * halfSize;
      Vector3 v1 = center - right * halfSize + up * halfSize;
      Vector3 v2 = center + right * halfSize + up * halfSize;
      Vector3 v3 = center + right * halfSize - up * halfSize;
      
      int baseIndex = subMesh.verts.Count;
      
      subMesh.verts.Add(v0);
      subMesh.verts.Add(v1);
      subMesh.verts.Add(v2);
      subMesh.verts.Add(v3);
      
      subMesh.uvs.Add(new Vector2(0f, 0f));
      subMesh.uvs.Add(new Vector2(0f, 1f));
      subMesh.uvs.Add(new Vector2(1f, 1f));
      subMesh.uvs.Add(new Vector2(1f, 0f));
      
      subMesh.tris.Add(baseIndex + 0);
      subMesh.tris.Add(baseIndex + 1);
      subMesh.tris.Add(baseIndex + 2);
      
      subMesh.tris.Add(baseIndex + 0);
      subMesh.tris.Add(baseIndex + 2);
      subMesh.tris.Add(baseIndex + 3);
    }
  }
}
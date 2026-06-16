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

namespace Astralum.Astronomy.BlackHoles
{
  public class GlobalDrawLayer_BlackHoles : WorldDrawLayerBase
  {
    private const float CameraRotationRegenerateThreshold = 0.25f;
    private readonly GlobalWorldDrawLayerDef _def;
    private readonly ModExt_BlackHoles _ext;
    
    private readonly float _blackHoleCanvasScale = 1f;
    private readonly float _blackHoleChance = 0.05f;
    private readonly FloatRange _blackHoleSize = new(0.5f, 2f);
    private readonly FloatRange _galacticPlaneBounds = new(-0.18f, 0.18f);
    private IntRange _blackHoleCount = new(0, 1);
    private bool _calculatedForStaticRotation = true;
    private Quaternion _calculatedForCameraRotation = Quaternion.identity;
    private bool _hasCalculatedCameraRotation;
    
    public GlobalDrawLayer_BlackHoles()
    {
      if (!AstraSettings.RenderBlackholes)
        return;
      
      _def = InternalDefOf.Astra_BlackHoles;
      _ext = _def?.GetModExtension<ModExt_BlackHoles>();
      
      if (_ext == null)
      {
        AstraLog.Warning("Astra_BlackHoles is missing ModExt_BlackHoles. Using fallback values.");
        return;
      }
      
      _blackHoleCanvasScale = 4f;
      _galacticPlaneBounds = _ext.galacticPlaneBounds;
      _blackHoleChance = Mathf.Clamp01(_ext.blackHoleChance);
      _blackHoleSize = new FloatRange(
        Mathf.Clamp(_ext.blackHoleSize.min, 0.5f, 2f),
        Mathf.Clamp(_ext.blackHoleSize.max, 0.5f, 2f)
      );
      _blackHoleCount = new IntRange(
        Mathf.Clamp(_ext.blackHoleCount.min, 0, 10),
        Mathf.Clamp(_ext.blackHoleCount.max, 0, 10)
      );
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
        if (!AstraSettings.RenderBlackholes)
          yield break;
        
        WorldComponent_BlackHoleData data = BlackHoleDataUtil.Data;
        
        if (data == null)
          yield break;
        
        if (!data.HasGeneratedBlackHoles)
          GenerateAndSaveBlackHoles(data);
        
        if (data.blackHoles.NullOrEmpty())
          yield break;
        
        LayerSubMesh subMesh = GetSubMesh(BlackHoleMatsUtil.BlackHole);
        
        PrintSavedBlackHoles(data.blackHoles, subMesh);
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
        
        _calculatedForStaticRotation = UseStaticRotation;
        
        FinalizeMesh(MeshParts.All);
      }
    }
    
    private void GenerateAndSaveBlackHoles(WorldComponent_BlackHoleData data)
    {
      data.Clear();

      if (Rand.Value > _blackHoleChance)
        return;

      List<SavedBlackHole> placed = [];
      int blackHoleCount = _blackHoleCount.RandomInRange;

      for (int i = 0; i < blackHoleCount; i++)
      {
        if (!BlackHolesUtil.TryPlaceBlackHole(
              placed,
              out Vector3 dir,
              out float size,
              _galacticPlaneBounds,
              _blackHoleSize,
              _blackHoleCanvasScale))
        {
          continue;
        }

        SavedBlackHole blackHole = BlackHoleDataUtil.Create(i, dir, size);
        
        placed.Add(blackHole);
        data.blackHoles.Add(blackHole);
      }
    }
    
    private void PrintSavedBlackHoles(List<SavedBlackHole> blackHoles, LayerSubMesh subMesh)
    {
      if (blackHoles.NullOrEmpty())
        return;
      
      for (int i = 0; i < blackHoles.Count; i++)
      {
        SavedBlackHole blackHole = blackHoles[i];
        
        PrintBlackHoleBillboard(
          blackHole.localSkyPos,
          blackHole.size,
          subMesh,
          Rotation
        );
      }
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
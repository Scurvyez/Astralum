using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Settings
{
  public static class SkyboxColorUtil
  {
    public static Color GetColor(SkyboxColorPreset preset)
    {
      return preset switch
      {
        SkyboxColorPreset.Vanilla => new Color32(16, 23, 30, 255),
        SkyboxColorPreset.DeepSpace => new Color32(3, 5, 9, 255),
        SkyboxColorPreset.Charcoal => new Color32(6, 8, 13, 255),
        SkyboxColorPreset.MidnightViolet => new Color32(8, 7, 17, 255),
        SkyboxColorPreset.Neutral => new Color32(8, 8, 8, 255),
        SkyboxColorPreset.Black => Color.black,
        _ => new Color32(3, 5, 9, 255)
      };
    }
    
    public static string GetLabel(SkyboxColorPreset preset)
    {
      return preset switch
      {
        SkyboxColorPreset.Vanilla => "Astra_SkyboxColorPreset_00".Translate(),
        SkyboxColorPreset.DeepSpace => "Astra_SkyboxColorPreset_01".Translate(),
        SkyboxColorPreset.Charcoal => "Astra_SkyboxColorPreset_02".Translate(),
        SkyboxColorPreset.MidnightViolet => "Astra_SkyboxColorPreset_03".Translate(),
        SkyboxColorPreset.Neutral => "Astra_SkyboxColorPreset_04".Translate(),
        SkyboxColorPreset.Black => "Astra_SkyboxColorPreset_05".Translate(),
        _ => preset.ToString()
      };
    }
    
    public static bool Apply(SkyboxColorPreset preset)
    {
      var camera = WorldCameraManager.WorldSkyboxCamera;
      
      if (camera == null)
        return false;
      
      camera.backgroundColor = GetColor(preset);
      
      return true;
    }
  }
}
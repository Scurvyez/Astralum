using System.Collections.Generic;
using Astralum.Astronomy.SkyGrid;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_SkyGridLabels : WorldComponent
  {
    private static List<SkyGridLabel> _cachedLabels;
    
    public WorldComponent_SkyGridLabels(RimWorld.Planet.World world) : base(world)
    {
    }
    
    public override void WorldComponentOnGUI()
    {
      if (!WorldUtils.ShouldDrawGUI() || !SkyGridSettings.DrawGrid)
        return;

      Camera skyboxCamera = WorldCameraManager.WorldSkyboxCamera;

      if (skyboxCamera == null)
        return;

      _cachedLabels ??= SkyGridLabelUtil.BuildLabels();

      Text.Font = GameFont.Tiny;
      Text.Anchor = TextAnchor.MiddleCenter;

      foreach (SkyGridLabel label in _cachedLabels)
        DrawLabel(skyboxCamera, label);

      Text.Anchor = TextAnchor.UpperLeft;
      GUI.color = Color.white;
    }

    private static void DrawLabel(Camera skyboxCamera, SkyGridLabel label)
    {
      Vector3 worldPos =
        skyboxCamera.transform.position +
        WorldUtils.GetCurrentRotationForWorldSpace() * label.localSkyPos;

      Vector3 screen = skyboxCamera.WorldToScreenPoint(worldPos);

      if (screen.z <= 0f)
        return;

      Vector2 guiPos = GUIUtility.ScreenToGUIPoint(
        new Vector2(screen.x, Screen.height - screen.y)
      ) + label.guiOffset;

      if (WorldUtils.GuiPointIsOverPlanetDisk(guiPos, label.guiOffset))
        return;

      GUI.color = new Color(0.65f, 0.85f, 1f, 0.85f);

      Vector2 size = Text.CalcSize(label.text) * label.scale;
      Rect rect = new(guiPos.x - size.x * 0.5f, guiPos.y - size.y * 0.5f, size.x, size.y);
      Matrix4x4 oldMatrix = GUI.matrix;

      GUIUtility.ScaleAroundPivot(new Vector2(label.scale, label.scale), guiPos);

      Widgets.Label(rect, label.text);
      GUI.matrix = oldMatrix;
    }
    
    public static void ClearCache()
    {
      _cachedLabels = null;
    }
  }
}
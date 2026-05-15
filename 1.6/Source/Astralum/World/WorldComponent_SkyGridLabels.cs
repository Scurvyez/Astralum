using System.Collections.Generic;
using Astralum.Astronomy.SkyGrid;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.World
{
    public class WorldComponent_SkyGridLabels : WorldComponent
    {
        private static List<SkyGridLabel> _cachedLabels;
        
        private const float PlanetRadius = 100f;

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
                GetCurrentGridRotation() * label.localSkyPos;
            
            Vector3 screen = skyboxCamera.WorldToScreenPoint(worldPos);
            
            if (screen.z <= 0f)
                return;
            
            Vector2 guiPos = GUIUtility.ScreenToGUIPoint(
                new Vector2(screen.x, Screen.height - screen.y)
            ) + label.guiOffset;
            
            if (GuiPointIsOverPlanetDisk(guiPos, label.guiOffset))
                return;
            
            GUI.color = new Color(0.65f, 0.85f, 1f, 0.85f);

            Vector2 size = Text.CalcSize(label.text) * label.scale;
            
            Rect rect = new(
                guiPos.x - size.x * 0.5f,
                guiPos.y - size.y * 0.5f,
                size.x,
                size.y
            );
            
            Matrix4x4 oldMatrix = GUI.matrix;
            
            GUIUtility.ScaleAroundPivot(
                new Vector2(label.scale, label.scale),
                guiPos
            );
            
            Rect unscaledRect = new(
                guiPos.x - size.x * 0.5f / label.scale,
                guiPos.y - size.y * 0.5f / label.scale,
                size.x / label.scale,
                size.y / label.scale
            );
            
            Widgets.Label(rect, label.text);
            GUI.matrix = oldMatrix;
        }
        
        private static Quaternion GetCurrentGridRotation()
        {
            if (Current.ProgramState == ProgramState.Entry)
                return Quaternion.identity;
            
            return Quaternion.LookRotation(GenCelestial.CurSunPositionInWorldSpace());
        }
        
        public static void ClearCache()
        {
            _cachedLabels = null;
        }
        
        private static bool GuiPointIsOverPlanetDisk(Vector2 guiPos, Vector2 guiOffset)
        {
            Camera worldCamera = Find.WorldCamera;
            
            if (worldCamera == null)
                return false;
            
            Vector3 planetCenter = Vector3.zero;
            Vector3 planetEdge = planetCenter + worldCamera.transform.right * PlanetRadius;
            
            Vector3 centerScreen = worldCamera.WorldToScreenPoint(planetCenter);
            Vector3 edgeScreen = worldCamera.WorldToScreenPoint(planetEdge);
            
            Vector2 centerGui = GUIUtility.ScreenToGUIPoint(
                new Vector2(centerScreen.x, Screen.height - centerScreen.y)
            );
            
            Vector2 edgeGui = GUIUtility.ScreenToGUIPoint(
                new Vector2(edgeScreen.x, Screen.height - edgeScreen.y)
            );
            
            float planetScreenRadius = Vector2.Distance(centerGui, edgeGui);
            
            return Vector2.Distance(guiPos - guiOffset, centerGui) <= planetScreenRadius;
        }
    }
}
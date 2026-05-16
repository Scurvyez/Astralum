using System.Collections.Generic;
using Astralum.Astronomy.Constellations;
using Astralum.Materials;
using Astralum.UI;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_ConstellationHover : WorldComponent
  {
    private const float DistanceToConstellations = 20f;

    private const float PlanetRadius = 100f;
    private const float HoverRadiusMultiplier = 0.65f;
    private const float MinHoverRadius = 0.25f;
    private const float TooltipPaddingX = 12f;
    private const float TooltipPaddingY = 8f;
    private const float TooltipMinWidth = 48f;
    private const float TooltipMinHeight = 24f;
    private const float TooltipMousePosOffsetX = 32f;
    private const float TooltipMousePosOffsetY = 16f;

    public WorldComponent_ConstellationHover(RimWorld.Planet.World world) : base(world)
    {
    }

    public override void WorldComponentOnGUI()
    {
      if (!WorldUtils.ShouldDrawGUI() || !ConstellationSettings.DrawConstellationLines)
        return;

      Camera skyboxCamera = WorldCameraManager.WorldSkyboxCamera;

      if (skyboxCamera == null)
        return;

      if (ConstellationInteractionRegistry.HoverStars.Count == 0)
        return;

      Ray ray = skyboxCamera.ScreenPointToRay(Verse.UI.MousePositionOnUI * Prefs.UIScale);

      if (!TryRaySphereIntersection(
            ray,
            skyboxCamera.transform.position,
            DistanceToConstellations,
            out Vector3 hitWorld))
        return;

      Vector3 localHit =
        Quaternion.Inverse(GetCurrentConstellationRotation()) *
        (hitWorld - skyboxCamera.transform.position);

      ConstellationInteractionRegistry.HoverStar? hovered = null;
      float bestDist = float.MaxValue;

      foreach (ConstellationInteractionRegistry.HoverStar star
               in ConstellationInteractionRegistry.HoverStars)
      {
        float hoverRadius = Mathf.Max(
          MinHoverRadius,
          star.radius * HoverRadiusMultiplier
        );

        float dist = Vector3.Distance(localHit, star.localSkyPos);

        if (dist > hoverRadius || dist >= bestDist)
          continue;

        bestDist = dist;
        hovered = star;
      }

      if (hovered == null || MouseIsOverPlanetDisk())
      {
        ConstellationHoverState.Clear();
        return;
      }

      ConstellationHoverState.SetHovered(hovered);

      ConstellationHoverMatsUtil.Ring.SetFloat(
        InternalShaderPropertyIds.PulseTime, ConstellationHoverState.PulseTime);

      DrawTooltip(ConstellationHoverInfoLineCache.GetLines(hovered.Value));
    }

    private static void DrawTooltip(List<ConstellationHoverInfoLine> lines)
    {
      if (lines.NullOrEmpty())
        return;

      Vector2 mousePos = Event.current.mousePosition;
      Vector2 tooltipPos = mousePos + new Vector2(TooltipMousePosOffsetX, TooltipMousePosOffsetY);

      Text.Font = GameFont.Small;
      Text.Anchor = TextAnchor.UpperLeft;

      float lineHeight = Text.LineHeight;
      float width = TooltipMinWidth;

      for (int i = 0; i < lines.Count; i++)
      {
        Vector2 textSize = Text.CalcSize(lines[i].Text);
        width = Mathf.Max(width, textSize.x + TooltipPaddingX * 2f);
      }

      float height = Mathf.Max(
        TooltipMinHeight,
        lines.Count * lineHeight + TooltipPaddingY * 2f
      );

      Rect rect = new(
        tooltipPos.x,
        tooltipPos.y,
        width,
        height
      );

      Widgets.DrawMenuSection(rect);

      float y = rect.y + TooltipPaddingY;

      for (int i = 0; i < lines.Count; i++)
      {
        Rect lineRect = new(
          rect.x + TooltipPaddingX,
          y,
          rect.width - TooltipPaddingX * 2f,
          lineHeight
        );

        DrawTooltipLine(lineRect, lines[i]);

        y += lineHeight;
      }

      Text.Anchor = TextAnchor.UpperLeft;
      GUI.color = Color.white;
    }

    private static void DrawTooltipLine(Rect rect, ConstellationHoverInfoLine line)
    {
      if (line.SwatchColor == null)
      {
        Widgets.Label(rect, line.Text);
        return;
      }

      const float swatchSize = 12f;
      const float swatchGap = 6f;

      Rect labelRect = new(
        rect.x,
        rect.y,
        rect.width - swatchSize - swatchGap,
        rect.height
      );

      Rect swatchRect = new(
        rect.xMax - swatchSize,
        rect.y + (rect.height - swatchSize) / 2f,
        swatchSize,
        swatchSize
      );

      Widgets.Label(labelRect, line.Text);

      Widgets.DrawBoxSolid(swatchRect, line.SwatchColor.Value);
      Widgets.DrawBox(swatchRect);
    }

    private static bool TryRaySphereIntersection(Ray ray, Vector3 sphereCenter, float radius, out Vector3 hit)
    {
      Vector3 oc = ray.origin - sphereCenter;

      float a = Vector3.Dot(ray.direction, ray.direction);
      float b = 2f * Vector3.Dot(oc, ray.direction);
      float c = Vector3.Dot(oc, oc) - radius * radius;

      float discriminant = b * b - 4f * a * c;

      if (discriminant < 0f)
      {
        hit = default;
        return false;
      }

      float sqrt = Mathf.Sqrt(discriminant);

      float t0 = (-b - sqrt) / (2f * a);
      float t1 = (-b + sqrt) / (2f * a);

      float t = t0 > 0f ? t0 : t1;

      if (t <= 0f)
      {
        hit = default;
        return false;
      }

      hit = ray.origin + ray.direction * t;
      return true;
    }

    private static bool MouseIsOverPlanetDisk()
    {
      Camera worldCamera = Find.WorldCamera;

      if (worldCamera == null)
        return false;

      Vector2 mousePos = Event.current.mousePosition;

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

      return Vector2.Distance(mousePos, centerGui) <= planetScreenRadius;
    }

    private static Quaternion GetCurrentConstellationRotation()
    {
      if (Current.ProgramState == ProgramState.Entry)
        return Quaternion.identity;

      return Quaternion.LookRotation(GenCelestial.CurSunPositionInWorldSpace());
    }
  }
}
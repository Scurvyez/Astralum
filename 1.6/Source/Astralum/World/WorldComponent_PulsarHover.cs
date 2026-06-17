using System.Collections.Generic;
using Astralum.Astronomy.Pulsars;
using Astralum.UI;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_PulsarHover : WorldComponent
  {
    private const float DistanceToPulsars = 20f;
    private const float HoverRadiusMultiplier = 0.35f;
    private const float MinHoverRadius = 0.35f;

    private const float TooltipPaddingX = 12f;
    private const float TooltipPaddingY = 8f;
    private const float TooltipMinWidth = 48f;
    private const float TooltipMinHeight = 24f;
    private const float TooltipMousePosOffsetX = 32f;
    private const float TooltipMousePosOffsetY = 16f;

    public WorldComponent_PulsarHover(RimWorld.Planet.World world) : base(world)
    {
    }

    public override void WorldComponentOnGUI()
    {
      if (!WorldUtils.ShouldDrawGUI() || !PulsarSettings.DrawPulsarInfo)
        return;

      Camera skyboxCamera = WorldCameraManager.WorldSkyboxCamera;

      if (skyboxCamera == null)
        return;

      if (PulsarInteractionRegistry.HoverPulsars.Count == 0)
        return;

      Ray ray = skyboxCamera.ScreenPointToRay(Verse.UI.MousePositionOnUI * Prefs.UIScale);

      if (!TryRaySphereIntersection(
            ray,
            skyboxCamera.transform.position,
            DistanceToPulsars,
            out Vector3 hitWorld))
        return;

      Vector3 localHit =
        Quaternion.Inverse(WorldUtils.GetCurrentRotationForWorldSpace()) *
        (hitWorld - skyboxCamera.transform.position);

      PulsarInteractionRegistry.HoverPulsar? hovered = null;
      float bestDist = float.MaxValue;

      foreach (PulsarInteractionRegistry.HoverPulsar pulsar in PulsarInteractionRegistry.HoverPulsars)
      {
        float hoverRadius = Mathf.Max(
          MinHoverRadius,
          pulsar.size * HoverRadiusMultiplier
        );

        float dist = Vector3.Distance(localHit, pulsar.localSkyPos);

        if (dist > hoverRadius || dist >= bestDist)
          continue;

        bestDist = dist;
        hovered = pulsar;
      }

      if (hovered == null || WorldUtils.MouseIsOverPlanetDisk())
        return;

      DrawTooltip(PulsarHoverInfoLineCache.GetLines(hovered.Value));
    }

    private static void DrawTooltip(List<PulsarHoverInfoLine> lines)
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

      Rect rect = new(tooltipPos.x, tooltipPos.y, width, height);

      Widgets.DrawMenuSection(rect);
      Widgets.DrawWindowBackground(rect);

      float y = rect.y + TooltipPaddingY;

      for (int i = 0; i < lines.Count; i++)
      {
        Rect lineRect = new(
          rect.x + TooltipPaddingX,
          y,
          rect.width - TooltipPaddingX * 2f,
          lineHeight
        );

        Widgets.Label(lineRect, lines[i].Text);
        y += lineHeight;
      }

      Text.Anchor = TextAnchor.UpperLeft;
      GUI.color = Color.white;
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
  }
}
using System.Collections.Generic;
using Astralum.API;
using Astralum.Astronomy;
using Astralum.UI;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_CelestialObjectHover : WorldComponent
  {
    private const float DistanceToCelestialObjects = 20f;
    private const float TooltipPaddingX = 12f;
    private const float TooltipPaddingY = 8f;
    private const float TooltipMinWidth = 48f;
    private const float TooltipMinHeight = 24f;
    private const float TooltipMousePosOffsetX = 32f;
    private const float TooltipMousePosOffsetY = 16f;
    
    private readonly List<HoveredObject> _hoveredObjects = [];
    
    public WorldComponent_CelestialObjectHover(RimWorld.Planet.World world) : base(world)
    {
      
    }
    
    public override void WorldComponentOnGUI()
    {
      if (!WorldUtils.ShouldDrawGUI())
      {
        ClearHoverState();
        return;
      }
      
      Camera skyboxCamera = WorldCameraManager.WorldSkyboxCamera;
      
      if (skyboxCamera == null)
      {
        ClearHoverState();
        return;
      }
      
      IReadOnlyList<CelestialObjectInteractionRegistry.HoverCelestialObject> objects =
        CelestialObjectInteractionRegistry.HoverObjects;
      
      if (objects.Count == 0)
      {
        ClearHoverState();
        return;
      }
      
      Ray ray = skyboxCamera.ScreenPointToRay(Verse.UI.MousePositionOnUI * Prefs.UIScale);
      
      if (!WorldUtils.TryRaySphereIntersectionForHoverInfo(ray, skyboxCamera.transform.position,
            DistanceToCelestialObjects, out Vector3 hitWorld))
      {
        ClearHoverState();
        return;
      }
      
      Vector3 localHit = Quaternion.Inverse(WorldUtils.GetCurrentRotationForWorldSpace()) 
                         * (hitWorld - skyboxCamera.transform.position);
      
      FindHoveredObjects(objects, localHit);
      
      if (_hoveredObjects.Count == 0 || WorldUtils.MouseIsOverPlanetDisk())
      {
        ClearHoverState();
        return;
      }
      
      _hoveredObjects.Sort(static (a, b) => a.distance.CompareTo(b.distance));
      
      DrawTooltip();
    }
    
    private void FindHoveredObjects(IReadOnlyList<CelestialObjectInteractionRegistry.HoverCelestialObject> objects,
      Vector3 localHit)
    {
      _hoveredObjects.Clear();
      
      for (int i = 0; i < objects.Count; i++)
      {
        CelestialObjectInteractionRegistry.HoverCelestialObject obj = objects[i];
        
        if (!ShouldShowInfo(obj.type))
          continue;
        
        float hoverRadius = Mathf.Max(MinHoverRadiusFor(obj.type), obj.size * HoverRadiusMultiplierFor(obj.type));
        float distance = Vector3.Distance(localHit, obj.localSkyPos);
        
        if (distance > hoverRadius)
          continue;
        
        _hoveredObjects.Add(new HoveredObject(obj, distance));
      }
    }
    
    private static bool ShouldShowInfo(CelestialObjectType type)
    {
      return type switch
      {
        CelestialObjectType.LocalStar => CelestialSettings.ShowLocalStarInfo,
        CelestialObjectType.BlackHole => CelestialSettings.DrawBlackHoleInfo,
        CelestialObjectType.Pulsar => CelestialSettings.DrawPulsarInfo,
        CelestialObjectType.ConstellationStar => CelestialSettings.DrawConstellationLines,
        _ => false
      };
    }
    
    private static float HoverRadiusMultiplierFor(CelestialObjectType type)
    {
      return type switch
      {
        CelestialObjectType.LocalStar => 0.1f,
        CelestialObjectType.BlackHole => 0.2f,
        CelestialObjectType.Pulsar => 0.35f,
        CelestialObjectType.ConstellationStar => 0.35f,
        _ => 0.25f
      };
    }
    
    private static float MinHoverRadiusFor(CelestialObjectType type)
    {
      return type switch
      {
        CelestialObjectType.LocalStar => 0.05f,
        CelestialObjectType.BlackHole => 0.25f,
        CelestialObjectType.Pulsar => 0.3f,
        CelestialObjectType.ConstellationStar => 0.3f,
        _ => 0.2f
      };
    }
    
    private void DrawTooltip()
    {
      List<CelestialObjectHoverInfoLine> lines = [];
      
      for (int i = 0; i < _hoveredObjects.Count; i++)
      {
        CelestialObjectInteractionRegistry.HoverCelestialObject obj = _hoveredObjects[i].hoverObject;
        AddHoverLines(obj, lines);
      }
      
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
      
      float height = Mathf.Max(TooltipMinHeight, lines.Count * lineHeight + TooltipPaddingY * 2f);
      Rect rect = new(tooltipPos.x, tooltipPos.y, width, height);
      Widgets.DrawMenuSection(rect);
      float y = rect.y + TooltipPaddingY;
      
      for (int i = 0; i < lines.Count; i++)
      {
        Rect lineRect = new(rect.x + TooltipPaddingX, y, rect.width - TooltipPaddingX * 2f, lineHeight);
        DrawTooltipLine(lineRect, lines[i]);
        y += lineHeight;
      }
      
      Text.Anchor = TextAnchor.UpperLeft;
      GUI.color = Color.white;
    }
    
    private static void AddHoverLines(CelestialObjectInteractionRegistry.HoverCelestialObject obj,
      List<CelestialObjectHoverInfoLine> lines)
    {
      List<CelestialObjectHoverInfoLine> objectLines = CelestialObjectHoverInfoLineCache.GetLines(obj);
      
      if (objectLines.NullOrEmpty())
        return;
      
      if (lines.Count > 0)
        lines.Add(new CelestialObjectHoverInfoLine(string.Empty));
      
      lines.AddRange(objectLines);
    }
    
    private static void DrawTooltipLine(Rect rect, CelestialObjectHoverInfoLine line)
    {
      if (line.SwatchColor == null)
      {
        Widgets.Label(rect, line.Text);
        return;
      }
      
      const float swatchSize = 12f;
      const float swatchGap = 6f;
      
      Rect labelRect = new(rect.x, rect.y, rect.width - swatchSize - swatchGap, rect.height);
      Rect swatchRect = new(rect.xMax - swatchSize, rect.y + (rect.height - swatchSize) 
        / 2f, swatchSize, swatchSize);
      
      Widgets.Label(labelRect, line.Text);
      Widgets.DrawBoxSolid(swatchRect, line.SwatchColor.Value);
      Widgets.DrawBox(swatchRect);
    }
    
    private void ClearHoverState()
    {
      _hoveredObjects.Clear();
    }
    
    private readonly struct HoveredObject
    {
      public readonly CelestialObjectInteractionRegistry.HoverCelestialObject hoverObject;
      public readonly float distance;
      
      public HoveredObject(CelestialObjectInteractionRegistry.HoverCelestialObject hoverObject, float distance)
      {
        this.hoverObject = hoverObject;
        this.distance = distance;
      }
    }
  }
}
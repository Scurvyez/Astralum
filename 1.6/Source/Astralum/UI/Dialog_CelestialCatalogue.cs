using System.Collections.Generic;
using Astralum.Astronomy;
using Astralum.Settings;
using Astralum.World;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.UI
{
  public class Dialog_CelestialCatalogue : Window
  {
    private const float WindowWidth = 420f;
    private const float WindowHeight = 560f;
    private const float Padding = 12f;
    private const float RowHeight = 28f;
    
    private const float CategorySpacing = 4f;
    private const float EntrySpacing = 2f;
    private const float CategoryBottomSpacing = 6f;
    
    private const float CategoryIndent = 16f;
    private const float ChildIndent = 40f;
    private const float ExpanderWidth = 28f;
    private const float ExpanderGap = 4f;
    
    private AstraSettings _settings;
    private Rect _lastSavedRect;
    private Vector2 _scrollPos;
    
    private readonly WorldComponent_CelestialObjectDataCache _dataCache;
    private readonly Dictionary<string, bool> _expandedByCategory = [];
    private readonly Dictionary<string, bool> _expandedConstellations = [];
    
    private List<CelestialCatalogueObjectEntry> _entries;
    private CelestialCatalogueObjectEntry? _selected;
    private string _nameBuffer = "";
    
    public override Vector2 InitialSize => new(WindowWidth, WindowHeight);
    
    public Dialog_CelestialCatalogue()
    {
      draggable = true;
      doCloseX = false;
      doCloseButton = false;
      closeOnCancel = false;
      closeOnClickedOutside = false;
      absorbInputAroundWindow = false;
      preventCameraMotion = false;
      drawShadow = true;
      resizeable = true;
      _dataCache = Find.World?.GetComponent<WorldComponent_CelestialObjectDataCache>();
    }
    
    public override void PreOpen()
    {
      base.PreOpen();
      
      _settings = AstraMod.Settings;
      
      windowRect = _settings is { HasCelestialCatalogueWindowRect: true }
        ? _settings.CelestialCatalogueWindowRect
        : new Rect(32f, 120f, WindowWidth, WindowHeight);
      
      _lastSavedRect = windowRect;
    }
    
    public override void PostClose()
    {
      base.PostClose();
      CelestialCatalogueCameraUtil.StopAnimation();
      CelestialCatalogueFocusVisualUtil.Clear();
    }

    public override void WindowUpdate()
    {
      base.WindowUpdate();
      
      if (WorldRendererUtility.CurrentWorldRenderMode != WorldRenderMode.Planet)
        Close();
    }

    public override void DoWindowContents(Rect inRect)
    {
      if (windowRect != _lastSavedRect)
      {
        SaveWindowState();
        _lastSavedRect = windowRect;
      }
      
      CelestialCatalogueCameraUtil.Update();
      
      _entries = CelestialCatalogueRegistry.BuildEntries();
      
      Text.Font = GameFont.Medium;
      Widgets.Label(new Rect(0f, 0f, inRect.width, 32f), 
        "Astra_UI_CelestialNames_Category".Translate() + $" {_dataCache.LocalStarSystem.systemName}");
      
      Text.Font = GameFont.Small;
      
      Rect listRect = new(0f, 38f, inRect.width, inRect.height - 158f);
      DrawObjectList(listRect);
      
      Rect editRect = new(0f, inRect.height - 112f, inRect.width, 112f);
      DrawRenameControls(editRect);
      
      GUI.color = Color.white;
      Text.Anchor = TextAnchor.UpperLeft;
    }
    
    private void DrawObjectList(Rect rect)
    {
      float viewHeight = CalculateViewHeight();
      Rect viewRect = new(0f, 0f, rect.width - 16f, viewHeight);
      
      Widgets.BeginScrollView(rect, ref _scrollPos, viewRect);
      
      float y = 0f;
      
      Dictionary<string, List<CelestialCatalogueObjectEntry>> byCategory = GroupByCategory(_entries);
      
      foreach (KeyValuePair<string, List<CelestialCatalogueObjectEntry>> pair in byCategory)
      {
        string category = pair.Key;
        List<CelestialCatalogueObjectEntry> entries = pair.Value;
        
        if (!_expandedByCategory.ContainsKey(category))
          _expandedByCategory[category] = false;
        
        Rect headerRect = new(0f, y, viewRect.width, RowHeight);
        
        if (Widgets.ButtonText(headerRect, 
              $"{(_expandedByCategory[category] ? "▼" : "▶")} {category} ({entries.Count})"))
          _expandedByCategory[category] = !_expandedByCategory[category];
        
        y += RowHeight + CategorySpacing;
        
        if (!_expandedByCategory[category])
          continue;
        
        for (int i = 0; i < entries.Count; i++)
        {
          CelestialCatalogueObjectEntry entry = entries[i];
          
          y = HasChildren(entry.Id)
            ? DrawParentEntry(viewRect, y, entry)
            : DrawNormalEntry(viewRect, y, entry);
        }

        y += CategoryBottomSpacing;
      }
      
      Widgets.EndScrollView();
    }
    
    private float DrawParentEntry(Rect viewRect, float y, CelestialCatalogueObjectEntry entry)
    {
      if (!_expandedConstellations.ContainsKey(entry.Id))
        _expandedConstellations[entry.Id] = false;
      
      bool expanded = _expandedConstellations[entry.Id];
      int childCount = CountChildren(entry.Id);

      Rect expanderRect = new(CategoryIndent, y, ExpanderWidth, RowHeight);
      Rect rowRect = new(expanderRect.xMax + ExpanderGap, y,
        viewRect.width - expanderRect.xMax - ExpanderGap, RowHeight);
      
      if (Widgets.ButtonText(expanderRect, expanded ? "▼" : "▶"))
      {
        _expandedConstellations[entry.Id] = !expanded;
      }
      
      if (IsSelected(entry))
        Widgets.DrawHighlight(rowRect);
      
      string label = BuildEntryLabel(entry);
      label += $" ({childCount})";
      
      if (Widgets.ButtonText(rowRect, label))
        Select(entry);
      
      y += RowHeight + EntrySpacing;
      
      if (!_expandedConstellations[entry.Id])
        return y;
      
      for (int i = 0; i < _entries.Count; i++)
      {
        CelestialCatalogueObjectEntry child = _entries[i];
        
        if (child.ParentId != entry.Id)
          continue;
        
        y = DrawChildEntry(viewRect, y, child);
        
      }
      
      y += EntrySpacing;
      return y;
    }
    
    private float DrawNormalEntry(Rect viewRect, float y, CelestialCatalogueObjectEntry entry)
    {
      Rect rowRect = new(CategoryIndent, y, viewRect.width - CategoryIndent, RowHeight);
      
      if (IsSelected(entry))
        Widgets.DrawHighlight(rowRect);
      
      if (Widgets.ButtonText(rowRect, BuildEntryLabel(entry)))
      {
        Select(entry);
      }
      
      return y + RowHeight + EntrySpacing;
    }
    
    private float DrawChildEntry(Rect viewRect, float y, CelestialCatalogueObjectEntry entry)
    {
      Rect rowRect = new(ChildIndent, y, viewRect.width - ChildIndent, RowHeight);
      
      if (IsSelected(entry))
        Widgets.DrawHighlight(rowRect);
      
      if (Widgets.ButtonText(rowRect, BuildEntryLabel(entry)))
      {
        Select(entry);
      }
      
      return y + RowHeight + EntrySpacing;
    }
    
    private static string BuildEntryLabel(CelestialCatalogueObjectEntry entry)
    {
      string label = entry.DisplayName.NullOrEmpty()
        ? "Astra_NameGenerator_Unknown".Translate() 
        : entry.DisplayName;
      
      if (entry.HasPlayerName)
        label += $"  ({entry.GeneratedName})";
      
      return label;
    }
    
    private bool IsSelected(CelestialCatalogueObjectEntry entry)
    {
      return _selected.HasValue && _selected.Value.Id == entry.Id 
                                && _selected.Value.CategoryLabel == entry.CategoryLabel;
    }
    
    private void DrawRenameControls(Rect rect)
    {
      Widgets.DrawMenuSection(rect);
      Rect inner = rect.ContractedBy(Padding);
      
      if (_selected?.Object == null)
      {
        Widgets.Label(inner, "Astra_UI_CelestialNames_SelectionText".Translate());
        return;
      }
      
      IPlayerNameableCelestialObject obj = _selected.Value.Object;
      Widgets.Label(new Rect(inner.x, inner.y, inner.width, 24f),
        "Astra_UI_CelestialNames_GeneratedName".Translate() + $" {obj.GeneratedName}");
      
      Rect fieldRect = new(inner.x, inner.y + 30f, inner.width, 28f);
      _nameBuffer = Widgets.TextField(fieldRect, _nameBuffer);
      
      float buttonY = inner.y + 68f;
      float buttonWidth = (inner.width - 12f) / 3f;
      
      if (Widgets.ButtonText(new Rect(inner.x, buttonY, buttonWidth, 28f),
            "Astra_UI_CelestialNames_Apply".Translate()))
      {
        PlayerNamedCelestialObjectUtil.TrySetPlayerName(obj, _nameBuffer);
        CelestialObjectHoverInfoLineCache.Clear();
        _nameBuffer = obj.DisplayName;
      }
      
      if (Widgets.ButtonText(new Rect(inner.x + buttonWidth + 6f, buttonY, buttonWidth, 28f),
            "Astra_UI_CelestialNames_Clear".Translate()))
      {
        PlayerNamedCelestialObjectUtil.ClearPlayerName(obj);
        CelestialObjectHoverInfoLineCache.Clear();
        _nameBuffer = obj.GeneratedName;
      }
      
      if (Widgets.ButtonText(new Rect(inner.x + (buttonWidth + 6f) * 2f, buttonY, buttonWidth, 28f),
            "Astra_UI_CelestialNames_View".Translate()))
      {
        CelestialCatalogueCameraUtil.FocusObject(
          _selected.Value.LocalSkyPos,
          windowRect
        );
      }
    }
    
    private void Select(CelestialCatalogueObjectEntry entry)
    {
      _selected = entry;

      _nameBuffer = entry.Object.PlayerSetName.NullOrEmpty()
          ? entry.Object.GeneratedName
          : entry.Object.PlayerSetName;
      
      CelestialCatalogueFocusVisualUtil.Focus(entry.Object);
      CelestialCatalogueCameraUtil.FocusObject(entry.LocalSkyPos, windowRect);
    }
    
    private float CalculateViewHeight()
    {
      Dictionary<string, List<CelestialCatalogueObjectEntry>> byCategory = GroupByCategory(_entries);
      float height = 0f;

      foreach (KeyValuePair<string, List<CelestialCatalogueObjectEntry>> pair in byCategory)
      {
        height += RowHeight + CategorySpacing;
        
        if (!_expandedByCategory.TryGetValue(pair.Key, out bool expanded) || !expanded)
          continue;
        
        List<CelestialCatalogueObjectEntry> entries = pair.Value;
        
        for (int i = 0; i < entries.Count; i++)
        {
          CelestialCatalogueObjectEntry entry = entries[i];
          height += RowHeight + EntrySpacing;
          
          if (!HasChildren(entry.Id))
            continue;
          
          if (!_expandedConstellations.TryGetValue(entry.Id, out bool constellationExpanded) || !constellationExpanded)
            continue;

          height += CountChildren(entry.Id) * (RowHeight + EntrySpacing);
          height += EntrySpacing;
        }
        
        height += CategoryBottomSpacing;
      }
      
      return Mathf.Max(height, 1f);
    }
    
    private static Dictionary<string, List<CelestialCatalogueObjectEntry>> GroupByCategory(
      List<CelestialCatalogueObjectEntry> entries)
    {
      Dictionary<string, List<CelestialCatalogueObjectEntry>> result = [];
      
      if (entries.NullOrEmpty())
        return result;
      
      for (int i = 0; i < entries.Count; i++)
      {
        CelestialCatalogueObjectEntry entry = entries[i];
        
        if (entry.HasParent)
          continue;
        
        if (!result.TryGetValue(entry.CategoryLabel, out List<CelestialCatalogueObjectEntry> list))
        {
          list = [];
          result[entry.CategoryLabel] = list;
        }
        
        list.Add(entry);
      }
      
      return result;
    }
    
    private bool HasChildren(string parentId)
    {
      if (parentId.NullOrEmpty() || _entries.NullOrEmpty())
      {
        return false;
      }
      
      for (int i = 0; i < _entries.Count; i++)
      {
        if (_entries[i].ParentId == parentId)
          return true;
      }
      
      return false;
    }
    
    private int CountChildren(string parentId)
    {
      if (parentId.NullOrEmpty() || _entries.NullOrEmpty())
      {
        return 0;
      }
      
      int count = 0;
      
      for (int i = 0; i < _entries.Count; i++)
      {
        if (_entries[i].ParentId == parentId)
          count++;
      }
      
      return count;
    }
    
    private void SaveWindowState()
    {
      AstraSettings settings = AstraMod.Settings;
      
      if (settings == null)
        return;
      
      settings.CelestialCatalogueWindowRect = windowRect;
      settings.HasCelestialCatalogueWindowRect = true;
      settings.Write();
    }
  }
}
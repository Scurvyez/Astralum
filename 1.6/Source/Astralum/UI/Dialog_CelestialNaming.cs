using System.Collections.Generic;
using Astralum.Astronomy;
using Astralum.Astronomy.BlackHoles;
using Astralum.Settings;
using UnityEngine;
using Verse;

namespace Astralum.UI
{
  public class Dialog_CelestialNaming : Window
  {
    private const float WindowWidth = 420f;
    private const float WindowHeight = 560f;
    private const float Padding = 12f;
    private const float RowHeight = 28f;

    private AstraSettings _settings;
    private Rect _lastSavedRect;
    private Vector2 _scrollPos;
    private readonly Dictionary<string, bool> _expandedByCategory = [];
    private List<CelestialNamingObjectEntry> _entries;
    private CelestialNamingObjectEntry? _selected;
    private string _nameBuffer = "";
    
    public override Vector2 InitialSize => new(WindowWidth, WindowHeight);
    
    public Dialog_CelestialNaming()
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
    }
    
    public override void PreOpen()
    {
      base.PreOpen();
      
      _settings = AstraMod.Settings;
      
      windowRect = _settings is { HasCelestialNamingWindowRect: true }
        ? _settings.CelestialNamingWindowRect
        : new Rect(32f, 120f, WindowWidth, WindowHeight);
      
      _lastSavedRect = windowRect;
    }
    
    public override void DoWindowContents(Rect inRect)
    {
      if (windowRect != _lastSavedRect)
      {
        SaveWindowState();
        _lastSavedRect = windowRect;
      }
      
      CelestialNamingCameraUtil.Update();
      
      _entries = CelestialNamingRegistry.BuildEntries();
      
      Text.Font = GameFont.Medium;
      Widgets.Label(new Rect(0f, 0f, inRect.width, 32f),
        "Astra_UI_CelestialNames_Category".Translate());
      
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
      
      Dictionary<string, List<CelestialNamingObjectEntry>> byCategory = GroupByCategory(_entries);
      
      foreach (KeyValuePair<string, List<CelestialNamingObjectEntry>> pair in byCategory)
      {
        string category = pair.Key;
        List<CelestialNamingObjectEntry> entries = pair.Value;
        
        if (!_expandedByCategory.ContainsKey(category))
          _expandedByCategory[category] = true;
        
        Rect headerRect = new(0f, y, viewRect.width, RowHeight);
        
        if (Widgets.ButtonText(headerRect, 
              $"{(_expandedByCategory[category] ? "▼" : "▶")} {category} ({entries.Count})"))
          _expandedByCategory[category] = !_expandedByCategory[category];
        
        y += RowHeight + 4f;
        
        if (!_expandedByCategory[category])
          continue;
        
        for (int i = 0; i < entries.Count; i++)
        {
          CelestialNamingObjectEntry entry = entries[i];
          
          Rect rowRect = new(16f, y, viewRect.width - 16f, RowHeight);
          
          bool selected = _selected.HasValue && _selected.Value.Id == entry.Id &&
                          _selected.Value.CategoryLabel == entry.CategoryLabel;
          
          if (selected)
            Widgets.DrawHighlight(rowRect);
          
          string label = entry.DisplayName.NullOrEmpty()
            ? "Astra_NameGenerator_Unknown".Translate()
            : entry.DisplayName;
          
          if (entry.HasPlayerName)
            label += $"  ({entry.GeneratedName})";
          
          if (Widgets.ButtonText(rowRect, label))
            Select(entry);
          
          y += RowHeight + 2f;
        }
        
        y += 6f;
      }
      
      Widgets.EndScrollView();
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
      
      IPlayerNamedCelestialObject obj = _selected.Value.Object;
      
      Widgets.Label(new Rect(inner.x, inner.y, inner.width, 24f),
        $"Astra_UI_CelestialNames_GeneratedName".Translate() + $" {obj.GeneratedName}");
      
      Rect fieldRect = new(inner.x, inner.y + 30f, inner.width, 28f);
      _nameBuffer = Widgets.TextField(fieldRect, _nameBuffer);
      
      float buttonY = inner.y + 68f;
      float buttonWidth = (inner.width - 12f) / 3f;
      
      if (Widgets.ButtonText(new Rect(inner.x, buttonY, buttonWidth, 28f), 
            "Astra_UI_CelestialNames_Apply".Translate()))
      {
        PlayerNamedCelestialObjectUtil.TrySetPlayerName(obj, _nameBuffer);
        
        // TODO: update later with a generic "CelestialNamingState.MarkDirty()" or something...
        BlackHoleInteractionRegistry.MarkDirty();
        BlackHoleInteractionRegistry.Clear();
        
        _nameBuffer = obj.DisplayName;
      }
      
      if (Widgets.ButtonText(new Rect(inner.x + buttonWidth + 6f, buttonY, buttonWidth, 28f),
            "Astra_UI_CelestialNames_Clear".Translate()))
      {
        PlayerNamedCelestialObjectUtil.ClearPlayerName(obj);
        _nameBuffer = obj.GeneratedName;
      }
      
      if (Widgets.ButtonText(new Rect(inner.x + (buttonWidth + 6f) * 2f, buttonY, buttonWidth, 28f),
            "Astra_UI_CelestialNames_View".Translate()))
      {
        CelestialNamingCameraUtil.FocusObject(_selected.Value.LocalSkyPos, windowRect);
      }
    }
    
    private void Select(CelestialNamingObjectEntry entry)
    {
      _selected = entry;
      _nameBuffer = entry.Object.PlayerSetName.NullOrEmpty()
        ? entry.Object.GeneratedName
        : entry.Object.PlayerSetName;
      
      CelestialNamingCameraUtil.FocusObject(entry.LocalSkyPos, windowRect);
    }
    
    private float CalculateViewHeight()
    {
      Dictionary<string, List<CelestialNamingObjectEntry>> byCategory = GroupByCategory(_entries);
      float height = 0f;
      
      foreach (KeyValuePair<string, List<CelestialNamingObjectEntry>> pair in byCategory)
      {
        height += RowHeight + 4f;
        
        if (_expandedByCategory.TryGetValue(pair.Key, out bool expanded) && expanded)
          height += pair.Value.Count * (RowHeight + 2f) + 6f;
      }
      
      return Mathf.Max(height, 1f);
    }
    
    private static Dictionary<string, List<CelestialNamingObjectEntry>> GroupByCategory(
      List<CelestialNamingObjectEntry> entries)
    {
      Dictionary<string, List<CelestialNamingObjectEntry>> result = [];
      
      if (entries.NullOrEmpty())
        return result;
      
      for (int i = 0; i < entries.Count; i++)
      {
        CelestialNamingObjectEntry entry = entries[i];
        
        if (!result.TryGetValue(entry.CategoryLabel, out List<CelestialNamingObjectEntry> list))
        {
          list = [];
          result[entry.CategoryLabel] = list;
        }
        
        list.Add(entry);
      }
      
      return result;
    }
    
    private void SaveWindowState()
    {
      AstraSettings settings = AstraMod.Settings;
      
      if (settings == null)
        return;
      
      settings.CelestialNamingWindowRect = windowRect;
      settings.HasCelestialNamingWindowRect = true;
      settings.Write();
    }
  }
}
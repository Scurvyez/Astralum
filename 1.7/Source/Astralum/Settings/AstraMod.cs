using System;
using System.Collections.Generic;
using Astralum.Debugging;
using Astralum.UI;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Settings
{
  public class AstraMod : Mod
  {
    private AstraSettings _settings;
    private float _halfWidth;
    private Vector2 _leftScrollPos = Vector2.zero;
    private Vector2 _rightScrollPos = Vector2.zero;
    
    private const int RowPadding = 30;
    private const float MainListingGap = 60f;
    private const float NewSectionGap = 12f;
    private const float SectionTitleGap = 6f;
    private const float Spacing = 2f;
    private const float SliderHeight = 30f;
    
    private const float MinCameraPanDuration = 0.1f;
    private const float MaxCameraPanDuration = 1.5f;
    private const float CameraPanDurationStep = 0.05f;
    
    public static AstraSettings Settings { get; private set; }
    
    public override string SettingsCategory() => "Astra_ModName".Translate();
    
    public AstraMod(ModContentPack content) : base(content)
    {
      _settings = GetSettings<AstraSettings>();
      Settings = _settings;
    }
    
    public override void DoSettingsWindowContents(Rect inRect)
    {
      _halfWidth = (inRect.width - RowPadding) / 2f;
      
      Rect leftRect = new(inRect.x, inRect.y, _halfWidth, inRect.height);
      Rect rightRect = new(inRect.x + _halfWidth + RowPadding, inRect.y, _halfWidth, inRect.height);
      
      LeftSideScrollViewHandler(leftRect);
      RightSideScrollViewHandler(rightRect);
    }
    
    private void LeftSideScrollViewHandler(Rect inRect)
    {
      Listing_Standard leftList = new();
      leftList.Gap(MainListingGap);
      Rect viewRect1 = new(inRect.x, inRect.y, inRect.width, inRect.height);
      Rect vROffset1 = new(0, 0, inRect.width - 20, inRect.height);
      
      Widgets.BeginScrollView(viewRect1, ref _leftScrollPos, vROffset1);
      leftList.Begin(vROffset1);
      leftList.Gap(NewSectionGap);
      
      DoCelestialRenderingSettings(leftList);
      DoLocalStarSettings(leftList);
      DoLocalMapLightingSettings(leftList);
      
      leftList.End();
      Widgets.EndScrollView();
    }
    
    private void RightSideScrollViewHandler(Rect inRect)
    {
      Listing_Standard rightList = new();
      rightList.Gap(MainListingGap);
      
      Rect viewRect = new(inRect.x, inRect.y, inRect.width, inRect.height);
      Rect viewOffset = new(0f, 0f, inRect.width - 20f, inRect.height);
      
      Widgets.BeginScrollView(viewRect, ref _rightScrollPos, viewOffset);
      
      rightList.Begin(viewOffset);
      rightList.Gap(NewSectionGap);
      
      DoCameraSettings(rightList);
      DoSkyboxColorSettings(rightList);
      
      rightList.End();
      Widgets.EndScrollView();
    }
    
    private void DoCelestialRenderingSettings(Listing_Standard list)
    {
      TaggedString deterministic = "\n\n" + "Astra_Deterministic".Translate();
      
      DrawSectionTitle(list, "Astra_Settings_CelestialRendering");
      
      list.CheckboxLabeled("Astra_RenderBackgroundsStars".Translate(), 
        ref _settings._renderBackgroundsStars, 
        "Astra_RenderBackgroundsStarsDesc".Translate() + deterministic);
      list.Gap(Spacing);
      
      list.CheckboxLabeled("Astra_RenderNebulae".Translate(),
        ref _settings._renderNebulae,
        "Astra_RenderNebulaeDesc".Translate() + deterministic);
      list.Gap(Spacing);
      
      list.CheckboxLabeled("Astra_RenderConstellations".Translate(),
        ref _settings._renderConstellations, 
        "Astra_RenderConstellationsDesc".Translate() + deterministic);
      list.Gap(Spacing);
      
      list.CheckboxLabeled("Astra_RenderBlackholes".Translate(),
        ref _settings._renderBlackholes,
        "Astra_RenderBlackholesDesc".Translate() + deterministic);
      list.Gap(Spacing);
      
      list.CheckboxLabeled("Astra_RenderPulsars".Translate(),
        ref _settings._renderPulsars,
        "Astra_RenderPulsarsDesc".Translate() + deterministic);
      list.Gap(Spacing);
      
      list.CheckboxLabeled("Astra_RenderShootingStars".Translate(),
        ref _settings._renderShootingStars,
        "Astra_RenderShootingStarsDesc".Translate());
      list.Gap(Spacing);
      
      list.CheckboxLabeled("Astra_RenderWorldAuroras".Translate(),
        ref _settings._renderWorldAuroras,
        "Astra_RenderWorldAurorasDesc".Translate());
      list.Gap(Spacing);
    }
    
    private void DoLocalStarSettings(Listing_Standard list)
    {
      DrawSectionTitle(list, "Astra_Settings_LocalStars");

      list.CheckboxLabeled(
        "Astra_OverrideVanillaSun".Translate(),
        ref _settings._overrideVanillaSun,
        "Astra_OverrideVanillaSunDesc".Translate());
      list.SubLabel("Astra_GameRestartRequired".Translate(), 1f);
      list.Gap(Spacing);
    }
    
    private void DoLocalMapLightingSettings(Listing_Standard list)
    {
      DrawSectionTitle(list, "Astra_Settings_LocalMapLighting");

      list.CheckboxLabeled(
        "Astra_AllowLocalStarLighting".Translate(),
        ref _settings._useLocalStarLighting,
        "Astra_AllowLocalStarLightingDesc".Translate());
      list.Gap(Spacing);
      
      list.CheckboxLabeled(
        "Astra_AllowLocalStarLightForMapGlow".Translate(),
        ref _settings._useLocalStarLightForMapGlow,
        "Astra_AllowLocalStarLightForMapGlowDesc".Translate());
      list.Gap(Spacing);
    }
    
    private void DoCameraSettings(Listing_Standard list)
    {
      DrawSectionTitle(list, "Astra_Settings_Camera");
      
      list.Label("Astra_CameraPanEasing".Translate());
      
      Rect dropdownRect = list.GetRect(30f);
      
      Widgets.Dropdown(dropdownRect, _settings._cameraPanEasing,
        easing => easing,
        easing => GetCameraPanEasingOptions(), GetCameraPanEasingLabel(_settings._cameraPanEasing));
      
      TooltipHandler.TipRegion(dropdownRect, "Astra_CameraPanEasingDesc".Translate());
      list.Gap(NewSectionGap);
      
      Rect durationRect = list.GetRect(SliderHeight);
      
      string durationLabel =
        "Astra_CameraPanAnimationDuration".Translate() + $": {_settings._cameraPanAnimationDuration:F2}s";
      
      float duration = Widgets.HorizontalSlider(durationRect, _settings._cameraPanAnimationDuration,
        MinCameraPanDuration, MaxCameraPanDuration, false, durationLabel);
      
      _settings._cameraPanAnimationDuration = Mathf.Round(duration / CameraPanDurationStep) * CameraPanDurationStep;
      
      TooltipHandler.TipRegion(durationRect, "Astra_CameraPanAnimationDurationDesc".Translate());
      
      list.Gap(Spacing);
    }
    
    private void DoSkyboxColorSettings(Listing_Standard list)
    {
      DrawSectionTitle(list, "Astra_Settings_SkyboxColor");

      const float swatchSize = 32f;
      const float checkboxSize = 24f;
      const float optionWidth = 48f;
      const float optionSpacing = 8f;

      SkyboxColorPreset[] presets =
      [
        SkyboxColorPreset.Vanilla,
        SkyboxColorPreset.DeepSpace,
        SkyboxColorPreset.Charcoal,
        SkyboxColorPreset.MidnightViolet,
        SkyboxColorPreset.Neutral,
        SkyboxColorPreset.Black
      ];
      
      Rect rowRect = list.GetRect(checkboxSize + swatchSize + 8f);
      
      for (int i = 0; i < presets.Length; i++)
      {
        SkyboxColorPreset preset = presets[i];
        
        float x = rowRect.x + i * (optionWidth + optionSpacing);
        
        Rect checkboxRect = new(x + (optionWidth - checkboxSize) * 0.5f, rowRect.y, 
          checkboxSize, checkboxSize);
        Rect swatchRect = new(x + (optionWidth - swatchSize) * 0.5f, rowRect.y + checkboxSize + 8f, 
          swatchSize, swatchSize);
        
        bool selected = _settings._skyboxColor == preset;
        bool newSelected = selected;
        
        Widgets.Checkbox(checkboxRect.position, ref newSelected, checkboxSize);
        
        if (newSelected && !selected)
        {
          _settings._skyboxColor = preset;
          SkyboxColorUtil.Apply(preset);
        }
        
        Color swatchColor = SkyboxColorUtil.GetColor(preset);

        Widgets.DrawBoxSolid(swatchRect, swatchColor);
        Widgets.DrawBox(swatchRect);
        TooltipHandler.TipRegion(checkboxRect, SkyboxColorUtil.GetLabel(preset));
        TooltipHandler.TipRegion(swatchRect, SkyboxColorUtil.GetLabel(preset));
        
        if (Widgets.ButtonInvisible(swatchRect))
        {
          _settings._skyboxColor = preset;
          SkyboxColorUtil.Apply(preset);
        }
      }
      
      list.Gap(NewSectionGap);
    }
    
    private static void DrawSectionTitle(Listing_Standard list, string translationKey)
    {
      list.Gap(NewSectionGap);
      list.Label(translationKey.Translate().Colorize(AstraLog.MessageMsgCol));
      list.Gap(SectionTitleGap);
    }
    
    private IEnumerable<Widgets.DropdownMenuElement<CameraPanEasing>> GetCameraPanEasingOptions()
    {
      foreach (CameraPanEasing easing in Enum.GetValues(typeof(CameraPanEasing)))
      {
        CameraPanEasing capturedEasing = easing;
        
        yield return new Widgets.DropdownMenuElement<CameraPanEasing>
        {
          option = new FloatMenuOption(
            GetCameraPanEasingLabel(capturedEasing), () => _settings._cameraPanEasing = capturedEasing),
          
          payload = capturedEasing
        };
      }
    }
    
    private static string GetCameraPanEasingLabel(CameraPanEasing easing)
    {
      return easing switch
      {
        CameraPanEasing.Linear => "Astra_CameraPanEasing_Linear".Translate(),
        CameraPanEasing.Sine => "Astra_CameraPanEasing_Sine".Translate(),
        CameraPanEasing.Quadratic => "Astra_CameraPanEasing_Quadratic".Translate(),
        CameraPanEasing.Cubic => "Astra_CameraPanEasing_Cubic".Translate(),
        CameraPanEasing.Quartic => "Astra_CameraPanEasing_Quartic".Translate(),
        _ => easing.ToString()
      };
    }
  }
}
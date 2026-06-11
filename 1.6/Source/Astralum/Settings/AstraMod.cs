using Astralum.Debugging;
using UnityEngine;
using Verse;

namespace Astralum.Settings
{
  public class AstraMod : Mod
  {
    private AstraSettings _settings;
    private float _halfWidth;
    private Vector2 _leftScrollPos = Vector2.zero;

    private const int RowPadding = 30;
    private const float MainListingGap = 100f;
    private const float NewSectionGap = 10f;
    private const float SectionTitleGap = 6f;
    private const float Spacing = 2f;
    private const float SliderSpacing = 120f;
    private const float LabelWidth = 200f;
    private const float TextFieldWidth = 100f;
    private const float ElementHeight = 25f;
    
    public override string SettingsCategory() => "Astra_ModName".Translate();
    
    public AstraMod(ModContentPack content) : base(content)
    {
      _settings = GetSettings<AstraSettings>();
    }
    
    public override void DoSettingsWindowContents(Rect inRect)
    {
      _halfWidth = (inRect.width - RowPadding) / 2;
      LeftSideScrollViewHandler(new Rect(inRect.x, inRect.y, _halfWidth, inRect.height));
    }
    
    private void LeftSideScrollViewHandler(Rect inRect)
    {
      Listing_Standard list1 = new();
      list1.Gap(MainListingGap);
      Rect viewRect1 = new(inRect.x, inRect.y, inRect.width, inRect.height);
      Rect vROffset1 = new(0, 0, inRect.width - 20, inRect.height);
      
      Widgets.BeginScrollView(viewRect1, ref _leftScrollPos, vROffset1);
      list1.Begin(vROffset1);
      list1.Gap(NewSectionGap);
      
      DoGeneralToggles(list1);
      
      list1.End();
      Widgets.EndScrollView();
    }
    
    private void DoGeneralToggles(Listing_Standard list1)
    {
      list1.Label("General Toggles".Colorize(AstraLog.MessageMsgCol));
      list1.Gap(SectionTitleGap);
      
      list1.CheckboxLabeled("Astra_RenderAdditionalBackgroundsStars".Translate(), 
        ref _settings._renderAdditionalBackgroundsStars, 
        "Astra_RenderAdditionalBackgroundsStarsDesc".Translate() + "Astra_Deterministic".Translate());
      list1.Gap(Spacing);
      
      list1.CheckboxLabeled("Astra_RenderNebulae".Translate(),
        ref _settings._renderNebulae,
        "Astra_RenderNebulaeDesc".Translate() + "Astra_Deterministic".Translate());
      list1.Gap(Spacing);
      
      list1.CheckboxLabeled("Astra_RenderConstellations".Translate(),
        ref _settings._renderConstellations, 
        "Astra_RenderConstellationsDesc".Translate() + "Astra_Deterministic".Translate());
      list1.Gap(Spacing);
      
      list1.CheckboxLabeled("Astra_RenderBlackholes".Translate(),
        ref _settings._renderBlackholes,
        "Astra_RenderBlackholesDesc".Translate() + "Astra_Deterministic".Translate());
      list1.Gap(Spacing);
      
      list1.CheckboxLabeled("Astra_RenderPulsars".Translate(),
        ref _settings._renderPulsars,
        "Astra_RenderPulsarsDesc".Translate() + "Astra_Deterministic".Translate());
      list1.Gap(Spacing);
      
      list1.CheckboxLabeled("Astra_RenderShootingStars".Translate(),
        ref _settings._renderShootingStars,
        "Astra_RenderShootingStarsDesc".Translate());
      list1.Gap(Spacing);7
      
      list1.CheckboxLabeled("Astra_OverrideVanillaSun".Translate(),
        ref _settings._overrideVanillaSun, 
        "Astra_OverrideVanillaSunDesc".Translate() + "Astra_Deterministic".Translate());
      list1.SubLabel("Astra_GameRestartRequired".Translate(), 1f);
      list1.Gap(Spacing);
    }
  }
}
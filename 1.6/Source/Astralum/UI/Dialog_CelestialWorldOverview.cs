using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.UI
{
  public class Dialog_CelestialWorldOverview : Window
  {
    private readonly Rect _buttonRect;
    
    private const float ButtonWindowGap = 5f;
    private const float WindowWidth = 420f;
    private const float RowHeight = 24f;
    private const int RowCount = 5;
    private const float BottomAreaHeight = 100f;
    
    public override Vector2 InitialSize => new(WindowWidth, RowHeight * RowCount + BottomAreaHeight);
    
    public Dialog_CelestialWorldOverview(Rect buttonRect)
    {
      _buttonRect = buttonRect;
      
      doCloseButton = true;
      doCloseX = true;
      closeOnClickedOutside = true;
      absorbInputAroundWindow = true;
    }
    
    public override void DoWindowContents(Rect inRect)
    {
      Text.Font = GameFont.Small;
      Rect listingRect = new(inRect.x, inRect.y, inRect.width, RowHeight * RowCount);
      
      Listing_Standard listing = new();
      listing.Begin(listingRect);
      
      WorldComponent_CelestialObjectDataCache data = Find.World?
        .GetComponent<WorldComponent_CelestialObjectDataCache>();
      WorldComponent_ConstellationDataCache constellationData = Find.World?
        .GetComponent<WorldComponent_ConstellationDataCache>();

      int localStars = data?.LocalStars?.Count ?? 0;
      int constellations = constellationData?.Constellations?.Count ?? 0;
      int nebulae = data?.Nebulae?.Count ?? 0;
      int blackHoles = data?.BlackHoles?.Count ?? 0;
      int pulsars = data?.Pulsars?.Count ?? 0;

      listing.Label("Astra_UI_CelestialNamingLocalStarsCategory".Translate() + $": {localStars}");
      listing.Label("Astra_UI_CelestialNamingConstellationsCategory".Translate() + $": {constellations}");
      listing.Label("Astra_UI_CelestialNamingNebulaeCategory".Translate() + $": {nebulae}");
      listing.Label("Astra_UI_CelestialNamingBlackHolesCategory".Translate() + $": {blackHoles}");
      listing.Label("Astra_UI_CelestialNamingPulsarsCategory".Translate() + $": {pulsars}");

      listing.End();
    }
    
    protected override void SetInitialSizeAndPosition()
    {
      Vector2 size = InitialSize;
      
      windowRect = new Rect(_buttonRect.center.x - size.x * 0.5f, _buttonRect.yMin - size.y - ButtonWindowGap, 
        size.x, size.y);
    }
  }
}
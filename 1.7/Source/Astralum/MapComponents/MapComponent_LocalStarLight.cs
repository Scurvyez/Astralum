using Astralum.DefOfs;
using Astralum.Settings;
using RimWorld;
using Verse;

namespace Astralum.MapComponents
{
  public class MapComponent_LocalStarLight : MapComponent
  {
    private const int CheckInterval = 2000;
    
    public MapComponent_LocalStarLight(Map map) : base(map)
    {
    }

    public override void FinalizeInit()
    {
      base.FinalizeInit();
      
      UpdateCondition();
    }

    public override void MapComponentTick()
    {
      base.MapComponentTick();
      
      if (!map.IsHashIntervalTick(CheckInterval))
        return;
      
      UpdateCondition();
    }
    
    private void UpdateCondition()
    {
      bool conditionActive = map.gameConditionManager.ConditionIsActive(InternalDefOf.Astra_LocalStarLight);
      
      if (AstraSettings.UseLocalStarLighting && AstraSettings.OverrideVanillaSun)
      {
        if (!conditionActive)
          StartCondition();
      }
      else
      {
        if (conditionActive)
          EndCondition();
      }
    }
    
    private void StartCondition()
    {
      var condition = GameConditionMaker.MakeConditionPermanent(InternalDefOf.Astra_LocalStarLight);
      map.gameConditionManager.RegisterCondition(condition);
    }
    
    private void EndCondition()
    {
      var condition = map.gameConditionManager.GetActiveCondition(InternalDefOf.Astra_LocalStarLight);
      condition?.End();
    }
  }
}
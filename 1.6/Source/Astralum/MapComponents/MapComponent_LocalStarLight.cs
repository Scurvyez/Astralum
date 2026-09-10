using Astralum.DefOfs;
using RimWorld;
using Verse;

namespace Astralum.MapComponents
{
  public class MapComponent_LocalStarLight : MapComponent
  {
    public MapComponent_LocalStarLight(Map map) : base(map)
    {
      
    }

    public override void FinalizeInit()
    {
      base.FinalizeInit();
      EnsureCondition();
    }

    private void EnsureCondition()
    {
      if (map.gameConditionManager.ConditionIsActive(InternalDefOf.Astra_LocalStarLight))
        return;
      
      var condition = GameConditionMaker.MakeConditionPermanent(InternalDefOf.Astra_LocalStarLight);
      map.gameConditionManager.RegisterCondition(condition);
    }
  }
}
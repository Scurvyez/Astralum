using Astralum.API;
using Astralum.DefOfs;
using Astralum.Settings;
using Astralum.WorldComponents;
using RimWorld;
using Verse;

namespace Astralum.MapComponents
{
  public class MapComponent_LocalStarEclipseTracker : MapComponent
  {
    private const int CheckInterval = 2000;
    private const float MinimumConditionStrength = 0.075f; // 7.5% 
    
    public MapComponent_LocalStarEclipseTracker(Map map) : base(map)
    {
    }
    
    public override void MapComponentTick()
    {
      base.MapComponentTick();
      
      if (!map.IsHashIntervalTick(CheckInterval))
        return;
      
      if (!AstraSettings.UseLocalStarLighting || !AstraSettings.OverrideVanillaSun)
      {
        EndEclipse(false);
        return;
      }
      
      var dataCache = Find.World.GetComponent<WorldComponent_CelestialObjectDataCache>();
      
      if (dataCache == null || dataCache.LocalStars.Count < 2)
      {
        EndEclipse(false);
        return;
      }
      
      UpdateEclipse();
    }
    
    private void UpdateEclipse()
    {
      bool eclipsedDetected = CelestialLightGetter.TryGetLocalStarEclipse(map, out LocalStarEclipseInfo eclipse);
      bool significantEclipse = eclipsedDetected && eclipse.EclipseStrength > MinimumConditionStrength;
      bool conditionActive = map.gameConditionManager.ConditionIsActive(InternalDefOf.Astra_LocalStarEclipse);
      
      if (significantEclipse == conditionActive)
        return;
      
      if (significantEclipse)
        StartEclipse(eclipse);
      else 
        EndEclipse();
    }
    
    private void StartEclipse(LocalStarEclipseInfo eclipse)
    {
      if (map.gameConditionManager.ConditionIsActive(InternalDefOf.Astra_LocalStarEclipse))
        return;
      
      var condition = GameConditionMaker.MakeConditionPermanent(InternalDefOf.Astra_LocalStarEclipse);
      
      map.gameConditionManager.RegisterCondition(condition);
      
      string message = eclipse.IsTotalEclipse
          ? "Astra_GameCon_Eclipse_Total".Translate()
          : "Astra_GameCon_Eclipse".Translate();
      
      message += "\n" + "Astra_GameCon_Eclipse_LightObscured".Translate($"{eclipse.EclipseStrength:P0}");
      message += "\n" + "Astra_GameCon_Eclipse_StarsEclipsed".Translate(eclipse.EclipsedStarCount);
      
      for (int i = 0; i < eclipse.EclipsedStarCount; i++)
      {
        CelestialObjectInfo star = eclipse.EclipsedStars[i];
        message += $"\n  • {star.Name}";
      }
      
      Messages.Message(message, MessageTypeDefOf.NeutralEvent);
    }
    
    private void EndEclipse(bool sendMessage = true)
    {
      var condition = map.gameConditionManager.GetActiveCondition(InternalDefOf.Astra_LocalStarEclipse);
      
      if (condition == null)
        return;
      
      condition.End();
      
      if (sendMessage)
        Messages.Message("Astra_GameCon_Eclipse_Ended".Translate(), MessageTypeDefOf.NeutralEvent);
    }
  }
}
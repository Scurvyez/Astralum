using Astralum.Astronomy.Auroras;
using Astralum.Materials;
using RimWorld.Planet;
using Verse;

namespace Astralum.WorldComponents
{
  public class WorldComponent_AuroraRenderer : WorldComponent
  {
    private const int UpdateInterval = 5;
    
    public WorldComponent_AuroraRenderer(World world) : base(world)
    {
    }
    
    public override void WorldComponentTick()
    {
      base.WorldComponentTick();
      
      if (Find.TickManager.TicksGame % UpdateInterval != 0)
        return;
      
      AuroraMaterialState.Update(AuroraMatsUtil.AuroraMaterial);
    }
  }
}
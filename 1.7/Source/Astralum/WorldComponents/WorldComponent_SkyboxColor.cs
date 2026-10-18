using Astralum.Settings;
using RimWorld.Planet;

namespace Astralum.WorldComponents
{
  public class WorldComponent_SkyboxColor : WorldComponent
  {
    private SkyboxColorPreset? _lastAppliedPreset;
    
    public WorldComponent_SkyboxColor(World world) : base(world)
    {
    }
    
    public override void FinalizeInit(bool fromLoad)
    {
      base.FinalizeInit(fromLoad);
      
      ApplySkyboxColor();
    }
    
    public override void WorldComponentTick()
    {
      base.WorldComponentTick();
      
      if (_lastAppliedPreset == AstraSettings.SkyboxColor)
        return;
      
      ApplySkyboxColor();
    }
    
    private void ApplySkyboxColor()
    {
      SkyboxColorPreset preset = AstraSettings.SkyboxColor;

      if (SkyboxColorUtil.Apply(preset))
        _lastAppliedPreset = preset;
    }
  }
}
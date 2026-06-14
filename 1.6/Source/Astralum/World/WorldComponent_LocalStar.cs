using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.Materials;
using Astralum.UI;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.World
{
  public class WorldComponent_LocalStar : WorldComponent
  {
    private SavedStar _star;

    public WorldComponent_LocalStar(RimWorld.Planet.World world) : base(world)
    {
    }

    public SavedStar Star
    {
      get
      {
        EnsureStarExists();
        return _star;
      }
    }

    public override void FinalizeInit(bool fromLoad)
    {
      base.FinalizeInit(fromLoad);

      EnsureStarExists();
      LocalSystemStarMatsUtil.RefreshSun01Mat();
    }

    public override void ExposeData()
    {
      base.ExposeData();

      if (Scribe.mode == LoadSaveMode.Saving)
        EnsureStarExists();
      
      Scribe_Deep.Look(ref _star, "astralumStar");
      
      if (Scribe.mode == LoadSaveMode.PostLoadInit)
      {
        EnsureStarExists();

        StarInfoLineCache.Rebuild(_star);
        LocalSystemStarMatsUtil.RefreshSun01Mat();
        WorldComponent_Debug.ResetTweaksFromCurrentStar();
      }
    }

    private void EnsureStarExists()
    {
      if (_star != null)
        return;

      GenerateStar();
    }

    private void GenerateStar()
    {
      GeneratedStar generatedStar = StarGenerator.GenerateRandomStar();
      _star = new SavedStar(generatedStar);

      StarInfoLineCache.Rebuild(_star);
      LocalSystemStarMatsUtil.RefreshSun01Mat();
      WorldComponent_Debug.ResetTweaksFromCurrentStar();
    }
  }
}
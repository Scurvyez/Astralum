using Astralum.Astronomy.Stars;
using Astralum.Debugging;
using Astralum.Materials;
using RimWorld.Planet;
using Verse;

namespace Astralum.World
{
    public class AstralumWorldComponent : WorldComponent
    {
        private SavedStar _star;

        public SavedStar Star
        {
            get
            {
                EnsureStarExists();
                return _star;
            }
        }

        public AstralumWorldComponent(RimWorld.Planet.World world) : base(world)
        {
        }

        public override void FinalizeInit(bool fromLoad)
        {
            base.FinalizeInit(fromLoad);
            
            EnsureStarExists();
            MaterialsUtil.RefreshSun01Mat();
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
                MaterialsUtil.RefreshSun01Mat();
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

            AstraLog.Message(
                $"Generated world star: Class={_star.spectralClass}, Temp={_star.temperatureKelvin:0}K"
            );
        }
    }
}
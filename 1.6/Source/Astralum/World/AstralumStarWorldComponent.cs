using Astralum.Astronomy.Stars;
using Astralum.Materials;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.World
{
    public class AstralumStarWorldComponent : WorldComponent
    {
        private SavedStar _star;

        public float starInfoWindowXPos = -1f;
        public float starInfoWindowYPos = -1f;

        public SavedStar Star
        {
            get
            {
                EnsureStarExists();
                return _star;
            }
        }
        
        public bool HasSavedStarInfoWindowPos => starInfoWindowXPos >= 0f && starInfoWindowYPos >= 0f;
        
        public Vector2 StarInfoWindowPos
        {
            get => new(starInfoWindowXPos, starInfoWindowYPos);
            set
            {
                starInfoWindowXPos = value.x;
                starInfoWindowYPos = value.y;
            }
        }
        
        public AstralumStarWorldComponent(RimWorld.Planet.World world) : base(world)
        {
        }
        
        public override void FinalizeInit(bool fromLoad)
        {
            base.FinalizeInit(fromLoad);
            
            EnsureStarExists();
            StarsMaterialsUtil.RefreshSun01Mat();
        }
        
        public override void ExposeData()
        {
            base.ExposeData();

            if (Scribe.mode == LoadSaveMode.Saving)
                EnsureStarExists();

            Scribe_Deep.Look(ref _star, "astralumStar");
            
            Scribe_Values.Look(ref starInfoWindowXPos, "starInfoWindowXPos", -1f);
            Scribe_Values.Look(ref starInfoWindowYPos, "starInfoWindowYPos", -1f);
            
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                EnsureStarExists();
                
                UI.StarInfoLineCache.Rebuild(_star);
                StarsMaterialsUtil.RefreshSun01Mat();
                AstralumDebugWorldComponent.ResetTweaksFromCurrentStar();
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
            
            UI.StarInfoLineCache.Rebuild(_star);
            StarsMaterialsUtil.RefreshSun01Mat();
            AstralumDebugWorldComponent.ResetTweaksFromCurrentStar();
        }
    }
}
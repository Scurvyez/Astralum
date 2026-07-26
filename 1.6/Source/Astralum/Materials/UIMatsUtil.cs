using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  [StaticConstructorOnStartup]
  public static class UIMatsUtil
  {
    public static readonly Texture2D ShowBlackHoleInfoIcon = ContentFinder<Texture2D>.Get("UI/Icons/ShowBlackHoleInfo");
    public static readonly Texture2D ShowConstellationLinesIcon = ContentFinder<Texture2D>.Get("UI/Icons/ShowConstellationLines");
    public static readonly Texture2D ShowLocalStarInfoIcon = ContentFinder<Texture2D>.Get("UI/Icons/ShowLocalStarInfo");
    public static readonly Texture2D ShowPulsarInfoIcon = ContentFinder<Texture2D>.Get("UI/Icons/ShowPulsarInfo");
    public static readonly Texture2D ShowSkyGridIcon = ContentFinder<Texture2D>.Get("UI/Icons/ShowSkyGrid");
    public static readonly Texture2D ShowNamingDialogueIcon = ContentFinder<Texture2D>.Get("UI/Icons/ShowNamingDialogue");
  }
}
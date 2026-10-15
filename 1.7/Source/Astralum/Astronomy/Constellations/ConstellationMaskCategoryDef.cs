using System.Collections.Generic;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public class ConstellationMaskCategoryDef : Def
  {
    public string categoryId;
    public string folderPath;

    public override IEnumerable<string> ConfigErrors()
    {
      foreach (var error in base.ConfigErrors())
        yield return error;
      
      if (categoryId.NullOrEmpty())
        yield return "categoryId is required.";
      
      if (folderPath.NullOrEmpty())
        yield return "folderPath is required.";
    }
  }
}
using System.Collections.Generic;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public class ConstellationNameRulePackDef : Def
  {
    public bool isFallback;
    public string categoryId;
    public RulePackDef rulePackDef;
    
    public override IEnumerable<string> ConfigErrors()
    {
      foreach (string error in base.ConfigErrors())
        yield return error;
      
      if (categoryId.NullOrEmpty())
        yield return "categoryId is required.";
      
      if (rulePackDef == null)
        yield return "rulePackDef is required.";
    }
  }
}
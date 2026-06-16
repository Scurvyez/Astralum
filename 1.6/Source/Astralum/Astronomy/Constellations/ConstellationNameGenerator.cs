using System.Collections.Generic;
using Astralum.Astronomy.LocalSystem.Stars;
using Verse;
using Verse.Grammar;

namespace Astralum.Astronomy.Constellations
{
  public static class ConstellationNameGenerator
  {
    private static readonly Dictionary<string, RulePackDef> RulePacksByCategory = [];
    private static readonly RulePackDef FallbackRulePack;
    
    static ConstellationNameGenerator()
    {
      foreach (ConstellationNameRulePackDef def in DefDatabase<ConstellationNameRulePackDef>.AllDefs)
      {
        if (def.rulePackDef == null)
          continue;
        
        if (def.isFallback)
          FallbackRulePack = def.rulePackDef;
        
        if (!def.categoryId.NullOrEmpty())
          RulePacksByCategory[def.categoryId] = def.rulePackDef;
      }
    }
    
    public static string Generate(string categoryId, HashSet<string> usedNames)
    {
      return StellarNamingUtil.GenerateUniqueName(
        usedNames,
        () => GenerateRaw(categoryId)
      );
    }
    
    private static string GenerateRaw(string categoryId)
    {
      RulePackDef rulePack = GetRulePack(categoryId);
      
      if (rulePack == null)
        return StellarNamingUtil.GenerateSemiUniqueSystemName();
      
      GrammarRequest request = default;
      request.Rules.AddRange(rulePack.RulesPlusIncludes);
      
      string result = GrammarResolver.Resolve(
        "r_name",
        request,
        capitalizeFirstSentence: false
      );
      
      return result.NullOrEmpty()
        ? StellarNamingUtil.GenerateSemiUniqueSystemName()
        : result;
    }
    
    private static RulePackDef GetRulePack(string categoryId)
    {
      if (!categoryId.NullOrEmpty() && RulePacksByCategory.TryGetValue(categoryId, out RulePackDef rulePack))
      {
        return rulePack;
      }
      
      return FallbackRulePack;
    }
  }
}
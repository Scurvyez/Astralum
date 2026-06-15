using System.Collections.Generic;
using Astralum.Astronomy.LocalSystem.Stars;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public static class ConstellationNameGenerator
  {
    private static readonly Dictionary<string, ConstellationNameSetDef> SetsByCategory = [];
    private static readonly ConstellationNameSetDef _fallback;

    static ConstellationNameGenerator()
    {
      foreach (ConstellationNameSetDef def in DefDatabase<ConstellationNameSetDef>.AllDefs)
      {
        if (def.isFallback)
          _fallback = def;

        if (!def.categoryId.NullOrEmpty())
          SetsByCategory[def.categoryId] = def;
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
      ConstellationNameSetDef set = GetSet(categoryId);
      ConstellationNameSetDef generic = _fallback;

      if (set == null)
        return StellarNamingUtil.GenerateSemiUniqueSystemName();

      int pattern = Rand.RangeInclusive(0, 8);

      string the = "Astra_NameGenerator_The".Translate();
      string of =  "Astra_NameGenerator_Of".Translate();
      
      string descriptor = Pick(set.descriptors, generic?.descriptors);
      string title = Pick(set.titles, generic?.titles);
      string noun = Pick(set.nouns, generic?.nouns);
      string concept = Pick(set.concepts, generic?.concepts);
      string obj = Pick(set.objects, generic?.objects);
      
      return pattern switch
      {
        0 => $"{the} {descriptor} {noun}",
        1 => $"{the} {title}",
        2 => $"{the} {obj} {of} {concept}",
        3 => $"{the} {descriptor} {obj}",
        4 => $"{noun} {of} {concept}",
        5 => $"{the} {title}'s {obj}",
        6 => $"{the} {descriptor} {noun} {of} {concept}",
        7 => $"{the} {noun}",
        _ => $"{the} {concept} {obj}"
      };
    }

    private static ConstellationNameSetDef GetSet(string categoryId)
    {
      if (!categoryId.NullOrEmpty() &&
          SetsByCategory.TryGetValue(categoryId, out ConstellationNameSetDef set))
        return set;

      return _fallback;
    }

    private static string Pick(List<string> primary, List<string> generic = null, float primaryChance = 0.75f)
    {
      if (!primary.NullOrEmpty() && (generic.NullOrEmpty() || Rand.Value < primaryChance))
        return primary.RandomElement();

      if (!generic.NullOrEmpty())
        return generic.RandomElement();

      return !primary.NullOrEmpty()
        ? primary.RandomElement()
        : "Astra_NameGenerator_Unknown".Translate();
    }
  }
}
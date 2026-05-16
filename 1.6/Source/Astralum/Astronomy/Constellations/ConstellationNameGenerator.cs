using System.Collections.Generic;
using Astralum.Astronomy.LocalSystem.Stars;
using Verse;

namespace Astralum.Astronomy.Constellations
{
    public static class ConstellationNameGenerator
    {
        private static readonly Dictionary<string, ConstellationNameSetDef> SetsByCategory = [];
        private static ConstellationNameSetDef _fallback;
        
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
            
            string descriptor = Pick(set.descriptors, generic?.descriptors);
            string title = Pick(set.titles, generic?.titles);
            string noun = Pick(set.nouns, generic?.nouns);
            string concept = Pick(set.concepts, generic?.concepts);
            string obj = Pick(set.objects, generic?.objects);
            
            return pattern switch
            {
                0 => $"The {descriptor} {noun}",
                1 => $"The {title}",
                2 => $"The {obj} of {concept}",
                3 => $"The {descriptor} {obj}",
                4 => $"{noun} of {concept}",
                5 => $"The {title}'s {obj}",
                6 => $"The {descriptor} {noun} of {concept}",
                7 => $"The {noun}",
                _ => $"The {concept} {obj}"
            };
        }
        
        private static ConstellationNameSetDef GetSet(string categoryId)
        {
            if (!categoryId.NullOrEmpty() &&
                SetsByCategory.TryGetValue(categoryId, out ConstellationNameSetDef set))
            {
                return set;
            }
            
            return _fallback;
        }
        
        private static string Pick(List<string> primary, List<string> generic = null, float primaryChance = 0.75f)
        {
            if (!primary.NullOrEmpty() && (generic.NullOrEmpty() || Rand.Value < primaryChance))
                return primary.RandomElement();
            
            if (!generic.NullOrEmpty())
                return generic.RandomElement();
            
            if (!primary.NullOrEmpty())
                return primary.RandomElement();
            
            return "Unknown";
        }
    }
}
using System.Collections.Generic;
using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.DefOfs;
using Verse;

namespace Astralum.Astronomy.Constellations
{
    public static class ConstellationNameGenerator
    {
        private static ConstellationNameSetDef _nameSet;
        
        static ConstellationNameGenerator()
        {
            _nameSet = InternalDefOf.Astra_ConstellationNames;
        }
        
        public static string Generate(string maskName, HashSet<string> usedNames)
        {
            ConstellationNameCategory category = GetCategoryFromMaskName(maskName);
            
            return StellarNamingUtil.GenerateUniqueName(
                usedNames,
                () => GenerateRaw(category)
            );
        }
        
        private static string GenerateRaw(ConstellationNameCategory category)
        {
            ConstellationNameSetDef set = _nameSet;
            
            if (set == null)
                return StellarNamingUtil.GenerateSemiUniqueSystemName();
            
            ConstellationCategoryNameSet categorySet = set.GetCategorySet(category);
            
            if (categorySet == null)
                categorySet = set.GetCategorySet(ConstellationNameCategory.Generic);
            
            if (categorySet == null)
                return StellarNamingUtil.GenerateSemiUniqueSystemName();
            
            int pattern = Rand.RangeInclusive(0, 8);
            
            string descriptor = Pick(categorySet.descriptors, set.sharedDescriptors);
            string title = Pick(set.sharedTitles, categorySet.nouns);
            string noun = Pick(categorySet.nouns, set.sharedObjects);
            string concept = Pick(categorySet.concepts, set.sharedConcepts);
            string obj = Pick(categorySet.objects, set.sharedObjects);
            
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
        
        private static string Pick(List<string> primary, List<string> fallback)
        {
            if (!primary.NullOrEmpty() && Rand.Value < 0.75f)
                return primary.RandomElement();
            
            if (!fallback.NullOrEmpty())
                return fallback.RandomElement();
            
            return !primary.NullOrEmpty() 
                ? primary.RandomElement() 
                : "Unknown";
        }
        
        public static ConstellationNameCategory GetCategoryFromMaskName(string maskName)
        {
            if (maskName.NullOrEmpty())
                return ConstellationNameCategory.Generic;
            
            string lower = maskName.ToLowerInvariant();
            
            if (lower.Contains("abstract")) return ConstellationNameCategory.Abstract;
            if (lower.Contains("animals")) return ConstellationNameCategory.Animals;
            if (lower.Contains("archotech")) return ConstellationNameCategory.Archotech;
            if (lower.Contains("circles")) return ConstellationNameCategory.Circles;
            if (lower.Contains("geometric")) return ConstellationNameCategory.Geometric;
            if (lower.Contains("nature")) return ConstellationNameCategory.Nature;
            if (lower.Contains("pirate")) return ConstellationNameCategory.Pirate;
            if (lower.Contains("round")) return ConstellationNameCategory.Round;
            if (lower.Contains("spirals")) return ConstellationNameCategory.Spirals;
            
            return ConstellationNameCategory.Generic;
        }
    }
}
using System.Collections.Generic;
using Verse;

namespace Astralum.Astronomy.Constellations
{
    public class ConstellationNameSetDef : Def
    {
        public List<string> sharedDescriptors = [];
        public List<string> sharedTitles = [];
        public List<string> sharedObjects = [];
        public List<string> sharedConcepts = [];
        
        public List<ConstellationCategoryNameSet> categories = [];
        
        public ConstellationCategoryNameSet GetCategorySet(ConstellationNameCategory category)
        {
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].category == category)
                    return categories[i];
            }
            
            return null;
        }
    }
    
    public class ConstellationCategoryNameSet
    {
        public ConstellationNameCategory category;
        
        public List<string> nouns = [];
        public List<string> descriptors = [];
        public List<string> concepts = [];
        public List<string> objects = [];
    }
}
using System.Collections.Generic;
using Verse;

namespace Astralum.Astronomy.Constellations
{
    public class ConstellationNameSetDef : Def
    {
        public string categoryId;

        public List<string> descriptors = [];
        public List<string> titles = [];
        public List<string> nouns = [];
        public List<string> objects = [];
        public List<string> concepts = [];

        public bool isFallback;
    }
}
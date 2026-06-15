using System.Collections.Generic;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public class ConstellationNameSetDef : Def
  {
    public bool isFallback;
    public string categoryId;
    public List<string> descriptors = [];
    public List<string> titles = [];
    public List<string> nouns = [];
    public List<string> objects = [];
    public List<string> concepts = [];

    public override IEnumerable<string> ConfigErrors()
    {
      foreach (var error in base.ConfigErrors())
        yield return error;
      
      if (categoryId.NullOrEmpty())
        yield return "categoryId is required.";
      
      if (descriptors.NullOrEmpty())
        yield return "descriptors is required.";
      
      if (titles.NullOrEmpty())
        yield return "titles is required.";
      
      if (nouns.NullOrEmpty())
        yield return "nouns is required.";
      
      if (objects.NullOrEmpty())
        yield return "objects is required.";
      
      if (concepts.NullOrEmpty())
        yield return "concepts is required.";
    }
  }
}
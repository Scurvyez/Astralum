using System.Collections.Generic;

namespace Astralum.Astronomy.LocalSystem.Stars
{
  public readonly struct GeneratedStellarComposition
  {
    public readonly Dictionary<string, float> Elements;
    public readonly float Metallicity;

    public GeneratedStellarComposition(Dictionary<string, float> elements)
    {
      Elements = elements;
      Metallicity = CalculateMetallicity(elements);
    }

    private static float CalculateMetallicity(Dictionary<string, float> elements)
    {
      float metallicity = 0f;

      foreach (KeyValuePair<string, float> element in elements)
      {
        if (element.Key is "H" or "He")
          continue;

        metallicity += element.Value;
      }

      return metallicity;
    }
  }
}
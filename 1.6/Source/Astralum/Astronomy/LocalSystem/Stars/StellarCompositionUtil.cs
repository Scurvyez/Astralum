using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Astralum.Astronomy.LocalSystem.Stars
{
  public static class StellarCompositionUtil
  {
    public static GeneratedStellarComposition GenerateComposition(SpectralClass spectralClass)
    {
      Dictionary<string, float> elements = spectralClass switch
      {
        SpectralClass.O => new Dictionary<string, float>
        {
          { "H", Rand.Range(74f, 76f) },
          { "He", Rand.Range(24f, 26f) }
        },

        SpectralClass.B => new Dictionary<string, float>
        {
          { "H", Rand.Range(58f, 70f) },
          { "He", Rand.Range(28f, 42f) },
          { "C", Rand.Range(0.1f, 2f) },
          { "N", Rand.Range(0.1f, 2f) },
          { "O", Rand.Range(0.1f, 2f) }
        },

        SpectralClass.A => new Dictionary<string, float>
        {
          { "H", Rand.Range(71f, 74f) },
          { "He", Rand.Range(25f, 28f) },
          { "C", Rand.Range(0.1f, 2f) },
          { "N", Rand.Range(0.1f, 2f) },
          { "O", Rand.Range(0.1f, 2f) },
          { "Ne", Rand.Range(0.1f, 2f) }
        },

        SpectralClass.F => new Dictionary<string, float>
        {
          { "H", Rand.Range(54f, 64f) },
          { "He", Rand.Range(35f, 45f) },
          { "C", Rand.Range(0.1f, 2f) },
          { "N", Rand.Range(0.1f, 2f) },
          { "O", Rand.Range(0.1f, 2f) },
          { "Ne", Rand.Range(0.1f, 2f) },
          { "Fe", Rand.Range(0.1f, 2f) }
        },

        SpectralClass.G => new Dictionary<string, float>
        {
          { "H", Rand.Range(74f, 84f) },
          { "He", Rand.Range(14f, 24f) },
          { "C", Rand.Range(0.1f, 2f) },
          { "N", Rand.Range(0.1f, 2f) },
          { "O", Rand.Range(0.1f, 2f) },
          { "Ne", Rand.Range(0.1f, 2f) },
          { "Fe", Rand.Range(0.1f, 2f) }
        },

        SpectralClass.K => new Dictionary<string, float>
        {
          { "H", Rand.Range(56f, 64f) },
          { "He", Rand.Range(36f, 44f) },
          { "C", Rand.Range(0.1f, 2f) },
          { "N", Rand.Range(0.1f, 2f) },
          { "O", Rand.Range(0.1f, 2f) },
          { "Ne", Rand.Range(0.1f, 2f) },
          { "Fe", Rand.Range(0.1f, 2f) },
          { "Si", Rand.Range(0.1f, 2f) },
          { "Mg", Rand.Range(0.1f, 2f) }
        },

        SpectralClass.M => new Dictionary<string, float>
        {
          { "H", Rand.Range(36f, 56f) },
          { "He", Rand.Range(44f, 64f) },
          { "C", Rand.Range(0.1f, 2f) },
          { "N", Rand.Range(0.1f, 2f) },
          { "O", Rand.Range(0.1f, 2f) },
          { "Ne", Rand.Range(0.1f, 2f) },
          { "Fe", Rand.Range(0.1f, 2f) },
          { "Si", Rand.Range(0.1f, 2f) },
          { "Mg", Rand.Range(0.1f, 2f) },
          { "S", Rand.Range(0.1f, 2f) },
          { "Cl", Rand.Range(0.1f, 2f) },
          { "K", Rand.Range(0.1f, 2f) }
        },

        _ => new Dictionary<string, float>
        {
          { "H", 74f },
          { "He", 24f },
          { "O", 1f },
          { "Fe", 1f }
        }
      };

      NormalizeToPercent(elements);

      return new GeneratedStellarComposition(elements);
    }

    public static string FormatMetallicity(float metallicity)
    {
      return $"{metallicity:0.##}%";
    }

    public static List<string> FormatCompositionLines(Dictionary<string, float> elements, int elementsPerLine = 4)
    {
      if (elements == null || elements.Count == 0)
        return ["Unknown"];

      List<string> parts = elements
        .OrderByDescending(kvp => kvp.Value)
        .Select(kvp => $"{kvp.Key} {kvp.Value:0.#}%")
        .ToList();

      List<string> lines = [];

      for (int i = 0; i < parts.Count; i += elementsPerLine)
        lines.Add(string.Join(", ", parts.Skip(i).Take(elementsPerLine)));

      return lines;
    }

    private static void NormalizeToPercent(Dictionary<string, float> elements)
    {
      float total = 0f;

      foreach (float value in elements.Values)
        total += value;

      if (total <= 0f)
        return;

      List<string> keys = new(elements.Keys);

      foreach (string key in keys)
        elements[key] = elements[key] / total * 100f;
    }
  }
}
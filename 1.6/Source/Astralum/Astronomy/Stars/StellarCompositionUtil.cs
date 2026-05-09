using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Astralum.Astronomy.Stars
{
    public static class StellarCompositionUtil
    {
        public static GeneratedStellarComposition GenerateComposition(SpectralClass spectralClass)
        {
            Dictionary<string, float> elements = spectralClass switch
            {
                SpectralClass.O => new Dictionary<string, float>
                {
                    { "H", Random.Range(74f, 76f) },
                    { "He", Random.Range(24f, 26f) }
                },
                
                SpectralClass.B => new Dictionary<string, float>
                {
                    { "H", Random.Range(58f, 70f) },
                    { "He", Random.Range(28f, 42f) },
                    { "C", Random.Range(0.1f, 2f) },
                    { "N", Random.Range(0.1f, 2f) },
                    { "O", Random.Range(0.1f, 2f) }
                },
                
                SpectralClass.A => new Dictionary<string, float>
                {
                    { "H", Random.Range(71f, 74f) },
                    { "He", Random.Range(25f, 28f) },
                    { "C", Random.Range(0.1f, 2f) },
                    { "N", Random.Range(0.1f, 2f) },
                    { "O", Random.Range(0.1f, 2f) },
                    { "Ne", Random.Range(0.1f, 2f) }
                },
                
                SpectralClass.F => new Dictionary<string, float>
                {
                    { "H", Random.Range(54f, 64f) },
                    { "He", Random.Range(35f, 45f) },
                    { "C", Random.Range(0.1f, 2f) },
                    { "N", Random.Range(0.1f, 2f) },
                    { "O", Random.Range(0.1f, 2f) },
                    { "Ne", Random.Range(0.1f, 2f) },
                    { "Fe", Random.Range(0.1f, 2f) }
                },
                
                SpectralClass.G => new Dictionary<string, float>
                {
                    { "H", Random.Range(74f, 84f) },
                    { "He", Random.Range(14f, 24f) },
                    { "C", Random.Range(0.1f, 2f) },
                    { "N", Random.Range(0.1f, 2f) },
                    { "O", Random.Range(0.1f, 2f) },
                    { "Ne", Random.Range(0.1f, 2f) },
                    { "Fe", Random.Range(0.1f, 2f) }
                },
                
                SpectralClass.K => new Dictionary<string, float>
                {
                    { "H", Random.Range(56f, 64f) },
                    { "He", Random.Range(36f, 44f) },
                    { "C", Random.Range(0.1f, 2f) },
                    { "N", Random.Range(0.1f, 2f) },
                    { "O", Random.Range(0.1f, 2f) },
                    { "Ne", Random.Range(0.1f, 2f) },
                    { "Fe", Random.Range(0.1f, 2f) },
                    { "Si", Random.Range(0.1f, 2f) },
                    { "Mg", Random.Range(0.1f, 2f) }
                },
                
                SpectralClass.M => new Dictionary<string, float>
                {
                    { "H", Random.Range(36f, 56f) },
                    { "He", Random.Range(44f, 64f) },
                    { "C", Random.Range(0.1f, 2f) },
                    { "N", Random.Range(0.1f, 2f) },
                    { "O", Random.Range(0.1f, 2f) },
                    { "Ne", Random.Range(0.1f, 2f) },
                    { "Fe", Random.Range(0.1f, 2f) },
                    { "Si", Random.Range(0.1f, 2f) },
                    { "Mg", Random.Range(0.1f, 2f) },
                    { "S", Random.Range(0.1f, 2f) },
                    { "Cl", Random.Range(0.1f, 2f) },
                    { "K", Random.Range(0.1f, 2f) }
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
            {
                lines.Add(string.Join(", ", parts.Skip(i).Take(elementsPerLine)));
            }
            
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
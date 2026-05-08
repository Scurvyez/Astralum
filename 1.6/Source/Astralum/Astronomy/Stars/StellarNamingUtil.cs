using System.Text;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Stars
{
    public static class StellarNamingUtil
    {
        private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Numbers = "0123456789";
        
        private static readonly char[] Consonants =
        [
            'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z'
        ];
        
        private static readonly char[] Vowels =
        [
            'a', 'e', 'i', 'o', 'u'
        ];
        
        private static readonly string[] RomanNumerals =
        [
            "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X"
        ];
        
        public static string GenerateSystemName()
        {
            return Random.Range(0, 100) < 8
                ? GenerateSemiUniqueSystemName()
                : GenerateGenericSystemName();
        }
        
        public static string GenerateStarName(string systemName, int starIndex = 0)
        {
            if (starIndex <= 0)
                return systemName;
            
            return $"{systemName} {GetStarLetterSuffix(starIndex)}";
        }
        
        private static string GenerateSemiUniqueSystemName()
        {
            int nameLength = Random.Range(3, 8);
            StringBuilder builder = new();
            
            for (int i = 0; i < nameLength; i++)
            {
                char letter = i % 2 == 0
                    ? Consonants.RandomElement()
                    : Vowels.RandomElement();
                
                builder.Append(letter);
            }
            
            string name = char.ToUpperInvariant(builder[0]) + builder.ToString().Substring(1);
            
            if (Random.Range(0, 100) < 50)
                return name;
            
            return $"{name}-{RomanNumerals.RandomElement()}";
        }
        
        private static string GenerateGenericSystemName()
        {
            int nameLength = Random.Range(3, 8);
            int splitIndex = nameLength / 2;
            
            StringBuilder builder = new();
            
            for (int i = 0; i < nameLength; i++)
            {
                if (i == splitIndex)
                    builder.Append("-");
                
                builder.Append(i < splitIndex
                    ? Letters[Random.Range(0, Letters.Length)]
                    : Numbers[Random.Range(0, Numbers.Length)]);
            }
            
            return builder.ToString();
        }
        
        private static string GetStarLetterSuffix(int starIndex)
        {
            // 0 = primary/no suffix, 1 = A, 2 = B, etc.
            int suffixIndex = Mathf.Max(0, starIndex - 1);
            char suffix = (char)('A' + Mathf.Clamp(suffixIndex, 0, 25));
            
            return suffix.ToString();
        }
        
        public static string SafeName(string value, string fallback)
        {
            return string.IsNullOrEmpty(value) ? fallback : value;
        }
    }
}
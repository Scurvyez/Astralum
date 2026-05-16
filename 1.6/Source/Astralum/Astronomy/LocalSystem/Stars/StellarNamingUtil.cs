using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.LocalSystem.Stars
{
  public static class StellarNamingUtil
  {
    private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Numbers = "0123456789";

    private static readonly string[] StartingConsonantChunks =
    [
      "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z",
      "br", "cr", "dr", "gr", "kr", "pr", "tr",
      "bl", "cl", "fl", "gl", "pl",
      "ch", "sh", "th", "ph", "st", "sk", "sp"
    ];

    private static readonly string[] MiddleConsonantChunks =
    [
      "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z",
      "br", "cr", "dr", "gr", "kr", "pr", "tr",
      "bl", "cl", "fl", "gl", "pl",
      "ch", "sh", "th", "ph", "st", "sk", "sp",
      "ll", "rr", "ss", "nd", "nt", "rn"
    ];

    private static readonly string[] EndingConsonantChunks =
    [
      "b", "c", "d", "f", "g", "h", "k", "l", "m", "n", "p", "r", "s", "t", "x", "z",
      "ch", "sh", "th", "nd", "nt", "rn", "ss", "ll"
    ];

    private static readonly string[] VowelChunks =
    [
      "a", "e", "i", "o", "u",
      "ae", "ai", "ea", "ee", "ei", "ia", "ie", "io", "oa", "oe", "oi", "oo", "ou", "ua", "ue"
    ];

    private static readonly string[] RomanNumerals =
    [
      "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X"
    ];

    public static string GenerateUniqueName(HashSet<string> usedNames, Func<string> generator)
    {
      for (int i = 0; i < 100; i++)
      {
        string name = generator();

        if (usedNames.Add(name))
          return name;
      }

      string fallback;

      do
      {
        fallback = $"{generator()}-{Rand.Range(1000, 9999)}";
      } while (!usedNames.Add(fallback));

      return fallback;
    }

    public static string GenerateSystemName()
    {
      return Rand.Range(0, 100) < 8
        ? GenerateSemiUniqueSystemName()
        : GenerateGenericSystemName();
    }

    public static string GenerateStarName(string systemName, int starIndex = 0)
    {
      if (starIndex <= 0)
        return systemName;

      return $"{systemName} {GetStarLetterSuffix(starIndex)}";
    }

    public static string GenerateSemiUniqueSystemName()
    {
      int targetLength = Rand.RangeInclusive(3, 13);
      StringBuilder builder = new();

      bool useConsonant = Rand.Value < 0.75f;

      while (builder.Length < targetLength)
      {
        string chunk = useConsonant
          ? RandomConsonantChunk(builder.Length, targetLength)
          : VowelChunks.RandomElement();

        int remaining = targetLength - builder.Length;

        if (chunk.Length > remaining)
          continue;

        builder.Append(chunk);

        useConsonant = !useConsonant;
      }

      string name = Capitalize(builder.ToString());

      if (Rand.Range(0, 100) < 20)
        return $"{name}-{RomanNumerals.RandomElement()}";

      return name;
    }

    public static string GenerateGenericSystemName()
    {
      int nameLength = Rand.Range(3, 10);
      int splitIndex = nameLength / 2;

      StringBuilder builder = new();

      for (int i = 0; i < nameLength; i++)
      {
        if (i == splitIndex)
          builder.Append("-");

        builder.Append(i < splitIndex
          ? Letters[Rand.Range(0, Letters.Length)]
          : Numbers[Rand.Range(0, Numbers.Length)]);
      }

      return builder.ToString();
    }

    public static string GenerateConstellationStarName(float uniqueNameChance)
    {
      return Rand.Value < uniqueNameChance
        ? GenerateSemiUniqueSystemName()
        : GenerateGenericSystemName();
    }

    private static string GetStarLetterSuffix(int starIndex)
    {
      // 0 = primary/no suffix, 1 = A, 2 = B, etc.
      int suffixIndex = Mathf.Max(0, starIndex - 1);
      char suffix = (char)('A' + Mathf.Clamp(suffixIndex, 0, 25));

      return suffix.ToString();
    }

    private static string RandomConsonantChunk(int currentLength, int targetLength)
    {
      bool atStart = currentLength == 0;
      bool nearEnd = targetLength - currentLength <= 2;

      if (atStart)
        return StartingConsonantChunks.RandomElement();

      if (nearEnd)
        return EndingConsonantChunks.RandomElement();

      return MiddleConsonantChunks.RandomElement();
    }

    private static string Capitalize(string value)
    {
      if (string.IsNullOrEmpty(value))
        return value;

      if (value.Length == 1)
        return char.ToUpperInvariant(value[0]).ToString();

      return char.ToUpperInvariant(value[0]) + value.Substring(1);
    }

    public static string SafeName(string value, string fallback)
    {
      return string.IsNullOrEmpty(value) ? fallback : value;
    }
  }
}
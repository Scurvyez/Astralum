using System.Collections.Generic;
using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.DefOfs;
using Astralum.World;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace Astralum.Astronomy.Nebulae
{
  public static class NebulaNamingUtil
  {
    public static string GenerateUniqueName(HashSet<string> usedNames, int index, Vector3 localSkyPos)
    {
      for (int i = 0; i < 100; i++)
      {
        string name = Generate(index, localSkyPos);
        
        if (!name.NullOrEmpty() && usedNames.Add(name))
          return name;
      }
      
      string fallback;
      
      do
      {
        fallback = $"Nebula_{index + 1}-{Rand.Range(1000, 9999)}";
      }
      while (!usedNames.Add(fallback));
      
      return fallback;
    }
    
    private static string Generate(int index, Vector3 localSkyPos)
    {
      float roll = Rand.Value;
      
      return roll switch
      {
        < 0.35f => GenerateCatalogName(),
        < 0.55f => GenerateCoordinateName(localSkyPos),
        < 0.78f => GenerateFromRulePack(InternalDefOf.Astra_NebulaName_Descriptive),
        < 0.92f => GenerateDiscovererName(),
        _ => GenerateFormalName(index)
      };
    }
    
    private static string GenerateCatalogName()
    {
      string number = Rand.RangeInclusive(100, 9999).ToString();
      
      return GenerateFromRulePack(
        InternalDefOf.Astra_NebulaName_Catalog,
        new Rule_String("number", number)
      );
    }
    
    private static string GenerateCoordinateName(Vector3 localSkyPos)
    {
      Vector3 dir = localSkyPos.normalized;
      SkyCoord coord = WorldUtils.DirectionToSkyCoord(dir);
      
      int raHour = Mathf.FloorToInt(Mathf.Repeat(coord.rightAscensionHours, 24f));
      int raMinute = Mathf.FloorToInt(
        (Mathf.Repeat(coord.rightAscensionHours, 24f) - raHour) * 60f);
      
      float decAbs = Mathf.Abs(coord.declinationDegrees);
      string sign = coord.declinationDegrees >= 0f ? "+" : "-";
      
      int decDegree = Mathf.FloorToInt(decAbs);
      int decMinute = Mathf.FloorToInt((decAbs - decDegree) * 60f);
      
      string coordinate = $"J{raHour:00}{raMinute:00}{sign}{decDegree:00}{decMinute:00}";
      
      return GenerateFromRulePack(
        InternalDefOf.Astra_NebulaName_Coordinates,
        new Rule_String("coordinate", coordinate)
      );
    }
    
    private static string GenerateDiscovererName()
    {
      string discoverer = GenerateDiscovererSurname();
      
      return GenerateFromRulePack(
        InternalDefOf.Astra_NebulaName_Discoverer,
        new Rule_String("discoverer", discoverer)
      );
    }
    
    private static string GenerateFormalName(int index)
    {
      string root = StellarNamingUtil.GenerateSemiUniqueSystemName();
      
      return GenerateFromRulePack(
        InternalDefOf.Astra_NebulaName_Formal,
        new Rule_String("root", root),
        new Rule_String("index", (index + 1).ToString())
      );
    }
    
    private static string GenerateFromRulePack(RulePackDef def, params Rule[] extraRules)
    {
      if (def == null)
        return null;
      
      GrammarRequest request = default;
      
      request.Rules.AddRange(def.RulesPlusIncludes);
      
      if (extraRules != null)
      {
        for (int i = 0; i < extraRules.Length; i++)
          request.Rules.Add(extraRules[i]);
      }
      
      return GrammarResolver.Resolve(
        "r_name",
        request,
        capitalizeFirstSentence: false
      );
    }
    
    private static string GenerateDiscovererSurname()
    {
      NameBank bank = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard);
      
      for (int i = 0; i < 30; i++)
      {
        string lastName = bank.GetName(PawnNameSlot.Last);
        
        if (!lastName.NullOrEmpty())
          return lastName;
      }
      
      return StellarNamingUtil.GenerateSemiUniqueSystemName();
    }
  }
}
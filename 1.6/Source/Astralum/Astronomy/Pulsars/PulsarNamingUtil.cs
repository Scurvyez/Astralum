using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Pulsars
{
  public static class PulsarNamingUtil
  {
    public static string GenerateName(Vector3 localSkyPos)
    {
      return Rand.Value < 0.85f
        ? GenerateJName(localSkyPos)
        : GenerateBName(localSkyPos);
    }
    
    private static string GenerateJName(Vector3 localSkyPos)
    {
      GetCoordinateParts(localSkyPos, out int raHour, out int raMinute, out string sign,
        out int decDegree, out int decMinute);
      
      return $"PSR J{raHour:00}{raMinute:00}{sign}{decDegree:00}{decMinute:00}";
    }
    
    private static string GenerateBName(Vector3 localSkyPos)
    {
      GetCoordinateParts(localSkyPos, out int raHour, out int raMinute, out string sign,
        out int decDegree, out _);
      
      return $"PSR B{raHour:00}{raMinute:00}{sign}{decDegree:00}";
    }
    
    private static void GetCoordinateParts(Vector3 localSkyPos, out int raHour, out int raMinute, out string sign,
      out int decDegree, out int decMinute)
    {
      Vector3 dir = localSkyPos.normalized;
      SkyCoord coord = WorldUtils.DirectionToSkyCoord(dir);
      
      float ra = Mathf.Repeat(coord.rightAscensionHours, 24f);
      
      raHour = Mathf.FloorToInt(ra);
      raMinute = Mathf.FloorToInt((ra - raHour) * 60f);
      
      float dec = coord.declinationDegrees;
      sign = dec >= 0f ? "+" : "-";
      
      float decAbs = Mathf.Abs(dec);
      
      decDegree = Mathf.FloorToInt(decAbs);
      decMinute = Mathf.FloorToInt((decAbs - decDegree) * 60f);
    }
  }
}
using System.Collections.Generic;
using Astralum.API;
using Astralum.Astronomy;
using Astralum.Astronomy.BlackHoles;
using Astralum.Astronomy.Constellations;
using Astralum.Astronomy.Pulsars;
using Verse;

namespace Astralum.UI
{
  public static class CelestialObjectHoverInfoLineCache
  {
    private static readonly Dictionary<string, List<CelestialObjectHoverInfoLine>> LinesByObject = [];
    
    public static List<CelestialObjectHoverInfoLine> GetLines(
      CelestialObjectInteractionRegistry.HoverCelestialObject obj)
    {
      string key = CacheKeyFor(obj);
      
      if (LinesByObject.TryGetValue(key, out List<CelestialObjectHoverInfoLine> lines))
        return lines;
      
      lines = BuildLines(obj);
      LinesByObject[key] = lines;
      
      return lines;
    }
    
    public static void Clear()
    {
      LinesByObject.Clear();
    }
    
    public static void Clear(CelestialObjectInteractionRegistry.HoverCelestialObject obj)
    {
      LinesByObject.Remove(CacheKeyFor(obj));
    }
    
    private static List<CelestialObjectHoverInfoLine> BuildLines(
      CelestialObjectInteractionRegistry.HoverCelestialObject obj)
    {
      return obj.type switch
      {
        CelestialObjectType.BlackHole => BuildBlackHoleLines(obj),
        CelestialObjectType.Pulsar => BuildPulsarLines(obj),
        CelestialObjectType.ConstellationStar => BuildConstellationStarLines(obj),
        _ => []
      };
    }
    
    private static List<CelestialObjectHoverInfoLine> BuildBlackHoleLines(
      CelestialObjectInteractionRegistry.HoverCelestialObject obj)
    {
      SavedBlackHole saved = BlackHoleDataUtil.GetById(obj.id);
      string displayName = saved?.DisplayName ?? obj.name;
      
      return
      [
        new CelestialObjectHoverInfoLine(displayName.NullOrEmpty() 
          ? "Astra_Blackholes_Unknown".Translate() 
          : displayName),
        
        new CelestialObjectHoverInfoLine("Astra_Blackholes_Type".Translate()),
        new CelestialObjectHoverInfoLine("Astra_Objects_Region".Translate() + $" {obj.hemisphere}"),
        new CelestialObjectHoverInfoLine("Astra_Objects_RightAscension".Translate() + $" {obj.rightAscension}"),
        new CelestialObjectHoverInfoLine("Astra_Objects_Declination".Translate() + $" {obj.declination}")
      ];
    }
    
    private static List<CelestialObjectHoverInfoLine> BuildPulsarLines(
      CelestialObjectInteractionRegistry.HoverCelestialObject obj)
    {
      SavedPulsar saved = PulsarDataUtil.GetById(obj.id);
      string displayName = saved?.DisplayName ?? obj.name;
      
      return
      [
        new CelestialObjectHoverInfoLine(displayName.NullOrEmpty()
          ? "Astra_Pulsar_Unknown".Translate() 
          : displayName),
        
        new CelestialObjectHoverInfoLine("Astra_Pulsars_Type".Translate()),
        new CelestialObjectHoverInfoLine("Astra_Objects_Region".Translate() + $" {obj.hemisphere}"),
        new CelestialObjectHoverInfoLine("Astra_Objects_RightAscension".Translate() + $" {obj.rightAscension}"),
        new CelestialObjectHoverInfoLine("Astra_Objects_Declination".Translate() + $" {obj.declination}")
      ];
    }
    
    private static List<CelestialObjectHoverInfoLine> BuildConstellationStarLines(
      CelestialObjectInteractionRegistry.HoverCelestialObject obj)
    {
      SavedConstellationStar saved = ConstellationDataUtil.GetStarById(obj.id);
      SavedConstellation constellation = ConstellationDataUtil.GetConstellationForStar(obj.id);
      string displayName = saved?.DisplayName ?? obj.name;
      string constellationDisplayName = constellation?.DisplayName ?? obj.constellationName;
      
      return
      [
        new CelestialObjectHoverInfoLine(displayName.NullOrEmpty() 
          ? "Astra_Stars_Unknown".Translate() 
          : displayName),

        new CelestialObjectHoverInfoLine("Astra_Stars_Class".Translate() + $": {obj.spectralClass}"),
        new CelestialObjectHoverInfoLine("Astra_Stars_Constellation".Translate() + $": {constellationDisplayName}"),
        new CelestialObjectHoverInfoLine("Astra_Stars_Region".Translate() + $": {obj.hemisphere}"),
        new CelestialObjectHoverInfoLine("Astra_Objects_RightAscension".Translate() + $" {obj.rightAscension}"),
        new CelestialObjectHoverInfoLine("Astra_Objects_Declination".Translate() + $" {obj.declination}")
      ];
    }
    
    private static string CacheKeyFor(CelestialObjectInteractionRegistry.HoverCelestialObject obj)
    {
      return $"{obj.type}:{obj.id}";
    }
  }
}
using System.Collections.Generic;
using Astralum.Astronomy.BackgroundStars;
using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.Debugging;
using Astralum.DefOfs;
using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public static class ConstellationGenerationUtil
  {
    public const float DistanceToConstellations = 20f;
    private const float MinCenterAngularDistance = 0.75f;
    private const int DefaultConstellationCount = 13;
    private const int DefaultMaxPlacementAttempts = 80;
    private const float DefaultBaseStarSize = 0.25f;
    private const float DefaultBrightStarSize = 0.85f;
    private const float DefaultConstellationSizeMin = 3.0f;
    private const float DefaultConstellationSizeMax = 3.5f;
    private const float DefaultMinViewRotationAngle = 160f;
    private const float DefaultMaxViewRotationAngle = 200f;
    
    public static void EnsureGenerated()
    {
      WorldComponent_ConstellationDataCache data = ConstellationDataUtil.Data;
      
      if (data == null || data.HasGeneratedConstellations)
        return;
      
      if (!ConstellationMaskUtil.HasMasks)
      {
        AstraLog.Warning("No constellation masks found.");
        return;
      }
      
      ModExt_Constellations extC = InternalDefOf.Astra_Constellations?.GetModExtension<ModExt_Constellations>();
      ModExt_ConstellationStars extCs = InternalDefOf.Astra_ConstellationStars.GetModExtension<ModExt_ConstellationStars>();
      
      if (extC == null && extCs == null)
      {
        AstraLog.Warning("Astra_Constellations is missing ModExt_Constellations. Using fallback values.");
      }
      
      int constellationCount = extC != null ? Mathf.Max(0, extC.constellationCount) : DefaultConstellationCount;
      int maxPlacementAttempts = extC != null ? Mathf.Max(0, extC.maxPlacementAttempts) : DefaultMaxPlacementAttempts;
      float baseStarSize = extC != null ? Mathf.Max(0f, extCs.baseStarSize) : DefaultBaseStarSize;
      float brightStarSize = extC != null ? Mathf.Max(0f, extCs.brightStarSize) : DefaultBrightStarSize;
      float constellationSizeMin = extC != null ? Mathf.Max(0f, extC.constellationSizeMin) : DefaultConstellationSizeMin;
      float constellationSizeMax = extC != null ? Mathf.Max(0f, extC.constellationSizeMax) : DefaultConstellationSizeMax;
      float minViewRotationAngle = extC != null ? Mathf.Max(0f, extC.minViewRotationAngle) : DefaultMinViewRotationAngle;
      float maxViewRotationAngle = extC != null ? Mathf.Max(0f, extC.maxViewRotationAngle) : DefaultMaxViewRotationAngle;
      
      Rand.PushState();
      Rand.Seed = Find.World.info.Seed ^ 0x5A17A11;
      
      try
      {
        GenerateAndSaveConstellations(data, constellationCount, maxPlacementAttempts, baseStarSize,
          brightStarSize, constellationSizeMin, constellationSizeMax, minViewRotationAngle, maxViewRotationAngle);
      }
      finally
      {
        Rand.PopState();
      }
    }
    
    private static void GenerateAndSaveConstellations(WorldComponent_ConstellationDataCache data,
      int constellationCount, int maxPlacementAttempts, float baseStarSize, float brightStarSize,
      float constellationSizeMin, float constellationSizeMax, float minViewRotationAngle, float maxViewRotationAngle)
    {
      data.Clear();
      
      List<Vector3> usedCenters = [];
      List<ConstellationMaskInfo> unusedMasks = ConstellationMaskUtil.CreateShuffledMaskPool();
      HashSet<string> usedNames = [];
      int count = Mathf.Min(constellationCount, unusedMasks.Count);
      
      for (int i = 0; i < count; i++)
      {
        TryGenerateAndSaveConstellation(data.Constellations, usedCenters, unusedMasks, usedNames, i,
          maxPlacementAttempts, baseStarSize, brightStarSize, constellationSizeMin, constellationSizeMax,
          minViewRotationAngle, maxViewRotationAngle);
      }
    }
    
    private static void TryGenerateAndSaveConstellation(List<SavedConstellation> savedConstellations,
      List<Vector3> usedCenters, List<ConstellationMaskInfo> unusedMasks, HashSet<string> usedNames,
      int constellationIndex, int maxPlacementAttempts, float baseStarSize, float brightStarSize,
      float constellationSizeMin, float constellationSizeMax, float minViewRotationAngle, float maxViewRotationAngle)
    {
      if (unusedMasks.NullOrEmpty())
        return;
      
      for (int attempt = 0; attempt < maxPlacementAttempts; attempt++)
      {
        Vector3 dir = RandomDensityWeightedSkyDirection();
        
        if (OverlapsExistingConstellation(dir, usedCenters))
          continue;

        ConstellationMaskInfo maskInfo = unusedMasks[unusedMasks.Count - 1];
        unusedMasks.RemoveAt(unusedMasks.Count - 1);
        
        Texture2D mask = maskInfo.texture;
        float size = Rand.Range(constellationSizeMin, constellationSizeMax);
        float rotation = Rand.Range(minViewRotationAngle, maxViewRotationAngle);
        string id = $"constellation_{Find.World.info.seedString}_{constellationIndex}";
        
        SavedConstellation constellation = ConstellationDataUtil.Create(id, dir, size, rotation, usedNames,
            maskInfo, mask);
        
        int starCount = ConstellationMaskUtil.RandomStarPointCount();
        Vector2[] starPoints = ConstellationMaskUtil.GetStarPoints(mask, starCount);
        
        BuildSavedStars(constellation, starPoints, usedNames, baseStarSize, brightStarSize);
        
        savedConstellations.Add(constellation);
        usedCenters.Add(dir);
        
        return;
      }
    }
    
    private static void BuildSavedStars(SavedConstellation constellation, Vector2[] uvPoints, HashSet<string> usedNames,
      float baseStarSize, float brightStarSize)
    {
      if (uvPoints.NullOrEmpty())
        return;
      
      Vector3 center = constellation.centerDir.normalized * DistanceToConstellations;
      
      GetConstellationBasis(constellation.centerDir, constellation.Rotation, out Vector3 tangentA, out Vector3 tangentB);
      
      for (int i = 0; i < uvPoints.Length; i++)
      {
        Vector2 uv = uvPoints[i];
        Vector2 local = new(uv.x * 2f - 1f, uv.y * 2f - 1f);
        Vector3 starPos = center + tangentA * local.x * 
          constellation.RenderSize * 0.5f + tangentB * local.y * constellation.RenderSize * 0.5f;
        
        SpectralClass spectralClass = BackgroundStarsUtil.RandomConstellationStarClass();
        float brightness = RandomMagnitudeBrightness(spectralClass);
        float visualSize = Mathf.Lerp(baseStarSize, brightStarSize, brightness);
        
        // Very dim stars = 15% chance for unique name
        // Medium stars = ~50%
        // Bright stars = 90%
        float uniqueNameChance = Mathf.Lerp(0.15f, 0.90f, brightness);
        
        string starName = StellarNamingUtil.GenerateUniqueName(usedNames,
            () => StellarNamingUtil.GenerateConstellationStarName(uniqueNameChance));

        string id = $"{constellation.Id}_star_{i}";
        
        SavedConstellationStar star = new()
        {
          id = id,
          renderSize = visualSize,
          rotation = 0f,
          localSkyPosition = starPos,
          //worldViewDirection = starPos,
          generatedName = starName,
          uv = uv,
          spectralClass = spectralClass
        };
        
        constellation.stars.Add(star);
      }
    }
    
    public static void GetConstellationBasis(Vector3 centerDir, float rotationDegrees, out Vector3 tangentA, 
      out Vector3 tangentB)
    {
      Vector3 normal = centerDir.normalized;
      tangentA = Vector3.Cross(normal, Vector3.up);
      
      if (tangentA.sqrMagnitude < 0.001f)
      {
        tangentA = Vector3.Cross(normal, Vector3.right);
      }
      
      tangentA.Normalize();
      tangentB = Vector3.Cross(normal, tangentA).normalized;
      
      Quaternion rotation = Quaternion.AngleAxis(rotationDegrees, normal);
      
      tangentA = rotation * tangentA;
      tangentB = rotation * tangentB;
    }
    
    private static float RandomMagnitudeBrightness(SpectralClass spectralClass)
    {
      float minBrightness;
      float maxBrightness;
      
      switch (spectralClass)
      {
        case SpectralClass.O:
          minBrightness = 0.75f;
          maxBrightness = 1.00f;
          break;
        
        case SpectralClass.B:
          minBrightness = 0.60f;
          maxBrightness = 0.95f;
          break;

        case SpectralClass.A:
          minBrightness = 0.45f;
          maxBrightness = 0.85f;
          break;

        case SpectralClass.F:
          minBrightness = 0.30f;
          maxBrightness = 0.70f;
          break;

        case SpectralClass.G:
          minBrightness = 0.20f;
          maxBrightness = 0.60f;
          break;

        case SpectralClass.K:
          minBrightness = 0.10f;
          maxBrightness = 0.45f;
          break;

        case SpectralClass.M:
          minBrightness = 0.03f;
          maxBrightness = 0.30f;
          break;

        default:
          minBrightness = 0.10f;
          maxBrightness = 0.60f;
          break;
      }
      
      float t = Rand.Value;
      float classBrightness = Mathf.Pow(1f - t, 2.8f);
      
      return Mathf.Lerp(minBrightness, maxBrightness, classBrightness);
    }
    
    private static Vector3 RandomDensityWeightedSkyDirection()
    {
      for (int i = 0; i < 32; i++)
      {
        Vector3 dir = Rand.UnitVector3.normalized;
        float density = StarDensity(dir);

        if (Rand.Value <= density)
          return dir;
      }
      
      return Rand.UnitVector3.normalized;
    }
    
    private static float StarDensity(Vector3 dir)
    {
      float galacticBand = 1f - Mathf.Abs(dir.y);
      galacticBand = Mathf.Pow(Mathf.Clamp01(galacticBand), 1.8f);
      
      float noiseA = Mathf.PerlinNoise(dir.x * 2.5f + 43.17f, dir.z * 2.5f + 91.73f);
      float noiseB = Mathf.PerlinNoise(dir.y * 4.0f + 12.91f, dir.x * 4.0f + 66.34f);
      
      float clusteredNoise = Mathf.Lerp(noiseA, noiseB, 0.35f);
      clusteredNoise = Mathf.Pow(clusteredNoise, 1.4f);
      
      return Mathf.Clamp01(0.18f + galacticBand * 0.45f + clusteredNoise * 0.55f);
    }
    
    private static bool OverlapsExistingConstellation(Vector3 centerDir, List<Vector3> usedCenters)
    {
      for (int i = 0; i < usedCenters.Count; i++)
      {
        float angularDistance = Vector3.Angle(centerDir, usedCenters[i]) * Mathf.Deg2Rad;
        
        if (angularDistance < MinCenterAngularDistance)
        {
          return true;
        }
      }
      
      return false;
    }
  }
}
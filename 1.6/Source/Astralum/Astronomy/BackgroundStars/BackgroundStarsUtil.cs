using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.BackgroundStars
{
  public static class BackgroundStarsUtil
  {
    public static IntRange ResolvedStarCountRange(ModExt_BackgroundStars ext)
    {
      if (ext == null)
        return new IntRange(10000, 50000);
      
      return new IntRange(
        Mathf.Clamp(ext.starCount.min, 10000, 50000),
        Mathf.Clamp(ext.starCount.max, 10000, 100000));
    }
    
    public static BackgroundStarsGenerationData GetGenerationData(int worldSeed, IntRange starCountRange)
    {
      Rand.PushState();

      try
      {
        Rand.Seed = worldSeed ^ 0x71C04ED;
        
        bool useNonUniformGalacticPlaneBand = Rand.Chance(0.5f);
        float galacticPlaneBandMaskOffset = Rand.Value;
        int starCount = starCountRange.RandomInRange;
        
        float normalizedStarCount = Mathf.InverseLerp(starCountRange.min, starCountRange.max, starCount);
        
        return new BackgroundStarsGenerationData(starCount, normalizedStarCount, useNonUniformGalacticPlaneBand,
          galacticPlaneBandMaskOffset);
      }
      finally
      {
        Rand.PopState();
      }
    }
    
    /// <summary>
    /// Selects a random spectral class for a background star, based on predefined probabilities
    /// that represent the relative abundance and distribution of each spectral class in the galaxy.
    /// </summary>
    /// <returns>A SpectralClass value indicating the randomly selected spectral type of the background star.</returns>
    public static SpectralClass RandomBackgroundStarClass()
    {
      float value = Rand.Value;
      
      return value switch
      {
        < 0.005f => SpectralClass.O,
        < 0.025f => SpectralClass.B,
        < 0.080f => SpectralClass.A,
        < 0.180f => SpectralClass.F,
        < 0.360f => SpectralClass.G,
        < 0.620f => SpectralClass.K,
        _ => SpectralClass.M
      };
    }

    /// <summary>
    /// Selects a random spectral class for a star in a constellation, based on predefined probabilities
    /// that reflect the relative abundance of each spectral class in the galaxy.
    /// </summary>
    /// <returns>A SpectralClass value indicating the randomly chosen spectral type of the star.</returns>
    public static SpectralClass RandomConstellationStarClass()
    {
      float value = Rand.Value;
      
      return value switch
      {
        < 0.001f => SpectralClass.O, // 0.1%
        < 0.041f => SpectralClass.B, // 4.0%
        < 0.221f => SpectralClass.A, // 18.0%
        < 0.461f => SpectralClass.F, // 24.0%
        < 0.681f => SpectralClass.G, // 22.0%
        < 0.921f => SpectralClass.K, // 24.0%
        _ => SpectralClass.M // 7.9%
      };
    }

    /// <summary>
    /// Generates a random star size based on the given spectral class and size range,
    /// using brightness calculations influenced by the apparent magnitude of the spectral class.
    /// </summary>
    /// <param name="spectralClass">The spectral class of the star, which determines the apparent magnitude and corresponding brightness.</param>
    /// <param name="starSizeRange">The range of possible sizes for the star, used to interpolate the final size.</param>
    /// <returns>A float representing the calculated size of the star within the specified range.</returns>
    public static float RandomStarSize(SpectralClass spectralClass, FloatRange starSizeRange)
    {
      float apparentMagnitude = spectralClass switch
      {
        SpectralClass.O => Rand.Range(-1.0f, 5.0f),
        SpectralClass.B => Rand.Range(0.0f, 6.0f),
        SpectralClass.A => Rand.Range(1.0f, 7.0f),
        SpectralClass.F => Rand.Range(2.0f, 8.0f),
        SpectralClass.G => Rand.Range(3.0f, 8.5f),
        SpectralClass.K => Rand.Range(4.0f, 9.0f),
        SpectralClass.M => Rand.Range(5.0f, 10.0f),
        _ => Rand.Range(3.0f, 9.0f)
      };

      float brightness = Mathf.Pow(2.512f, -apparentMagnitude);

      float minBrightness = Mathf.Pow(2.512f, -10.0f);
      float maxBrightness = Mathf.Pow(2.512f, 1.0f);

      float visual = Mathf.InverseLerp(minBrightness, maxBrightness, brightness);

      // makes most stars tiny while preserving rare bright ones
      // increase power to "flatten" the curve and bring size variance closer to a single limit
      // decrease power to exaggerate size variance and make brighter stars even larger
      visual = Mathf.Pow(visual, 0.75f);

      return starSizeRange.LerpThroughRange(visual);
    }

    /// <summary>
    /// Generates a random direction in space influenced by the density of stars for a given spectral class,
    /// prioritizing areas with higher stellar density. Falls back to a random direction within galactic plane bounds if no suitable direction is found.
    /// </summary>
    /// <param name="spectralClass">The spectral class of stars, used to calculate the star density weighting for the direction.</param>
    /// <param name="galacticPlaneBounds">The y-axis bounds that define the region of the galactic plane to use when falling back to a random direction.</param>
    /// <returns>A normalized Vector3 representing the weighted random direction in space.</returns>
    public static Vector3 RandomDensityWeightedDirection(SpectralClass spectralClass, FloatRange galacticPlaneBounds)
    {
      for (int i = 0; i < 32; i++)
      {
        Vector3 dir = Rand.UnitVector3.normalized;

        if (Rand.Value <= StarDensity(dir, spectralClass))
          return dir;
      }

      return WorldUtils.RandomGalacticPlaneDirection(galacticPlaneBounds);
    }

    /// <summary>
    /// Generates a random direction in space influenced by normal galactic density and an additional
    /// horizontal density mask along the galactic plane.
    /// </summary>
    /// <param name="spectralClass">The spectral class of stars, used to calculate the star density weighting for the direction.</param>
    /// <param name="galacticPlaneBounds">The y-axis bounds that define the region of the galactic plane to use when falling back to a random direction.</param>
    /// <param name="galacticPlaneMaskOffset">A normalized 0..1 offset that rotates the horizontal mask around the galactic plane.</param>
    /// <returns>A normalized Vector3 representing the weighted random direction in space.</returns>
    public static Vector3 RandomDensityWeightedDirection(SpectralClass spectralClass, FloatRange galacticPlaneBounds,
      float galacticPlaneMaskOffset)
    {
      for (int i = 0; i < 32; i++)
      {
        Vector3 dir = Rand.UnitVector3.normalized;
    
        if (Rand.Value <= StarDensity(dir, spectralClass, galacticPlaneMaskOffset))
          return dir;
      }
      
      for (int i = 0; i < 32; i++)
      {
        Vector3 dir = WorldUtils.RandomGalacticPlaneDirection(galacticPlaneBounds);

        if (Rand.Value <= GalacticPlaneBandMask(dir, galacticPlaneMaskOffset))
          return dir;
      }
      
      return WorldUtils.RandomGalacticPlaneDirection(galacticPlaneBounds);
    }

    /// <summary>
    /// Calculates the density of stars in a given direction based on the spectral class
    /// and their distribution relative to the galactic plane and cluster patterns.
    /// </summary>
    /// <param name="dir">The direction vector where the star density is being calculated.</param>
    /// <param name="spectralClass">The spectral class of stars, influencing the density distribution and plane concentration.</param>
    /// <returns>A float value between 0 and 1 representing the relative density of stars in the specified direction.</returns>
    private static float StarDensity(Vector3 dir, SpectralClass spectralClass)
    {
      float galacticLatitude = Mathf.Abs(Vector3.Dot(dir.normalized,
        WorldUtils.GalacticPole.normalized));
      float galacticPlane = 1f - galacticLatitude;

      // higher = sharper plane concentration
      float planeBias = spectralClass switch
      {
        SpectralClass.O => Mathf.Pow(galacticPlane, 18.0f),
        SpectralClass.B => Mathf.Pow(galacticPlane, 14.0f),
        SpectralClass.A => Mathf.Pow(galacticPlane, 10.0f),
        SpectralClass.F => Mathf.Pow(galacticPlane, 7.0f),
        SpectralClass.G => Mathf.Pow(galacticPlane, 4.5f),
        SpectralClass.K => Mathf.Pow(galacticPlane, 3.0f),
        SpectralClass.M => Mathf.Pow(galacticPlane, 2.0f),
        _ => galacticPlane
      };

      float clusterNoise = GalacticClusterNoise(dir, spectralClass);

      // multiply by plane bias so off-plane areas actually thin out
      float density = planeBias * Mathf.Lerp(0.35f, 1.0f, clusterNoise);

      // small background population so poles are not totally empty
      float backgroundFloor = spectralClass switch
      {
        SpectralClass.O => 0.0001f,
        SpectralClass.B => 0.0001f,
        SpectralClass.A => 0.0002f,
        SpectralClass.F => 0.0006f,
        SpectralClass.G => 0.0012f,
        SpectralClass.K => 0.0020f,
        SpectralClass.M => 0.0030f,
        _ => 0.002f
      };

      return Mathf.Clamp01(backgroundFloor + density);
    }
    
    /// <summary>
    /// Calculates the density of stars in a given direction, with an additional horizontal mask
    /// that can create gaps or faded regions along the galactic plane.
    /// </summary>
    /// <param name="dir">The direction vector where the star density is being calculated.</param>
    /// <param name="spectralClass">The spectral class of stars, influencing the density distribution and plane concentration.</param>
    /// <param name="galacticPlaneMaskOffset">A normalized 0..1 offset that rotates the horizontal mask around the galactic plane.</param>
    /// <returns>A float value between 0 and 1 representing the relative density of stars in the specified direction.</returns>
    private static float StarDensity(Vector3 dir, SpectralClass spectralClass, float galacticPlaneMaskOffset)
    {
      float density = StarDensity(dir, spectralClass);
      float horizontalMask = GalacticPlaneBandMask(dir, galacticPlaneMaskOffset);
      
      return Mathf.Clamp01(density * horizontalMask);
    }
    
    /// <summary>
    /// Calculates a normalized horizontal coordinate along the galactic plane.
    /// The result wraps from 0 to 1 around the sky.
    /// </summary>
    /// <param name="dir">The world-space direction to evaluate.</param>
    /// <param name="offset">A normalized 0..1 offset that rotates the coordinate around the galactic plane.</param>
    /// <returns>A normalized value from 0 to 1 representing position along the galactic plane.</returns>
    private static float GalacticPlaneLongitude01(Vector3 dir, float offset = 0f)
    {
      Vector3 pole = WorldUtils.GalacticPole.normalized;
      Vector3 reference = Vector3.ProjectOnPlane(Vector3.forward, pole).normalized;
      
      if (reference.sqrMagnitude <= 0.0001f)
        reference = Vector3.ProjectOnPlane(Vector3.right, pole).normalized;
      
      Vector3 tangent = Vector3.Cross(pole, reference).normalized;
      Vector3 projectedDir = Vector3.ProjectOnPlane(dir.normalized, pole).normalized;
      
      float x = Vector3.Dot(projectedDir, reference);
      float y = Vector3.Dot(projectedDir, tangent);
      
      float angle = Mathf.Atan2(y, x);
      float longitude = Mathf.Repeat(angle / (Mathf.PI * 2f), 1f);
      
      return Mathf.Repeat(longitude + offset, 1f);
    }
    
    /// <summary>
    /// Returns a density multiplier for a direction based on local patches around the galactic plane.
    /// This can be used to create empty gaps, soft fades, or denser regions without pole-to-pole stripes.
    /// </summary>
    /// <param name="dir">The world-space direction to evaluate.</param>
    /// <param name="offset">A normalized 0..1 seed offset that changes the mask pattern.</param>
    /// <returns>A density multiplier between 0 and 1.</returns>
    private static float GalacticPlaneBandMask(Vector3 dir, float offset = 0f)
    {
      Vector3 normalizedDir = dir.normalized;
      float latitude = Vector3.Dot(normalizedDir, WorldUtils.GalacticPole.normalized);
      float longitude = GalacticPlaneLongitude01(normalizedDir, offset);
      float seed = offset * 997.31f;

      float lowFrequencyNoise = WrappedPlaneNoise(longitude, latitude, 3.5f, seed + 11.17f);
      float mediumFrequencyNoise = WrappedPlaneNoise(longitude, latitude, 7.0f, seed + 43.91f);
      float fineNoise = DirectionalNoise(normalizedDir, 5.5f, seed + 77.53f);
      float pocketNoise = Mathf.Clamp01((lowFrequencyNoise * 0.55f) + (mediumFrequencyNoise * 0.30f) + 
        (fineNoise * 0.15f));

      float mask = Mathf.Lerp(0.38f, 1.55f, Mathf.SmoothStep(0f, 1f, pocketNoise));
      float deepGap = Mathf.SmoothStep(0.18f, 0.44f, pocketNoise);
      float densePatch = Mathf.SmoothStep(0.62f, 0.88f, pocketNoise);

      mask *= Mathf.Lerp(0.22f, 1f, deepGap);
      mask *= Mathf.Lerp(1f, 1.35f, densePatch);

      return Mathf.Clamp(mask, 0.05f, 1.75f);
    }

    private static float WrappedPlaneNoise(float longitude, float latitude, float scale, float seed)
    {
      float angle = longitude * Mathf.PI * 2f;
      float radius = scale * (1f + Mathf.Abs(latitude) * 2.2f);
      float x = Mathf.Cos(angle) * radius + seed;
      float y = Mathf.Sin(angle) * radius + latitude * scale * 6f + seed * 0.37f;

      return Mathf.PerlinNoise(x, y);
    }
    
    /// <summary>
    /// Samples blended Perlin noise from multiple axes of a direction vector to produce a stable,
    /// seedable directional noise value for localized sky variation.
    /// </summary>
    /// <param name="dir">The normalized world-space direction to sample.</param>
    /// <param name="scale">The frequency multiplier applied to the direction components.</param>
    /// <param name="seed">The seed offset used to vary the sampled noise pattern.</param>
    /// <returns>A blended Perlin noise value between 0 and 1.</returns>
    private static float DirectionalNoise(Vector3 dir, float scale, float seed)
    {
      float noiseA = Mathf.PerlinNoise(dir.x * scale + seed, dir.y * scale + seed * 0.31f);
      float noiseB = Mathf.PerlinNoise(dir.z * scale + seed * 0.73f, dir.x * scale + seed * 0.17f);

      return Mathf.Lerp(noiseA, noiseB, 0.5f);
    }
    
    /// <summary>
    /// Calculates a perlin noise-based value that simulates galactic cluster patterns
    /// in a given direction for a specific spectral class.
    /// </summary>
    /// <param name="dir">The direction vector for which the noise will be calculated.</param>
    /// <param name="spectralClass">The spectral class of the star, which determines the scale of clustering.</param>
    /// <returns>A float value between 0 and 1 representing the galactic cluster noise intensity at the specified direction.</returns>
    private static float GalacticClusterNoise(Vector3 dir, SpectralClass spectralClass)
    {
      float scale = spectralClass switch
      {
        SpectralClass.O => 7.5f,
        SpectralClass.B => 6.5f,
        SpectralClass.A => 5.5f,
        SpectralClass.F => 4.5f,
        SpectralClass.G => 3.5f,
        SpectralClass.K => 3.0f,
        SpectralClass.M => 2.5f,
        _ => 3.5f
      };

      float noiseA = Mathf.PerlinNoise(dir.x * scale + 17.31f, dir.z * scale + 88.72f);
      float noiseB = Mathf.PerlinNoise(dir.y * scale + 41.93f, dir.x * scale + 12.44f);
      float noise = Mathf.Lerp(noiseA, noiseB, 0.35f);

      return Mathf.Pow(noise, 1.4f);
    }
  }
}

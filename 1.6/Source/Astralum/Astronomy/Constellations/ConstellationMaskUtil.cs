using System.Collections.Generic;
using Astralum.Debugging;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  [StaticConstructorOnStartup]
  public static class ConstellationMaskUtil
  {
    private const float StarAlphaThreshold = 0.95f;
    private const int MinStarPoints = 5;
    private const int MaxStarPoints = 7;
    private const float MinPointSpacingNormalized = 0.25f;

    private static readonly List<ConstellationMaskInfo> CachedMasks = [];
    private static readonly Dictionary<string, ConstellationMaskInfo> MaskInfoByName = [];
    private static readonly Dictionary<(Texture2D Mask, int StarCount), Vector2[]> StarPointCache = new();

    static ConstellationMaskUtil()
    {
      foreach (ConstellationMaskCategoryDef categoryDef
               in DefDatabase<ConstellationMaskCategoryDef>.AllDefs)
      {
        if (categoryDef.folderPath.NullOrEmpty())
          continue;

        IEnumerable<Texture2D> textures = ContentFinder<Texture2D>.GetAllInFolder(categoryDef.folderPath);

        foreach (Texture2D texture in textures)
        {
          if (texture == null)
            continue;

          ConstellationMaskInfo info = new(texture, categoryDef.categoryId);

          CachedMasks.Add(info);
          MaskInfoByName[texture.name] = info;
        }
      }

      //AstraLog.Message($"Loaded {CachedMasks.Count} constellation masks.");
    }

    public static bool HasMasks => !CachedMasks.NullOrEmpty();

    /// <summary>
    /// Creates a shuffled pool of constellation masks from the cached mask data.
    /// </summary>
    /// <returns>
    /// A <see cref="List{T}"/> containing shuffled instances of <see cref="ConstellationMaskInfo"/>.
    /// </returns>
    public static List<ConstellationMaskInfo> CreateShuffledMaskPool()
    {
      List<ConstellationMaskInfo> pool = new(CachedMasks);
      pool.Shuffle();
      return pool;
    }

    /// <summary>
    /// Retrieves the texture associated with a constellation mask given its name.
    /// </summary>
    /// <param name="maskName">
    /// The name of the mask to search for. If the name is null or empty, the method will return null.
    /// </param>
    /// <returns>
    /// The <see cref="Texture2D"/> of the specified mask if it exists; otherwise, null.
    /// </returns>
    public static Texture2D GetMaskByName(string maskName)
    {
      return TryGetMaskInfo(maskName, out ConstellationMaskInfo info)
        ? info.texture
        : null;
    }

    /// <summary>
    /// Attempts to retrieve information about a constellation mask based on its name.
    /// </summary>
    /// <param name="maskName">
    /// The name of the mask to search for. If the name is null or empty, the method will fail.
    /// </param>
    /// <param name="info">
    /// When this method returns, contains the <see cref="ConstellationMaskInfo"/> associated
    /// with the specified mask name if it exists; otherwise, contains the default value.
    /// </param>
    /// <returns>
    /// True if the mask information is found and successfully retrieved; otherwise, false.
    /// </returns>
    private static bool TryGetMaskInfo(string maskName, out ConstellationMaskInfo info)
    {
      if (!maskName.NullOrEmpty() && MaskInfoByName.TryGetValue(maskName, out info))
        return true;

      info = default;
      return false;
    }

    /// <summary>
    /// Generates a set of star points based on the given mask and desired star count.
    /// The points are distributed within the mask area, with consideration for minimum spacing
    /// between the points, and are cached for improved performance.
    /// </summary>
    /// <param name="mask">
    /// The texture mask used to define the area where star points can be generated.
    /// </param>
    /// <param name="starCount">
    /// The desired number of star points to generate. This value will be clamped
    /// between the predefined minimum and maximum star point thresholds.
    /// </param>
    /// <returns>
    /// An array of Vector2 representing the normalized coordinates of the generated star points.
    /// Returns an empty array if the mask is null or if no valid points can be generated.
    /// </returns>
    public static Vector2[] GetStarPoints(Texture2D mask, int starCount)
    {
      if (mask == null)
        return [];

      starCount = Mathf.Clamp(starCount, MinStarPoints, MaxStarPoints);

      if (StarPointCache.TryGetValue((mask, starCount), out Vector2[] cached))
        return cached;

      Color[] pixels = GetReadablePixels(mask);

      if (pixels == null || pixels.Length == 0)
        return [];

      List<Vector2> candidates = [];

      int width = mask.width;
      int height = mask.height;

      for (int y = 1; y < height - 1; y++)
      for (int x = 1; x < width - 1; x++)
      {
        Color color = pixels[y * width + x];

        if (!IsConstellationLinePixel(color))
          continue;

        float nx = x / (width - 1f);
        float ny = y / (height - 1f);

        candidates.Add(new Vector2(nx, ny));
      }

      Vector2[] selected = SelectSpacedPoints(candidates, starCount, MinPointSpacingNormalized);

      StarPointCache[(mask, starCount)] = selected;
      return selected;
    }

    /// <summary>
    /// Generates a random number of star points within a predefined range.
    /// </summary>
    /// <returns>
    /// An integer representing the number of star points, randomly selected
    /// between the minimum and maximum star point thresholds.
    /// </returns>
    public static int RandomStarPointCount()
    {
      return Rand.RangeInclusive(MinStarPoints, MaxStarPoints);
    }

    /// <summary>
    /// Determines whether a given color represents a valid pixel for a constellation line
    /// based on its alpha value exceeding a predefined threshold.
    /// </summary>
    /// <param name="color">The color of the pixel to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the pixel's alpha value is equal to or greater than the threshold;
    /// otherwise, <c>false</c>.
    /// </returns>
    private static bool IsConstellationLinePixel(Color color)
    {
      return color.a >= StarAlphaThreshold;
    }

    /// <summary>
    /// Selects a specified number of points from a collection of candidates, ensuring that the selected points
    /// are spaced at least a minimum distance apart from one another.
    /// </summary>
    /// <param name="candidates">A list of candidate points from which to select spaced points.</param>
    /// <param name="maxPoints">The maximum number of points to select.</param>
    /// <param name="minSpacing">The minimum required spacing between selected points, in normalized units.</param>
    /// <returns>
    /// An array of <c>Vector2</c> representing the selected points that meet the spacing criteria.
    /// Returns an empty array if the provided candidates list is null or empty.
    /// </returns>
    private static Vector2[] SelectSpacedPoints(List<Vector2> candidates, int maxPoints, float minSpacing)
    {
      if (candidates.NullOrEmpty())
        return [];

      candidates.Shuffle();

      List<Vector2> result = [];

      for (int i = 0; i < candidates.Count; i++)
      {
        Vector2 candidate = candidates[i];
        bool tooClose = false;

        for (int j = 0; j < result.Count; j++)
        {
          if (!(Vector2.Distance(candidate, result[j]) < minSpacing))
            continue;

          tooClose = true;
          break;
        }

        if (tooClose)
          continue;

        result.Add(candidate);

        if (result.Count >= maxPoints)
          break;
      }

      return result.ToArray();
    }

    /// <summary>
    /// Retrieves the pixel data from the specified texture as an array of colors. If the texture is not
    /// readable, this method attempts to create a readable copy of it.
    /// </summary>
    /// <param name="texture">The texture to read pixel data from.</param>
    /// <returns>
    /// An array of <c>Color</c> representing the pixel data of the texture.
    /// Returns null if the texture is null, unreadable, or a readable copy cannot be created.
    /// </returns>
    private static Color[] GetReadablePixels(Texture2D texture)
    {
      try
      {
        return texture.GetPixels();
      }
      catch
      {
        Texture2D readableCopy = CreateReadableTexture(texture);

        if (readableCopy != null)
          return readableCopy.GetPixels();

        AstraLog.Warning($"Could not create readable copy for constellation mask texture {texture.name}.");
        return null;
      }
    }

    /// <summary>
    /// Creates a readable Texture2D from a source texture. This method copies the source texture
    /// into a new texture that can be read from a script, even if the original texture was not readable.
    /// </summary>
    /// <param name="source">The source texture to create a readable copy from.</param>
    /// <returns>
    /// A new Texture2D object that is a readable copy of the source texture.
    /// Returns null if the source texture is null or if the readable copy cannot be created.
    /// </returns>
    private static Texture2D CreateReadableTexture(Texture2D source)
    {
      if (source == null)
        return null;

      RenderTexture previous = RenderTexture.active;

      RenderTexture renderTex = RenderTexture.GetTemporary(
        source.width,
        source.height,
        0,
        RenderTextureFormat.Default,
        RenderTextureReadWrite.Linear
      );

      Graphics.Blit(source, renderTex);
      RenderTexture.active = renderTex;

      Texture2D readableTexture = new(
        source.width,
        source.height,
        TextureFormat.RGBA32,
        source.mipmapCount > 1
      )
      {
        name = source.name,
        wrapMode = source.wrapMode,
        filterMode = source.filterMode,
        anisoLevel = source.anisoLevel
      };

      readableTexture.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
      readableTexture.Apply();

      RenderTexture.active = previous;
      RenderTexture.ReleaseTemporary(renderTex);

      return readableTexture;
    }
  }
}
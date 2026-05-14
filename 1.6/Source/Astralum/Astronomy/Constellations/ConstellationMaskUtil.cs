using System.Collections.Generic;
using Astralum.Debugging;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
    [StaticConstructorOnStartup]
    public static class ConstellationMaskUtil
    {
        private const string MaskFolder = "Astralum/Astronomy/Constellations";
        
        private const float StarAlphaThreshold = 0.95f;
        private const int MinStarPoints = 5;
        private const int MaxStarPoints = 7;
        private const float MinPointSpacingNormalized = 0.25f;
        
        private static readonly List<Texture2D> CachedMasks = [];
        private static readonly Dictionary<(Texture2D Mask, int StarCount), Vector2[]> StarPointCache = new();

        public static List<Texture2D> CreateShuffledMaskPool()
        {
            List<Texture2D> pool = new(CachedMasks);
            pool.Shuffle();
            return pool;
        }
        
        static ConstellationMaskUtil()
        {
            IEnumerable<Texture2D> textures = ContentFinder<Texture2D>.GetAllInFolder(MaskFolder);
            
            foreach (Texture2D texture in textures)
            {
                if (texture != null)
                    CachedMasks.Add(texture);
            }
            
            AstraLog.Message($"Loaded {CachedMasks.Count} constellation masks.");
        }
        
        public static bool HasMasks => !CachedMasks.NullOrEmpty();
        
        public static Texture2D RandomMask()
        {
            if (CachedMasks.NullOrEmpty())
                return null;
            
            return CachedMasks.RandomElement();
        }
        
        public static Texture2D GetMaskByName(string maskName)
        {
            if (maskName.NullOrEmpty())
                return null;
            
            for (int i = 0; i < CachedMasks.Count; i++)
            {
                Texture2D mask = CachedMasks[i];
                
                if (mask != null && mask.name == maskName)
                    return mask;
            }
            
            return null;
        }
        
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
            {
                for (int x = 1; x < width - 1; x++)
                {
                    Color color = pixels[y * width + x];
                    
                    if (!IsConstellationLinePixel(color))
                        continue;
                    
                    float nx = x / (width - 1f);
                    float ny = y / (height - 1f);
                    
                    candidates.Add(new Vector2(nx, ny));
                }
            }
            
            Vector2[] selected = SelectSpacedPoints(candidates, starCount, MinPointSpacingNormalized);
            
            StarPointCache[(mask, starCount)] = selected;
            return selected;
        }

        public static int RandomStarPointCount()
        {
            return Rand.RangeInclusive(MinStarPoints, MaxStarPoints);
        }
        
        private static bool IsConstellationLinePixel(Color color)
        {
            return color.a >= StarAlphaThreshold;
        }
        
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
                    if (Vector2.Distance(candidate, result[j]) < minSpacing)
                    {
                        tooClose = true;
                        break;
                    }
                }
                
                if (tooClose)
                    continue;
                
                result.Add(candidate);
                
                if (result.Count >= maxPoints)
                    break;
            }
            return result.ToArray();
        }
        
        private static Color[] GetReadablePixels(Texture2D texture)
        {
            try
            {
                return texture.GetPixels();
            }
            catch
            {
                Texture2D readableCopy = CreateReadableTexture(texture);
                
                if (readableCopy == null)
                {
                    AstraLog.Warning($"Could not create readable copy for constellation mask texture {texture.name}.");
                    return null;
                }
                
                return readableCopy.GetPixels();
            }
        }
        
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
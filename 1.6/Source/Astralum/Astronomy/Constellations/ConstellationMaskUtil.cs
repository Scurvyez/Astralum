using System.Collections.Generic;
using Astralum.Debugging;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
    [StaticConstructorOnStartup]
    public static class ConstellationMaskUtil
    {
        public readonly struct MaskConstellationData
        {
            public readonly Vector2[] points;
            
            // Stored as pairs:
            // [0, 1, 1, 2, 2, 3] = lines 0->1, 1->2, 2->3
            public readonly int[] connections;
            
            public bool Valid => points is { Length: >= 2 } && connections is { Length: >= 2 };
            
            public MaskConstellationData(Vector2[] points, int[] connections)
            {
                this.points = points;
                this.connections = connections;
            }
        }
        
        private const string MaskFolder = "Astralum/Astronomy/Constellations";
        
        private const float AlphaThreshold = 0.95f;
        private const int MaxPoints = 64;
        private const float MaxLineLengthNormalized = 0.15f;
        
        private static readonly List<Texture2D> CachedMasks;
        private static readonly Dictionary<Texture2D, MaskConstellationData> ShapeCache = new();

        static ConstellationMaskUtil()
        {
            CachedMasks = [];
            LoadMasksFromFolder(MaskFolder);
        }
        
        private static void LoadMasksFromFolder(string folder)
        {
            IEnumerable<Texture2D> textures = ContentFinder<Texture2D>.GetAllInFolder(folder);
            
            foreach (Texture2D tex in textures)
            {
                if (tex == null)
                    continue;
                
                CachedMasks.Add(tex);
            }
            
            AstraLog.Message($"Loaded {CachedMasks.Count} constellation masks.");
        }
        
        public static Texture2D RandomMask()
        {
            if (CachedMasks.NullOrEmpty())
                return null;
            
            return CachedMasks.RandomElement();
        }
        
        public static MaskConstellationData GenerateShapeFromMask(Texture2D mask)
        {
            if (ShapeCache.TryGetValue(mask, out MaskConstellationData cached))
                return cached;

            if (mask == null)
                return default;

            Color[] pixels;

            try
            {
                pixels = mask.GetPixels();
            }
            catch
            {
                Texture2D readableCopy = CreateReadableTexture(mask);

                if (readableCopy == null)
                {
                    AstraLog.Warning($"Could not create readable copy for constellation mask texture {mask.name}.");
                    return default;
                }

                pixels = readableCopy.GetPixels();
            }

            int width = mask.width;
            int height = mask.height;

            bool[] validPixels = BuildValidPixelMap(pixels, width, height);
            bool[] edgePixels = BuildEdgePixelMap(validPixels, width, height);

            List<List<Vector2Int>> components = FindConnectedComponents(edgePixels, width, height);

            if (components.NullOrEmpty())
                return default;

            MaskConstellationData data = BuildConstellationDataFromComponents(
                components,
                edgePixels,
                width,
                height
            );

            ShapeCache[mask] = data;
         
            return data;
        }
        
        private static List<List<Vector2Int>> FindConnectedComponents(bool[] edgePixels, int width, int height)
        {
            bool[] visited = new bool[edgePixels.Length];
            List<List<Vector2Int>> components = [];

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int index = y * width + x;

                    if (!edgePixels[index] || visited[index])
                        continue;

                    List<Vector2Int> component = FloodFillEdgeComponent(
                        x,
                        y,
                        edgePixels,
                        visited,
                        width,
                        height
                    );

                    if (component.Count >= 2)
                        components.Add(component);
                }
            }

            components.Sort((a, b) => b.Count.CompareTo(a.Count));

            return components;
        }
        
        private static MaskConstellationData BuildConstellationDataFromComponents(
            List<List<Vector2Int>> components,
            bool[] edgePixels,
            int width,
            int height)
        {
            List<Vector2> allPoints = [];
            List<int> allConnections = [];

            int remainingPointBudget = MaxPoints;

            for (int i = 0; i < components.Count; i++)
            {
                if (remainingPointBudget <= 0)
                    break;

                List<Vector2Int> component = components[i];

                if (component.Count < 2)
                    continue;

                List<Vector2Int> ordered = OrderEdgePixels(component);

                if (ordered.Count < 2)
                    continue;

                bool closed = IsClosedEdgeComponent(ordered, edgePixels, width, height);

                int componentPointBudget = Mathf.Clamp(
                    Mathf.RoundToInt(MaxPoints * (component.Count / (float)GetTotalComponentPixelCount(components))),
                    3,
                    remainingPointBudget
                );

                componentPointBudget = Mathf.Min(componentPointBudget, ordered.Count);

                Vector2Int[] selectedPixels = SelectEvenlyAlongOrderedPath(ordered, componentPointBudget);

                if (selectedPixels.Length < 2)
                    continue;

                int pointOffset = allPoints.Count;

                Vector2[] localPoints = PixelsToLocalPoints(selectedPixels, width, height);
                allPoints.AddRange(localPoints);

                for (int p = 0; p < selectedPixels.Length - 1; p++)
                {
                    if (!CanConnect(localPoints[p], localPoints[p + 1]))
                        continue;

                    allConnections.Add(pointOffset + p);
                    allConnections.Add(pointOffset + p + 1);
                }

                if (closed && selectedPixels.Length > 2)
                {
                    int last = selectedPixels.Length - 1;

                    if (CanConnect(localPoints[last], localPoints[0]))
                    {
                        allConnections.Add(pointOffset + last);
                        allConnections.Add(pointOffset);
                    }
                }

                remainingPointBudget -= selectedPixels.Length;
            }

            return new MaskConstellationData(
                allPoints.ToArray(),
                allConnections.ToArray()
            );
        }
        
        private static bool CanConnect(Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b) <= MaxLineLengthNormalized;
        }
        
        private static Vector2Int[] SelectEvenlyAlongOrderedPath(List<Vector2Int> orderedPixels, int count)
        {
            count = Mathf.Min(count, orderedPixels.Count);

            if (count <= 0)
                return [];

            List<Vector2Int> selected = [];

            float step = orderedPixels.Count / (float)count;

            for (int i = 0; i < count; i++)
            {
                int index = Mathf.Clamp(
                    Mathf.RoundToInt(i * step),
                    0,
                    orderedPixels.Count - 1
                );

                selected.Add(orderedPixels[index]);
            }

            return selected.ToArray();
        }
        
        private static int GetTotalComponentPixelCount(List<List<Vector2Int>> components)
        {
            int total = 0;

            for (int i = 0; i < components.Count; i++)
                total += components[i].Count;

            return Mathf.Max(1, total);
        }

        private static Vector2[] PixelsToLocalPoints(Vector2Int[] pixels, int width, int height)
        {
            Vector2[] result = new Vector2[pixels.Length];

            for (int i = 0; i < pixels.Length; i++)
            {
                Vector2Int p = pixels[i];

                float nx = p.x / (width - 1f);
                float ny = p.y / (height - 1f);

                result[i] = new Vector2(
                    nx * 2f - 1f,
                    ny * 2f - 1f
                );
            }

            return result;
        }
        
        private static List<Vector2Int> OrderEdgePixels(List<Vector2Int> component)
        {
            if (component.Count <= 2)
                return component;

            Vector2 center = Vector2.zero;

            for (int i = 0; i < component.Count; i++)
                center += component[i];

            center /= component.Count;

            component.Sort((a, b) =>
            {
                float angleA = Mathf.Atan2(a.y - center.y, a.x - center.x);
                float angleB = Mathf.Atan2(b.y - center.y, b.x - center.x);

                return angleA.CompareTo(angleB);
            });

            return component;
        }
        
        private static bool IsClosedEdgeComponent(
            List<Vector2Int> orderedPixels,
            bool[] edgePixels,
            int width,
            int height)
        {
            if (orderedPixels.Count < 3)
                return false;

            int endpointCount = 0;

            for (int i = 0; i < orderedPixels.Count; i++)
            {
                Vector2Int p = orderedPixels[i];
                int neighborCount = 0;

                for (int oy = -1; oy <= 1; oy++)
                {
                    for (int ox = -1; ox <= 1; ox++)
                    {
                        if (ox == 0 && oy == 0)
                            continue;

                        int nx = p.x + ox;
                        int ny = p.y + oy;

                        if (nx <= 0 || ny <= 0 || nx >= width - 1 || ny >= height - 1)
                            continue;

                        if (edgePixels[ny * width + nx])
                            neighborCount++;
                    }
                }

                if (neighborCount <= 1)
                    endpointCount++;
            }

            return endpointCount == 0;
        }
        
        private static bool IsValidPixel(Color[] pixels, int width, int x, int y)
        {
            Color c = pixels[y * width + x];
            float brightness = (c.r + c.g + c.b) / 3f;
            
            return c.a >= AlphaThreshold && brightness < 0.25f;
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
            
            Texture2D readableTexture = new Texture2D(
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
            
            readableTexture.ReadPixels(
                new Rect(0, 0, renderTex.width, renderTex.height),
                0,
                0
            );
            
            readableTexture.Apply();
            
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            
            return readableTexture;
        }
        
        private static bool[] BuildValidPixelMap(Color[] pixels, int width, int height)
        {
            bool[] result = new bool[pixels.Length];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    result[y * width + x] = IsValidPixel(pixels, width, x, y);
            }

            return result;
        }

        private static bool[] BuildEdgePixelMap(bool[] validPixels, int width, int height)
        {
            bool[] result = new bool[validPixels.Length];

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int index = y * width + x;

                    if (!validPixels[index])
                        continue;

                    bool edge = false;

                    for (int oy = -1; oy <= 1 && !edge; oy++)
                    {
                        for (int ox = -1; ox <= 1; ox++)
                        {
                            if (ox == 0 && oy == 0)
                                continue;

                            int nx = x + ox;
                            int ny = y + oy;

                            if (!validPixels[ny * width + nx])
                            {
                                edge = true;
                                break;
                            }
                        }
                    }

                    result[index] = edge;
                }
            }

            return result;
        }
        
        private static List<Vector2Int> FloodFillEdgeComponent(
            int startX,
            int startY,
            bool[] edgePixels,
            bool[] visited,
            int width,
            int height)
        {
            List<Vector2Int> result = [];
            Queue<Vector2Int> queue = new();

            Vector2Int start = new(startX, startY);
            queue.Enqueue(start);
            visited[startY * width + startX] = true;

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                result.Add(current);

                for (int oy = -1; oy <= 1; oy++)
                {
                    for (int ox = -1; ox <= 1; ox++)
                    {
                        if (ox == 0 && oy == 0)
                            continue;

                        int nx = current.x + ox;
                        int ny = current.y + oy;

                        if (nx <= 0 || ny <= 0 || nx >= width - 1 || ny >= height - 1)
                            continue;

                        int index = ny * width + nx;

                        if (!edgePixels[index] || visited[index])
                            continue;

                        visited[index] = true;
                        queue.Enqueue(new Vector2Int(nx, ny));
                    }
                }
            }

            return result;
        }
    }
}
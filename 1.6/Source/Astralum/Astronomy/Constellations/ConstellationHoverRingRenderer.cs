using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
  public static class ConstellationHoverRingRenderer
  {
    private const float RingQuadSize = 0.5f;

    public static void PrintRing(Vector3 localSkyPos, LayerSubMesh subMesh)
    {
      WorldRendererUtility.PrintQuadTangentialToPlanet(localSkyPos, RingQuadSize, 0f,
        subMesh, true);
    }
  }
}
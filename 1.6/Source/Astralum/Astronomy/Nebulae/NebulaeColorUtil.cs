using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Nebulae
{
  public static class NebulaeColorUtil
  {
    public static Color[] RandomNebulaPalette()
    {
      int palette = Rand.RangeInclusive(0, 11);

      switch (palette)
      {
        case 0: // blue / magenta
          return
          [
            new Color(0.04f, 0.06f, 0.12f, 1f),
            new Color(0.16f, 0.28f, 0.75f, 1f),
            new Color(0.65f, 0.20f, 0.85f, 1f),
            new Color(1.00f, 0.62f, 0.28f, 1f)
          ];

        case 1: // dusty amber
          return
          [
            new Color(0.08f, 0.055f, 0.035f, 1f),
            new Color(0.35f, 0.18f, 0.08f, 1f),
            new Color(0.85f, 0.38f, 0.12f, 1f),
            new Color(1.00f, 0.78f, 0.35f, 1f)
          ];

        case 2: // cyan / violet
          return
          [
            new Color(0.035f, 0.07f, 0.09f, 1f),
            new Color(0.08f, 0.48f, 0.65f, 1f),
            new Color(0.35f, 0.22f, 0.95f, 1f),
            new Color(0.85f, 0.65f, 1.00f, 1f)
          ];

        case 3: // muted dust cloud
          return
          [
            new Color(0.035f, 0.035f, 0.04f, 1f),
            new Color(0.16f, 0.13f, 0.10f, 1f),
            new Color(0.38f, 0.28f, 0.20f, 1f),
            new Color(0.70f, 0.55f, 0.38f, 1f)
          ];

        case 4: // red / purple emission
          return
          [
            new Color(0.08f, 0.02f, 0.04f, 1f),
            new Color(0.45f, 0.05f, 0.12f, 1f),
            new Color(0.78f, 0.18f, 0.55f, 1f),
            new Color(1.00f, 0.55f, 0.82f, 1f)
          ];

        case 5: // green / teal
          return
          [
            new Color(0.02f, 0.06f, 0.05f, 1f),
            new Color(0.05f, 0.35f, 0.25f, 1f),
            new Color(0.10f, 0.72f, 0.58f, 1f),
            new Color(0.70f, 1.00f, 0.88f, 1f)
          ];

        case 6: // magenta / pink
          return
          [
            new Color(0.06f, 0.02f, 0.08f, 1f),
            new Color(0.38f, 0.08f, 0.45f, 1f),
            new Color(0.90f, 0.20f, 0.75f, 1f),
            new Color(1.00f, 0.72f, 0.92f, 1f)
          ];

        case 7: // green / blue
          return
          [
            new Color(0.02f, 0.05f, 0.08f, 1f),
            new Color(0.04f, 0.22f, 0.45f, 1f),
            new Color(0.08f, 0.62f, 0.48f, 1f),
            new Color(0.72f, 0.95f, 1.00f, 1f)
          ];

        case 8: // gold / violet
          return
          [
            new Color(0.05f, 0.035f, 0.08f, 1f),
            new Color(0.32f, 0.16f, 0.55f, 1f),
            new Color(0.95f, 0.58f, 0.18f, 1f),
            new Color(1.00f, 0.92f, 0.55f, 1f)
          ];

        case 9: // crimson / orange / smoke
          return
          [
            new Color(0.07f, 0.025f, 0.018f, 1f),
            new Color(0.42f, 0.05f, 0.035f, 1f),
            new Color(0.95f, 0.28f, 0.08f, 1f),
            new Color(1.00f, 0.78f, 0.38f, 1f)
          ];

        case 10: // icy blue / white
          return
          [
            new Color(0.025f, 0.04f, 0.08f, 1f),
            new Color(0.08f, 0.28f, 0.62f, 1f),
            new Color(0.38f, 0.78f, 1.00f, 1f),
            new Color(0.92f, 0.98f, 1.00f, 1f)
          ];

        default: // toxic green / yellow
          return
          [
            new Color(0.025f, 0.055f, 0.025f, 1f),
            new Color(0.12f, 0.42f, 0.10f, 1f),
            new Color(0.62f, 0.95f, 0.18f, 1f),
            new Color(1.00f, 0.92f, 0.35f, 1f)
          ];
      }
    }
  }
}
using Astralum.Astronomy.LocalStars;
using UnityEngine;
using Verse;

namespace Astralum.Materials
{
  public static class CelestialColorGetter
  {
    public static Color ConstellationLineColor = new(0.45f, 0.60f, 1.0f, 0.35f);
    public static Color PulsarCoreColor = Color.white;
    public static Color PulsarJetColor = new(0.55f, 0.85f, 1.0f, 1f);
    public static Color PulsarShellDarkColor = new(0.01f, 0.025f, 0.075f, 1f);
    public static Color PulsarShellBrightColor = new(0.12f, 0.45f, 1.0f, 1f);
    public static Color ShootingStarColor = new(0.75f, 0.9f, 1f, 0.85f);
    public static Color SkyCoordGridLineColor = new(0.35f, 0.65f, 1f, 0.28f);
    
    public static Color TryGetStarColorFor(SpectralClass spectralClass)
    {
      return spectralClass switch
      {
        SpectralClass.O => new Color(0.62f, 0.78f, 1f, 1f),
        SpectralClass.B => new Color(0.70f, 0.85f, 1f, 1f),
        SpectralClass.A => new Color(0.85f, 0.93f, 1f, 1f),
        SpectralClass.F => new Color(1f, 0.98f, 0.88f, 1f),
        SpectralClass.G => new Color(1f, 0.90f, 0.62f, 1f),
        SpectralClass.K => new Color(1f, 0.62f, 0.32f, 1f),
        SpectralClass.M => new Color(1f, 0.34f, 0.22f, 1f),
        _ => Color.white
      };
    }
    
    public static void TryGetRandomDustLanePalette(out Color colorA, out Color colorB)
    {
      int palette = Rand.RangeInclusive(0, 4);
      
      switch (palette)
      {
        case 0: // cool blue-gray
          colorA = new Color(0.08f, 0.10f, 0.14f, 1f);
          colorB = new Color(0.18f, 0.22f, 0.28f, 1f);
          break;
        
        case 1: // blue
          colorA = new Color(0.06f, 0.10f, 0.18f, 1f);
          colorB = new Color(0.14f, 0.22f, 0.35f, 1f);
          break;
        
        case 2: // brown dust
          colorA = new Color(0.10f, 0.08f, 0.06f, 1f);
          colorB = new Color(0.24f, 0.18f, 0.12f, 1f);
          break;
        
        case 3: // purple
          colorA = new Color(0.08f, 0.05f, 0.12f, 1f);
          colorB = new Color(0.20f, 0.12f, 0.28f, 1f);
          break;
        
        default: // neutral gray
          colorA = new Color(0.10f, 0.10f, 0.10f, 1f);
          colorB = new Color(0.22f, 0.22f, 0.22f, 1f);
          break;
      }
    }
    
    public static Color[] TryGetRandomNebulaPalette()
    {
      int palette = Rand.RangeInclusive(0, 21);
      
      switch (palette)
      {
        case 0: // blue / magenta
          return
          [
            new Color(0.04f, 0.06f, 0.12f, 1f), new Color(0.16f, 0.28f, 0.75f, 1f),
            new Color(0.65f, 0.20f, 0.85f, 1f), new Color(1.00f, 0.62f, 0.28f, 1f)
          ];

        case 1: // dusty amber
          return
          [
            new Color(0.08f, 0.055f, 0.035f, 1f), new Color(0.35f, 0.18f, 0.08f, 1f),
            new Color(0.85f, 0.38f, 0.12f, 1f), new Color(1.00f, 0.78f, 0.35f, 1f)
          ];

        case 2: // cyan / violet
          return
          [
            new Color(0.035f, 0.07f, 0.09f, 1f), new Color(0.08f, 0.48f, 0.65f, 1f),
            new Color(0.35f, 0.22f, 0.95f, 1f), new Color(0.85f, 0.65f, 1.00f, 1f)
          ];

        case 3: // muted dust cloud
          return
          [
            new Color(0.035f, 0.035f, 0.04f, 1f), new Color(0.16f, 0.13f, 0.10f, 1f),
            new Color(0.38f, 0.28f, 0.20f, 1f), new Color(0.70f, 0.55f, 0.38f, 1f)
          ];

        case 4: // red / purple emission
          return
          [
            new Color(0.08f, 0.02f, 0.04f, 1f), new Color(0.45f, 0.05f, 0.12f, 1f),
            new Color(0.78f, 0.18f, 0.55f, 1f), new Color(1.00f, 0.55f, 0.82f, 1f)
          ];

        case 5: // green / teal
          return
          [
            new Color(0.02f, 0.06f, 0.05f, 1f), new Color(0.05f, 0.35f, 0.25f, 1f),
            new Color(0.10f, 0.72f, 0.58f, 1f), new Color(0.70f, 1.00f, 0.88f, 1f)
          ];

        case 6: // magenta / pink
          return
          [
            new Color(0.06f, 0.02f, 0.08f, 1f), new Color(0.38f, 0.08f, 0.45f, 1f),
            new Color(0.90f, 0.20f, 0.75f, 1f), new Color(1.00f, 0.72f, 0.92f, 1f)
          ];

        case 7: // green / blue
          return
          [
            new Color(0.02f, 0.05f, 0.08f, 1f), new Color(0.04f, 0.22f, 0.45f, 1f),
            new Color(0.08f, 0.62f, 0.48f, 1f), new Color(0.72f, 0.95f, 1.00f, 1f)
          ];

        case 8: // gold / violet
          return
          [
            new Color(0.05f, 0.035f, 0.08f, 1f), new Color(0.32f, 0.16f, 0.55f, 1f),
            new Color(0.95f, 0.58f, 0.18f, 1f), new Color(1.00f, 0.92f, 0.55f, 1f)
          ];

        case 9: // crimson / orange / smoke
          return
          [
            new Color(0.07f, 0.025f, 0.018f, 1f), new Color(0.42f, 0.05f, 0.035f, 1f),
            new Color(0.95f, 0.28f, 0.08f, 1f), new Color(1.00f, 0.78f, 0.38f, 1f)
          ];

        case 10: // icy blue / white
          return
          [
            new Color(0.025f, 0.04f, 0.08f, 1f), new Color(0.08f, 0.28f, 0.62f, 1f),
            new Color(0.38f, 0.78f, 1.00f, 1f), new Color(0.92f, 0.98f, 1.00f, 1f)
          ];

        case 11: // toxic green / yellow
          return
          [
            new Color(0.025f, 0.055f, 0.025f, 1f), new Color(0.12f, 0.42f, 0.10f, 1f),
            new Color(0.62f, 0.95f, 0.18f, 1f), new Color(1.00f, 0.92f, 0.35f, 1f)
          ];

        case 12: // cobalt / rust / pearl
          return
          [
            new Color(0.015f, 0.025f, 0.09f, 1f), new Color(0.04f, 0.22f, 0.85f, 1f),
            new Color(0.95f, 0.28f, 0.06f, 1f), new Color(0.98f, 0.90f, 0.74f, 1f)
          ];

        case 13: // emerald / violet / ember
          return
          [
            new Color(0.015f, 0.06f, 0.035f, 1f), new Color(0.00f, 0.72f, 0.38f, 1f),
            new Color(0.48f, 0.10f, 0.92f, 1f), new Color(1.00f, 0.36f, 0.08f, 1f)
          ];

        case 14: // indigo / acid green / rose
          return
          [
            new Color(0.025f, 0.018f, 0.10f, 1f), new Color(0.18f, 0.08f, 0.78f, 1f),
            new Color(0.58f, 1.00f, 0.08f, 1f), new Color(1.00f, 0.34f, 0.62f, 1f)
          ];

        case 15: // black teal / solar orange / cyan
          return
          [
            new Color(0.010f, 0.045f, 0.055f, 1f), new Color(0.00f, 0.55f, 0.62f, 1f),
            new Color(1.00f, 0.44f, 0.04f, 1f), new Color(0.70f, 1.00f, 0.96f, 1f)
          ];

        case 16: // deep purple / lime / hot magenta
          return
          [
            new Color(0.055f, 0.015f, 0.075f, 1f), new Color(0.34f, 0.04f, 0.72f, 1f),
            new Color(0.74f, 0.98f, 0.12f, 1f), new Color(1.00f, 0.18f, 0.82f, 1f)
          ];

        case 17: // navy / copper / glacier
          return
          [
            new Color(0.012f, 0.022f, 0.065f, 1f), new Color(0.08f, 0.18f, 0.52f, 1f),
            new Color(0.86f, 0.34f, 0.10f, 1f), new Color(0.78f, 0.96f, 1.00f, 1f)
          ];

        case 18: // blood-red / cyan lightning
          return
          [
            new Color(0.075f, 0.008f, 0.012f, 1f), new Color(0.58f, 0.015f, 0.035f, 1f),
            new Color(0.02f, 0.78f, 0.95f, 1f), new Color(1.00f, 0.72f, 0.48f, 1f)
          ];

        case 19: // jade / ultramarine / gold
          return
          [
            new Color(0.012f, 0.055f, 0.045f, 1f), new Color(0.00f, 0.58f, 0.42f, 1f),
            new Color(0.06f, 0.12f, 0.95f, 1f), new Color(1.00f, 0.82f, 0.16f, 1f)
          ];

        case 20: // violet shadow / orange / mint
          return
          [
            new Color(0.045f, 0.018f, 0.070f, 1f), new Color(0.44f, 0.06f, 0.82f, 1f),
            new Color(1.00f, 0.48f, 0.06f, 1f), new Color(0.58f, 1.00f, 0.74f, 1f)
          ];

        default: // sapphire / crimson / pale gold
          return
          [
            new Color(0.010f, 0.028f, 0.085f, 1f), new Color(0.02f, 0.32f, 0.95f, 1f),
            new Color(0.88f, 0.04f, 0.16f, 1f), new Color(1.00f, 0.88f, 0.42f, 1f)
          ];
      }
    }
  }
}
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Nebulae
{
  public class SavedNebula : IExposable
  {
    public float alpha;

    public float cloudThreshold;

    public Color colorA;
    public Color colorB;
    public float colorBandSharpness;
    public Color colorC;
    public Color colorD;

    public float colorStopB;
    public float colorStopC;
    public Vector4 coreOffset;
    public float edgeSoftness;

    public float intensity;
    public Vector3 localSkyPos;
    public string name;
    public int nebulaId;

    public float noiseScale;
    public float noiseStrength;
    public float rotationDegrees;
    public float seed;

    public Vector4 seedOffset;
    public float shaderRotation;
    public float shapePower;
    public float size;

    public float stretchX;
    public float stretchY;

    public float warpScale;
    public float warpStrength;

    public void ExposeData()
    {
      Scribe_Values.Look(ref name, "name");
      Scribe_Values.Look(ref nebulaId, "nebulaId");
      Scribe_Values.Look(ref localSkyPos, "localSkyPos");
      Scribe_Values.Look(ref size, "size");
      Scribe_Values.Look(ref rotationDegrees, "rotationDegrees");

      Scribe_Values.Look(ref colorA, "colorA");
      Scribe_Values.Look(ref colorB, "colorB");
      Scribe_Values.Look(ref colorC, "colorC");
      Scribe_Values.Look(ref colorD, "colorD");

      Scribe_Values.Look(ref colorStopB, "colorStopB");
      Scribe_Values.Look(ref colorStopC, "colorStopC");
      Scribe_Values.Look(ref colorBandSharpness, "colorBandSharpness");

      Scribe_Values.Look(ref seedOffset, "seedOffset");
      Scribe_Values.Look(ref seed, "seed");

      Scribe_Values.Look(ref intensity, "intensity");
      Scribe_Values.Look(ref alpha, "alpha");

      Scribe_Values.Look(ref noiseScale, "noiseScale");
      Scribe_Values.Look(ref noiseStrength, "noiseStrength");

      Scribe_Values.Look(ref cloudThreshold, "cloudThreshold");
      Scribe_Values.Look(ref edgeSoftness, "edgeSoftness");

      Scribe_Values.Look(ref warpScale, "warpScale");
      Scribe_Values.Look(ref warpStrength, "warpStrength");
      Scribe_Values.Look(ref shapePower, "shapePower");
      Scribe_Values.Look(ref coreOffset, "coreOffset");

      Scribe_Values.Look(ref stretchX, "stretchX");
      Scribe_Values.Look(ref stretchY, "stretchY");
      Scribe_Values.Look(ref shaderRotation, "shaderRotation");
    }
  }
}
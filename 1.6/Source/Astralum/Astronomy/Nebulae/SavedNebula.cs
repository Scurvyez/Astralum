using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Nebulae
{
    public class SavedNebula : IExposable
    {
        public string name;
        public int nebulaId;
        public Vector3 localSkyPos;
        public float size;
        public float rotationDegrees;
        
        public Color colorA;
        public Color colorB;
        public Color colorC;
        public Color colorD;
        
        public float colorStopB;
        public float colorStopC;
        public float colorBandSharpness;
        
        public Vector4 seedOffset;
        public float seed;
        
        public float intensity;
        public float alpha;
        
        public float noiseScale;
        public float noiseStrength;
        
        public float cloudThreshold;
        public float edgeSoftness;
        
        public float warpScale;
        public float warpStrength;
        public float shapePower;
        public Vector4 coreOffset;
        
        public float stretchX;
        public float stretchY;
        public float shaderRotation;
        
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
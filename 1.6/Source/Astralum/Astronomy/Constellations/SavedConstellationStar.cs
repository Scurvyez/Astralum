using Astralum.Astronomy.LocalSystem.Stars;
using UnityEngine;
using Verse;

namespace Astralum.Astronomy.Constellations
{
    public class SavedConstellationStar : IExposable
    {
        public string name;
        public Vector2 uv;
        public Vector3 localSkyPos;
        public SpectralClass spectralClass;
        public float visualSize;
        public float rotationDegrees;
        
        public void ExposeData()
        {
            Scribe_Values.Look(ref name, "name");
            Scribe_Values.Look(ref uv, "uv");
            Scribe_Values.Look(ref localSkyPos, "localSkyPos");
            Scribe_Values.Look(ref spectralClass, "spectralClass");
            Scribe_Values.Look(ref visualSize, "visualSize");
            Scribe_Values.Look(ref rotationDegrees, "rotationDegrees");
        }
    }
}
using UnityEngine;

namespace Astralum.Astronomy.SkyGrid
{
    public readonly struct SkyGridLabel
    {
        public readonly string text;
        public readonly Vector3 localSkyPos;
        public readonly Vector2 guiOffset;
        public readonly float scale;
        
        public SkyGridLabel(string text, Vector3 localSkyPos, Vector2 guiOffset = default, float scale = 1f)
        {
            this.text = text;
            this.localSkyPos = localSkyPos;
            this.guiOffset = guiOffset;
            this.scale = scale;
        }
    }
}
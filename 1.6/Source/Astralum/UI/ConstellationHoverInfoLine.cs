using UnityEngine;

namespace Astralum.UI
{
    public readonly struct ConstellationHoverInfoLine
    {
        public readonly string Text;
        public readonly Color? SwatchColor;
        
        public ConstellationHoverInfoLine(string text, Color? swatchColor = null)
        {
            Text = text;
            SwatchColor = swatchColor;
        }
    }
}
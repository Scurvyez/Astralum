using UnityEngine;

namespace Astralum.UI
{
  public readonly struct CelestialObjectHoverInfoLine
  {
    public readonly string Text;
    public readonly Color? SwatchColor;
    
    public CelestialObjectHoverInfoLine(string text, Color? swatchColor = null)
    {
      Text = text;
      SwatchColor = swatchColor;
    }
  }
}
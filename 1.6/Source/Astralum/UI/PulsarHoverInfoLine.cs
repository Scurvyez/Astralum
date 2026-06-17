using UnityEngine;

namespace Astralum.UI
{
  public readonly struct PulsarHoverInfoLine
  {
    public readonly string Text;
    public readonly Color? SwatchColor;
    
    public PulsarHoverInfoLine(string text, Color? swatchColor = null)
    {
      Text = text;
      SwatchColor = swatchColor;
    }
  }
}
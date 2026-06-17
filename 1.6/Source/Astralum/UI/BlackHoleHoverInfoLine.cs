using UnityEngine;

namespace Astralum.UI
{
  public readonly struct BlackHoleHoverInfoLine
  {
    public readonly string Text;
    public readonly Color? SwatchColor;
  
    public BlackHoleHoverInfoLine(string text, Color? swatchColor = null)
    {
      Text = text;
      SwatchColor = swatchColor;
    }
  }
}
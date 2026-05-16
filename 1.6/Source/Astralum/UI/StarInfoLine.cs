using UnityEngine;

namespace Astralum.UI
{
  public struct StarInfoLine
  {
    public readonly string Text;
    public readonly Color? SwatchColor;

    public StarInfoLine(string text, Color? swatchColor = null)
    {
      Text = text;
      SwatchColor = swatchColor;
    }
  }
}
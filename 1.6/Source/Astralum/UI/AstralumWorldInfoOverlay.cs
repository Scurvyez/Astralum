using System.Collections.Generic;
using Astralum.Astronomy.Stars;
using Astralum.World;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Astralum.UI
{
    public static class AstralumWorldInfoOverlay
    {
        private const float WindowWidth = 260f;
        private const float Padding = 12f;
        private const float TopPadding = 10f;
        private const float BottomPadding = 10f;

        public static void DrawOnGUI(bool requirePlaying = false)
        {
            if (!ShouldDraw(requirePlaying))
                return;

            SavedStar star = WorldUtils.CurrentStar;

            if (star == null)
                return;

            List<StarInfoLine> lines = StarInfoLineCache.GetLines(star);

            Rect rect = GetRect(lines);

            Widgets.DrawWindowBackground(rect);
            DrawContents(rect, lines);
        }

        private static bool ShouldDraw(bool requirePlaying)
        {
            if (requirePlaying && Current.ProgramState != ProgramState.Playing)
                return false;

            if (Find.World == null)
                return false;

            if (Find.WorldCamera == null || !Find.WorldCamera.gameObject.activeInHierarchy)
                return false;

            if (!WorldRendererUtility.WorldSelected)
                return false;

            if (Find.UIRoot?.screenshotMode?.FiltersCurrentEvent == true)
                return false;

            return true;
        }

        private static Rect GetRect(List<StarInfoLine> lines)
        {
            return new Rect(
                Screen.width - WindowWidth - 16f,
                120f,
                WindowWidth,
                GetHeight(lines)
            );
        }

        private static float GetHeight(List<StarInfoLine> lines)
        {
            return TopPadding + BottomPadding + lines.Count * GetLineHeight();
        }

        private static float GetLineHeight()
        {
            Text.Font = GameFont.Small;
            return Text.LineHeight;
        }

        private static void DrawContents(Rect windowRect, List<StarInfoLine> lines)
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            float lineHeight = GetLineHeight();
            float y = windowRect.y + TopPadding;

            foreach (StarInfoLine line in lines)
            {
                Rect lineRect = new(
                    windowRect.x + Padding,
                    y,
                    windowRect.width - Padding * 2f,
                    lineHeight
                );

                DrawInfoLine(lineRect, line);

                y += lineHeight;
            }

            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }

        private static void DrawInfoLine(Rect rect, StarInfoLine line)
        {
            if (line.SwatchColor == null)
            {
                Widgets.Label(rect, line.Text);
                return;
            }

            const float swatchSize = 14f;
            const float swatchGap = 6f;

            Rect labelRect = new(
                rect.x,
                rect.y,
                rect.width - swatchSize - swatchGap,
                rect.height
            );

            Rect swatchRect = new(
                rect.xMax - swatchSize,
                rect.y + (rect.height - swatchSize) / 2f,
                swatchSize,
                swatchSize
            );

            Widgets.Label(labelRect, line.Text);

            Widgets.DrawBoxSolid(swatchRect, line.SwatchColor.Value);
            Widgets.DrawBox(swatchRect);
        }
    }
}
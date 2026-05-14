using System.Collections.Generic;
using Astralum.Astronomy.LocalSystem.Stars;
using Astralum.World;
using UnityEngine;
using Verse;

namespace Astralum.UI
{
    public class AstralumWorldInfoWindow : Window
    {
        private const float WindowWidth = 260f;
        private const float Padding = 12f;
        private const float TopPadding = 10f;
        private const float BottomPadding = 10f;
        
        public override Vector2 InitialSize => new(WindowWidth, GetWindowHeight());
        
        public AstralumWorldInfoWindow()
        {
            draggable = true;
            doCloseX = false;
            doCloseButton = false;
            closeOnCancel = false;
            absorbInputAroundWindow = false;
            closeOnClickedOutside = false;
            preventCameraMotion = false;
            drawShadow = false;
        }
        
        public override void DoWindowContents(Rect inRect)
        {
            SavedStar star = WorldUtils.CurrentStar;
            
            if (star == null)
                return;
            
            SaveCurrentWindowPos();
            
            List<StarInfoLine> lines = StarInfoLineCache.GetLines(star);
            
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            
            float lineHeight = GetLineHeight();
            float y = TopPadding;
            
            foreach (StarInfoLine line in lines)
            {
                Rect lineRect = new(Padding, y, inRect.width - Padding * 2f, lineHeight);
                
                DrawInfoLine(lineRect, line);
                y += lineHeight;
            }
            
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }
        
        public override void PreOpen()
        {
            base.PreOpen();
            
            WorldComponent_LocalStar comp = Find.World.GetComponent<WorldComponent_LocalStar>();
            
            windowRect = comp.HasSavedStarInfoWindowPos
                ? new Rect(comp.StarInfoWindowPos.x, comp.StarInfoWindowPos.y, WindowWidth, GetWindowHeight())
                : GetDefaultWindowRect();
        }
        
        public void RefreshSize()
        {
            windowRect.height = GetWindowHeight();
        }
        
        private static Rect GetDefaultWindowRect()
        {
            return new Rect(Screen.width - WindowWidth - 16f, 120f, WindowWidth, GetWindowHeight());
        }
        
        private static float GetWindowHeight()
        {
            SavedStar star = WorldUtils.CurrentStar;
            
            if (star == null)
                return 90f;
            
            return TopPadding + BottomPadding + StarInfoLineCache.GetLines(star).Count * GetLineHeight();
        }
        
        private static float GetLineHeight()
        {
            Text.Font = GameFont.Small;
            return Text.LineHeight;
        }
        
        private void SaveCurrentWindowPos()
        {
            WorldComponent_LocalStar comp = Find.World.GetComponent<WorldComponent_LocalStar>();
            comp?.StarInfoWindowPos = windowRect.position;
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
            
            Rect labelRect = new(rect.x, rect.y, rect.width - swatchSize - swatchGap, rect.height);
            
            Rect swatchRect = new(rect.xMax - swatchSize, rect.y + (rect.height - swatchSize) / 2f,
                swatchSize, swatchSize
            );
            
            Widgets.Label(labelRect, line.Text);
            
            Color oldColor = GUI.color;
            Widgets.DrawBoxSolid(swatchRect, line.SwatchColor.Value);
            GUI.color = oldColor;
            
            Widgets.DrawBox(swatchRect);
        }
    }
}
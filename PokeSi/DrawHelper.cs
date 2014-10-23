using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace PokeSi
{
    public static class DrawHelper
    {
        public static Texture2D PixelBlanc;

        public static void Load(PokeSiGame game)
        {
            PixelBlanc = game.Content.Load<Texture2D>("pixelBlanc.png");
        }

        public static Rectangle ClampRectangle(Rectangle toClamp, Rectangle limit)
        {
            toClamp.Left = Math.Max(toClamp.Left, limit.Left);
            toClamp.Right = Math.Min(toClamp.Right, limit.Right);
            toClamp.Top = Math.Max(toClamp.Top, limit.Top);
            toClamp.Bottom = Math.Min(toClamp.Bottom, limit.Bottom);
            return toClamp;
        }

        public static Rectangle ExtendToContain(Rectangle toExtend, Rectangle toContain, int centerX = 0, int centerY = 0)
        {
            float horizontalRatio = toContain.Width / (float)toExtend.Width;
            float verticalRatio = toContain.Height / (float)toExtend.Height;
            float ratio = Math.Max(horizontalRatio, verticalRatio);
            Rectangle result = toExtend;
            result.X -= (int)(centerX * (ratio - 1));
            result.Y -= (int)(centerY * (ratio - 1));
            result.Width = (int)(toExtend.Width * ratio);
            result.Height = (int)(toExtend.Height * ratio);
            return result;
        }

        public static Vector2 ClampVector(Vector2 toClamp, Vector2 min, Vector2 max)
        {
            float xMin = Math.Min(min.X, max.X);
            float yMin = Math.Min(min.Y, max.Y);
            float xMax = Math.Max(min.X, max.X);
            float yMax = Math.Max(min.Y, max.Y);
            toClamp.X = MathUtil.Clamp(toClamp.X, xMin, xMax);
            toClamp.Y = MathUtil.Clamp(toClamp.Y, yMin, yMax);
            return toClamp;
        }

        public static void DrawCross(SpriteBatch spriteBatch, Vector2 position, Rectangle containedIn, Color color)
        {
            spriteBatch.Draw(PixelBlanc, new Rectangle(containedIn.X, (int)position.Y, containedIn.Width, 1), color);
            spriteBatch.Draw(PixelBlanc, new Rectangle((int)position.X, containedIn.Y, 1, containedIn.Height), color);
        }

        public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            spriteBatch.Draw(PixelBlanc, new Rectangle(rect.X, rect.Y, rect.Width, 1), color);
            spriteBatch.Draw(PixelBlanc, new Rectangle(rect.X, rect.Y, 1, rect.Height), color);
            spriteBatch.Draw(PixelBlanc, new Rectangle(rect.X, rect.Y + rect.Height, rect.Width, 1), color);
            spriteBatch.Draw(PixelBlanc, new Rectangle(rect.X + rect.Width, rect.Y, 1, rect.Height), color);
        }

        public static void DrawFilledRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            spriteBatch.Draw(PixelBlanc, rect, color);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Sprites;

namespace PokeSi.Screens.Controls
{
    public class TextureDrawer : Control
    {
        public Texture2D Texture { get; set; }

        public int PreferableWidth { get { return Texture.Width; } }
        public int PreferableHeight { get { return Texture.Height; } }

        public float Zoom { get; protected set; }
        public Vector2 Position { get; protected set; }

        public Rectangle? SelectedRectangle { get; protected set; }

        public TextureDrawer(Screen screen, Rectangle bound, Texture2D texture)
            : base(screen)
        {
            Bound = bound;
            Texture = texture;
            Zoom = 1f;
            Position = new Vector2(0, 0);
        }

        private Vector2 selectionStartPoint;
        private bool isSelecting;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (DestinationRect.Contains(Input.X, Input.Y) && Texture != null)
            {
                if (Input.WheelDelta != 0)
                    Zoom += 10 * Zoom / Input.WheelDelta;
                Zoom = MathUtil.Clamp(Zoom, 0.05f, 1f);

                if (Input.MiddleButton.Down)
                    Position -= new Vector2(Input.XDelta, Input.YDelta) * Zoom;
                Vector2 max = new Vector2((float)(Texture.Width - DestinationRect.Width * ratioWPixelScreenPicture),
                            (float)(Texture.Height - DestinationRect.Height * ratioHPixelScreenPicture));
                max.X = max.X < 0 ? 0 : max.X;
                max.Y = max.Y < 0 ? 0 : max.Y;
                Position = DrawHelper.ClampVector(Position, Vector2.Zero, max);

                if (Input.LeftButton.Pressed)
                {
                    isSelecting = true;
                    SelectedRectangle = null;
                    selectionStartPoint = ConvertScreenToPicture(new Vector2(Input.X, Input.Y));
                }
            }

            if (Input.LeftButton.Released && isSelecting == true)
            {
                isSelecting = false;
                Vector2 selectionEndPoint = ConvertScreenToPicture(new Vector2(Input.X, Input.Y));
                Rectangle selectionRect = new Rectangle();
                selectionRect.Left = Math.Min((int)selectionStartPoint.X, (int)selectionEndPoint.X);
                selectionRect.Right = Math.Max((int)selectionStartPoint.X, (int)selectionEndPoint.X) + 1;
                selectionRect.Top = Math.Min((int)selectionStartPoint.Y, (int)selectionEndPoint.Y);
                selectionRect.Bottom = Math.Max((int)selectionStartPoint.Y, (int)selectionEndPoint.Y) + 1;
                SelectedRectangle = selectionRect;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (Texture != null)
            {
                RectangleF rect = DestinationRect;
                float rectRatio = rect.Width / (float)rect.Height;
                float textureRatio = Texture.Width / (float)Texture.Height;
                if (rectRatio < textureRatio)
                {
                    float ratio = rectRatio / textureRatio;
                    rect.Height = rect.Height * ratio;
                }
                if (rectRatio > textureRatio)
                {
                    float ratio = textureRatio / rectRatio;
                    rect.Width = rect.Width * ratio;
                }
                rect.Width = rect.Width / Zoom;
                rect.Height = rect.Height / Zoom;

                RectangleF final = rect;
                final.Width = Math.Min(DestinationRect.Width, rect.Width);
                final.Height = Math.Min(DestinationRect.Height, rect.Height);

                double wRatio = final.Width / (double)rect.Width;
                double hRatio = final.Height / (double)rect.Height;

                Rectangle sourceRect = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * wRatio), (int)(Texture.Height * hRatio));

                ratioWPixelScreenPicture = sourceRect.Width / final.Width;
                ratioHPixelScreenPicture = sourceRect.Height / final.Height;

                spriteBatch.Draw(Texture, new Rectangle((int)final.X, (int)final.Y, (int)final.Width, (int)final.Height), sourceRect, Color.White);

                Vector2 crossPos = new Vector2(Input.X, Input.Y);
                if (DestinationRect.Contains(crossPos))
                    DrawHelper.DrawCross(spriteBatch, crossPos, DestinationRect, Color.DeepPink);

                if (isSelecting)
                {
                    Rectangle selectionRect = new Rectangle();
                    Vector2 selectPoint = ConvertPictureToScreen(new Vector2(selectionStartPoint.X + 0.4f, selectionStartPoint.Y + 0.4f));
                    selectionRect.Left = Math.Min((int)selectPoint.X, Input.X);
                    selectionRect.Right = Math.Max((int)selectPoint.X, Input.X);
                    selectionRect.Top = Math.Min((int)selectPoint.Y, Input.Y);
                    selectionRect.Bottom = Math.Max((int)selectPoint.Y, Input.Y);
                    DrawHelper.DrawRectangle(spriteBatch, selectionRect, Color.Blue);

                    Color greenA = Color.Green; greenA.A = 100;
                    DrawHelper.DrawFilledRectangle(spriteBatch, DrawHelper.ClampRectangle(MagnetToPicture(selectionRect), DestinationRect), greenA);
                }
                else if (SelectedRectangle != null)
                {
                    Color greenA = Color.Green; greenA.A = 100;
                    Rectangle selected = (Rectangle)SelectedRectangle;
                    selected.Right -= 1; selected.Bottom -= 1;
                    DrawHelper.DrawFilledRectangle(spriteBatch, DrawHelper.ClampRectangle(ConvertPictureToScreen(selected), DestinationRect), greenA);
                }
            }
        }

        private double ratioWPixelScreenPicture;
        private double ratioHPixelScreenPicture;
        public Vector2 ConvertScreenToPicture(Vector2 onScreen)
        {
            if (Texture == null || ratioWPixelScreenPicture == 0 || ratioHPixelScreenPicture == 0)
                return Vector2.Zero;

            Vector2 onControl = onScreen - DestinationRect.Location;

            Rectangle rect = Bound;
            rect.X = 0; rect.Y = 0;
            if (!rect.Contains(onControl))
                return Vector2.Zero;

            Vector2 onImage = new Vector2((int)(onControl.X * ratioWPixelScreenPicture), (int)(onControl.Y * ratioHPixelScreenPicture));
            Vector2 onPicture = onImage + new Vector2((int)Position.X, (int)Position.Y);
            while (onPicture.X < 0)
                onPicture.X += Texture.Width;
            while (onPicture.Y < 0)
                onPicture.Y += Texture.Height;
            onPicture.X %= Texture.Width;
            onPicture.Y %= Texture.Height;
            return onPicture;
        }
        public Vector2 ConvertPictureToScreen(Vector2 onPicture)
        {
            if (Texture == null || ratioWPixelScreenPicture == 0 || ratioHPixelScreenPicture == 0)
                return Vector2.Zero;

            Vector2 onImage = onPicture - new Vector2((int)Position.X, (int)Position.Y);
            onImage.X %= Texture.Width;
            onImage.Y %= Texture.Height;
            Vector2 onControl = new Vector2((int)(onImage.X / ratioWPixelScreenPicture), (int)(onImage.Y / ratioHPixelScreenPicture));
            return onControl + DestinationRect.Location;
        }
        public Rectangle ConvertPictureToScreen(Rectangle rect)
        {
            Vector2 topLeft = new Vector2(rect.X, rect.Y);
            Vector2 botRight = new Vector2(rect.Right, rect.Bottom);
            topLeft = ConvertPictureToScreen(topLeft);
            botRight.X += 0.99f; botRight.Y += 0.99f;
            botRight = ConvertPictureToScreen(botRight);
            return new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)botRight.X - (int)topLeft.X, (int)botRight.Y - (int)topLeft.Y);
        }

        public Rectangle MagnetToPicture(Rectangle rect)
        {
            Vector2 topLeft = new Vector2(rect.X, rect.Y);
            Vector2 botRight = new Vector2(rect.Right, rect.Bottom);
            topLeft = ConvertScreenToPicture(topLeft);
            topLeft = ConvertPictureToScreen(topLeft);
            botRight = ConvertScreenToPicture(botRight);
            botRight.X += 0.99f; botRight.Y += 0.99f;
            botRight = ConvertPictureToScreen(botRight);
            return new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)botRight.X - (int)topLeft.X, (int)botRight.Y - (int)topLeft.Y);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace PokeSi.Sprites
{
    public class AnimationPlayer
    {
        public Animation Animation { get; protected set; }
        public int FrameIndex { get; protected set; }

        public Sprite CurrentSprite
        {
            get
            {
                if (Animation == null)
                    return null;
                return Animation.Sprites[FrameIndex];
            }
        }

        public AnimationPlayer()
        {

        }

        public void PlayAnimation(Animation animation)
        {
            // If this animation is already running, do not restart it.
            if (Animation == animation)
                return;

            // Start the new animation.
            Animation = animation;
            FrameIndex = 0;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle destinationRect, Color color, float depth = 0.5f, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            FrameIndex = (int)(gameTime.TotalGameTime.TotalSeconds / Animation.FrameTime % Animation.FrameCount);

            if (CurrentSprite != null)
                spriteBatch.Draw(CurrentSprite.Sheet.Texture, destinationRect,
                    Animation.Sprites[FrameIndex].SourceRect,
                    color, 0.0f, Vector2.Zero, spriteEffects, depth);
        }
    }
}

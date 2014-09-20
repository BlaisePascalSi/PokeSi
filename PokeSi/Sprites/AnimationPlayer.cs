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

        private float time;

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
            time = 0.0f;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle destinationRect, Color color, float depth = 0.5f, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            FrameIndex = (int)(gameTime.TotalGameTime.TotalSeconds / Animation.FrameTime % Animation.FrameCount);

            spriteBatch.Draw(Animation.SpriteSheet.Sheet, destinationRect,
                Animation.SpriteSheet.GetSpriteRect(Animation.XBase + Animation.XMult * FrameIndex, Animation.YBase + Animation.YMult * FrameIndex),
                color, 0.0f, Vector2.Zero, spriteEffects, depth);
        }
    }
}

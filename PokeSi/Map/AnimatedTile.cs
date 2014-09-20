using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Sprites;

namespace PokeSi.Map
{
    public class AnimatedTile : Tile
    {
        public Animation Animation { get; protected set; }
        public AnimationPlayer AnimationPlayer { get; protected set; }

        public AnimatedTile(World world, Animation animation)
            : base(world)
        {
            Animation = animation;
            AnimationPlayer = new AnimationPlayer();
            AnimationPlayer.PlayAnimation(Animation);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, int x, int y, Rectangle destinationRect)
        {
            AnimationPlayer.Draw(gameTime, spriteBatch, destinationRect, Color.White);
        }
    }
}

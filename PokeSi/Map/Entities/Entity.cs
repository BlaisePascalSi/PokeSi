using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Sprites;

namespace PokeSi.Map.Entities
{
    public class Entity
    {
        public World World { get; protected set; }
        public string Name { get; protected set; }
        public float X { get; set; }
        public float Y { get; set; }
        public Vector2 Position
        {
            get { return new Vector2(X, Y); }
            set { X = value.X; Y = value.Y; }
        }
        private Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public AnimationPlayer AnimationPlayer { get; protected set; }
        private Vector2 origin;
        public Vector2 Origin
        {
            get { return origin; }
            protected set { origin = value; }
        }

        public Rectangle DestinationRect
        {
            get { return new Rectangle((int)(X - Origin.X), (int)(Y - Origin.Y), (int)(AnimationPlayer.Animation.SpriteSheet.SpriteWidth * World.ScalingFactor), (int)(AnimationPlayer.Animation.SpriteSheet.SpriteHeight * World.ScalingFactor)); }
        }

        public Entity(World world)
        {
            World = world;

            AnimationPlayer = new AnimationPlayer();
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            AnimationPlayer.Draw(gameTime, spriteBatch, DestinationRect, Color.White, 0.6f, AnimationPlayer.Animation.SpriteEffect);
        }
    }
}

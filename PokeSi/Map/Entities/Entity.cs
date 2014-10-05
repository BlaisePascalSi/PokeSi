using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Sprites;
using PokeSi.Map.Tiles;

namespace PokeSi.Map.Entities
{
    public class Entity
    {
        public static class Controllers
        {
            public readonly static Controller Keyboard = new KeyboardController();
            public readonly static Controller SimpleAI = new SimpleAIController();

            public static Controller Get(string name)
            {
                if (name == "Keyboard")
                    return Keyboard;
                if (name == "SimpleAI")
                    return SimpleAI;
                return null;
            }

            public static string GetName(Controller controller)
            {
                if (controller == Keyboard)
                    return "Keyboard";
                if (controller == SimpleAI)
                    return "SimpleAI";
                return "";
            }

            public static List<string> GetAll()
            {
                return new List<string>(new string[]{
                    "Keyboard",
                    "SimpleAI"
                });
            }
        };

        public World World { get; protected set; }
        public Controller Controller { get; protected set; }
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
        protected float SpeedCoefficient = 1;
        public float Speed { get { return SpeedCoefficient * Tile.Width * World.ScalingFactor; } }

        public AnimationPlayer AnimationPlayer { get; protected set; }
        public Vector2 Origin
        {
            get { return AnimationPlayer.CurrentSprite.Origin; }
        }

        public Rectangle DestinationRect
        {
            get { return new Rectangle((int)(X - Origin.X), (int)(Y - Origin.Y), (int)(AnimationPlayer.CurrentSprite.Width * World.ScalingFactor), (int)(AnimationPlayer.CurrentSprite.Height * World.ScalingFactor)); }
        }

        public Entity(World world)
        {
            World = world;
            AnimationPlayer = new AnimationPlayer();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Controller != null)
                Controller.Update(gameTime, this);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            AnimationPlayer.Draw(gameTime, spriteBatch, DestinationRect, Color.White, 0.6f, AnimationPlayer.Animation.SpriteEffect);
        }

        public virtual void Save(XmlDocument doc, XmlElement parent)
        {
            parent.AppendChild(XmlHelper.CreateSimpleNode("Type", this.GetType().Name, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("X", X, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("Y", Y, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("Speed", SpeedCoefficient, doc));
        }

        public static Entity Unserialize(XmlDocument doc, XmlElement parent, World world)
        {
            if (!XmlHelper.HasChild("Type", parent))
                return null;

            string type = (string)XmlHelper.GetSimpleNodeContent<string>("Type", parent, "");
            Entity result = null;
            switch (type)
            {
                case "Person":
                    {
                        result = new Person(doc, parent, world);
                        break;
                    }
            }
            result.X = (float)XmlHelper.GetSimpleNodeContent<float>("X", parent, 0);
            result.Y = (float)XmlHelper.GetSimpleNodeContent<float>("Y", parent, 0);
            result.SpeedCoefficient = (float)XmlHelper.GetSimpleNodeContent<float>("Speed", parent, 3);
            return result;
        }
    }
}

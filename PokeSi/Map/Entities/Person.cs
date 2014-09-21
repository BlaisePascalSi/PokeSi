using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using PokeSi.Sprites;
using PokeSi.Map.Tiles;

namespace PokeSi.Map.Entities
{
    public class Person : Entity
    {
        public enum Direction
        {
            Down,
            Up,
            Right,
            Left
        };

        public SpriteSheet SpriteSheet;
        private Animation[] Idle;
        private Animation[] Walking;
        private Animation[] Running;
        public Direction CurrentDirection { get; protected set; }

        private float SpeedCoefficient;
        public float Speed { get { return SpeedCoefficient * Tile.Width * World.ScalingFactor; } }

        public Person(World world, string name)
            : base(world)
        {
            SpriteSheet = new SpriteSheet(World.Game, "Entities/player.png", 32, 32);
            Idle = new Animation[4];
            Idle[(int)Direction.Down] = new Animation(SpriteSheet, 0, 1, 0, 0);
            Idle[(int)Direction.Up] = new Animation(SpriteSheet, 0, 1, 1, 0);
            Idle[(int)Direction.Right] = new Animation(SpriteSheet, 0, 1, 2, 0);
            Idle[(int)Direction.Left] = new Animation(SpriteSheet, 0, 1, 2, 0);
            Idle[(int)Direction.Left].SpriteEffect = SpriteEffects.FlipHorizontally;
            Walking = new Animation[4];
            Walking[(int)Direction.Down] = new Animation(SpriteSheet, 0.25f, 4, 0, 1);
            Walking[(int)Direction.Up] = new Animation(SpriteSheet, 0.25f, 4, 4, 1);
            Walking[(int)Direction.Right] = new Animation(SpriteSheet, 0.25f, 4, 8, 1);
            Walking[(int)Direction.Left] = new Animation(SpriteSheet, 0.25f, 4, 8, 1);
            Walking[(int)Direction.Left].SpriteEffect = SpriteEffects.FlipHorizontally;
            Running = new Animation[4];
            Running[(int)Direction.Down] = new Animation(SpriteSheet, 0.25f, 4, 0, 2);
            Running[(int)Direction.Up] = new Animation(SpriteSheet, 0.25f, 4, 4, 2);
            Running[(int)Direction.Right] = new Animation(SpriteSheet, 0.25f, 4, 8, 2);
            Running[(int)Direction.Left] = new Animation(SpriteSheet, 0.25f, 4, 8, 2);
            Running[(int)Direction.Left].SpriteEffect = SpriteEffects.FlipHorizontally;

            Name = name;
            AnimationPlayer.PlayAnimation(Idle[(int)Direction.Down]);
            CurrentDirection = Direction.Down;
            SpeedCoefficient = 3;
        }
        public Person(XmlDocument doc, XmlElement parent, World world)
            : base(world)
        {
            Name = (string)XmlHelper.GetSimpleNodeContent<string>("Name", parent, "Player");
            CurrentDirection = (Direction)Enum.Parse(typeof(Direction), (string)XmlHelper.GetSimpleNodeContent<string>("Direction", parent, "Down"));
            X = (float)XmlHelper.GetSimpleNodeContent<float>("X", parent, 0);
            Y = (float)XmlHelper.GetSimpleNodeContent<float>("Y", parent, 0);
            SpeedCoefficient = (float)XmlHelper.GetSimpleNodeContent<float>("Speed", parent, 3);

            XmlElement sheetElem = XmlHelper.GetElement("Sheet", parent);
            SpriteSheet = new SpriteSheet(doc, sheetElem, World.Game);

            bool idleLoaded = true;
            XmlElement idleElem = XmlHelper.GetElement("Idle", parent);
            Idle = new Animation[4];
            for (int i = 0; i < 4; i++)
            {
                XmlElement elem = XmlHelper.GetElement(((Direction)i).ToString(), idleElem);
                if (elem != null)
                    Idle[i] = new Animation(doc, elem, SpriteSheet);
                else
                    idleLoaded = false;
            }
            if (!idleLoaded)
            {
                Idle = new Animation[4];
                Idle[(int)Direction.Down] = new Animation(SpriteSheet, 0, 1, 0, 0);
                Idle[(int)Direction.Up] = new Animation(SpriteSheet, 0, 1, 1, 0);
                Idle[(int)Direction.Right] = new Animation(SpriteSheet, 0, 1, 2, 0);
                Idle[(int)Direction.Left] = new Animation(SpriteSheet, 0, 1, 2, 0);
                Idle[(int)Direction.Left].SpriteEffect = SpriteEffects.FlipHorizontally;
            }

            bool walkingLoaded = true;
            XmlElement walkingElem = XmlHelper.GetElement("Walking", parent);
            Walking = new Animation[4];
            for (int i = 0; i < 4; i++)
            {
                XmlElement elem = XmlHelper.GetElement(((Direction)i).ToString(), walkingElem);
                if (elem != null)
                    Walking[i] = new Animation(doc, elem, SpriteSheet);
                else
                    walkingLoaded = false;
            }
            if (!walkingLoaded)
            {
                Walking = new Animation[4];
                Walking[(int)Direction.Down] = new Animation(SpriteSheet, 0.25f, 4, 0, 1);
                Walking[(int)Direction.Up] = new Animation(SpriteSheet, 0.25f, 4, 4, 1);
                Walking[(int)Direction.Right] = new Animation(SpriteSheet, 0.25f, 4, 8, 1);
                Walking[(int)Direction.Left] = new Animation(SpriteSheet, 0.25f, 4, 8, 1);
                Walking[(int)Direction.Left].SpriteEffect = SpriteEffects.FlipHorizontally;
            }

            bool runningLoaded = true;
            XmlElement runningElem = XmlHelper.GetElement("Running", parent);
            Running = new Animation[4];
            for (int i = 0; i < 4; i++)
            {
                XmlElement elem = XmlHelper.GetElement(((Direction)i).ToString(), runningElem);
                if (elem != null)
                    Running[i] = new Animation(doc, elem, SpriteSheet);
                else
                    runningLoaded = false;
            }
            if (!runningLoaded)
            {
                Running = new Animation[4];
                Running[(int)Direction.Down] = new Animation(SpriteSheet, 0.25f, 4, 0, 2);
                Running[(int)Direction.Up] = new Animation(SpriteSheet, 0.25f, 4, 4, 2);
                Running[(int)Direction.Right] = new Animation(SpriteSheet, 0.25f, 4, 8, 2);
                Running[(int)Direction.Left] = new Animation(SpriteSheet, 0.25f, 4, 8, 2);
                Running[(int)Direction.Left].SpriteEffect = SpriteEffects.FlipHorizontally;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 toMove = Vector2.Zero;
            if (Input.IsKeyDown(Keys.S) || Input.IsKeyDown(Keys.Down))
                toMove.Y += 1;
            if (Input.IsKeyDown(Keys.Z) || Input.IsKeyDown(Keys.Up))
                toMove.Y -= 1;
            if (Input.IsKeyDown(Keys.D) || Input.IsKeyDown(Keys.Right))
                toMove.X += 1;
            if (Input.IsKeyDown(Keys.Q) || Input.IsKeyDown(Keys.Left))
                toMove.X -= 1;
            toMove.Normalize();

            Velocity = toMove * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity;

            if (Velocity.Y > 0)
                CurrentDirection = Direction.Down;
            if (Velocity.Y < 0)
                CurrentDirection = Direction.Up;
            if (Velocity.X > 0)
                CurrentDirection = Direction.Right;
            if (Velocity.X < 0)
                CurrentDirection = Direction.Left;

            if (Velocity == Vector2.Zero)
                AnimationPlayer.PlayAnimation(Idle[(int)CurrentDirection]);
            else
                AnimationPlayer.PlayAnimation(Walking[(int)CurrentDirection]);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

        public void Save(XmlDocument doc, XmlNode parent)
        {
            parent.AppendChild(XmlHelper.CreateSimpleNode("Name", Name, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("Direction", CurrentDirection, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("X", X, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("Y", Y, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("Speed", SpeedCoefficient, doc));

            XmlElement sheetElem = doc.CreateElement("Sheet");
            SpriteSheet.Save(doc, sheetElem);
            parent.AppendChild(sheetElem);

            XmlElement idleElem = doc.CreateElement("Idle");
            for (int i = 0; i < 4; i++)
            {
                XmlElement elem = doc.CreateElement(((Direction)i).ToString());
                Idle[i].Save(doc, elem);
                idleElem.AppendChild(elem);
            }
            parent.AppendChild(idleElem);

            XmlElement walkingElem = doc.CreateElement("Walking");
            for (int i = 0; i < 4; i++)
            {
                XmlElement elem = doc.CreateElement(((Direction)i).ToString());
                Walking[i].Save(doc, elem);
                walkingElem.AppendChild(elem);
            }
            parent.AppendChild(walkingElem);

            XmlElement runningElem = doc.CreateElement("Running");
            for (int i = 0; i < 4; i++)
            {
                XmlElement elem = doc.CreateElement(((Direction)i).ToString());
                Running[i].Save(doc, elem);
                runningElem.AppendChild(elem);
            }
            parent.AppendChild(runningElem);
        }
    }
}

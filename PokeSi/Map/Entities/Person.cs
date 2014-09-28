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

namespace PokeSi.Map.Entities
{
    public class Person : Entity, IEditable
    {
        public enum Direction
        {
            Down,
            Up,
            Right,
            Left
        };

        public string Name { get; protected set; }
        public SpriteSheet SpriteSheet;
        private Animation[] Idle;
        private Animation[] Walking;
        private Animation[] Running;
        public Direction CurrentDirection { get; protected set; }
        public Rectangle Bound { get { return DestinationRect; } }

        public Person(World world, string name, Controller controller = null)
            : base(world)
        {
            Controller = controller;
            SpriteSheet = new SpriteSheet(World.Screen.Manager.Game, "Entities/player.png", 32, 32);
            Idle = new Animation[4];
            Idle[(int)Direction.Down] = new Animation(SpriteSheet, world.Resources, 0, 1, 0, 0);
            Idle[(int)Direction.Up] = new Animation(SpriteSheet, world.Resources, 0, 1, 1, 0);
            Idle[(int)Direction.Right] = new Animation(SpriteSheet, world.Resources, 0, 1, 2, 0);
            Idle[(int)Direction.Left] = new Animation(SpriteSheet, world.Resources, 0, 1, 2, 0);
            Idle[(int)Direction.Left].SpriteEffect = SpriteEffects.FlipHorizontally;
            Walking = new Animation[4];
            Walking[(int)Direction.Down] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 0, 1);
            Walking[(int)Direction.Up] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 4, 1);
            Walking[(int)Direction.Right] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 8, 1);
            Walking[(int)Direction.Left] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 8, 1);
            Walking[(int)Direction.Left].SpriteEffect = SpriteEffects.FlipHorizontally;
            Running = new Animation[4];
            Running[(int)Direction.Down] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 0, 2);
            Running[(int)Direction.Up] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 4, 2);
            Running[(int)Direction.Right] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 8, 2);
            Running[(int)Direction.Left] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 8, 2);
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
            Controller = Entity.Controllers.Get((string)XmlHelper.GetSimpleNodeContent<string>("Controller", parent, ""));

            SpriteSheet = World.Resources.GetSpriteSheet((string)XmlHelper.GetSimpleNodeContent<string>("Sheet", parent, "Entities_player.png"));
            if (SpriteSheet == null)
            {
                SpriteSheet = new SpriteSheet(World.Screen.Manager.Game, "Entities/player.png", 32, 32);
                World.Resources.Add("Entities_player.png", SpriteSheet);
            }

            bool idleLoaded = true;
            XmlElement idleElem = XmlHelper.GetElement("Idle", parent);
            if (idleElem != null)
            {
                Idle = new Animation[4];
                for (int i = 0; i < 4; i++)
                {
                    Idle[i] = world.Resources.GetAnimation((string)XmlHelper.GetSimpleNodeContent<string>(((Direction)i).ToString(), idleElem, "idle_" + ((Direction)i).ToString().ToLower()));
                    if (Idle[i] == null)
                        idleLoaded = false;
                }
            }
            else
                idleLoaded = false;
            if (!idleLoaded)
            {
                Idle = new Animation[4];
                Idle[(int)Direction.Down] = new Animation(SpriteSheet, world.Resources, 0, 1, 0, 0);
                Idle[(int)Direction.Up] = new Animation(SpriteSheet, world.Resources, 0, 1, 1, 0);
                Idle[(int)Direction.Right] = new Animation(SpriteSheet, world.Resources, 0, 1, 2, 0);
                Idle[(int)Direction.Left] = new Animation(SpriteSheet, world.Resources, 0, 1, 2, 0);
                Idle[(int)Direction.Left].SpriteEffect = SpriteEffects.FlipHorizontally;
                World.Resources.Add("idle_down", Idle[(int)Direction.Down]);
                World.Resources.Add("idle_up", Idle[(int)Direction.Up]);
                World.Resources.Add("idle_right", Idle[(int)Direction.Right]);
                World.Resources.Add("idle_left", Idle[(int)Direction.Left]);
            }

            bool walkingLoaded = true;
            XmlElement walkingElem = XmlHelper.GetElement("Walking", parent);
            if (walkingElem != null)
            {
                Walking = new Animation[4];
                for (int i = 0; i < 4; i++)
                {
                    Walking[i] = world.Resources.GetAnimation((string)XmlHelper.GetSimpleNodeContent<string>(((Direction)i).ToString(), walkingElem, "walking_" + ((Direction)i).ToString().ToLower()));
                    if (Walking[i] == null)
                        walkingLoaded = false;
                }
            }
            else
                walkingLoaded = false;
            if (!walkingLoaded)
            {
                Walking = new Animation[4];
                Walking[(int)Direction.Down] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 0, 1);
                Walking[(int)Direction.Up] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 4, 1);
                Walking[(int)Direction.Right] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 8, 1);
                Walking[(int)Direction.Left] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 8, 1);
                Walking[(int)Direction.Left].SpriteEffect = SpriteEffects.FlipHorizontally;
                World.Resources.Add("walking_down", Walking[(int)Direction.Down]);
                World.Resources.Add("walking_up", Walking[(int)Direction.Up]);
                World.Resources.Add("walking_right", Walking[(int)Direction.Right]);
                World.Resources.Add("walking_left", Walking[(int)Direction.Left]);
            }

            bool runningLoaded = true;
            XmlElement runningElem = XmlHelper.GetElement("Running", parent);
            if (runningElem != null)
            {
                Running = new Animation[4];
                for (int i = 0; i < 4; i++)
                {
                    Running[i] = world.Resources.GetAnimation((string)XmlHelper.GetSimpleNodeContent<string>(((Direction)i).ToString(), runningElem, "running_" + ((Direction)i).ToString().ToLower()));
                    if (Running[i] == null)
                        runningLoaded = false;
                }
            }
            else
                runningLoaded = false;
            if (!runningLoaded)
            {
                Running = new Animation[4];
                Running[(int)Direction.Down] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 0, 2);
                Running[(int)Direction.Up] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 4, 2);
                Running[(int)Direction.Right] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 8, 2);
                Running[(int)Direction.Left] = new Animation(SpriteSheet, world.Resources, 0.25f, 4, 8, 2);
                Running[(int)Direction.Left].SpriteEffect = SpriteEffects.FlipHorizontally;
                World.Resources.Add("running_down", Walking[(int)Direction.Down]);
                World.Resources.Add("running_up", Walking[(int)Direction.Up]);
                World.Resources.Add("running_right", Walking[(int)Direction.Right]);
                World.Resources.Add("running_left", Walking[(int)Direction.Left]);
            }
            AnimationPlayer.PlayAnimation(Idle[(int)CurrentDirection]);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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

        public override void Save(XmlDocument doc, XmlElement parent)
        {
            base.Save(doc, parent);

            parent.AppendChild(XmlHelper.CreateSimpleNode("Name", Name, doc));
            parent.AppendChild(XmlHelper.CreateSimpleNode("Direction", CurrentDirection, doc));
            if (Controller != null)
                parent.AppendChild(XmlHelper.CreateSimpleNode("Controller", Controller, doc));

            parent.AppendChild(XmlHelper.CreateSimpleNode("Sheet", World.Resources.GetName(SpriteSheet), doc));
            /*XmlElement sheetElem = doc.CreateElement("Sheet");
            SpriteSheet.Save(doc, sheetElem);
            parent.AppendChild(sheetElem);*/

            XmlElement idleElem = doc.CreateElement("Idle");
            for (int i = 0; i < 4; i++)
            {
                idleElem.AppendChild(XmlHelper.CreateSimpleNode(((Direction)i).ToString(), World.Resources.GetName(Idle[i]), doc));
                /*XmlElement elem = doc.CreateElement(((Direction)i).ToString());
                Idle[i].Save(doc, elem);
                idleElem.AppendChild(elem);*/
            }
            parent.AppendChild(idleElem);

            XmlElement walkingElem = doc.CreateElement("Walking");
            for (int i = 0; i < 4; i++)
            {
                walkingElem.AppendChild(XmlHelper.CreateSimpleNode(((Direction)i).ToString(), World.Resources.GetName(Walking[i]), doc));
                /*XmlElement elem = doc.CreateElement(((Direction)i).ToString());
                Walking[i].Save(doc, elem);
                walkingElem.AppendChild(elem);*/
            }
            parent.AppendChild(walkingElem);

            XmlElement runningElem = doc.CreateElement("Running");
            for (int i = 0; i < 4; i++)
            {
                runningElem.AppendChild(XmlHelper.CreateSimpleNode(((Direction)i).ToString(), World.Resources.GetName(Running[i]), doc));
                /*XmlElement elem = doc.CreateElement(((Direction)i).ToString());
                Running[i].Save(doc, elem);
                runningElem.AppendChild(elem);*/
            }
            parent.AppendChild(runningElem);
        }

        public Form GetEditingForm()
        {
            Form result = new Form();

            result.Datas.Add("X", X);
            result.Datas.Add("Y", Y);
            result.Datas.Add("Speed", SpeedCoefficient);
            result.Datas.Add("Name", Name);
            result.Datas.Add("Direction", CurrentDirection);
            result.Datas.Add("Controller", Controller);
            result.Datas.Add("Sheet", SpriteSheet);

            return result;
        }

        public void SubmitForm(Form form)
        {
            X = (float)form.Datas["X"];
            Y = (float)form.Datas["Y"];
            SpeedCoefficient = (float)form.Datas["Speed"];
            Name = (string)form.Datas["Name"];
            CurrentDirection = (Direction)form.Datas["Direction"];
            Controller = (Controller)form.Datas["Controller"];
            SpriteSheet = (SpriteSheet)form.Datas["Sheet"];
        }
    }
}

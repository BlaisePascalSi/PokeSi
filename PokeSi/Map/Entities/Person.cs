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
            SpriteSheet = new SpriteSheet(World.Screen.Manager.Game, "Entities/player.png"); // TODO : Revome with editor
            world.Resources.Add("player.png", SpriteSheet);
            Idle = new Animation[4];
            Walking = new Animation[4];
            Running = new Animation[4];

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

            SpriteSheet = World.Resources.GetSpriteSheet((string)XmlHelper.GetSimpleNodeContent<string>("Sheet", parent, "player.png"));
            if (SpriteSheet == null)
            {
                SpriteSheet = new SpriteSheet(World.Screen.Manager.Game, "Entities/player.png"); // TODO : Revome with editor
                World.Resources.Add("player.png", SpriteSheet);
            }

            XmlElement idleElem = XmlHelper.GetElement("Idle", parent);
            Idle = new Animation[4];
            if (idleElem != null)
            {
                for (int i = 0; i < 4; i++)
                    Idle[i] = world.Resources.GetAnimation((string)XmlHelper.GetSimpleNodeContent<string>(((Direction)i).ToString(), idleElem, "idle_" + ((Direction)i).ToString().ToLower()));
            }
            if (Idle[(int)Direction.Down] == null)
                Idle[(int)Direction.Down] = World.Resources.GetAnimation("");
            if (Idle[(int)Direction.Up] == null)
                Idle[(int)Direction.Up] = World.Resources.GetAnimation("");
            if (Idle[(int)Direction.Right] == null)
                Idle[(int)Direction.Right] = World.Resources.GetAnimation("");
            if (Idle[(int)Direction.Left] == null)
                Idle[(int)Direction.Left] = World.Resources.GetAnimation("");

            XmlElement walkingElem = XmlHelper.GetElement("Walking", parent);
            Walking = new Animation[4];
            if (walkingElem != null)
            {
                for (int i = 0; i < 4; i++)
                    Walking[i] = world.Resources.GetAnimation((string)XmlHelper.GetSimpleNodeContent<string>(((Direction)i).ToString(), walkingElem, "walking_" + ((Direction)i).ToString().ToLower()));
            }
            if (Walking[(int)Direction.Down] == null)
                Walking[(int)Direction.Down] = World.Resources.GetAnimation("");
            if (Walking[(int)Direction.Up] == null)
                Walking[(int)Direction.Up] = World.Resources.GetAnimation("");
            if (Walking[(int)Direction.Right] == null)
                Walking[(int)Direction.Right] = World.Resources.GetAnimation("");
            if (Walking[(int)Direction.Left] == null)
                Walking[(int)Direction.Left] = World.Resources.GetAnimation("");

            XmlElement runningElem = XmlHelper.GetElement("Running", parent);
                Running = new Animation[4];
            if (runningElem != null)
            {
                for (int i = 0; i < 4; i++)
                    Running[i] = world.Resources.GetAnimation((string)XmlHelper.GetSimpleNodeContent<string>(((Direction)i).ToString(), runningElem, "running_" + ((Direction)i).ToString().ToLower()));
            }
            if (Running[(int)Direction.Down] == null)
                Running[(int)Direction.Down] = World.Resources.GetAnimation("");
            if (Running[(int)Direction.Up] == null)
                Running[(int)Direction.Up] = World.Resources.GetAnimation("");
            if (Running[(int)Direction.Right] == null)
                Running[(int)Direction.Right] = World.Resources.GetAnimation("");
            if (Running[(int)Direction.Left] == null)
                Running[(int)Direction.Left] = World.Resources.GetAnimation("");
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

            XmlElement idleElem = doc.CreateElement("Idle");
            for (int i = 0; i < 4; i++)
                idleElem.AppendChild(XmlHelper.CreateSimpleNode(((Direction)i).ToString(), World.Resources.GetName(Idle[i]), doc));
            parent.AppendChild(idleElem);

            XmlElement walkingElem = doc.CreateElement("Walking");
            for (int i = 0; i < 4; i++)
                walkingElem.AppendChild(XmlHelper.CreateSimpleNode(((Direction)i).ToString(), World.Resources.GetName(Walking[i]), doc));
            parent.AppendChild(walkingElem);

            XmlElement runningElem = doc.CreateElement("Running");
            for (int i = 0; i < 4; i++)
                runningElem.AppendChild(XmlHelper.CreateSimpleNode(((Direction)i).ToString(), World.Resources.GetName(Running[i]), doc));
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
            result.Datas.Add("Idle Down", Idle[(int)Direction.Down]);
            result.Datas.Add("Idle Up", Idle[(int)Direction.Up]);
            result.Datas.Add("Idle Right", Idle[(int)Direction.Right]);
            result.Datas.Add("Idle Left", Idle[(int)Direction.Left]);
            result.Datas.Add("Walk Down", Walking[(int)Direction.Down]);
            result.Datas.Add("Walk Up", Walking[(int)Direction.Up]);
            result.Datas.Add("Walk Right", Walking[(int)Direction.Right]);
            result.Datas.Add("Walk Left", Walking[(int)Direction.Left]);

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
            Idle[(int)Direction.Down] = (Animation)form.Datas["Idle Down"];
            Idle[(int)Direction.Up] = (Animation)form.Datas["Idle Up"];
            Idle[(int)Direction.Right] = (Animation)form.Datas["Idle Right"];
            Idle[(int)Direction.Left] = (Animation)form.Datas["Idle Left"];
            Walking[(int)Direction.Down] = (Animation)form.Datas["Walk Down"];
            Walking[(int)Direction.Up] = (Animation)form.Datas["Walk Up"];
            Walking[(int)Direction.Right] = (Animation)form.Datas["Walk Right"];
            Walking[(int)Direction.Left] = (Animation)form.Datas["Walk Left"];
        }
    }
}

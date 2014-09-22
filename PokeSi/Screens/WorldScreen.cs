using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Map;

namespace PokeSi.Screens
{
    public class WorldScreen : Screen
    {
        public World World { get; protected set; }

        public WorldScreen(ScreenManager manager)
            : base(manager)
        {
            OpeningTransition = new NoneTransition(Transition.Types.Opening);
            ClosingTransition = new NoneTransition(Transition.Types.Closing);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            World = new World(this);

            XmlDocument doc = new XmlDocument();
            doc.Load("save.xml");

            XmlElement worldElem = (XmlElement)doc.GetElementsByTagName("World").Item(0);
            World.Load(doc, worldElem);
        }

        public override void Close()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement world = doc.CreateElement("World");
            World.Save(doc, world);
            doc.AppendChild(world);
            doc.Save("save.xml");

            base.Close();
        }

        public override void Update(GameTime gameTime, bool isInForeground)
        {
            base.Update(gameTime, isInForeground);

            if (!isInForeground)
                return;

            World.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, bool isInForeground, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, isInForeground, spriteBatch);

            World.Draw(gameTime, spriteBatch);
        }
    }
}

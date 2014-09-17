using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace PokeSi.Screens
{
    public class MainMenuScreen : Screen
    {
        private Texture2D background;

        public MainMenuScreen(ScreenManager manager)
            : base(manager)
        {
            OpeningTransition = new NoneTransition(Transition.Types.Opening);
            ClosingTransition = new NoneTransition(Transition.Types.Closing);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            background = Manager.Game.Content.Load<Texture2D>("background.bmp");
        }

        public override void Update(GameTime gameTime, bool isInForeground)
        {
            base.Update(gameTime, isInForeground);


        }

        public override void Draw(GameTime gameTime, bool isInForeground, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, isInForeground, spriteBatch);

            spriteBatch.Draw(background, Manager.Game.Viewport, Color.White);
        }
    }
}

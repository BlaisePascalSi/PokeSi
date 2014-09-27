using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace PokeSi.Screens
{
    public class ScreenManager
    {
        public PokeSiGame Game;

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        private Stack<Screen> Screens;
        private Screen toOpenWhenCleared;

        public ScreenManager(PokeSiGame game)
        {
            Game = game;
            Screens = new Stack<Screen>();
        }
        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;

            foreach (Screen screen in Screens)
                screen.Resize(width, height);
        }

        public void OpenScreen(Screen toOpen)
        {
            Screens.Push(toOpen);
            toOpen.Open();
        }
        public void CloseScreen()
        {
            if (Screens.Count > 0)
                Screens.Peek().Close();
        }
        public void CloseAllAndThenOpen(Screen toOpen)
        {
            foreach (Screen toClose in Screens)
                toClose.Close();
            toOpenWhenCleared = toOpen;
        }

        public void Update(GameTime gameTime)
        {
            if (Screens.Count > 0)
            {
                Screen foregroundScreen = Screens.Peek();
                List<Screen> Temp = Screens.ToList();
                foreach (Screen screen in Temp)
                {
                    if (screen.State == Screen.States.FullyClosed)
                    {
                        if (screen == foregroundScreen)
                            Screens.Pop();
                        else
                            continue;
                    }
                    screen.Update(gameTime, screen == foregroundScreen);
                }
            }

            if (toOpenWhenCleared != null && Screens.Count == 0)
            {
                OpenScreen(toOpenWhenCleared);
                toOpenWhenCleared = null;
            }
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Screens.Count < 1)
                return;

            Screen foregroundScreen = Screens.Peek();
            List<Screen> Temp = Screens.ToList();
            Temp.Reverse();
            foreach (Screen screen in Temp)
            {
                screen.Draw(gameTime, screen == foregroundScreen, spriteBatch);
            }
        }
    }
}

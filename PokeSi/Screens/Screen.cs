using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace PokeSi.Screens
{
    public abstract class Screen
    {
        public enum States
        {
            Sleeping,
            Opening,
            Opened,
            Closing,
            FullyClosed
        };

        public ScreenManager Manager { get; private set; }
        //Transition part
        public States State { get; protected set; }
        public Transition OpeningTransition;
        public Transition ClosingTransition;

        public Screen(ScreenManager manager)
        {
            Manager = manager;
            State = States.Sleeping;
        }

        public virtual void Open()
        {
            State = States.Opening;
            LoadContent();
            OpeningTransition.Start();
        }
        public virtual void Close()
        {
            if (State != States.FullyClosed && State != States.Closing)
            {
                State = States.Closing;
                ClosingTransition.Start();
            }
        }

        public virtual void Resize(int width, int height)
        {

        }

        public virtual void LoadContent()
        {

        }

        public virtual void Update(GameTime gameTime, bool isInForeground)
        {
            switch (State)
            {
                case States.Sleeping: return;
                case States.Opening:
                    {
                        if (OpeningTransition.ActualState == Transition.States.Finish)
                            State = States.Opened;
                        break;
                    }
                case States.Opened: break;
                case States.Closing:
                    {
                        if (ClosingTransition.ActualState == Transition.States.Finish)
                            State = States.FullyClosed;
                        break;
                    }
                case States.FullyClosed: return;
            }
        }
        public virtual void Draw(GameTime gameTime, bool isInForeground, SpriteBatch spriteBatch)
        {
            if (State == States.Opening)
                OpeningTransition.Update(gameTime);
            else if (State == States.Closing)
                ClosingTransition.Update(gameTime);
        }
    }
}

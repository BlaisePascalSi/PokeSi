using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;

namespace PokeSi.Screens
{
    public abstract class Transition
    {
        public enum States
        {
            NotStart,
            Progressing,
            Paused,
            Finish
        };
        public enum Types
        {
            Opening,
            Closing
        };

        public States ActualState { get; protected set; }
        public TimeSpan ActualTime { get; protected set; }
        public TimeSpan TotalTime { get; set; }
        public double Avancement { get; protected set; }
        public Types Type { get; protected set; }

        public Transition(Types type)
        {
            ActualState = States.NotStart;
            ActualTime = TimeSpan.Zero;
            TotalTime = TimeSpan.FromMilliseconds(1);
            Avancement = 0;
            Type = type;
        }

        public void SetTransitionDuration(TimeSpan totalTime)
        {
            if (ActualState != States.Progressing)
                TotalTime = totalTime;
        }

        public virtual bool Start()
        {
            if (ActualState != States.NotStart && ActualState != States.Paused)
                return false;
            ActualState = States.Progressing;
            return true;
        }

        public virtual bool Pause()
        {
            if (ActualState != States.Progressing)
                return false;
            ActualState = States.Paused;
            return true;
        }

        public virtual bool Reset()
        {
            if (ActualState == States.NotStart)
                return false;
            ActualState = States.NotStart;
            ActualTime = TimeSpan.Zero;
            return true;
        }

        public virtual bool Update(GameTime gameTime)
        {
            switch (ActualState)
            {
                case States.NotStart: { return false; }
                case States.Progressing:
                    {
                        ActualTime += gameTime.ElapsedGameTime;
                        if (ActualTime >= TotalTime)
                        {
                            ActualState = States.Finish;
                            ActualTime = TotalTime;
                        }
                        Avancement = ActualTime.TotalMilliseconds / TotalTime.TotalMilliseconds;
                        break;
                    }
                case States.Paused: { return false; }
                case States.Finish: { return false; }
            }
            return true;
        }

        public abstract Matrix GetTransformation(Screen screen);
    }
}

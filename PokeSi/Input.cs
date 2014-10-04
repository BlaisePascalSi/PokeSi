using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Toolkit.Input;

namespace PokeSi
{
    public static class Input
    {
        public static PokeSiGame Game;
        private static KeyboardState state;
        private static MouseState mouseState;

        public static void Initilize(PokeSiGame game)
        {
            Game = game;
        }

        public static void Update()
        {
            lastWheel = Wheel;
            lastX = X;
            lastY = Y;
            state = Game.Keyboard.GetState();
            mouseState = Game.Mouse.GetState();
        }

        public static bool IsKeyDown(Keys key)
        {
            return state.IsKeyDown(key);
        }
        public static bool IsKeyUp(Keys key)
        {
            return !state.IsKeyDown(key);
        }
        public static bool IsKeyPressed(Keys key)
        {
            return state.IsKeyPressed(key);
        }
        public static bool IsKeyReleased(Keys key)
        {
            return state.IsKeyReleased(key);
        }

        public static Keys[] GetDownKeys()
        {
            List<Keys> keys = new List<Keys>();
            state.GetDownKeys(keys);
            return keys.ToArray();
        }

        public static ButtonState LeftButton { get { return mouseState.LeftButton; } }
        public static ButtonState RightButton { get { return mouseState.RightButton; } }
        public static ButtonState MiddleButton { get { return mouseState.MiddleButton; } }
        public static int Wheel { get { return mouseState.WheelDelta; } }
        private static int lastWheel;
        public static int WheelDelta { get { return Wheel - lastWheel; } }

        public static int X { get { return (int)(mouseState.X * Game.Viewport.Width); } }
        private static int lastX;
        public static int XDelta { get { return X - lastX; } }
        public static int Y { get { return (int)(mouseState.Y * Game.Viewport.Height); } }
        private static int lastY;
        public static int YDelta { get { return Y - lastY; } }
    }
}

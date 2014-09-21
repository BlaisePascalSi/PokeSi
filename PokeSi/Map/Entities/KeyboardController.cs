using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace PokeSi.Map.Entities
{
    public class KeyboardController : Controller
    {
        public override void Update(GameTime gameTime, Entity toControl)
        {
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

            toControl.Velocity = toMove * toControl.Speed;
            toControl.Position += toControl.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}

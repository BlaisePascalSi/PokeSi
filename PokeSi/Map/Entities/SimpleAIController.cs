using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;

namespace PokeSi.Map.Entities
{
    public class SimpleAIController : Controller
    {
        public override void Update(GameTime gameTime, Entity toControl)
        {
            Random random = new Random();
            Vector2 toMove = Vector2.Zero;
            if (random.Next(100) < 10)
            {
                if (random.Next(100) < 70)
                {
                    toMove.X = random.Next(10) - 5;
                    toMove.Y = random.Next(10) - 5;
                    toMove.Normalize();
                }
                toControl.Velocity = toMove * toControl.Speed;
            }
            toControl.Position += toControl.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}

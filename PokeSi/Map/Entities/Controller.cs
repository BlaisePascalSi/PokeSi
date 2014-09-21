using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Toolkit;

namespace PokeSi.Map.Entities
{
    public abstract class Controller
    {
        public abstract void Update(GameTime gameTime, Entity toControl);

        public override string ToString()
        {
            return GetType().Name.Remove(GetType().Name.Length - "Controller".Length);
        }
    }
}

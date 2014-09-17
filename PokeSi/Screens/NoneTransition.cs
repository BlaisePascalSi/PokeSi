using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace PokeSi.Screens
{
    public class NoneTransition : Transition
    {
        public NoneTransition(Types type) : base(type) { }

        public override Matrix GetTransformation(Screen screen) 
        {
            return Matrix.Identity;
        }
    }
}

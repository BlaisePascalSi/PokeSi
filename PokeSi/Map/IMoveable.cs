using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace PokeSi.Map
{
    public interface IMoveable
    {
        float X { get; set; }
        float Y { get; set; }
        Vector2 Position { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace PokeSi.Map
{
    public interface IBounded
    {
        Rectangle Bound { get; }
    }
}

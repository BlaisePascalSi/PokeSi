using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeSi
{
    class Program
    {
        static void Main(string[] args)
        {
            using (PokeSiGame game = new PokeSiGame())
            {
                game.Run();
            }
        }
    }
}

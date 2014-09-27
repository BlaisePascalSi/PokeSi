using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeSi.Map
{
    public class Form
    {
        public Dictionary<string, object> Datas { get; protected set; }

        public Form()
        {
            Datas = new Dictionary<string, object>();
        }
    }
}

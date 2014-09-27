using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace PokeSi.Map
{
    public interface IEditable
    {
        Rectangle Bound { get; }
        Form GetEditingForm();
        void SubmitForm(Form form);
    }
}

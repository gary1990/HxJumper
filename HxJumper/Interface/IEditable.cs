using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HxJumper.Interface
{
    public interface IEditable<Model>
    {
        void Edit(Model model);
    }
}
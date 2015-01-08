using HxJumper.Interface;
using HxJumper.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HxJumper.Models
{
    public class LineNumber : BaseModel, IEditable<LineNumber>
    {
        public void Edit(LineNumber model)
        {
            this.Name = model.Name;
        }
    }
}
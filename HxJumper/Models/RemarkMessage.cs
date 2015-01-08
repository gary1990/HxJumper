using HxJumper.Interface;
using HxJumper.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HxJumper.Models
{
    public class RemarkMessage : BaseModel, IEditable<RemarkMessage>
    {
        public void Edit(RemarkMessage model)
        {
            this.Name = model.Name;
        }
    }
}
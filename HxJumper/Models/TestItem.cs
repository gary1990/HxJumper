using HxJumper.Interface;
using HxJumper.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HxJumper.Models
{
    public class TestItem : BaseModel, IEditable<TestItem>
    {
        public void Edit(TestItem model)
        {
            this.Name = model.Name;
        }
    }
}
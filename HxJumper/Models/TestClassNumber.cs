using HxJumper.Interface;
using HxJumper.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HxJumper.Models
{
    public class TestClassNumber : BaseModel, IEditable<TestClassNumber>
    {
        public void Edit(TestClassNumber model)
        {
            this.Name = model.Name;
        }
    }
}
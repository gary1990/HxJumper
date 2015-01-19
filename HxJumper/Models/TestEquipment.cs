using HxJumper.Interface;
using HxJumper.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HxJumper.Models
{
    public class TestEquipment : BaseModel, IEditable<TestEquipment>
    {
        public string SerialNumber { get; set; }

        public void Edit(TestEquipment model)
        {
            this.Name = model.Name;
            this.SerialNumber = model.SerialNumber;
        }
    }
}
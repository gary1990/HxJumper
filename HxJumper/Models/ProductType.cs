using HxJumper.Interface;
using HxJumper.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HxJumper.Models
{
    public class ProductType : BaseModel, IEditable<ProductType>
    {
        public void Edit(ProductType model)
        {
            this.Name = model.Name;
        }
    }
}
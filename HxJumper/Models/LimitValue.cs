using HxJumper.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HxJumper.Models
{
    public class LimitValue : IEditable<LimitValue>
    {
        public int Id { get; set; }
        [DisplayName("最小值")]
        public decimal? MinVal { get; set; }
        [DisplayName("最大值")]
        public decimal? MaxVal { get; set; }
        [DisplayName("已删除")]
        public bool IsDeleted { get; set; }

        public void Edit(LimitValue model)
        {
            this.MinVal = model.MinVal;
            this.MaxVal = model.MaxVal;
            this.IsDeleted = model.IsDeleted;
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HxJumper.Models.Base
{
    public class BaseModel
    {
        public BaseModel() {
            this.IsDeleted = false;
        }
        public int Id { get; set; }
        [Required]
        [DisplayName("名称")]
        [MaxLength(50)]
        public string Name { get; set; }
        [DisplayName("已删除")]//false is active, true is inactive
        public bool IsDeleted { get; set; }
    }
}
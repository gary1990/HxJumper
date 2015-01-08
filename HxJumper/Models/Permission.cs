using HxJumper.Interface;
using HxJumper.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HxJumper.Models
{
    public class Permission : BaseModel, IEditable<Permission>
    {
        public Permission() {
            this.JumperRoles = new List<JumperRole> { };
        }
        [Required, StringLength(256)]
        [DisplayName("控制器名")]
        public string ControllerName { get; set; }
        [Required, StringLength(256)]
        [DisplayName("方法名")]
        public string ActionName { get; set; }
        public virtual ICollection<JumperRole> JumperRoles { get; set; }
        public void Edit(Permission model)
        {
            this.Name = model.Name;
            this.ControllerName = model.ControllerName;
            this.ActionName = model.ActionName;
        }
    }
}
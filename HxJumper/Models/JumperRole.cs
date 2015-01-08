using HxJumper.Interface;
using HxJumper.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HxJumper.Models
{
    public class JumperRole : BaseModel, IEditable<JumperRole>
    {
        public JumperRole() {
            this.Permissions = new List<Permission> { };
        }
        [DisplayName("权限")]
        public virtual ICollection<Permission> Permissions { get; set; }
        public void Edit(JumperRole model)
        {
            this.Name = model.Name;
            this.Permissions = model.Permissions;
        }
    }
}
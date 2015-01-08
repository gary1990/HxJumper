using HxJumper.Interface;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HxJumper.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class JumperUser : IdentityUser, IEditable<JumperUser>
    {
        public JumperUser() { 
            
        }
        [Required]
        [DisplayName("工号")]
        [MaxLength(20)]
        public string JobNumber { get; set; }
        [DisplayName("角色")]
        public int JumperRoleId { get; set; }
        [DisplayName("已删除")]//false is active, 0 in db. true is inactive, 1 in db
        public bool IsDeleted { get; set; }
        public virtual JumperRole JumperRole { get; set; }
        public void Edit(JumperUser model)
        {
            this.JobNumber = model.JobNumber;
            this.JumperRoleId = model.JumperRoleId;
            this.UserName = model.UserName;
        }
    }
}
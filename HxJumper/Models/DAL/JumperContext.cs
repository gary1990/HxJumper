using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace HxJumper.Models.DAL
{
    public class JumperContext : IdentityDbContext<JumperUser>
    {
        public JumperContext() :
            base("JumperContext")
        {
            UserManager = new UserManager<JumperUser>(new UserStore<JumperUser>(this));
        }
        public UserManager<JumperUser> UserManager { get; set; }
        public DbSet<JumperRole> JumperRole { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<ProductType> ProductType { get; set; }
        public DbSet<RemarkMessage> RemarkMessage { get; set; }
        public DbSet<TestResult> TestResult { get; set; }
        public DbSet<TestResultValue> TestResultValue { get; set; }
        public DbSet<TestClassNumber> TestClassNumber { get; set; }
        public DbSet<LineNumber> LineNumber { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            //rename AspNetUsers table to JumperUser
            modelBuilder.Entity<IdentityUser>().ToTable("JumperUser");
            modelBuilder.Entity<JumperUser>().ToTable("JumperUser");

            modelBuilder.Entity<JumperRole>().HasMany(a => a.Permissions).WithMany(b => b.JumperRoles).Map(c => c.MapLeftKey("JumperRoleId").MapRightKey("PermissionId").ToTable("JumperRolePermission"));
            modelBuilder.Entity<TestResult>().Property(a => a.TestTime).HasColumnType("datetime2").HasPrecision(0);
            modelBuilder.Entity<TestResultValue>().Property(a => a.MarkValue).HasPrecision(15, 4);
        }
    }
}
using HxJumper.Models.DAL;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HxJumper.Models.Initializer
{
    public class JumperInitailizer : CreateDatabaseIfNotExists<JumperContext>
    {
        protected override void Seed(JumperContext db)
        {
            UniqueIndexInitial.Create(db);
            var roles = new List<JumperRole>
            {
                new JumperRole { Name = "系统管理员"},
                new JumperRole { Name = "测试员"}
            };
            roles.ForEach(a => db.JumperRole.Add(a));
            db.SaveChanges();

            var productTypes = new List<ProductType> 
            {
                new ProductType { Name = "NM-NM"},
                new ProductType { Name = "DINM-QMAMA PLMR240 6.1m"},
            };
            productTypes.ForEach(a => db.ProductType.Add(a));
            db.SaveChanges();

            var testClassNumbers = new List<TestClassNumber> 
            {
                new TestClassNumber { Name = "1"},
                new TestClassNumber { Name = "13"}
            };
            testClassNumbers.ForEach(a => db.TestClassNumber.Add(a));
            db.SaveChanges();

            var lineNumbers = new List<LineNumber>
            {
                new LineNumber { Name = "1"},
                new LineNumber { Name = "2"}
            };
            lineNumbers.ForEach(a => db.LineNumber.Add(a));
            db.SaveChanges();

            var UserManager = db.UserManager;
            UserManager.UserValidator = new UserValidator<JumperUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
            var userName = "系统管理员";
            var jobNumber = "001";
            var passWord = "123456";
            var userAdmin = new JumperUser { JobNumber = jobNumber, UserName = userName, JumperRoleId = db.JumperRole.Where(a => a.Name == "系统管理员").Single().Id };
            UserManager.Create(userAdmin, passWord);
            userName = "NO001";
            jobNumber = "NO001";
            var userTester = new JumperUser { JobNumber = jobNumber, UserName = userName, JumperRoleId = db.JumperRole.Where(a => a.Name == "测试员").Single().Id };
            UserManager.Create(userTester, passWord);
            db.SaveChanges();

            base.Seed(db);
        }
    }

    public class UniqueIndexInitial
    {
        public static void Create(JumperContext context) 
        {
            context.Database.ExecuteSqlCommand("Create UNIQUE INDEX index_RoleName ON JumperRole(Name)");
            context.Database.ExecuteSqlCommand("Create UNIQUE INDEX index_permission ON Permission(ControllerName, ActionName)");
            context.Database.ExecuteSqlCommand("Create UNIQUE INDEX index_ProductTypeName ON ProductType(Name)");
            context.Database.ExecuteSqlCommand("Create UNIQUE INDEX index_RemarkMessageName ON RemarkMessage(Name)");
            context.Database.ExecuteSqlCommand("Create UNIQUE INDEX index_TestClassNumber ON TestClassNumber(Name)");
            context.Database.ExecuteSqlCommand("Create UNIQUE INDEX index_Jobnumber ON JumperUser(JobNumber)");
            context.Database.ExecuteSqlCommand("Create UNIQUE INDEX index_LineNumberName ON LineNumber(Name)");
        }
    }
}
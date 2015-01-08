namespace HxJumper.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JumperRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Permission",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ControllerName = c.String(nullable: false, maxLength: 256),
                        ActionName = c.String(nullable: false, maxLength: 256),
                        Name = c.String(nullable: false, maxLength: 50),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LineNumber",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RemarkMessage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TestClassNumber",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TestResult",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TestTime = c.DateTime(nullable: false, precision: 0, storeType: "datetime2"),
                        ProductTypeId = c.Int(nullable: false),
                        TestClassNumberId = c.Int(nullable: false),
                        TestCode = c.String(),
                        RemarkMessageId = c.Int(),
                        TestImg = c.String(),
                        Result = c.Boolean(nullable: false),
                        LineNumberId = c.Int(nullable: false),
                        JumperUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.JumperUser", t => t.JumperUserId)
                .ForeignKey("dbo.LineNumber", t => t.LineNumberId)
                .ForeignKey("dbo.ProductType", t => t.ProductTypeId)
                .ForeignKey("dbo.RemarkMessage", t => t.RemarkMessageId)
                .ForeignKey("dbo.TestClassNumber", t => t.TestClassNumberId)
                .Index(t => t.JumperUserId)
                .Index(t => t.LineNumberId)
                .Index(t => t.ProductTypeId)
                .Index(t => t.RemarkMessageId)
                .Index(t => t.TestClassNumberId);
            
            CreateTable(
                "dbo.JumperUser",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        JobNumber = c.String(maxLength: 20),
                        JumperRoleId = c.Int(),
                        IsDeleted = c.Boolean(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.JumperRole", t => t.JumperRoleId)
                .Index(t => t.JumperRoleId);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.JumperUser", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.JumperUser", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .ForeignKey("dbo.JumperUser", t => t.UserId)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TestResultValue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TestResultId = c.Int(nullable: false),
                        MarkValue = c.Decimal(nullable: false, precision: 15, scale: 4),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestResult", t => t.TestResultId)
                .Index(t => t.TestResultId);
            
            CreateTable(
                "dbo.JumperRolePermission",
                c => new
                    {
                        JumperRoleId = c.Int(nullable: false),
                        PermissionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.JumperRoleId, t.PermissionId })
                .ForeignKey("dbo.JumperRole", t => t.JumperRoleId)
                .ForeignKey("dbo.Permission", t => t.PermissionId)
                .Index(t => t.JumperRoleId)
                .Index(t => t.PermissionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestResultValue", "TestResultId", "dbo.TestResult");
            DropForeignKey("dbo.TestResult", "TestClassNumberId", "dbo.TestClassNumber");
            DropForeignKey("dbo.TestResult", "RemarkMessageId", "dbo.RemarkMessage");
            DropForeignKey("dbo.TestResult", "ProductTypeId", "dbo.ProductType");
            DropForeignKey("dbo.TestResult", "LineNumberId", "dbo.LineNumber");
            DropForeignKey("dbo.TestResult", "JumperUserId", "dbo.JumperUser");
            DropForeignKey("dbo.JumperUser", "JumperRoleId", "dbo.JumperRole");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.JumperUser");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.JumperUser");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.JumperUser");
            DropForeignKey("dbo.JumperRolePermission", "PermissionId", "dbo.Permission");
            DropForeignKey("dbo.JumperRolePermission", "JumperRoleId", "dbo.JumperRole");
            DropIndex("dbo.TestResultValue", new[] { "TestResultId" });
            DropIndex("dbo.TestResult", new[] { "TestClassNumberId" });
            DropIndex("dbo.TestResult", new[] { "RemarkMessageId" });
            DropIndex("dbo.TestResult", new[] { "ProductTypeId" });
            DropIndex("dbo.TestResult", new[] { "LineNumberId" });
            DropIndex("dbo.TestResult", new[] { "JumperUserId" });
            DropIndex("dbo.JumperUser", new[] { "JumperRoleId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.JumperRolePermission", new[] { "PermissionId" });
            DropIndex("dbo.JumperRolePermission", new[] { "JumperRoleId" });
            DropTable("dbo.JumperRolePermission");
            DropTable("dbo.TestResultValue");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.JumperUser");
            DropTable("dbo.TestResult");
            DropTable("dbo.TestClassNumber");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.RemarkMessage");
            DropTable("dbo.ProductType");
            DropTable("dbo.LineNumber");
            DropTable("dbo.Permission");
            DropTable("dbo.JumperRole");
        }
    }
}

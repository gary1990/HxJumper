namespace HxJumper.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Carrier",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SetFreq = c.Decimal(nullable: false, precision: 8, scale: 2),
                        StartFreq = c.Decimal(nullable: false, precision: 8, scale: 2),
                        StopFreq = c.Decimal(nullable: false, precision: 8, scale: 2),
                        FreqUnit = c.Int(nullable: false),
                        Power = c.Decimal(nullable: false, precision: 8, scale: 2),
                        ImUnit = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TestResultPim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TestTime = c.DateTime(nullable: false, precision: 0, storeType: "datetime2"),
                        SerialNumber = c.String(),
                        JumperUserId = c.String(maxLength: 128),
                        TestEquipmentId = c.Int(nullable: false),
                        ImOrderId = c.Int(nullable: false),
                        TestMeans = c.Int(nullable: false),
                        TestDescription = c.String(),
                        TestResult = c.Boolean(nullable: false),
                        TestImage = c.String(),
                        IsLatest = c.Boolean(nullable: false),
                        LimitLine = c.Decimal(nullable: false, precision: 8, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ImOrder", t => t.ImOrderId)
                .ForeignKey("dbo.JumperUser", t => t.JumperUserId)
                .ForeignKey("dbo.TestEquipment", t => t.TestEquipmentId)
                .Index(t => t.ImOrderId)
                .Index(t => t.JumperUserId)
                .Index(t => t.TestEquipmentId);
            
            CreateTable(
                "dbo.ImOrder",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNumber = c.Int(nullable: false),
                        ImUnit = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                "dbo.TestEquipment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SerialNumber = c.String(),
                        Name = c.String(nullable: false, maxLength: 50),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TestResultPimPoint",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TestResultPimId = c.Int(nullable: false),
                        CarrierOneFreq = c.Decimal(nullable: false, precision: 8, scale: 2),
                        CarrierTwoFreq = c.Decimal(nullable: false, precision: 8, scale: 2),
                        ImFreq = c.Decimal(nullable: false, precision: 8, scale: 2),
                        ImPower = c.Decimal(nullable: false, precision: 8, scale: 2),
                        isWorst = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestResultPim", t => t.TestResultPimId)
                .Index(t => t.TestResultPimId);
            
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
                "dbo.TestClassNumber",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TestItem",
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
                        IsLatest = c.Boolean(nullable: false),
                        NotStatistic = c.Boolean(nullable: false),
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
                "dbo.TestResultItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TestItemId = c.Int(nullable: false),
                        TestResultId = c.Int(nullable: false),
                        TestResultItemResult = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestItem", t => t.TestItemId)
                .ForeignKey("dbo.TestResult", t => t.TestResultId)
                .Index(t => t.TestItemId)
                .Index(t => t.TestResultId);
            
            CreateTable(
                "dbo.TestResultValue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TestResultItemId = c.Int(nullable: false),
                        Channel = c.Int(nullable: false),
                        Trace = c.Int(nullable: false),
                        XValue = c.Decimal(nullable: false, precision: 15, scale: 4),
                        XValueUnit = c.Int(nullable: false),
                        MarkValue = c.Decimal(nullable: false, precision: 15, scale: 4),
                        Paremeter = c.String(),
                        Formart = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestResultItem", t => t.TestResultItemId)
                .Index(t => t.TestResultItemId);
            
            CreateTable(
                "dbo.TestResultPimCarrier",
                c => new
                    {
                        TestResultPimId = c.Int(nullable: false),
                        CarrierId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TestResultPimId, t.CarrierId })
                .ForeignKey("dbo.TestResultPim", t => t.TestResultPimId)
                .ForeignKey("dbo.Carrier", t => t.CarrierId)
                .Index(t => t.TestResultPimId)
                .Index(t => t.CarrierId);
            
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
            DropForeignKey("dbo.TestResultValue", "TestResultItemId", "dbo.TestResultItem");
            DropForeignKey("dbo.TestResultItem", "TestResultId", "dbo.TestResult");
            DropForeignKey("dbo.TestResultItem", "TestItemId", "dbo.TestItem");
            DropForeignKey("dbo.TestResult", "TestClassNumberId", "dbo.TestClassNumber");
            DropForeignKey("dbo.TestResult", "RemarkMessageId", "dbo.RemarkMessage");
            DropForeignKey("dbo.TestResult", "ProductTypeId", "dbo.ProductType");
            DropForeignKey("dbo.TestResult", "LineNumberId", "dbo.LineNumber");
            DropForeignKey("dbo.TestResult", "JumperUserId", "dbo.JumperUser");
            DropForeignKey("dbo.TestResultPimPoint", "TestResultPimId", "dbo.TestResultPim");
            DropForeignKey("dbo.TestResultPim", "TestEquipmentId", "dbo.TestEquipment");
            DropForeignKey("dbo.TestResultPim", "JumperUserId", "dbo.JumperUser");
            DropForeignKey("dbo.JumperUser", "JumperRoleId", "dbo.JumperRole");
            DropForeignKey("dbo.JumperRolePermission", "PermissionId", "dbo.Permission");
            DropForeignKey("dbo.JumperRolePermission", "JumperRoleId", "dbo.JumperRole");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.JumperUser");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.JumperUser");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.JumperUser");
            DropForeignKey("dbo.TestResultPim", "ImOrderId", "dbo.ImOrder");
            DropForeignKey("dbo.TestResultPimCarrier", "CarrierId", "dbo.Carrier");
            DropForeignKey("dbo.TestResultPimCarrier", "TestResultPimId", "dbo.TestResultPim");
            DropIndex("dbo.TestResultValue", new[] { "TestResultItemId" });
            DropIndex("dbo.TestResultItem", new[] { "TestResultId" });
            DropIndex("dbo.TestResultItem", new[] { "TestItemId" });
            DropIndex("dbo.TestResult", new[] { "TestClassNumberId" });
            DropIndex("dbo.TestResult", new[] { "RemarkMessageId" });
            DropIndex("dbo.TestResult", new[] { "ProductTypeId" });
            DropIndex("dbo.TestResult", new[] { "LineNumberId" });
            DropIndex("dbo.TestResult", new[] { "JumperUserId" });
            DropIndex("dbo.TestResultPimPoint", new[] { "TestResultPimId" });
            DropIndex("dbo.TestResultPim", new[] { "TestEquipmentId" });
            DropIndex("dbo.TestResultPim", new[] { "JumperUserId" });
            DropIndex("dbo.JumperUser", new[] { "JumperRoleId" });
            DropIndex("dbo.JumperRolePermission", new[] { "PermissionId" });
            DropIndex("dbo.JumperRolePermission", new[] { "JumperRoleId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.TestResultPim", new[] { "ImOrderId" });
            DropIndex("dbo.TestResultPimCarrier", new[] { "CarrierId" });
            DropIndex("dbo.TestResultPimCarrier", new[] { "TestResultPimId" });
            DropTable("dbo.JumperRolePermission");
            DropTable("dbo.TestResultPimCarrier");
            DropTable("dbo.TestResultValue");
            DropTable("dbo.TestResultItem");
            DropTable("dbo.TestResult");
            DropTable("dbo.TestItem");
            DropTable("dbo.TestClassNumber");
            DropTable("dbo.RemarkMessage");
            DropTable("dbo.ProductType");
            DropTable("dbo.LineNumber");
            DropTable("dbo.TestResultPimPoint");
            DropTable("dbo.TestEquipment");
            DropTable("dbo.Permission");
            DropTable("dbo.JumperRole");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.JumperUser");
            DropTable("dbo.ImOrder");
            DropTable("dbo.TestResultPim");
            DropTable("dbo.Carrier");
        }
    }
}

namespace HxJumper.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_pim_kaelus_upload : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TestResultPim", "TestEquipmentId", "dbo.TestEquipment");
            DropIndex("dbo.TestResultPim", new[] { "TestEquipmentId" });
            CreateTable(
                "dbo.TestImage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        path = c.String(),
                        TestResultPimId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestResultPim", t => t.TestResultPimId)
                .Index(t => t.TestResultPimId);
            
            AlterColumn("dbo.TestResultPim", "TestEquipmentId", c => c.Int());
            CreateIndex("dbo.TestResultPim", "TestEquipmentId");
            AddForeignKey("dbo.TestResultPim", "TestEquipmentId", "dbo.TestEquipment", "Id");
            DropColumn("dbo.TestResultPim", "TestImage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TestResultPim", "TestImage", c => c.String());
            DropForeignKey("dbo.TestResultPim", "TestEquipmentId", "dbo.TestEquipment");
            DropForeignKey("dbo.TestImage", "TestResultPimId", "dbo.TestResultPim");
            DropIndex("dbo.TestResultPim", new[] { "TestEquipmentId" });
            DropIndex("dbo.TestImage", new[] { "TestResultPimId" });
            AlterColumn("dbo.TestResultPim", "TestEquipmentId", c => c.Int(nullable: false));
            DropTable("dbo.TestImage");
            CreateIndex("dbo.TestResultPim", "TestEquipmentId");
            AddForeignKey("dbo.TestResultPim", "TestEquipmentId", "dbo.TestEquipment", "Id");
        }
    }
}

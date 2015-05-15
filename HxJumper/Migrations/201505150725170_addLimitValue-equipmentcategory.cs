namespace HxJumper.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addLimitValueequipmentcategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LimitValue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MinVal = c.Decimal(precision: 8, scale: 2),
                        MaxVal = c.Decimal(precision: 8, scale: 2),
                        IsDeleted = c.Boolean(nullable: false, defaultValue:false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TestEquipment", "isVna", c => c.Boolean(nullable: false, defaultValue:false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestEquipment", "isVna");
            DropTable("dbo.LimitValue");
        }
    }
}

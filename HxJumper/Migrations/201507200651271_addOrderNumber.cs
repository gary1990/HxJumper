namespace HxJumper.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOrderNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestResult", "OrderNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestResult", "OrderNumber");
        }
    }
}

namespace HxJumper.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_remarkmessge_testresult_fields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestResult", "IsLatest", c => c.Boolean(nullable: false, defaultValue:true));
            AddColumn("dbo.TestResult", "NotStatistic", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestResult", "NotStatistic");
            DropColumn("dbo.TestResult", "IsLatest");
        }
    }
}

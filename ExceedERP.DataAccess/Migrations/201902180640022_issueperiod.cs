namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class issueperiod : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetIssue", "GLPeriodId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetIssue", "GLPeriodId");
        }
    }
}

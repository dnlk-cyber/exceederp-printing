namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class workitem : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.IssueItem", "WorkItemCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.IssueItem", "WorkItemCode");
        }
    }
}

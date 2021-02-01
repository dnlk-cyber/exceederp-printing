namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class issue2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("EXCEED.IssueItem", "GLAccountCode");
            DropColumn("EXCEED.IssueItem", "IssueGoodReceiveItemID");
            DropColumn("EXCEED.IssueItem", "IssueGoodReceiveItemPrice");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.IssueItem", "IssueGoodReceiveItemPrice", c => c.String());
            AddColumn("EXCEED.IssueItem", "IssueGoodReceiveItemID", c => c.String());
            AddColumn("EXCEED.IssueItem", "GLAccountCode", c => c.String(nullable: false));
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class issuerecipient : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetIssue", "RecipientId", c => c.Int(nullable: false));
            AddColumn("EXCEED.Issue", "RecipientId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.Issue", "RecipientId");
            DropColumn("EXCEED.FixedAssetIssue", "RecipientId");
        }
    }
}

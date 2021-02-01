namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class approval1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("EXCEED.IssueItem", "FixedAssetSelector");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.IssueItem", "FixedAssetSelector", c => c.String());
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetIssueItem", "Size", c => c.String());
            AddColumn("EXCEED.FixedAssetRequisitionItem", "Size", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetRequisitionItem", "Size");
            DropColumn("EXCEED.FixedAssetIssueItem", "Size");
        }
    }
}

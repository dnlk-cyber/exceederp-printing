namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset28 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetTransferItem", "Remark", c => c.String());
            AddColumn("EXCEED.AssetTransferReceiveItem", "Remark", c => c.String());
            AddColumn("EXCEED.FixedAssetIssueItem", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetIssueItem", "Remark");
            DropColumn("EXCEED.AssetTransferReceiveItem", "Remark");
            DropColumn("EXCEED.AssetTransferItem", "Remark");
        }
    }
}

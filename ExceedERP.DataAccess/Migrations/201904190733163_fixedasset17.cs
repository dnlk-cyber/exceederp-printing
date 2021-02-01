namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetTransfer", "Type", c => c.Int(nullable: false));
            AddColumn("EXCEED.AssetTransfer", "FromStoreCode", c => c.String());
            AddColumn("EXCEED.AssetTransfer", "ToStoreCode", c => c.String());
            AddColumn("EXCEED.AssetTransfer", "FromBranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.AssetTransfer", "ToBranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetIssueItem", "Name", c => c.String());
            AddColumn("EXCEED.FixedAssetIssueItem", "Unit", c => c.String());
            AddColumn("EXCEED.FixedAssetIssueItem", "GrnNo", c => c.String());
            AddColumn("EXCEED.FixedAssetIssue", "GatePass", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetIssue", "GatePass");
            DropColumn("EXCEED.FixedAssetIssueItem", "GrnNo");
            DropColumn("EXCEED.FixedAssetIssueItem", "Unit");
            DropColumn("EXCEED.FixedAssetIssueItem", "Name");
            DropColumn("EXCEED.AssetTransfer", "ToBranchId");
            DropColumn("EXCEED.AssetTransfer", "FromBranchId");
            DropColumn("EXCEED.AssetTransfer", "ToStoreCode");
            DropColumn("EXCEED.AssetTransfer", "FromStoreCode");
            DropColumn("EXCEED.AssetTransfer", "Type");
        }
    }
}

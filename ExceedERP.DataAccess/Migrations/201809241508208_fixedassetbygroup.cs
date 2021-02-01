namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassetbygroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetRegistration", "IsGroup", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetReceiveItem", "IsGroup", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetIssueItem", "IsGroup", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetRequisitionItem", "IsGroup", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetReturnItem", "IsGroup", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetReturnItem", "IsGroup");
            DropColumn("EXCEED.FixedAssetRequisitionItem", "IsGroup");
            DropColumn("EXCEED.FixedAssetIssueItem", "IsGroup");
            DropColumn("EXCEED.FixedAssetReceiveItem", "IsGroup");
            DropColumn("EXCEED.AssetRegistration", "IsGroup");
        }
    }
}

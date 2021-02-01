namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixereq : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetRequisition", "IsMiddleOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetRequisition", "MiddleOnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.FixedAssetRequisition", "MiddleOnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.FixedAssetRequisition", "IsMiddleSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetRequisition", "MiddleSendForApprovalBy", c => c.String());
            AddColumn("EXCEED.FixedAssetRequisition", "MiddleSendForApprovalTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetRequisition", "MiddleSendForApprovalTime");
            DropColumn("EXCEED.FixedAssetRequisition", "MiddleSendForApprovalBy");
            DropColumn("EXCEED.FixedAssetRequisition", "IsMiddleSendForApproval");
            DropColumn("EXCEED.FixedAssetRequisition", "MiddleOnlineApprovedTime");
            DropColumn("EXCEED.FixedAssetRequisition", "MiddleOnlineApprovedBy");
            DropColumn("EXCEED.FixedAssetRequisition", "IsMiddleOnlineApproved");
        }
    }
}

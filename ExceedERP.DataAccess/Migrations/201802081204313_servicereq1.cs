namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class servicereq1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseRequisition", "NextApprover", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "IsMiddleOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "MiddleOnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "MiddleOnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.PurchaseRequisition", "IsMiddleSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "MiddleSendForApprovalBy", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "MiddleSendForApprovalTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseRequisition", "MiddleSendForApprovalTime");
            DropColumn("EXCEED.PurchaseRequisition", "MiddleSendForApprovalBy");
            DropColumn("EXCEED.PurchaseRequisition", "IsMiddleSendForApproval");
            DropColumn("EXCEED.PurchaseRequisition", "MiddleOnlineApprovedTime");
            DropColumn("EXCEED.PurchaseRequisition", "MiddleOnlineApprovedBy");
            DropColumn("EXCEED.PurchaseRequisition", "IsMiddleOnlineApproved");
            DropColumn("EXCEED.PurchaseRequisition", "NextApprover");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inven7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisition", "IsSecondSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "SecondSendForApprovalBy", c => c.String());
            AddColumn("EXCEED.StoreRequisition", "SecondSendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.ItemTransfer", "IsSecondSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemTransfer", "SecondSendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ItemTransfer", "SecondSendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.PurchaseRequisition", "IsSecondSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "SecondSendForApprovalBy", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "SecondSendForApprovalTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseRequisition", "SecondSendForApprovalTime");
            DropColumn("EXCEED.PurchaseRequisition", "SecondSendForApprovalBy");
            DropColumn("EXCEED.PurchaseRequisition", "IsSecondSendForApproval");
            DropColumn("EXCEED.ItemTransfer", "SecondSendForApprovalTime");
            DropColumn("EXCEED.ItemTransfer", "SecondSendForApprovalBy");
            DropColumn("EXCEED.ItemTransfer", "IsSecondSendForApproval");
            DropColumn("EXCEED.StoreRequisition", "SecondSendForApprovalTime");
            DropColumn("EXCEED.StoreRequisition", "SecondSendForApprovalBy");
            DropColumn("EXCEED.StoreRequisition", "IsSecondSendForApproval");
        }
    }
}

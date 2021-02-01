namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class purchasefollow : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseRequisition", "ProcurementMethod", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "PurchaseCategory", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "IsSecondOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "SecondOnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "SecondOnlineApprovedTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseRequisition", "SecondOnlineApprovedTime");
            DropColumn("EXCEED.PurchaseRequisition", "SecondOnlineApprovedBy");
            DropColumn("EXCEED.PurchaseRequisition", "IsSecondOnlineApproved");
            DropColumn("EXCEED.PurchaseRequisition", "PurchaseCategory");
            DropColumn("EXCEED.PurchaseRequisition", "ProcurementMethod");
        }
    }
}

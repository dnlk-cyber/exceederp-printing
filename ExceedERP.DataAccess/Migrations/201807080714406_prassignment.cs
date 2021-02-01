namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prassignment : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.LotPurchaseRequisition", "PurchaseRequisitionAssignmentId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ProformaPurchaseRequisition", "PurchaseRequisitionAssignmentId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PoPurchaseRequisition", "PurchaseRequisitionAssignmentId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PoPurchaseRequisition", "PurchaseRequisitionAssignmentId");
            DropColumn("EXCEED.ProformaPurchaseRequisition", "PurchaseRequisitionAssignmentId");
            DropColumn("EXCEED.LotPurchaseRequisition", "PurchaseRequisitionAssignmentId");
        }
    }
}

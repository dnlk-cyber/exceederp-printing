namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prfromprevious : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseRequisition", "FromPreviousPurchase", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "PreviousProcurementMethod", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "PreviousPurchaseReference", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseRequisition", "PreviousPurchaseReference");
            DropColumn("EXCEED.PurchaseRequisition", "PreviousProcurementMethod");
            DropColumn("EXCEED.PurchaseRequisition", "FromPreviousPurchase");
        }
    }
}

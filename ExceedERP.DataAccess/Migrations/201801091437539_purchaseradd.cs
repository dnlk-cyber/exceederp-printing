namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class purchaseradd : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Tender", "ProcurementOfficer", c => c.Int(nullable: false));
            AddColumn("EXCEED.ProformaPurchase", "ProcurementOfficer", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseOrder", "ProcurementOfficer", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseOrder", "ProcurementOfficer");
            DropColumn("EXCEED.ProformaPurchase", "ProcurementOfficer");
            DropColumn("EXCEED.Tender", "ProcurementOfficer");
        }
    }
}

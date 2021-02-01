namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.PurchaseRequisition", "ProcurementOfficer", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PurchaseRequisition", "ProcurementOfficer", c => c.String(nullable: false));
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProformaUpdated : DbMigration
    {
        public override void Up()
        {
            AlterColumn("ProductFollowUp.PFProformaItems", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("ProductFollowUp.Proforma", "InvoiceDate", c => c.DateTime());
            AlterColumn("ProductFollowUp.Proforma", "DeliveryDate", c => c.DateTime());
            AlterColumn("ProductFollowUp.Proforma", "ValidityDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("ProductFollowUp.Proforma", "ValidityDate", c => c.DateTime(nullable: false));
            AlterColumn("ProductFollowUp.Proforma", "DeliveryDate", c => c.DateTime(nullable: false));
            AlterColumn("ProductFollowUp.Proforma", "InvoiceDate", c => c.DateTime(nullable: false));
            AlterColumn("ProductFollowUp.PFProformaItems", "Quantity", c => c.Int(nullable: false));
        }
    }
}

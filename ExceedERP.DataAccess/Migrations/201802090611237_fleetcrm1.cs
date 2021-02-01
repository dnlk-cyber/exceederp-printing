namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetcrm1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "CRM.AdvancePaymentLine",
                c => new
                    {
                        AdvancePaymentLineId = c.Int(nullable: false, identity: true),
                        AdvancePaymentId = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AdvancePaymentLineId);
            
            CreateTable(
                "CRM.AdvancePayment",
                c => new
                    {
                        AdvancePaymentId = c.Int(nullable: false, identity: true),
                        Customer = c.String(),
                        GrandTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AdvancePaymentId);
            
            AddColumn("CRM.CashSalesInvoice", "AdvancPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.CashSalesInvoice", "OnHandPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.LineItem", "FreightCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.LineItem", "InsuranceCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.LineItem", "OtherCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.OrganizationCustomer", "CustomerType", c => c.Int(nullable: false));
            AlterColumn("CRM.Campaign", "CartID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("CRM.Campaign", "CartID", c => c.String(nullable: false));
            DropColumn("CRM.OrganizationCustomer", "CustomerType");
            DropColumn("CRM.LineItem", "OtherCost");
            DropColumn("CRM.LineItem", "InsuranceCost");
            DropColumn("CRM.LineItem", "FreightCost");
            DropColumn("CRM.CashSalesInvoice", "OnHandPayment");
            DropColumn("CRM.CashSalesInvoice", "AdvancPayment");
            DropTable("CRM.AdvancePayment");
            DropTable("CRM.AdvancePaymentLine");
        }
    }
}

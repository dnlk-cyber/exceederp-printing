namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customeridsupplierid : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetRegistration", "Condition", c => c.Int(nullable: false));
            AddColumn("EXCEED.BinCardLine", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PropertyCount", "IsClosed", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ReconciliationBankAccount", "BankAccount", c => c.String(nullable: false));
            AlterColumn("EXCEED.ApPaybleInvoices", "SupplierName", c => c.String());
            AlterColumn("EXCEED.ApPaybleInvoices", "SupplierId", c => c.String(nullable: false));
            AlterColumn("EXCEED.ApPayment", "SupplierName", c => c.String());
            AlterColumn("EXCEED.ApPayment", "SupplierId", c => c.String(nullable: false));
            AlterColumn("EXCEED.ArInvoice", "CustomerId", c => c.String(nullable: false));
            AlterColumn("EXCEED.ArStandardCollection", "CustomerId", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ArStandardCollection", "CustomerId", c => c.String());
            AlterColumn("EXCEED.ArInvoice", "CustomerId", c => c.String());
            AlterColumn("EXCEED.ApPayment", "SupplierId", c => c.String());
            AlterColumn("EXCEED.ApPayment", "SupplierName", c => c.String(nullable: false));
            AlterColumn("EXCEED.ApPaybleInvoices", "SupplierId", c => c.String());
            AlterColumn("EXCEED.ApPaybleInvoices", "SupplierName", c => c.String(nullable: false));
            DropColumn("EXCEED.ReconciliationBankAccount", "BankAccount");
            DropColumn("EXCEED.PropertyCount", "IsClosed");
            DropColumn("EXCEED.BinCardLine", "IsVoid");
            DropColumn("EXCEED.AssetRegistration", "Condition");
        }
    }
}

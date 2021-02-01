namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contractpayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractPayment", "SupplierInvoice", c => c.String());
            AddColumn("EXCEED.ContractPayment", "NetPayable", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ContractPayment", "NetPayable");
            DropColumn("EXCEED.ContractPayment", "SupplierInvoice");
        }
    }
}

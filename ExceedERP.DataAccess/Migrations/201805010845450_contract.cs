namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contract : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractManagementItem", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("EXCEED.ContractManagementItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ContractManagement", "IsNewSupplier", c => c.Boolean(nullable: false));
            AlterColumn("EXCEED.BidAwardedSupplier", "DeliveryDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.BidAwardedSupplier", "DeliveryDate", c => c.DateTime());
            DropColumn("EXCEED.ContractManagement", "IsNewSupplier");
            DropColumn("EXCEED.ContractManagementItem", "TaxAmount");
            DropColumn("EXCEED.ContractManagementItem", "GLTaxRateID");
        }
    }
}

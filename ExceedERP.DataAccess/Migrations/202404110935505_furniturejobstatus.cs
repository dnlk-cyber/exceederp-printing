namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class furniturejobstatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FurnitureJobOrderForm", "JobClosed", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureJobOrderItem", "IsClosed", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureProformaInvoiceItem", "TotalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureUnfinishedMaterialTransferForm", "IsSent", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureUnfinishedMaterialTransferForm", "SentBy", c => c.String());
            AddColumn("Manufacturing.FurnitureUnfinishedMaterialTransferForm", "SentDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("Manufacturing.FurnitureUnfinishedMaterialTransferForm", "SentDate");
            DropColumn("Manufacturing.FurnitureUnfinishedMaterialTransferForm", "SentBy");
            DropColumn("Manufacturing.FurnitureUnfinishedMaterialTransferForm", "IsSent");
            DropColumn("Manufacturing.FurnitureProformaInvoiceItem", "TotalPrice");
            DropColumn("Manufacturing.FurnitureJobOrderItem", "IsClosed");
            DropColumn("EXCEED.FurnitureJobOrderForm", "JobClosed");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductDeliveryVatAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.ProductDelivery", "DeliveryStatus", c => c.Int(nullable: false));
            AddColumn("ProductFollowUp.ProductDeliveryItems", "VAT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("ProductFollowUp.ProductDeliveryItems", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("ProductFollowUp.ProductDeliveryItems", "Amount");
            DropColumn("ProductFollowUp.ProductDeliveryItems", "VAT");
            DropColumn("ProductFollowUp.ProductDelivery", "DeliveryStatus");
        }
    }
}

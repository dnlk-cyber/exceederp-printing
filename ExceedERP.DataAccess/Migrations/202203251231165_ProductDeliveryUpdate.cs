namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductDeliveryUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.ProductDelivery", "DeliveryStatus", c => c.Int(nullable: false));
            DropColumn("ProductFollowUp.ProductDeliveryItems", "Status");
        }
        
        public override void Down()
        {
            AddColumn("ProductFollowUp.ProductDeliveryItems", "Status", c => c.Int(nullable: false));
            DropColumn("ProductFollowUp.ProductDelivery", "DeliveryStatus");
        }
    }
}

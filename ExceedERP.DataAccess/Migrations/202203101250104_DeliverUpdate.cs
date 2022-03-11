namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeliverUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.ProductDeliveryItems", "JobId", c => c.Int(nullable: false));
            AddColumn("ProductFollowUp.ProductDeliveryItems", "ItemCategory", c => c.String());
            DropColumn("ProductFollowUp.ProductDelivery", "JobId");
        }
        
        public override void Down()
        {
            AddColumn("ProductFollowUp.ProductDelivery", "JobId", c => c.Int(nullable: false));
            DropColumn("ProductFollowUp.ProductDeliveryItems", "ItemCategory");
            DropColumn("ProductFollowUp.ProductDeliveryItems", "JobId");
        }
    }
}

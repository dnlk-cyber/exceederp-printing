namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductDeliveryItemStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.ProductDeliveryItems", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("ProductFollowUp.ProductDeliveryItems", "Status");
        }
    }
}

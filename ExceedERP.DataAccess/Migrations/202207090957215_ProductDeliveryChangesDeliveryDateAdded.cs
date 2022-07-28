namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductDeliveryChangesDeliveryDateAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.ProductDelivery", "DeliveryDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("ProductFollowUp.ProductDelivery", "DeliveryDate");
        }
    }
}

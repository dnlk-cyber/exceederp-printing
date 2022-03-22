namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeliveryAddCasAndCredit : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.ProductDelivery", "Cash", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("ProductFollowUp.ProductDelivery", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("ProductFollowUp.ProductDelivery", "Credit");
            DropColumn("ProductFollowUp.ProductDelivery", "Cash");
        }
    }
}

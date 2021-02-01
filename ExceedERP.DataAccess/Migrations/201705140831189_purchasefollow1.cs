namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class purchasefollow1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ProformaPurchase", "PurchaseCategory", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ProformaPurchase", "PurchaseCategory");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class foreignfollowup : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ForeignPurchase", "PurchaseRef", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ForeignPurchase", "PurchaseRef");
        }
    }
}

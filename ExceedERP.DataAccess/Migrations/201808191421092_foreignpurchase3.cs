namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class foreignpurchase3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ForeignPurchase", "Advance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ForeignPurchase", "LC", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ForeignPurchase", "Retention", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("EXCEED.ForeignPurchase", "Percentage");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ForeignPurchase", "Percentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("EXCEED.ForeignPurchase", "Retention");
            DropColumn("EXCEED.ForeignPurchase", "LC");
            DropColumn("EXCEED.ForeignPurchase", "Advance");
        }
    }
}

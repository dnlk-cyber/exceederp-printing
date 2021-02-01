namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class priceindex2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ItemPriceIndex", "ItemName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ItemPriceIndex", "ItemName");
        }
    }
}

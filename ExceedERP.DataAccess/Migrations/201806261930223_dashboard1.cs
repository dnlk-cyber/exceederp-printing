namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dashboard1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.InventoryItembalance", "StoreCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.InventoryItembalance", "StoreCode");
        }
    }
}

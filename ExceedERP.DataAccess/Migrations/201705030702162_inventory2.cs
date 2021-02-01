namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisition", "ToStoreCode", c => c.String());
            DropColumn("EXCEED.StoreRequisition", "ToStore");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.StoreRequisition", "ToStore", c => c.String());
            DropColumn("EXCEED.StoreRequisition", "ToStoreCode");
        }
    }
}

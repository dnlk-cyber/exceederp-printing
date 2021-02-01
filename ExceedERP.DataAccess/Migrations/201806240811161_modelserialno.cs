namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modelserialno : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisitionItem", "SerialNo", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "ChasisNo", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionItem", "SerialNo", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionItem", "ChasisNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseRequisitionItem", "ChasisNo");
            DropColumn("EXCEED.PurchaseRequisitionItem", "SerialNo");
            DropColumn("EXCEED.StoreRequisitionItem", "ChasisNo");
            DropColumn("EXCEED.StoreRequisitionItem", "SerialNo");
        }
    }
}

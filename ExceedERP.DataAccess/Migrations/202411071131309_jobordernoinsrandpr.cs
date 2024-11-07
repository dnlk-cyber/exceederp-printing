namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jobordernoinsrandpr : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseRequisitionItem", "ProductionJobNo", c => c.String());
           // AddColumn("HRM.RetirementSetting", "RetireSetting", c => c.Int(nullable: false));
            AddColumn("Inventory_Archive.StoreRequisitionItemArchive", "ProductionJobNo", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "ProductionJobNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StoreRequisitionItem", "ProductionJobNo");
            DropColumn("Inventory_Archive.StoreRequisitionItemArchive", "ProductionJobNo");
          //  DropColumn("HRM.RetirementSetting", "RetireSetting");
            DropColumn("EXCEED.PurchaseRequisitionItem", "ProductionJobNo");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proformapr : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ProformaPurchase", "StoreRequisitionID", c => c.Int());
            AlterColumn("EXCEED.ProformaPurchase", "PurchaseRequisitionID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ProformaPurchase", "PurchaseRequisitionID", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ProformaPurchase", "StoreRequisitionID", c => c.Int(nullable: false));
        }
    }
}

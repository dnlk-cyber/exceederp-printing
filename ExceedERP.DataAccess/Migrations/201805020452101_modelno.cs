namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modelno : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisitionItem", "ModelNo", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionItem", "ModelNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseRequisitionItem", "ModelNo");
            DropColumn("EXCEED.StoreRequisitionItem", "ModelNo");
        }
    }
}

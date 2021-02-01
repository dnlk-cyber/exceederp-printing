namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modelnoheader : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisition", "ModelNo", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "ModelNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseRequisition", "ModelNo");
            DropColumn("EXCEED.StoreRequisition", "ModelNo");
        }
    }
}

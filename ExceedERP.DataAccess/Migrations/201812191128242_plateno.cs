namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class plateno : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetRequisition", "PlateNo", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "PlateNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseRequisition", "PlateNo");
            DropColumn("EXCEED.FixedAssetRequisition", "PlateNo");
        }
    }
}

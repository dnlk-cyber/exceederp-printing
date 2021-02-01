namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassetreceive : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetReceive", "SupplierId", c => c.String());
            AddColumn("EXCEED.FixedAssetReceive", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetRequisition", "GLPeriodId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetRequisition", "GLPeriodId");
            DropColumn("EXCEED.FixedAssetReceive", "GLPeriodId");
            DropColumn("EXCEED.FixedAssetReceive", "SupplierId");
        }
    }
}

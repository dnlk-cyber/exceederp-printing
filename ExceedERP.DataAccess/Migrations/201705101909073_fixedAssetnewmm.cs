namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedAssetnewmm : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetReceiveItem", "FixedAseetCategoryCode", c => c.String());
            AddColumn("EXCEED.FixedAssetReceiveItem", "FixedAseetSubCategoryCode", c => c.String());
            //AddColumn("EXCEED.LocationTracking", "StoreCode", c => c.String());
            //AddColumn("EXCEED.LocationTracking", "EmployeeId", c => c.Int(nullable: false));
            //AddColumn("EXCEED.LocationTracking", "ProjectId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.LocationTracking", "ProjectId");
            DropColumn("EXCEED.LocationTracking", "EmployeeId");
            DropColumn("EXCEED.LocationTracking", "StoreCode");
            DropColumn("EXCEED.FixedAssetReceiveItem", "FixedAseetSubCategoryCode");
            DropColumn("EXCEED.FixedAssetReceiveItem", "FixedAseetCategoryCode");
        }
    }
}

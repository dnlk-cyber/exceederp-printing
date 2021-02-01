namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset24 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetRegistration", "OrgStructureId", c => c.String());
            AddColumn("EXCEED.AssetRegistration", "EmployeeId", c => c.String());
            AddColumn("EXCEED.AssetRegistration", "GroupCode", c => c.String());
            AddColumn("EXCEED.LocationTracking", "OrgStructureId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.LocationTracking", "OrgStructureId");
            DropColumn("EXCEED.AssetRegistration", "GroupCode");
            DropColumn("EXCEED.AssetRegistration", "EmployeeId");
            DropColumn("EXCEED.AssetRegistration", "OrgStructureId");
        }
    }
}

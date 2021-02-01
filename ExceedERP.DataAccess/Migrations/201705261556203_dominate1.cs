namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dominate1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetIssue", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("EXCEED.LocationTracking", "BranchId", c => c.Int(nullable: false));
            DropColumn("EXCEED.LocationTracking", "ProjectId");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.LocationTracking", "ProjectId", c => c.Int(nullable: false));
            DropColumn("EXCEED.LocationTracking", "BranchId");
            DropColumn("EXCEED.FixedAssetIssue", "EmployeeId");
        }
    }
}

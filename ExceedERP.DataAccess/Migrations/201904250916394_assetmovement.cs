namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assetmovement : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.LocationTracking", "EmployeeId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.LocationTracking", "EmployeeId", c => c.Int(nullable: false));
        }
    }
}

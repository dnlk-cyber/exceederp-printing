namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset23 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.LocationTracking", "EmployeeId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.LocationTracking", "EmployeeId", c => c.Int());
        }
    }
}

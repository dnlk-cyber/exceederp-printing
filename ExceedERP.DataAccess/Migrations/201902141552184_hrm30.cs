namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm30 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("HRM.Experience", "TotalDuration", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("HRM.Experience", "TotalDuration", c => c.Int(nullable: false));
        }
    }
}

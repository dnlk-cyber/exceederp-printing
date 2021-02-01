namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.LeaveEntitle", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("HRM.LeaveEntitle", "Balance");
        }
    }
}

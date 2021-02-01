namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm55 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.WeeklyAttendance", "Department", c => c.Int(nullable: false));
            AlterColumn("EXCEED.PaymentRequestLine", "ItemName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PaymentRequestLine", "ItemName", c => c.String());
            DropColumn("HRM.WeeklyAttendance", "Department");
        }
    }
}

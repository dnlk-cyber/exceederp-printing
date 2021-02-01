namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateforeign : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.PurchaseFollowUpItems", "LCAppliedDate", c => c.DateTime());
            DropColumn("HRM.Employee", "EmployeeType");
        }
        
        public override void Down()
        {
            AddColumn("HRM.Employee", "EmployeeType", c => c.Int(nullable: false));
            AlterColumn("EXCEED.PurchaseFollowUpItems", "LCAppliedDate", c => c.DateTime(nullable: false));
        }
    }
}

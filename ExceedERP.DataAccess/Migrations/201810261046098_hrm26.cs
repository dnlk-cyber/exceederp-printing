namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm26 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ARMiscelaneousCollection", "BranchId", c => c.Int(nullable: false));
            AddColumn("HRM.LeaveRequest", "Day", c => c.Int(nullable: false));
            AddColumn("HRM.LeaveRequest", "MorningOrAfternoon", c => c.Int(nullable: false));
            AddColumn("HRM.LeaveRequest", "Hours", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.LeaveRequest", "Hours");
            DropColumn("HRM.LeaveRequest", "MorningOrAfternoon");
            DropColumn("HRM.LeaveRequest", "Day");
            DropColumn("EXCEED.ARMiscelaneousCollection", "BranchId");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LeaveTypeAndRequestModifications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.DatePreferenceSetting",
                c => new
                    {
                        DatePreferenceSettingId = c.Int(nullable: false, identity: true),
                        UseEthiopianDate = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DatePreferenceSettingId);
            
            AddColumn("HRM.LeaveRequest", "ExactDeserveLeaveDays", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeaveRequest", "FromTime", c => c.DateTime());
            AddColumn("HRM.LeaveRequest", "ToTime", c => c.DateTime());
            AddColumn("HRM.LeaveTypeSetting", "MaxDaysBack", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("HRM.LeaveTypeSetting", "MaxDaysBack");
            DropColumn("HRM.LeaveRequest", "ToTime");
            DropColumn("HRM.LeaveRequest", "FromTime");
            DropColumn("HRM.LeaveRequest", "ExactDeserveLeaveDays");
            DropTable("EXCEED.DatePreferenceSetting");
        }
    }
}

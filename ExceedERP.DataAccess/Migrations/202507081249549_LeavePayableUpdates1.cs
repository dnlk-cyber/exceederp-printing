namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LeavePayableUpdates1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.LeaveTypeSetting",
                c => new
                    {
                        LeaveTypeSettingId = c.Int(nullable: false, identity: true),
                        LeaveType = c.Int(nullable: false),
                        IsWorkingDays = c.Boolean(nullable: false),
                        IsConsecutiveDays = c.Boolean(nullable: false),
                        MaximumAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BlockBackdatedLeave = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LeaveTypeSettingId);
            
        }
        
        public override void Down()
        {
            DropTable("HRM.LeaveTypeSetting");
        }
    }
}

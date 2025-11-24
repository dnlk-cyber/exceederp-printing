namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LeaveandOtherModelChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.LeaveEntitlementSettings",
                c => new
                    {
                        LeaveEntitlementSettingId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        IsGenerated = c.Boolean(nullable: false),
                        IsRegeneratingAllowed = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LeaveEntitlementSettingId);
            
            AddColumn("HRM.LeaveTypeSetting", "AllowFullAnnualLeave", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.LeaveTypeSetting", "AllowFullAnnualLeave");
            DropTable("HRM.LeaveEntitlementSettings");
        }
    }
}

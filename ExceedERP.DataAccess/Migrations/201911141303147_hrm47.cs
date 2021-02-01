namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm47 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.ManPowerPlanningRequest",
                c => new
                    {
                        ManPowerPlanningRequestId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(nullable: false),
                        EmploymentType = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        NoOfVaccantPostion = c.Int(nullable: false),
                        NoOfCurrentStaff = c.Int(nullable: false),
                        NoOfShouldBeStaff = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        PlanType = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        IsForOther = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ManPowerPlanningRequestId);
            
        }
        
        public override void Down()
        {
            DropTable("HRM.ManPowerPlanningRequest");
        }
    }
}

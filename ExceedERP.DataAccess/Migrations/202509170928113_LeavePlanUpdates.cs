namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LeavePlanUpdates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.LeavePlanValidation",
                c => new
                    {
                        LeavePlanValidationId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        LeavePlanId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LeavePlanValidationId)
                .ForeignKey("HRM.LeavePlan", t => t.LeavePlanId, cascadeDelete: true)
                .Index(t => t.LeavePlanId);
            
            AddColumn("HRM.LeavePlan", "Day", c => c.Int(nullable: false));
            AddColumn("HRM.LeavePlan", "GLPeriodId", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.LeavePlanValidation", "LeavePlanId", "HRM.LeavePlan");
            DropIndex("EXCEED.LeavePlanValidation", new[] { "LeavePlanId" });
            DropColumn("HRM.LeavePlan", "GLPeriodId");
            DropColumn("HRM.LeavePlan", "Day");
            DropTable("EXCEED.LeavePlanValidation");
        }
    }
}

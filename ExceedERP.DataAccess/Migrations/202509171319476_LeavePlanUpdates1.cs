namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LeavePlanUpdates1 : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "EXCEED.LeavePlanValidation", newSchema: "HRM");
            CreateTable(
                "HRM.LeavePlanFinalValidation",
                c => new
                    {
                        LeavePlanFinalValidationId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        LeavePlanId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LeavePlanFinalValidationId)
                .ForeignKey("HRM.LeavePlan", t => t.LeavePlanId, cascadeDelete: true)
                .Index(t => t.LeavePlanId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.LeavePlanFinalValidation", "LeavePlanId", "HRM.LeavePlan");
            DropIndex("HRM.LeavePlanFinalValidation", new[] { "LeavePlanId" });
            DropTable("HRM.LeavePlanFinalValidation");
            MoveTable(name: "HRM.LeavePlanValidation", newSchema: "EXCEED");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HealthRankAndValueandHealthAprovalSetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Health.HealthApprovalSettings",
                c => new
                    {
                        HealthApprovalSettingId = c.Int(nullable: false, identity: true),
                        RequiresHrApprovalForPaymentRequest = c.Boolean(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.HealthApprovalSettingId);
            CreateTable(
                "Health.HealthRankAndValueFinalValidation",
                c => new
                    {
                        HealthRankAndValueFinalValidationId = c.Int(nullable: false, identity: true),
                        HealthRankAndValueId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.HealthRankAndValueFinalValidationId)
                .ForeignKey("Health.HealthRankAndValues", t => t.HealthRankAndValueId, cascadeDelete: true)
                .Index(t => t.HealthRankAndValueId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Health.HealthRankAndValueFinalValidation", "HealthRankAndValueId", "Health.HealthRankAndValues");
            DropIndex("Health.HealthRankAndValueFinalValidation", new[] { "HealthRankAndValueId" });
            DropTable("Health.HealthRankAndValueFinalValidation");
            DropTable("Health.HealthApprovalSettings");
        }
    }
}

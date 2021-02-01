namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class schedulebank : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ReconciliationBankServiceChargeDistribution",
                c => new
                    {
                        ReconciliationBankServiceChargeDistributionId = c.Int(nullable: false, identity: true),
                        ReconciliationBankServiceChargeId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ReconciliationBankServiceChargeDistributionId)
                .ForeignKey("EXCEED.ReconciliationBankServiceCharge", t => t.ReconciliationBankServiceChargeId, cascadeDelete: true)
                .Index(t => t.ReconciliationBankServiceChargeId);
            
            CreateTable(
                "EXCEED.ReconciliationBankServiceCharge",
                c => new
                    {
                        ReconciliationBankServiceChargeId = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        ReconciliationSchedule_ReconciliationScheduleId = c.Int(),
                    })
                .PrimaryKey(t => t.ReconciliationBankServiceChargeId)
                .ForeignKey("EXCEED.ReconciliationSchedule", t => t.ReconciliationSchedule_ReconciliationScheduleId)
                .Index(t => t.ReconciliationSchedule_ReconciliationScheduleId);
            
            CreateTable(
                "EXCEED.ReconciliationBankServiceChargeValidation",
                c => new
                    {
                        ReconciliationBankServiceChargeValidationId = c.Int(nullable: false, identity: true),
                        ReconciliationBankServiceChargeId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ReconciliationBankServiceChargeValidationId)
                .ForeignKey("EXCEED.ReconciliationBankServiceCharge", t => t.ReconciliationBankServiceChargeId, cascadeDelete: true)
                .Index(t => t.ReconciliationBankServiceChargeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ReconciliationBankServiceCharge", "ReconciliationSchedule_ReconciliationScheduleId", "EXCEED.ReconciliationSchedule");
            DropForeignKey("EXCEED.ReconciliationBankServiceChargeValidation", "ReconciliationBankServiceChargeId", "EXCEED.ReconciliationBankServiceCharge");
            DropForeignKey("EXCEED.ReconciliationBankServiceChargeDistribution", "ReconciliationBankServiceChargeId", "EXCEED.ReconciliationBankServiceCharge");
            DropIndex("EXCEED.ReconciliationBankServiceChargeValidation", new[] { "ReconciliationBankServiceChargeId" });
            DropIndex("EXCEED.ReconciliationBankServiceCharge", new[] { "ReconciliationSchedule_ReconciliationScheduleId" });
            DropIndex("EXCEED.ReconciliationBankServiceChargeDistribution", new[] { "ReconciliationBankServiceChargeId" });
            DropTable("EXCEED.ReconciliationBankServiceChargeValidation");
            DropTable("EXCEED.ReconciliationBankServiceCharge");
            DropTable("EXCEED.ReconciliationBankServiceChargeDistribution");
        }
    }
}

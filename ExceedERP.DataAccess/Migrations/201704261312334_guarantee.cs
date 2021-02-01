namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guarantee : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GuaranteeRenewal",
                c => new
                    {
                        GuaranteeRenewalId = c.Int(nullable: false, identity: true),
                        GuaranteeId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ValidityPeriod = c.Int(nullable: false),
                        GuaranteeRefNo = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GuaranteeRenewalId)
                .ForeignKey("EXCEED.Guarantee", t => t.GuaranteeId, cascadeDelete: true)
                .Index(t => t.GuaranteeId);
            
            CreateTable(
                "EXCEED.Guarantee",
                c => new
                    {
                        GuaranteeId = c.Int(nullable: false, identity: true),
                        SupplierName = c.String(nullable: false),
                        SupplierId = c.String(),
                        ProcurementCategory = c.Int(nullable: false),
                        GuaranteeType = c.Int(nullable: false),
                        GuaranteeMode = c.Int(nullable: false),
                        GuaranteeRefNo = c.String(),
                        ProcurementRefNo = c.String(),
                        ContractRefNo = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Currency = c.String(),
                        IssueDate = c.DateTime(nullable: false),
                        Bank = c.String(),
                        BankBranch = c.String(),
                        ValidityPeriod = c.Int(nullable: false),
                        ExpireDate = c.DateTime(nullable: false),
                        ValidityStatus = c.Int(nullable: false),
                        GuaranteePresenterName = c.String(),
                        GuaranteePresenterAddress = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GuaranteeId);
            
            CreateTable(
                "EXCEED.GuaranteeValidation",
                c => new
                    {
                        GuaranteeValidationId = c.Int(nullable: false, identity: true),
                        GuaranteeId = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GuaranteeValidationId)
                .ForeignKey("EXCEED.Guarantee", t => t.GuaranteeId, cascadeDelete: true)
                .Index(t => t.GuaranteeId);
            
            CreateTable(
                "EXCEED.GuaranteeRule",
                c => new
                    {
                        GuaranteeRuleId = c.Int(nullable: false, identity: true),
                        ProcurementCategory = c.Int(nullable: false),
                        GuaranteeType = c.Int(nullable: false),
                        GuaranteeMode = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GuaranteeRuleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.GuaranteeValidation", "GuaranteeId", "EXCEED.Guarantee");
            DropForeignKey("EXCEED.GuaranteeRenewal", "GuaranteeId", "EXCEED.Guarantee");
            DropIndex("EXCEED.GuaranteeValidation", new[] { "GuaranteeId" });
            DropIndex("EXCEED.GuaranteeRenewal", new[] { "GuaranteeId" });
            DropTable("EXCEED.GuaranteeRule");
            DropTable("EXCEED.GuaranteeValidation");
            DropTable("EXCEED.Guarantee");
            DropTable("EXCEED.GuaranteeRenewal");
        }
    }
}

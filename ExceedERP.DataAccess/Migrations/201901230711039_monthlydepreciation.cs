namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class monthlydepreciation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.MonthlyDepreciationDistribution",
                c => new
                    {
                        MonthlyDepreciationDistributionId = c.Int(nullable: false, identity: true),
                        MonthlyDepreciationId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.MonthlyDepreciationDistributionId)
                .ForeignKey("EXCEED.MonthlyDepreciation", t => t.MonthlyDepreciationId, cascadeDelete: true)
                .Index(t => t.MonthlyDepreciationId);
            
            CreateTable(
                "EXCEED.MonthlyDepreciation",
                c => new
                    {
                        MonthlyDepreciationId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyDepreciationId);
            
            CreateTable(
                "EXCEED.MonthlyDepreciationItem",
                c => new
                    {
                        MonthlyDepreciationItemId = c.Int(nullable: false, identity: true),
                        MonthlyDepreciationId = c.Int(nullable: false),
                        AssetRegistrationId = c.Int(nullable: false),
                        DepreciationAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BookValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyDepreciationItemId)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .ForeignKey("EXCEED.MonthlyDepreciation", t => t.MonthlyDepreciationId, cascadeDelete: true)
                .Index(t => t.MonthlyDepreciationId)
                .Index(t => t.AssetRegistrationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.MonthlyDepreciationItem", "MonthlyDepreciationId", "EXCEED.MonthlyDepreciation");
            DropForeignKey("EXCEED.MonthlyDepreciationItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.MonthlyDepreciationDistribution", "MonthlyDepreciationId", "EXCEED.MonthlyDepreciation");
            DropIndex("EXCEED.MonthlyDepreciationItem", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.MonthlyDepreciationItem", new[] { "MonthlyDepreciationId" });
            DropIndex("EXCEED.MonthlyDepreciationDistribution", new[] { "MonthlyDepreciationId" });
            DropTable("EXCEED.MonthlyDepreciationItem");
            DropTable("EXCEED.MonthlyDepreciation");
            DropTable("EXCEED.MonthlyDepreciationDistribution");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassetdepre : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.AssetAccessory",
                c => new
                    {
                        AssetAccessoryId = c.Int(nullable: false, identity: true),
                        AssetRegistrationId = c.Int(nullable: false),
                        Description = c.String(),
                        Quantity = c.Int(nullable: false),
                        SerialNo = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AssetAccessoryId)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .Index(t => t.AssetRegistrationId);
            
            CreateTable(
                "EXCEED.IfrsDepreciationDistribution",
                c => new
                    {
                        IfrsDepreciationDistributionId = c.Int(nullable: false, identity: true),
                        IfrsDepreciationId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.IfrsDepreciationDistributionId)
                .ForeignKey("EXCEED.IfrsDepreciation", t => t.IfrsDepreciationId, cascadeDelete: true)
                .Index(t => t.IfrsDepreciationId);
            
            CreateTable(
                "EXCEED.IfrsDepreciation",
                c => new
                    {
                        IfrsDepreciationId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        GlPeriodId = c.Int(),
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
                .PrimaryKey(t => t.IfrsDepreciationId);
            
            CreateTable(
                "EXCEED.IfrsDepreciationItem",
                c => new
                    {
                        IfrsDepreciationItemId = c.Int(nullable: false, identity: true),
                        IfrsDepreciationId = c.Int(nullable: false),
                        AssetRegistrationId = c.Int(nullable: false),
                        DepreciationAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AccumulatedDepreciationAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BookValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.IfrsDepreciationItemId)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .ForeignKey("EXCEED.IfrsDepreciation", t => t.IfrsDepreciationId, cascadeDelete: true)
                .Index(t => t.IfrsDepreciationId)
                .Index(t => t.AssetRegistrationId);
            
            CreateTable(
                "EXCEED.IfrsDepreciationValidation",
                c => new
                    {
                        IfrsDepreciationValidationId = c.Int(nullable: false, identity: true),
                        IfrsDepreciationId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.IfrsDepreciationValidationId)
                .ForeignKey("EXCEED.IfrsDepreciation", t => t.IfrsDepreciationId, cascadeDelete: true)
                .Index(t => t.IfrsDepreciationId);
            
            AddColumn("EXCEED.AssetRegistration", "AcqusitionDate", c => c.DateTime());
            AddColumn("EXCEED.AssetRegistration", "IFRSBookValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.AssetRegistration", "UsefulLifeYears", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.AssetRegistration", "SalvageValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.AssetRegistration", "SalvageValueType", c => c.Int(nullable: false));
            AddColumn("EXCEED.AnnualDepreciation", "GlPeriodId", c => c.Int());
            AddColumn("EXCEED.AnnualDepreciationItem", "AccumulatedDepreciationAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.AnnualDepreciationItem", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.AnnualDepreciationItem", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.AnnualDepreciationItem", "FixedAssetGroupID", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetRevaluation", "RevaluationType", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetRevaluation", "BookValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.FixedAssetRevaluation", "UsefulLifeYears", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("EXCEED.Appreciation", "RegistrationId");
            DropColumn("EXCEED.AnnualDepreciation", "BranchId");
            DropColumn("EXCEED.AnnualDepreciation", "BusinessUnitId");
            DropColumn("EXCEED.AnnualDepreciation", "FixedAssetGroupID");
            DropColumn("EXCEED.FixedAssetRevaluation", "AddedValue");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FixedAssetRevaluation", "AddedValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.AnnualDepreciation", "FixedAssetGroupID", c => c.Int(nullable: false));
            AddColumn("EXCEED.AnnualDepreciation", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.AnnualDepreciation", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.Appreciation", "RegistrationId", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.IfrsDepreciationValidation", "IfrsDepreciationId", "EXCEED.IfrsDepreciation");
            DropForeignKey("EXCEED.IfrsDepreciationItem", "IfrsDepreciationId", "EXCEED.IfrsDepreciation");
            DropForeignKey("EXCEED.IfrsDepreciationItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.IfrsDepreciationDistribution", "IfrsDepreciationId", "EXCEED.IfrsDepreciation");
            DropForeignKey("EXCEED.AssetAccessory", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.IfrsDepreciationValidation", new[] { "IfrsDepreciationId" });
            DropIndex("EXCEED.IfrsDepreciationItem", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.IfrsDepreciationItem", new[] { "IfrsDepreciationId" });
            DropIndex("EXCEED.IfrsDepreciationDistribution", new[] { "IfrsDepreciationId" });
            DropIndex("EXCEED.AssetAccessory", new[] { "AssetRegistrationId" });
            DropColumn("EXCEED.FixedAssetRevaluation", "UsefulLifeYears");
            DropColumn("EXCEED.FixedAssetRevaluation", "BookValue");
            DropColumn("EXCEED.FixedAssetRevaluation", "RevaluationType");
            DropColumn("EXCEED.AnnualDepreciationItem", "FixedAssetGroupID");
            DropColumn("EXCEED.AnnualDepreciationItem", "BusinessUnitId");
            DropColumn("EXCEED.AnnualDepreciationItem", "BranchId");
            DropColumn("EXCEED.AnnualDepreciationItem", "AccumulatedDepreciationAmount");
            DropColumn("EXCEED.AnnualDepreciation", "GlPeriodId");
            DropColumn("EXCEED.AssetRegistration", "SalvageValueType");
            DropColumn("EXCEED.AssetRegistration", "SalvageValue");
            DropColumn("EXCEED.AssetRegistration", "UsefulLifeYears");
            DropColumn("EXCEED.AssetRegistration", "IFRSBookValue");
            DropColumn("EXCEED.AssetRegistration", "AcqusitionDate");
            DropTable("EXCEED.IfrsDepreciationValidation");
            DropTable("EXCEED.IfrsDepreciationItem");
            DropTable("EXCEED.IfrsDepreciation");
            DropTable("EXCEED.IfrsDepreciationDistribution");
            DropTable("EXCEED.AssetAccessory");
        }
    }
}

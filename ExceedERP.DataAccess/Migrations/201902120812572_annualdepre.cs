namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class annualdepre : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.AnnualDepreciation", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.AnnualDepreciation", new[] { "AssetRegistrationId" });
            RenameColumn(table: "EXCEED.AnnualDepreciation", name: "AssetRegistrationId", newName: "AssetRegistration_AssetRegistrationId");
            CreateTable(
                "EXCEED.AnnualDepreciationDistribution",
                c => new
                    {
                        AnnualDepreciationDistributionId = c.Int(nullable: false, identity: true),
                        AnnualDepreciationId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.AnnualDepreciationDistributionId)
                .ForeignKey("EXCEED.AnnualDepreciation", t => t.AnnualDepreciationId, cascadeDelete: true)
                .Index(t => t.AnnualDepreciationId);
            
            CreateTable(
                "EXCEED.AnnualDepreciationItem",
                c => new
                    {
                        AnnualDepreciationItemId = c.Int(nullable: false, identity: true),
                        AnnualDepreciationId = c.Int(nullable: false),
                        AssetRegistrationId = c.Int(nullable: false),
                        DepreciationAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BookValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AnnualDepreciationItemId)
                .ForeignKey("EXCEED.AnnualDepreciation", t => t.AnnualDepreciationId, cascadeDelete: true)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .Index(t => t.AnnualDepreciationId)
                .Index(t => t.AssetRegistrationId);
            
            CreateTable(
                "EXCEED.AnnualDepreciationValidation",
                c => new
                    {
                        AnnualDepreciationValidationId = c.Int(nullable: false, identity: true),
                        AnnualDepreciationId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AnnualDepreciationValidationId)
                .ForeignKey("EXCEED.AnnualDepreciation", t => t.AnnualDepreciationId, cascadeDelete: true)
                .Index(t => t.AnnualDepreciationId);
            
            AddColumn("EXCEED.AnnualDepreciation", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.AnnualDepreciation", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AnnualDepreciation", "Status", c => c.String());
            AddColumn("EXCEED.AnnualDepreciation", "Remark", c => c.String());
            AddColumn("EXCEED.AnnualDepreciation", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AnnualDepreciation", "VoidBy", c => c.String());
            AddColumn("EXCEED.AnnualDepreciation", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.AnnualDepreciation", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AnnualDepreciation", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.AnnualDepreciation", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.AnnualDepreciation", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AnnualDepreciation", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.AnnualDepreciation", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.AnnualDepreciation", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AnnualDepreciation", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.AnnualDepreciation", "SendForApprovalTime", c => c.DateTime());
            AlterColumn("EXCEED.AnnualDepreciation", "AssetRegistration_AssetRegistrationId", c => c.Int());
            CreateIndex("EXCEED.AnnualDepreciation", "AssetRegistration_AssetRegistrationId");
            AddForeignKey("EXCEED.AnnualDepreciation", "AssetRegistration_AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId");
            DropColumn("EXCEED.AnnualDepreciation", "DepreciationAmount");
            DropColumn("EXCEED.AnnualDepreciation", "BookValue");
            DropColumn("EXCEED.AnnualDepreciation", "AccumulatedDepreciationAmount");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.AnnualDepreciation", "AccumulatedDepreciationAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.AnnualDepreciation", "BookValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.AnnualDepreciation", "DepreciationAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("EXCEED.AnnualDepreciation", "AssetRegistration_AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.AnnualDepreciationValidation", "AnnualDepreciationId", "EXCEED.AnnualDepreciation");
            DropForeignKey("EXCEED.AnnualDepreciationItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.AnnualDepreciationItem", "AnnualDepreciationId", "EXCEED.AnnualDepreciation");
            DropForeignKey("EXCEED.AnnualDepreciationDistribution", "AnnualDepreciationId", "EXCEED.AnnualDepreciation");
            DropIndex("EXCEED.AnnualDepreciationValidation", new[] { "AnnualDepreciationId" });
            DropIndex("EXCEED.AnnualDepreciationItem", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.AnnualDepreciationItem", new[] { "AnnualDepreciationId" });
            DropIndex("EXCEED.AnnualDepreciationDistribution", new[] { "AnnualDepreciationId" });
            DropIndex("EXCEED.AnnualDepreciation", new[] { "AssetRegistration_AssetRegistrationId" });
            AlterColumn("EXCEED.AnnualDepreciation", "AssetRegistration_AssetRegistrationId", c => c.Int(nullable: false));
            DropColumn("EXCEED.AnnualDepreciation", "SendForApprovalTime");
            DropColumn("EXCEED.AnnualDepreciation", "SendForApprovalBy");
            DropColumn("EXCEED.AnnualDepreciation", "IsSendForApproval");
            DropColumn("EXCEED.AnnualDepreciation", "OnlineTransferredTime");
            DropColumn("EXCEED.AnnualDepreciation", "IsOnlineTransferredBy");
            DropColumn("EXCEED.AnnualDepreciation", "IsOnlineTransferred");
            DropColumn("EXCEED.AnnualDepreciation", "OnlineApprovedTime");
            DropColumn("EXCEED.AnnualDepreciation", "OnlineApprovedBy");
            DropColumn("EXCEED.AnnualDepreciation", "IsOnlineApproved");
            DropColumn("EXCEED.AnnualDepreciation", "VoidTime");
            DropColumn("EXCEED.AnnualDepreciation", "VoidBy");
            DropColumn("EXCEED.AnnualDepreciation", "IsVoid");
            DropColumn("EXCEED.AnnualDepreciation", "Remark");
            DropColumn("EXCEED.AnnualDepreciation", "Status");
            DropColumn("EXCEED.AnnualDepreciation", "IsPostedToSecondLedger");
            DropColumn("EXCEED.AnnualDepreciation", "BranchId");
            DropTable("EXCEED.AnnualDepreciationValidation");
            DropTable("EXCEED.AnnualDepreciationItem");
            DropTable("EXCEED.AnnualDepreciationDistribution");
            RenameColumn(table: "EXCEED.AnnualDepreciation", name: "AssetRegistration_AssetRegistrationId", newName: "AssetRegistrationId");
            CreateIndex("EXCEED.AnnualDepreciation", "AssetRegistrationId");
            AddForeignKey("EXCEED.AnnualDepreciation", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
        }
    }
}

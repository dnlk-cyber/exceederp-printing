namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction33 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.ContractClaim", "ContractClaimTypeId", "Construction.ContractClaimType");
            DropForeignKey("Construction.EarnedValuePerProject", "ProjectInformationId", "Construction.ProjectInformation");
            DropIndex("Construction.ContractClaim", new[] { "ContractClaimTypeId" });
            DropIndex("Construction.EarnedValuePerProject", new[] { "ProjectInformationId" });
            CreateTable(
                "Construction.MaterialTransportationCost",
                c => new
                    {
                        MaterialTransportationCostId = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(),
                        ItemCode = c.String(),
                        UoM = c.String(),
                        TrackCapacityPerTrip = c.Decimal(nullable: false, precision: 18, scale: 4),
                        AverageSpeedOfTrack = c.Decimal(nullable: false, precision: 18, scale: 4),
                        RentalCostWithFuel = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DriverPerdiem = c.Decimal(nullable: false, precision: 18, scale: 4),
                        AssistantDriverPerdiem = c.Decimal(nullable: false, precision: 18, scale: 4),
                        AverageDistanceFromSourceToSite = c.Decimal(nullable: false, precision: 18, scale: 4),
                        RequiredTime = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CostPerUnit = c.Decimal(nullable: false, precision: 18, scale: 4),
                        LoadingUnloadingCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCostPerUnit = c.Decimal(nullable: false, precision: 18, scale: 4),
                        BranchId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MaterialTransportationCostId);
            
            AddColumn("Construction.ContractClaim", "Description", c => c.String());
            AddColumn("Construction.ContractClaim", "RequestedDate", c => c.DateTime(nullable: false));
            AddColumn("Construction.ContractClaim", "ReferanceLetter", c => c.String());
            AddColumn("Construction.ContractClaim", "RespondedDate", c => c.DateTime(nullable: false));
            AddColumn("Construction.ContractClaim", "ResponseRefNumber", c => c.String());
            AddColumn("Construction.ContractClaim", "ResponseReferanceLatter", c => c.String());
            AddColumn("Construction.ContractClaim", "TimeConsumed", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ContractClaim", "OrgBranchName", c => c.String());
            AddColumn("Construction.ContractClaim", "DateCreated", c => c.DateTime());
            AddColumn("Construction.ContractClaim", "LastModified", c => c.DateTime());
            AddColumn("Construction.ContractClaim", "CreatedBy", c => c.String());
            AddColumn("Construction.ContractClaim", "ModifiedBy", c => c.String());
            AddColumn("Construction.EarnedValuePerItem", "GlFiscalYear", c => c.String());
            AddColumn("Construction.EarnedValuePerProject", "ConstructionPeriodId", c => c.Int(nullable: false));
            AddColumn("Construction.EarnedValuePerProject", "GlFiscalYear", c => c.String());
            AddColumn("Construction.EarnedValuePerProject", "BCWSRaw", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.EarnedValuePerProject", "BCWPRaw", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("Construction.EarnedValuePerProject", "ACWPRaw", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "BAC", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "BCWS", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "BCWP", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "ACWP", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "PCP", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "PCA", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "CV", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "CVPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "CPI", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "SV", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "SVPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "SPI", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "CR", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "TCPI", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "TSPI", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "EAC", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "VAC", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "ETC", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerItem", "ISAC", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "BAC", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "BCWS", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "BCWP", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "ACWP", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "PCP", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "PCA", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "CV", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "CVPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "CPI", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "SV", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "SVPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "SPI", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "CR", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "TCPI", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "TSPI", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "EAC", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "VAC", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "ETC", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EarnedValuePerProject", "ISAC", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            DropColumn("Construction.ContractClaim", "ContractClaimTypeId");
            DropColumn("Construction.ContractClaim", "IssuedDate");
            DropColumn("Construction.ContractClaim", "IssuedBy");
            DropColumn("Construction.ContractClaim", "ClaimStatus");
            DropColumn("Construction.EarnedValuePerProject", "Year");
            DropColumn("Construction.EarnedValuePerProject", "Month");
        }
        
        public override void Down()
        {
            AddColumn("Construction.EarnedValuePerProject", "Month", c => c.Int(nullable: false));
            AddColumn("Construction.EarnedValuePerProject", "Year", c => c.String());
            AddColumn("Construction.ContractClaim", "ClaimStatus", c => c.String());
            AddColumn("Construction.ContractClaim", "IssuedBy", c => c.String());
            AddColumn("Construction.ContractClaim", "IssuedDate", c => c.DateTime(nullable: false));
            AddColumn("Construction.ContractClaim", "ContractClaimTypeId", c => c.Int(nullable: false));
            AlterColumn("Construction.EarnedValuePerProject", "ISAC", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "ETC", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "VAC", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "EAC", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "TSPI", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "TCPI", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "CR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "SPI", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "SVPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "SV", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "CPI", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "CVPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "CV", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "PCA", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "PCP", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "ACWP", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "BCWP", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "BCWS", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerProject", "BAC", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "ISAC", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "ETC", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "VAC", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "EAC", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "TSPI", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "TCPI", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "CR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "SPI", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "SVPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "SV", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "CPI", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "CVPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "CV", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "PCA", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "PCP", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "ACWP", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "BCWP", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "BCWS", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EarnedValuePerItem", "BAC", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Construction.EarnedValuePerProject", "ACWPRaw");
            DropColumn("Construction.EarnedValuePerProject", "BCWPRaw");
            DropColumn("Construction.EarnedValuePerProject", "BCWSRaw");
            DropColumn("Construction.EarnedValuePerProject", "GlFiscalYear");
            DropColumn("Construction.EarnedValuePerProject", "ConstructionPeriodId");
            DropColumn("Construction.EarnedValuePerItem", "GlFiscalYear");
            DropColumn("Construction.ContractClaim", "ModifiedBy");
            DropColumn("Construction.ContractClaim", "CreatedBy");
            DropColumn("Construction.ContractClaim", "LastModified");
            DropColumn("Construction.ContractClaim", "DateCreated");
            DropColumn("Construction.ContractClaim", "OrgBranchName");
            DropColumn("Construction.ContractClaim", "TimeConsumed");
            DropColumn("Construction.ContractClaim", "ResponseReferanceLatter");
            DropColumn("Construction.ContractClaim", "ResponseRefNumber");
            DropColumn("Construction.ContractClaim", "RespondedDate");
            DropColumn("Construction.ContractClaim", "ReferanceLetter");
            DropColumn("Construction.ContractClaim", "RequestedDate");
            DropColumn("Construction.ContractClaim", "Description");
            DropTable("Construction.MaterialTransportationCost");
            CreateIndex("Construction.EarnedValuePerProject", "ProjectInformationId");
            CreateIndex("Construction.ContractClaim", "ContractClaimTypeId");
            AddForeignKey("Construction.EarnedValuePerProject", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
            AddForeignKey("Construction.ContractClaim", "ContractClaimTypeId", "Construction.ContractClaimType", "ContractClaimTypeId", cascadeDelete: true);
        }
    }
}

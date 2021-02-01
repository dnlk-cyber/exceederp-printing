namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onepage : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.Appreciation", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.AnnualDepreciation", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.Disposal", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.FixedAssetRevaluation", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.Location", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.Sales", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.DepreciationForCost", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.DepreciationRule", "FixedAssetGroupID", "EXCEED.FixedAssetGroup");
            DropForeignKey("EXCEED.LocationTracking", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.AnnualDepreciation", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Appreciation", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Disposal", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.FixedAssetRevaluation", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Location", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Sales", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.DepreciationForCost", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.DepreciationRule", new[] { "FixedAssetGroupID" });
            DropIndex("EXCEED.LocationTracking", new[] { "AssetRegistrationId" });
            DropTable("EXCEED.AnnualDepreciation");
            DropTable("EXCEED.AssetRegistration");
            DropTable("EXCEED.Appreciation");
            DropTable("EXCEED.Disposal");
            DropTable("EXCEED.FixedAssetRevaluation");
            DropTable("EXCEED.Location");
            DropTable("EXCEED.Sales");
            DropTable("EXCEED.DeprciationReverse");
            DropTable("EXCEED.DepreciationForCost");
            DropTable("EXCEED.DepreciationRule");
            DropTable("EXCEED.LocationTracking");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.LocationTracking",
                c => new
                    {
                        LocationTrackingId = c.Int(nullable: false, identity: true),
                        AssetRegistrationId = c.Int(nullable: false),
                        LocationType = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDateDate = c.DateTime(),
                        Description = c.String(),
                        StoreCode = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        ProjectId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LocationTrackingId);
            
            CreateTable(
                "EXCEED.DepreciationRule",
                c => new
                    {
                        DepreciationRuleId = c.Int(nullable: false, identity: true),
                        CategoryCode = c.String(nullable: false),
                        AccountCode = c.String(),
                        DepreciationCode = c.String(),
                        ClearingAccount = c.String(),
                        AccumulatedAccount = c.String(),
                        FixedAssetGroupID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DepreciationRuleId);
            
            CreateTable(
                "EXCEED.DepreciationForCost",
                c => new
                    {
                        DepreciationForCostId = c.Int(nullable: false, identity: true),
                        AssetRegistrationId = c.Int(nullable: false),
                        DepreciationAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BookValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLPeriodId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DepreciationForCostId);
            
            CreateTable(
                "EXCEED.DeprciationReverse",
                c => new
                    {
                        DeprciationReverseId = c.Int(nullable: false, identity: true),
                        RegistrationID = c.Int(nullable: false),
                        StoreCode = c.String(),
                        Period = c.String(),
                        FiscalYear = c.String(),
                        BookValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DeprciationReverseId);
            
            CreateTable(
                "EXCEED.Sales",
                c => new
                    {
                        SalesID = c.Int(nullable: false, identity: true),
                        AssetRegistrationId = c.Int(nullable: false),
                        Amount = c.String(nullable: false),
                        TaxAmount = c.String(),
                        TotalAmount = c.String(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesID);
            
            CreateTable(
                "EXCEED.Location",
                c => new
                    {
                        LocationID = c.Int(nullable: false, identity: true),
                        AssetRegistrationId = c.Int(nullable: false),
                        LocationUser = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        ReturnDate = c.DateTime(),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LocationID);
            
            CreateTable(
                "EXCEED.FixedAssetRevaluation",
                c => new
                    {
                        FixedAssetRevaluationID = c.Int(nullable: false, identity: true),
                        AssetRegistrationId = c.Int(nullable: false),
                        AddedValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        RevaluationDate = c.DateTime(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetRevaluationID);
            
            CreateTable(
                "EXCEED.Disposal",
                c => new
                    {
                        DisposalID = c.Int(nullable: false, identity: true),
                        AssetRegistrationId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DisposalID);
            
            CreateTable(
                "EXCEED.Appreciation",
                c => new
                    {
                        AppreciationID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RegistrationId = c.Int(nullable: false),
                        AppreciationDate = c.DateTime(nullable: false),
                        AssetRegistrationId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AppreciationID);
            
            CreateTable(
                "EXCEED.AssetRegistration",
                c => new
                    {
                        AssetRegistrationId = c.Int(nullable: false, identity: true),
                        StoreCode = c.String(),
                        CategoryCode = c.String(nullable: false),
                        SubCategoryCode = c.String(nullable: false),
                        Name = c.String(),
                        Tag = c.String(),
                        SIV = c.String(),
                        PurchaseDate = c.DateTime(),
                        PurchasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GRN = c.String(),
                        GRNDate = c.DateTime(),
                        OriginalValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BookValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Int(nullable: false),
                        IsMachinery = c.Boolean(nullable: false),
                        Supplier = c.String(),
                        Manufacturer = c.String(),
                        SerialNo = c.String(),
                        PlateNo = c.String(),
                        PlateType = c.String(),
                        ChasisNo = c.String(),
                        MotorNo = c.String(),
                        ModelNo = c.String(),
                        LoadCapacity = c.String(),
                        FuelType = c.String(),
                        ManufacturDate = c.Int(nullable: false),
                        NetWeight = c.String(),
                        TotalWeight = c.String(),
                        Battery = c.String(),
                        Tyre = c.String(),
                        IsBuilding = c.Boolean(nullable: false),
                        Location = c.String(),
                        Contractor = c.String(),
                        ConstructionStartDate = c.DateTime(),
                        ConstructionEndDate = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AssetRegistrationId);
            
            CreateTable(
                "EXCEED.AnnualDepreciation",
                c => new
                    {
                        AnnualDepreciationID = c.Int(nullable: false, identity: true),
                        AssetRegistrationId = c.Int(nullable: false),
                        DepreciationAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BookValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GlFiscalYearId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AnnualDepreciationID);
            
            CreateIndex("EXCEED.LocationTracking", "AssetRegistrationId");
            CreateIndex("EXCEED.DepreciationRule", "FixedAssetGroupID");
            CreateIndex("EXCEED.DepreciationForCost", "AssetRegistrationId");
            CreateIndex("EXCEED.Sales", "AssetRegistrationId");
            CreateIndex("EXCEED.Location", "AssetRegistrationId");
            CreateIndex("EXCEED.FixedAssetRevaluation", "AssetRegistrationId");
            CreateIndex("EXCEED.Disposal", "AssetRegistrationId");
            CreateIndex("EXCEED.Appreciation", "AssetRegistrationId");
            CreateIndex("EXCEED.AnnualDepreciation", "AssetRegistrationId");
            AddForeignKey("EXCEED.LocationTracking", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.DepreciationRule", "FixedAssetGroupID", "EXCEED.FixedAssetGroup", "FixedAssetGroupID", cascadeDelete: true);
            AddForeignKey("EXCEED.DepreciationForCost", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.Sales", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.Location", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.FixedAssetRevaluation", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.Disposal", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.AnnualDepreciation", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.Appreciation", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
        }
    }
}

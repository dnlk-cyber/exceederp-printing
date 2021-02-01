namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onepage1 : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.AnnualDepreciationID)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .Index(t => t.AssetRegistrationId);
            
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
                .PrimaryKey(t => t.AppreciationID)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .Index(t => t.AssetRegistrationId);
            
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
                .PrimaryKey(t => t.DisposalID)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .Index(t => t.AssetRegistrationId);
            
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
                .PrimaryKey(t => t.FixedAssetRevaluationID)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .Index(t => t.AssetRegistrationId);
            
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
                .PrimaryKey(t => t.LocationID)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .Index(t => t.AssetRegistrationId);
            
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
                .PrimaryKey(t => t.SalesID)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .Index(t => t.AssetRegistrationId);
            
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
                .PrimaryKey(t => t.DepreciationForCostId)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .Index(t => t.AssetRegistrationId);
            
            //CreateTable(
            //    "EXCEED.DepreciationRule",
            //    c => new
            //        {
            //            DepreciationRuleId = c.Int(nullable: false, identity: true),
            //            CategoryCode = c.String(nullable: false),
            //            AccountCode = c.String(),
            //            DepreciationCode = c.String(),
            //            ClearingAccount = c.String(),
            //            AccumulatedAccount = c.String(),
            //            FixedAssetGroupID = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.DepreciationRuleId)
            //    .ForeignKey("EXCEED.FixedAssetGroup", t => t.FixedAssetGroupID, cascadeDelete: true)
            //    .Index(t => t.FixedAssetGroupID);
            
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
                .PrimaryKey(t => t.LocationTrackingId)
                .ForeignKey("EXCEED.AssetRegistration", t => t.AssetRegistrationId, cascadeDelete: true)
                .Index(t => t.AssetRegistrationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.LocationTracking", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.DepreciationRule", "FixedAssetGroupID", "EXCEED.FixedAssetGroup");
            DropForeignKey("EXCEED.DepreciationForCost", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.Sales", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.Location", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.FixedAssetRevaluation", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.Disposal", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.AnnualDepreciation", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.Appreciation", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.LocationTracking", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.DepreciationRule", new[] { "FixedAssetGroupID" });
            DropIndex("EXCEED.DepreciationForCost", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Sales", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Location", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.FixedAssetRevaluation", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Disposal", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Appreciation", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.AnnualDepreciation", new[] { "AssetRegistrationId" });
            DropTable("EXCEED.LocationTracking");
            DropTable("EXCEED.DepreciationRule");
            DropTable("EXCEED.DepreciationForCost");
            DropTable("EXCEED.DeprciationReverse");
            DropTable("EXCEED.Sales");
            DropTable("EXCEED.Location");
            DropTable("EXCEED.FixedAssetRevaluation");
            DropTable("EXCEED.Disposal");
            DropTable("EXCEED.Appreciation");
            DropTable("EXCEED.AssetRegistration");
            DropTable("EXCEED.AnnualDepreciation");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class fia : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("EXCEED.Appreciation", "RegistrationId", "EXCEED.Registration");
            //DropForeignKey("EXCEED.Registration", "DepreciationRuleId", "EXCEED.DepreciationRule");
            //DropForeignKey("EXCEED.Depreciation", "RegistrationID", "EXCEED.Registration");
            //DropForeignKey("EXCEED.Disposal", "RegistrationID", "EXCEED.Registration");
            //DropForeignKey("EXCEED.FixedAssetRevaluation", "RegistrationID", "EXCEED.Registration");
            //DropForeignKey("EXCEED.Location", "RegistrationID", "EXCEED.Registration");
            //DropForeignKey("EXCEED.Sales", "RegistrationID", "EXCEED.Registration");
            //DropForeignKey("EXCEED.LocationTracking", "RegistrationID", "EXCEED.Registration");
            //DropIndex("EXCEED.Appreciation", new[] { "RegistrationId" });
            //DropIndex("EXCEED.Registration", new[] { "DepreciationRuleId" });
            //DropIndex("EXCEED.Depreciation", new[] { "RegistrationID" });
            //DropIndex("EXCEED.Disposal", new[] { "RegistrationID" });
            //DropIndex("EXCEED.FixedAssetRevaluation", new[] { "RegistrationID" });
            //DropIndex("EXCEED.Location", new[] { "RegistrationID" });
            //DropIndex("EXCEED.Sales", new[] { "RegistrationID" });
            //DropIndex("EXCEED.LocationTracking", new[] { "RegistrationID" });
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
                        FixedAseetCategoryCode = c.String(),
                        FixedAseetSubCategoryCode = c.String(),
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
                        StoreCode = c.String(),
                        LoadCapacity = c.String(),
                        FuelType = c.String(),
                        ManufacturDate = c.Int(nullable: false),
                        NetWeight = c.String(),
                        TotalWeight = c.String(),
                        Battery = c.String(),
                        Tyre = c.String(),
                        IsBuilding = c.Boolean(nullable: false),
                        Location = c.String(),
                        CostCenter = c.String(),
                        Contractor = c.String(),
                        ConstructionStartDate = c.DateTime(),
                        // DepreciationRuleId = c.Int(nullable: false),
                        ConstructionEndDate = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AssetRegistrationId);
            //.ForeignKey("EXCEED.DepreciationRule", t => t.DepreciationRuleId, cascadeDelete: true)
            // .Index(t => t.DepreciationRuleId);

            AddColumn("EXCEED.Appreciation", "AssetRegistrationId", c => c.Int(nullable: false));
            //AddColumn("EXCEED.Disposal", "AssetRegistrationId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetRevaluation", "AssetRegistrationId", c => c.Int(nullable: false));
            AddColumn("EXCEED.Location", "AssetRegistrationId", c => c.Int(nullable: false));
            AddColumn("EXCEED.Sales", "AssetRegistrationId", c => c.Int(nullable: false));
            AddColumn("EXCEED.LocationTracking", "AssetRegistrationId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.Appreciation", "AssetRegistrationId");
            //CreateIndex("EXCEED.Disposal", "AssetRegistrationId");
            CreateIndex("EXCEED.FixedAssetRevaluation", "AssetRegistrationId");
            CreateIndex("EXCEED.Location", "AssetRegistrationId");
            CreateIndex("EXCEED.Sales", "AssetRegistrationId");
            CreateIndex("EXCEED.LocationTracking", "AssetRegistrationId");
            AddForeignKey("EXCEED.Appreciation", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            //AddForeignKey("EXCEED.Disposal", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.FixedAssetRevaluation", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.Location", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.Sales", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.LocationTracking", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            //DropColumn("EXCEED.Disposal", "RegistrationID");
            //DropColumn("EXCEED.FixedAssetRevaluation", "RegistrationID");
            //DropColumn("EXCEED.Location", "RegistrationID");
            //DropColumn("EXCEED.Sales", "RegistrationID");
            //DropColumn("EXCEED.LocationTracking", "RegistrationID");
            // DropTable("EXCEED.Registration");
            // DropTable("EXCEED.Depreciation");
        }

        public override void Down()
        {
            CreateTable(
                "EXCEED.Depreciation",
                c => new
                    {
                        DepreciationID = c.Int(nullable: false, identity: true),
                        RegistrationID = c.Int(nullable: false),
                        DepreciationAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BookValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GlFiscalYearId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DepreciationID);

            CreateTable(
                "EXCEED.Registration",
                c => new
                    {
                        RegistrationID = c.Int(nullable: false, identity: true),
                        FixedAseetCategoryCode = c.String(),
                        FixedAseetSubCategoryCode = c.String(),
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
                        StoreCode = c.String(),
                        LoadCapacity = c.String(),
                        FuelType = c.String(),
                        ManufacturDate = c.Int(nullable: false),
                        NetWeight = c.String(),
                        TotalWeight = c.String(),
                        Battery = c.String(),
                        Tyre = c.String(),
                        IsBuilding = c.Boolean(nullable: false),
                        Location = c.String(),
                        CostCenter = c.String(),
                        Contractor = c.String(),
                        ConstructionStartDate = c.DateTime(),
                        DepreciationRuleId = c.Int(nullable: false),
                        ConstructionEndDate = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RegistrationID);

            AddColumn("EXCEED.LocationTracking", "RegistrationID", c => c.Int(nullable: false));
            AddColumn("EXCEED.Sales", "RegistrationID", c => c.Int(nullable: false));
            AddColumn("EXCEED.Location", "RegistrationID", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetRevaluation", "RegistrationID", c => c.Int(nullable: false));
            AddColumn("EXCEED.Disposal", "RegistrationID", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.LocationTracking", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.Sales", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.Location", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.FixedAssetRevaluation", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.Disposal", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.AnnualDepreciation", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.AssetRegistration", "DepreciationRuleId", "EXCEED.DepreciationRule");
            DropForeignKey("EXCEED.Appreciation", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.LocationTracking", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Sales", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Location", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.FixedAssetRevaluation", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Disposal", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.Appreciation", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.AssetRegistration", new[] { "DepreciationRuleId" });
            DropIndex("EXCEED.AnnualDepreciation", new[] { "AssetRegistrationId" });
            DropColumn("EXCEED.LocationTracking", "AssetRegistrationId");
            DropColumn("EXCEED.Sales", "AssetRegistrationId");
            DropColumn("EXCEED.Location", "AssetRegistrationId");
            DropColumn("EXCEED.FixedAssetRevaluation", "AssetRegistrationId");
            DropColumn("EXCEED.Disposal", "AssetRegistrationId");
            DropColumn("EXCEED.Appreciation", "AssetRegistrationId");
            DropTable("EXCEED.AssetRegistration");
            DropTable("EXCEED.AnnualDepreciation");
            CreateIndex("EXCEED.LocationTracking", "RegistrationID");
            CreateIndex("EXCEED.Sales", "RegistrationID");
            CreateIndex("EXCEED.Location", "RegistrationID");
            CreateIndex("EXCEED.FixedAssetRevaluation", "RegistrationID");
            CreateIndex("EXCEED.Disposal", "RegistrationID");
            CreateIndex("EXCEED.Depreciation", "RegistrationID");
            CreateIndex("EXCEED.Registration", "DepreciationRuleId");
            CreateIndex("EXCEED.Appreciation", "RegistrationId");
            AddForeignKey("EXCEED.LocationTracking", "RegistrationID", "EXCEED.Registration", "RegistrationID", cascadeDelete: true);
            AddForeignKey("EXCEED.Sales", "RegistrationID", "EXCEED.Registration", "RegistrationID", cascadeDelete: true);
            AddForeignKey("EXCEED.Location", "RegistrationID", "EXCEED.Registration", "RegistrationID", cascadeDelete: true);
            AddForeignKey("EXCEED.FixedAssetRevaluation", "RegistrationID", "EXCEED.Registration", "RegistrationID", cascadeDelete: true);
            AddForeignKey("EXCEED.Disposal", "RegistrationID", "EXCEED.Registration", "RegistrationID", cascadeDelete: true);
            AddForeignKey("EXCEED.Depreciation", "RegistrationID", "EXCEED.Registration", "RegistrationID", cascadeDelete: true);
            AddForeignKey("EXCEED.Registration", "DepreciationRuleId", "EXCEED.DepreciationRule", "DepreciationRuleId", cascadeDelete: true);
            AddForeignKey("EXCEED.Appreciation", "RegistrationId", "EXCEED.Registration", "RegistrationID", cascadeDelete: true);
        }
    }
}

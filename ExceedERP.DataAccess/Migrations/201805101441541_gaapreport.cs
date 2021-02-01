namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gaapreport : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.GapBalanceSheetRange", "GapBalanceSheetId", "EXCEED.GapBalanceSheet");
            DropIndex("EXCEED.GapBalanceSheetRange", new[] { "GapBalanceSheetId" });
            CreateTable(
                "EXCEED.GaapBalanceSheetAssetRange",
                c => new
                    {
                        GaapBalanceSheetAssetRangeId = c.Int(nullable: false, identity: true),
                        GapBalanceSheetId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                        Asset_GaapBalanceSheetAssetId = c.Int(),
                    })
                .PrimaryKey(t => t.GaapBalanceSheetAssetRangeId)
                .ForeignKey("EXCEED.GaapBalanceSheetAsset", t => t.Asset_GaapBalanceSheetAssetId)
                .Index(t => t.Asset_GaapBalanceSheetAssetId);
            
            CreateTable(
                "EXCEED.GaapBalanceSheetAsset",
                c => new
                    {
                        GaapBalanceSheetAssetId = c.Int(nullable: false, identity: true),
                        GaapBalanceSheetId = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        NoteNumber = c.String(),
                    })
                .PrimaryKey(t => t.GaapBalanceSheetAssetId)
                .ForeignKey("EXCEED.GaapBalanceSheet", t => t.GaapBalanceSheetId, cascadeDelete: true)
                .Index(t => t.GaapBalanceSheetId);
            
            CreateTable(
                "EXCEED.GaapBalanceSheet",
                c => new
                    {
                        GaapBalanceSheetId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.GaapBalanceSheetId);
            
            CreateTable(
                "EXCEED.GaapBalanceSheetCapital",
                c => new
                    {
                        GaapBalanceSheetCapitalId = c.Int(nullable: false, identity: true),
                        GaapBalanceSheetId = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        NoteNumber = c.String(),
                    })
                .PrimaryKey(t => t.GaapBalanceSheetCapitalId)
                .ForeignKey("EXCEED.GaapBalanceSheet", t => t.GaapBalanceSheetId, cascadeDelete: true)
                .Index(t => t.GaapBalanceSheetId);
            
            CreateTable(
                "EXCEED.GaapBalanceSheetCapitalRange",
                c => new
                    {
                        GaapBalanceSheetCapitalRangeId = c.Int(nullable: false, identity: true),
                        GapBalanceSheetId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                        Capital_GaapBalanceSheetCapitalId = c.Int(),
                    })
                .PrimaryKey(t => t.GaapBalanceSheetCapitalRangeId)
                .ForeignKey("EXCEED.GaapBalanceSheetCapital", t => t.Capital_GaapBalanceSheetCapitalId)
                .Index(t => t.Capital_GaapBalanceSheetCapitalId);
            
            CreateTable(
                "EXCEED.GaapBalanceSheetLiability",
                c => new
                    {
                        GaapBalanceSheetLiabilityId = c.Int(nullable: false, identity: true),
                        GaapBalanceSheetId = c.Int(nullable: false),
                        Category = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        NoteNumber = c.String(),
                    })
                .PrimaryKey(t => t.GaapBalanceSheetLiabilityId)
                .ForeignKey("EXCEED.GaapBalanceSheet", t => t.GaapBalanceSheetId, cascadeDelete: true)
                .Index(t => t.GaapBalanceSheetId);
            
            CreateTable(
                "EXCEED.GaapBalanceSheetLiabilityRange",
                c => new
                    {
                        GaapBalanceSheetLiabilityRangeId = c.Int(nullable: false, identity: true),
                        GapBalanceSheetId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                        Liability_GaapBalanceSheetLiabilityId = c.Int(),
                    })
                .PrimaryKey(t => t.GaapBalanceSheetLiabilityRangeId)
                .ForeignKey("EXCEED.GaapBalanceSheetLiability", t => t.Liability_GaapBalanceSheetLiabilityId)
                .Index(t => t.Liability_GaapBalanceSheetLiabilityId);
            
            DropTable("EXCEED.GapBalanceSheetRange");
            DropTable("EXCEED.GapBalanceSheet");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.GapBalanceSheet",
                c => new
                    {
                        GapBalanceSheetId = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        Parent = c.String(),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapBalanceSheetId);
            
            CreateTable(
                "EXCEED.GapBalanceSheetRange",
                c => new
                    {
                        GapBalanceSheetRangeId = c.Int(nullable: false, identity: true),
                        GapBalanceSheetId = c.Int(nullable: false),
                        AccountFrom = c.String(nullable: false),
                        AccountTo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.GapBalanceSheetRangeId);
            
            DropForeignKey("EXCEED.GaapBalanceSheetAssetRange", "Asset_GaapBalanceSheetAssetId", "EXCEED.GaapBalanceSheetAsset");
            DropForeignKey("EXCEED.GaapBalanceSheetLiabilityRange", "Liability_GaapBalanceSheetLiabilityId", "EXCEED.GaapBalanceSheetLiability");
            DropForeignKey("EXCEED.GaapBalanceSheetLiability", "GaapBalanceSheetId", "EXCEED.GaapBalanceSheet");
            DropForeignKey("EXCEED.GaapBalanceSheetCapitalRange", "Capital_GaapBalanceSheetCapitalId", "EXCEED.GaapBalanceSheetCapital");
            DropForeignKey("EXCEED.GaapBalanceSheetCapital", "GaapBalanceSheetId", "EXCEED.GaapBalanceSheet");
            DropForeignKey("EXCEED.GaapBalanceSheetAsset", "GaapBalanceSheetId", "EXCEED.GaapBalanceSheet");
            DropIndex("EXCEED.GaapBalanceSheetLiabilityRange", new[] { "Liability_GaapBalanceSheetLiabilityId" });
            DropIndex("EXCEED.GaapBalanceSheetLiability", new[] { "GaapBalanceSheetId" });
            DropIndex("EXCEED.GaapBalanceSheetCapitalRange", new[] { "Capital_GaapBalanceSheetCapitalId" });
            DropIndex("EXCEED.GaapBalanceSheetCapital", new[] { "GaapBalanceSheetId" });
            DropIndex("EXCEED.GaapBalanceSheetAsset", new[] { "GaapBalanceSheetId" });
            DropIndex("EXCEED.GaapBalanceSheetAssetRange", new[] { "Asset_GaapBalanceSheetAssetId" });
            DropTable("EXCEED.GaapBalanceSheetLiabilityRange");
            DropTable("EXCEED.GaapBalanceSheetLiability");
            DropTable("EXCEED.GaapBalanceSheetCapitalRange");
            DropTable("EXCEED.GaapBalanceSheetCapital");
            DropTable("EXCEED.GaapBalanceSheet");
            DropTable("EXCEED.GaapBalanceSheetAsset");
            DropTable("EXCEED.GaapBalanceSheetAssetRange");
            CreateIndex("EXCEED.GapBalanceSheetRange", "GapBalanceSheetId");
            AddForeignKey("EXCEED.GapBalanceSheetRange", "GapBalanceSheetId", "EXCEED.GapBalanceSheet", "GapBalanceSheetId", cascadeDelete: true);
        }
    }
}

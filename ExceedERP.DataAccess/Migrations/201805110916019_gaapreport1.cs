namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gaapreport1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.GaapBalanceSheetAssetRange", "Asset_GaapBalanceSheetAssetId", "EXCEED.GaapBalanceSheetAsset");
            DropForeignKey("EXCEED.GaapBalanceSheetCapitalRange", "Capital_GaapBalanceSheetCapitalId", "EXCEED.GaapBalanceSheetCapital");
            DropForeignKey("EXCEED.GaapBalanceSheetLiabilityRange", "Liability_GaapBalanceSheetLiabilityId", "EXCEED.GaapBalanceSheetLiability");
            DropIndex("EXCEED.GaapBalanceSheetAssetRange", new[] { "Asset_GaapBalanceSheetAssetId" });
            DropIndex("EXCEED.GaapBalanceSheetCapitalRange", new[] { "Capital_GaapBalanceSheetCapitalId" });
            DropIndex("EXCEED.GaapBalanceSheetLiabilityRange", new[] { "Liability_GaapBalanceSheetLiabilityId" });
            RenameColumn(table: "EXCEED.GaapBalanceSheetAssetRange", name: "Asset_GaapBalanceSheetAssetId", newName: "GaapBalanceSheetAssetId");
            RenameColumn(table: "EXCEED.GaapBalanceSheetCapitalRange", name: "Capital_GaapBalanceSheetCapitalId", newName: "GaapBalanceSheetCapitalId");
            RenameColumn(table: "EXCEED.GaapBalanceSheetLiabilityRange", name: "Liability_GaapBalanceSheetLiabilityId", newName: "GaapBalanceSheetLiabilityId");
            AlterColumn("EXCEED.GaapBalanceSheetAssetRange", "GaapBalanceSheetAssetId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.GaapBalanceSheetCapitalRange", "GaapBalanceSheetCapitalId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.GaapBalanceSheetLiabilityRange", "GaapBalanceSheetLiabilityId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.GaapBalanceSheetAssetRange", "GaapBalanceSheetAssetId");
            CreateIndex("EXCEED.GaapBalanceSheetCapitalRange", "GaapBalanceSheetCapitalId");
            CreateIndex("EXCEED.GaapBalanceSheetLiabilityRange", "GaapBalanceSheetLiabilityId");
            AddForeignKey("EXCEED.GaapBalanceSheetAssetRange", "GaapBalanceSheetAssetId", "EXCEED.GaapBalanceSheetAsset", "GaapBalanceSheetAssetId", cascadeDelete: true);
            AddForeignKey("EXCEED.GaapBalanceSheetCapitalRange", "GaapBalanceSheetCapitalId", "EXCEED.GaapBalanceSheetCapital", "GaapBalanceSheetCapitalId", cascadeDelete: true);
            AddForeignKey("EXCEED.GaapBalanceSheetLiabilityRange", "GaapBalanceSheetLiabilityId", "EXCEED.GaapBalanceSheetLiability", "GaapBalanceSheetLiabilityId", cascadeDelete: true);
            DropColumn("EXCEED.GaapBalanceSheetAssetRange", "GapBalanceSheetId");
            DropColumn("EXCEED.GaapBalanceSheetCapitalRange", "GapBalanceSheetId");
            DropColumn("EXCEED.GaapBalanceSheetLiabilityRange", "GapBalanceSheetId");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.GaapBalanceSheetLiabilityRange", "GapBalanceSheetId", c => c.Int(nullable: false));
            AddColumn("EXCEED.GaapBalanceSheetCapitalRange", "GapBalanceSheetId", c => c.Int(nullable: false));
            AddColumn("EXCEED.GaapBalanceSheetAssetRange", "GapBalanceSheetId", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.GaapBalanceSheetLiabilityRange", "GaapBalanceSheetLiabilityId", "EXCEED.GaapBalanceSheetLiability");
            DropForeignKey("EXCEED.GaapBalanceSheetCapitalRange", "GaapBalanceSheetCapitalId", "EXCEED.GaapBalanceSheetCapital");
            DropForeignKey("EXCEED.GaapBalanceSheetAssetRange", "GaapBalanceSheetAssetId", "EXCEED.GaapBalanceSheetAsset");
            DropIndex("EXCEED.GaapBalanceSheetLiabilityRange", new[] { "GaapBalanceSheetLiabilityId" });
            DropIndex("EXCEED.GaapBalanceSheetCapitalRange", new[] { "GaapBalanceSheetCapitalId" });
            DropIndex("EXCEED.GaapBalanceSheetAssetRange", new[] { "GaapBalanceSheetAssetId" });
            AlterColumn("EXCEED.GaapBalanceSheetLiabilityRange", "GaapBalanceSheetLiabilityId", c => c.Int());
            AlterColumn("EXCEED.GaapBalanceSheetCapitalRange", "GaapBalanceSheetCapitalId", c => c.Int());
            AlterColumn("EXCEED.GaapBalanceSheetAssetRange", "GaapBalanceSheetAssetId", c => c.Int());
            RenameColumn(table: "EXCEED.GaapBalanceSheetLiabilityRange", name: "GaapBalanceSheetLiabilityId", newName: "Liability_GaapBalanceSheetLiabilityId");
            RenameColumn(table: "EXCEED.GaapBalanceSheetCapitalRange", name: "GaapBalanceSheetCapitalId", newName: "Capital_GaapBalanceSheetCapitalId");
            RenameColumn(table: "EXCEED.GaapBalanceSheetAssetRange", name: "GaapBalanceSheetAssetId", newName: "Asset_GaapBalanceSheetAssetId");
            CreateIndex("EXCEED.GaapBalanceSheetLiabilityRange", "Liability_GaapBalanceSheetLiabilityId");
            CreateIndex("EXCEED.GaapBalanceSheetCapitalRange", "Capital_GaapBalanceSheetCapitalId");
            CreateIndex("EXCEED.GaapBalanceSheetAssetRange", "Asset_GaapBalanceSheetAssetId");
            AddForeignKey("EXCEED.GaapBalanceSheetLiabilityRange", "Liability_GaapBalanceSheetLiabilityId", "EXCEED.GaapBalanceSheetLiability", "GaapBalanceSheetLiabilityId");
            AddForeignKey("EXCEED.GaapBalanceSheetCapitalRange", "Capital_GaapBalanceSheetCapitalId", "EXCEED.GaapBalanceSheetCapital", "GaapBalanceSheetCapitalId");
            AddForeignKey("EXCEED.GaapBalanceSheetAssetRange", "Asset_GaapBalanceSheetAssetId", "EXCEED.GaapBalanceSheetAsset", "GaapBalanceSheetAssetId");
        }
    }
}

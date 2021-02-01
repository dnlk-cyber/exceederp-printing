namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction44 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Construction.MonthlyProjectCost", "FuelUtilizationBasedOnStandard", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCost", "CostsFromExpectation", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCost", "ProperAndAccurateEarnedValueAnalysis", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCost", "ProductivitiyOfLaborAndEquipmentMeaure", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCost", "ReusingOfConstructionMaterials", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Construction.MonthlyProjectCost", "IsFuelUtilizedBasedOnStandard");
            DropColumn("Construction.MonthlyProjectCost", "IsProperAndAccurateEarnedValueAnalysisUsed");
            DropColumn("Construction.MonthlyProjectCost", "IsProductivitiyOfLaborAndEquipmentRecorded");
            DropColumn("Construction.MonthlyProjectCost", "IsReusingOfConstructionMaterialsApplied");
        }
        
        public override void Down()
        {
            AddColumn("Construction.MonthlyProjectCost", "IsReusingOfConstructionMaterialsApplied", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectCost", "IsProductivitiyOfLaborAndEquipmentRecorded", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectCost", "IsProperAndAccurateEarnedValueAnalysisUsed", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectCost", "IsFuelUtilizedBasedOnStandard", c => c.Boolean(nullable: false));
            DropColumn("Construction.MonthlyProjectCost", "ReusingOfConstructionMaterials");
            DropColumn("Construction.MonthlyProjectCost", "ProductivitiyOfLaborAndEquipmentMeaure");
            DropColumn("Construction.MonthlyProjectCost", "ProperAndAccurateEarnedValueAnalysis");
            DropColumn("Construction.MonthlyProjectCost", "CostsFromExpectation");
            DropColumn("Construction.MonthlyProjectCost", "FuelUtilizationBasedOnStandard");
        }
    }
}

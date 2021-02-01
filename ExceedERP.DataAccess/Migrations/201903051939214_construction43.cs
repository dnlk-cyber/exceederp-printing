namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction43 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Construction.GenericWorkItemPerProject", "ProductivityOfCrewPerDay", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.GenericWorkItemPerProject", "DurationPerCrew", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.GenericWorkItemPerProject", "NumberOfCrew", c => c.Int());
            AddColumn("Construction.GenericWorkItemPerProject", "EstimatedDuration", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCommunication", "CommunicationAndSupportBetweenProjects", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCommunication", "ResponseFromHeadOffice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCost", "EquipmentUtilizationEfficiency", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCost", "IsFuelUtilizedBasedOnStandard", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectCost", "IsProperAndAccurateEarnedValueAnalysisUsed", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectCost", "IsProductivitiyOfLaborAndEquipmentRecorded", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectCost", "IsReusingOfConstructionMaterialsApplied", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "ConductQualityAssurance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "PRPImplementationLevel", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "ReworkHappened", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "ConsultantComplaint", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "DefectOccured", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectProgress", "ActionPlanIsPreparedWithAmountAndProblemsAreAddressed", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectProgress", "WellDefinedTeamStructureExists", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Construction.MonthlyProjectProgress", "WellDefinedTeamStructureExists");
            DropColumn("Construction.MonthlyProjectProgress", "ActionPlanIsPreparedWithAmountAndProblemsAreAddressed");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "DefectOccured");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "ConsultantComplaint");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "ReworkHappened");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "PRPImplementationLevel");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "ConductQualityAssurance");
            DropColumn("Construction.MonthlyProjectCost", "IsReusingOfConstructionMaterialsApplied");
            DropColumn("Construction.MonthlyProjectCost", "IsProductivitiyOfLaborAndEquipmentRecorded");
            DropColumn("Construction.MonthlyProjectCost", "IsProperAndAccurateEarnedValueAnalysisUsed");
            DropColumn("Construction.MonthlyProjectCost", "IsFuelUtilizedBasedOnStandard");
            DropColumn("Construction.MonthlyProjectCost", "EquipmentUtilizationEfficiency");
            DropColumn("Construction.MonthlyProjectCommunication", "ResponseFromHeadOffice");
            DropColumn("Construction.MonthlyProjectCommunication", "CommunicationAndSupportBetweenProjects");
            DropColumn("Construction.GenericWorkItemPerProject", "EstimatedDuration");
            DropColumn("Construction.GenericWorkItemPerProject", "NumberOfCrew");
            DropColumn("Construction.GenericWorkItemPerProject", "DurationPerCrew");
            DropColumn("Construction.GenericWorkItemPerProject", "ProductivityOfCrewPerDay");
        }
    }
}

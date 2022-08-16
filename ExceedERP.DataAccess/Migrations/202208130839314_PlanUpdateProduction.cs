namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanUpdateProduction : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("ProductFollowUp.JobStatusLineItems", "JobStatusId", "ProductFollowUp.JobStatusEntityTypeConfiguration");
            DropForeignKey("ProductFollowUp.QuarterlyProductionPlans", "HalfYearlyProductionPlan_HalfYearlyProductionPlanId", "ProductFollowUp.HalfYearlyProductionPlans");
            DropForeignKey("ProductFollowUp.HalfYearlyProductionPlans", "YearlyPlan_YearlyProductionPlanId", "ProductFollowUp.YearlyProductionPlans");
            DropForeignKey("ProductFollowUp.MonthlyProductionPlans", "QuarterlyProductionPlan_QuarterlyProductionPlanId", "ProductFollowUp.QuarterlyProductionPlans");
            DropForeignKey("ProductFollowUp.WeeklyProductionPlans", "MonthlyPlan_MonthlyProductionPlanId", "ProductFollowUp.MonthlyProductionPlans");
            DropIndex("ProductFollowUp.HalfYearlyProductionPlans", new[] { "YearlyPlan_YearlyProductionPlanId" });
            DropIndex("ProductFollowUp.QuarterlyProductionPlans", new[] { "HalfYearlyProductionPlan_HalfYearlyProductionPlanId" });
            DropIndex("ProductFollowUp.MonthlyProductionPlans", new[] { "QuarterlyProductionPlan_QuarterlyProductionPlanId" });
            DropIndex("ProductFollowUp.WeeklyProductionPlans", new[] { "MonthlyPlan_MonthlyProductionPlanId" });
            DropIndex("ProductFollowUp.JobStatusLineItems", new[] { "JobStatusId" });
            RenameColumn(table: "ProductFollowUp.QuarterlyProductionPlans", name: "HalfYearlyProductionPlan_HalfYearlyProductionPlanId", newName: "HalfYearlyProductionPlanId");
            RenameColumn(table: "ProductFollowUp.HalfYearlyProductionPlans", name: "YearlyPlan_YearlyProductionPlanId", newName: "YearlyProductionPlanId");
            RenameColumn(table: "ProductFollowUp.MonthlyProductionPlans", name: "QuarterlyProductionPlan_QuarterlyProductionPlanId", newName: "QuarterlyProductionPlanId");
            RenameColumn(table: "ProductFollowUp.WeeklyProductionPlans", name: "MonthlyPlan_MonthlyProductionPlanId", newName: "MonthlyProductionPlanId");
            AlterColumn("ProductFollowUp.HalfYearlyProductionPlans", "YearlyProductionPlanId", c => c.Int(nullable: false));
            AlterColumn("ProductFollowUp.QuarterlyProductionPlans", "HalfYearlyProductionPlanId", c => c.Int(nullable: false));
            AlterColumn("ProductFollowUp.MonthlyProductionPlans", "QuarterlyProductionPlanId", c => c.Int(nullable: false));
            AlterColumn("ProductFollowUp.WeeklyProductionPlans", "MonthlyProductionPlanId", c => c.Int(nullable: false));
            CreateIndex("ProductFollowUp.HalfYearlyProductionPlans", "YearlyProductionPlanId");
            CreateIndex("ProductFollowUp.QuarterlyProductionPlans", "HalfYearlyProductionPlanId");
            CreateIndex("ProductFollowUp.MonthlyProductionPlans", "QuarterlyProductionPlanId");
            CreateIndex("ProductFollowUp.WeeklyProductionPlans", "MonthlyProductionPlanId");
            AddForeignKey("ProductFollowUp.QuarterlyProductionPlans", "HalfYearlyProductionPlanId", "ProductFollowUp.HalfYearlyProductionPlans", "HalfYearlyProductionPlanId", cascadeDelete: true);
            AddForeignKey("ProductFollowUp.HalfYearlyProductionPlans", "YearlyProductionPlanId", "ProductFollowUp.YearlyProductionPlans", "YearlyProductionPlanId", cascadeDelete: true);
            AddForeignKey("ProductFollowUp.MonthlyProductionPlans", "QuarterlyProductionPlanId", "ProductFollowUp.QuarterlyProductionPlans", "QuarterlyProductionPlanId", cascadeDelete: true);
            AddForeignKey("ProductFollowUp.WeeklyProductionPlans", "MonthlyProductionPlanId", "ProductFollowUp.MonthlyProductionPlans", "MonthlyProductionPlanId", cascadeDelete: true);
            DropColumn("ProductFollowUp.HalfYearlyProductionPlans", "YearlyPlanId");
            DropColumn("ProductFollowUp.QuarterlyProductionPlans", "HalfYearlyPlanId");
            DropColumn("ProductFollowUp.MonthlyProductionPlans", "QuarterlyPlanId");
            DropColumn("ProductFollowUp.WeeklyProductionPlans", "MonthlyPlanId");
        }
        
        public override void Down()
        {
            AddColumn("ProductFollowUp.WeeklyProductionPlans", "MonthlyPlanId", c => c.Int(nullable: false));
            AddColumn("ProductFollowUp.MonthlyProductionPlans", "QuarterlyPlanId", c => c.Int(nullable: false));
            AddColumn("ProductFollowUp.QuarterlyProductionPlans", "HalfYearlyPlanId", c => c.Int(nullable: false));
            AddColumn("ProductFollowUp.HalfYearlyProductionPlans", "YearlyPlanId", c => c.Int(nullable: false));
            DropForeignKey("ProductFollowUp.WeeklyProductionPlans", "MonthlyProductionPlanId", "ProductFollowUp.MonthlyProductionPlans");
            DropForeignKey("ProductFollowUp.MonthlyProductionPlans", "QuarterlyProductionPlanId", "ProductFollowUp.QuarterlyProductionPlans");
            DropForeignKey("ProductFollowUp.HalfYearlyProductionPlans", "YearlyProductionPlanId", "ProductFollowUp.YearlyProductionPlans");
            DropForeignKey("ProductFollowUp.QuarterlyProductionPlans", "HalfYearlyProductionPlanId", "ProductFollowUp.HalfYearlyProductionPlans");
            DropIndex("ProductFollowUp.WeeklyProductionPlans", new[] { "MonthlyProductionPlanId" });
            DropIndex("ProductFollowUp.MonthlyProductionPlans", new[] { "QuarterlyProductionPlanId" });
            DropIndex("ProductFollowUp.QuarterlyProductionPlans", new[] { "HalfYearlyProductionPlanId" });
            DropIndex("ProductFollowUp.HalfYearlyProductionPlans", new[] { "YearlyProductionPlanId" });
            AlterColumn("ProductFollowUp.WeeklyProductionPlans", "MonthlyProductionPlanId", c => c.Int());
            AlterColumn("ProductFollowUp.MonthlyProductionPlans", "QuarterlyProductionPlanId", c => c.Int());
            AlterColumn("ProductFollowUp.QuarterlyProductionPlans", "HalfYearlyProductionPlanId", c => c.Int());
            AlterColumn("ProductFollowUp.HalfYearlyProductionPlans", "YearlyProductionPlanId", c => c.Int());
            RenameColumn(table: "ProductFollowUp.WeeklyProductionPlans", name: "MonthlyProductionPlanId", newName: "MonthlyPlan_MonthlyProductionPlanId");
            RenameColumn(table: "ProductFollowUp.MonthlyProductionPlans", name: "QuarterlyProductionPlanId", newName: "QuarterlyProductionPlan_QuarterlyProductionPlanId");
            RenameColumn(table: "ProductFollowUp.HalfYearlyProductionPlans", name: "YearlyProductionPlanId", newName: "YearlyPlan_YearlyProductionPlanId");
            RenameColumn(table: "ProductFollowUp.QuarterlyProductionPlans", name: "HalfYearlyProductionPlanId", newName: "HalfYearlyProductionPlan_HalfYearlyProductionPlanId");
            CreateIndex("ProductFollowUp.JobStatusLineItems", "JobStatusId");
            CreateIndex("ProductFollowUp.WeeklyProductionPlans", "MonthlyPlan_MonthlyProductionPlanId");
            CreateIndex("ProductFollowUp.MonthlyProductionPlans", "QuarterlyProductionPlan_QuarterlyProductionPlanId");
            CreateIndex("ProductFollowUp.QuarterlyProductionPlans", "HalfYearlyProductionPlan_HalfYearlyProductionPlanId");
            CreateIndex("ProductFollowUp.HalfYearlyProductionPlans", "YearlyPlan_YearlyProductionPlanId");
            AddForeignKey("ProductFollowUp.WeeklyProductionPlans", "MonthlyPlan_MonthlyProductionPlanId", "ProductFollowUp.MonthlyProductionPlans", "MonthlyProductionPlanId");
            AddForeignKey("ProductFollowUp.MonthlyProductionPlans", "QuarterlyProductionPlan_QuarterlyProductionPlanId", "ProductFollowUp.QuarterlyProductionPlans", "QuarterlyProductionPlanId");
            AddForeignKey("ProductFollowUp.HalfYearlyProductionPlans", "YearlyPlan_YearlyProductionPlanId", "ProductFollowUp.YearlyProductionPlans", "YearlyProductionPlanId");
            AddForeignKey("ProductFollowUp.QuarterlyProductionPlans", "HalfYearlyProductionPlan_HalfYearlyProductionPlanId", "ProductFollowUp.HalfYearlyProductionPlans", "HalfYearlyProductionPlanId");
            AddForeignKey("ProductFollowUp.JobStatusLineItems", "JobStatusId", "ProductFollowUp.JobStatusEntityTypeConfiguration", "JobStatusId", cascadeDelete: true);
        }
    }
}

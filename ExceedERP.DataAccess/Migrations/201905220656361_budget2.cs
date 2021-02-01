namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class budget2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Budget.BudgetPlan", "BudgetCategoryID", "Budget.PlanCategory");
            DropIndex("Budget.BudgetPlan", new[] { "BudgetCategoryID" });
            AddColumn("Budget.PlanCategory", "CurrentTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Budget.PlanCategory", "Progress", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Budget.PlanCategory", "ActualConsumption", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Budget.PlanCategory", "BudgetStatus", c => c.String());
            AddColumn("Budget.PlanCategory", "BudgetType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Budget.PlanCategory", "BudgetType");
            DropColumn("Budget.PlanCategory", "BudgetStatus");
            DropColumn("Budget.PlanCategory", "ActualConsumption");
            DropColumn("Budget.PlanCategory", "Progress");
            DropColumn("Budget.PlanCategory", "CurrentTotal");
            CreateIndex("Budget.BudgetPlan", "BudgetCategoryID");
            AddForeignKey("Budget.BudgetPlan", "BudgetCategoryID", "Budget.PlanCategory", "PlanCategoryID", cascadeDelete: true);
        }
    }
}

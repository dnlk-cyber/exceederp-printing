namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class budget3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Budget.TaskPlan", "GLChartOfAccountCostCenter_Values", "EXCEED.GLChartOfAccountCostCenter");
            DropForeignKey("Budget.TaskPerformance", "GLChartOfAccountCostCenter_Values", "EXCEED.GLChartOfAccountCostCenter");
            DropIndex("Budget.TaskPlan", new[] { "GLChartOfAccountCostCenter_Values" });
            DropIndex("Budget.TaskPerformance", new[] { "GLChartOfAccountCostCenter_Values" });
            AlterColumn("Budget.TaskPlan", "GLChartOfAccountCostCenterId", c => c.String());
            AlterColumn("Budget.TaskPerformance", "GLChartOfAccountCostCenterId", c => c.String(nullable: false));
            DropColumn("Budget.TaskPlan", "GLChartOfAccountCostCenter_Values");
            DropColumn("Budget.TaskPerformance", "GLChartOfAccountCostCenter_Values");
        }
        
        public override void Down()
        {
            AddColumn("Budget.TaskPerformance", "GLChartOfAccountCostCenter_Values", c => c.String(maxLength: 128));
            AddColumn("Budget.TaskPlan", "GLChartOfAccountCostCenter_Values", c => c.String(maxLength: 128));
            AlterColumn("Budget.TaskPerformance", "GLChartOfAccountCostCenterId", c => c.Int(nullable: false));
            AlterColumn("Budget.TaskPlan", "GLChartOfAccountCostCenterId", c => c.Int(nullable: false));
            CreateIndex("Budget.TaskPerformance", "GLChartOfAccountCostCenter_Values");
            CreateIndex("Budget.TaskPlan", "GLChartOfAccountCostCenter_Values");
            AddForeignKey("Budget.TaskPerformance", "GLChartOfAccountCostCenter_Values", "EXCEED.GLChartOfAccountCostCenter", "Values");
            AddForeignKey("Budget.TaskPlan", "GLChartOfAccountCostCenter_Values", "EXCEED.GLChartOfAccountCostCenter", "Values");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.AnnualNeedAssesment", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("EXCEED.InventoryPlan", "GLPeriodId", "EXCEED.GLPeriod");
            DropIndex("EXCEED.AnnualNeedAssesment", new[] { "GLPeriodId" });
            DropIndex("EXCEED.InventoryPlan", new[] { "GLPeriodId" });
            AddColumn("EXCEED.AnnualNeedAssesment", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.AnnualNeedAssesment", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.AnnualNeedAssesment", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("EXCEED.InventoryPlan", "BranchId", c => c.Int(nullable: false));
            DropColumn("EXCEED.AnnualNeedAssesment", "Branch");
            DropColumn("EXCEED.AnnualNeedAssesment", "RequestingDepartment");
            DropColumn("EXCEED.AnnualNeedAssesment", "RequestingName");
            DropColumn("EXCEED.AnnualNeedAssesment", "PeriodName");
            DropColumn("EXCEED.InventoryPlan", "PeriodName");
            DropColumn("EXCEED.InventoryPlan", "Branch");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.InventoryPlan", "Branch", c => c.String());
            AddColumn("EXCEED.InventoryPlan", "PeriodName", c => c.String());
            AddColumn("EXCEED.AnnualNeedAssesment", "PeriodName", c => c.String());
            AddColumn("EXCEED.AnnualNeedAssesment", "RequestingName", c => c.String());
            AddColumn("EXCEED.AnnualNeedAssesment", "RequestingDepartment", c => c.String());
            AddColumn("EXCEED.AnnualNeedAssesment", "Branch", c => c.String());
            DropColumn("EXCEED.InventoryPlan", "BranchId");
            DropColumn("EXCEED.AnnualNeedAssesment", "EmployeeId");
            DropColumn("EXCEED.AnnualNeedAssesment", "OrgStructureId");
            DropColumn("EXCEED.AnnualNeedAssesment", "BranchId");
            CreateIndex("EXCEED.InventoryPlan", "GLPeriodId");
            CreateIndex("EXCEED.AnnualNeedAssesment", "GLPeriodId");
            AddForeignKey("EXCEED.InventoryPlan", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("EXCEED.AnnualNeedAssesment", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
        }
    }
}

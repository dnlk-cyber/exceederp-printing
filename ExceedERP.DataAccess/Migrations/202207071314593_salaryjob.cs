namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salaryjob : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.SalaryJobTitle", "LookupId", "Common.Lookup");
            DropForeignKey("HRM.SalaryJobTitle", "RankId", "HRM.Rank");
            DropForeignKey("HRM.SalaryJobTitle", "StepId", "HRM.Step");
            DropIndex("HRM.SalaryJobTitle", new[] { "RankId" });
            DropIndex("HRM.SalaryJobTitle", new[] { "StepId" });
            DropIndex("HRM.SalaryJobTitle", new[] { "LookupId" });
            AlterColumn("HRM.SalaryJobTitle", "LookupId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("HRM.SalaryJobTitle", "LookupId", c => c.Int(nullable: false));
            CreateIndex("HRM.SalaryJobTitle", "LookupId");
            CreateIndex("HRM.SalaryJobTitle", "StepId");
            CreateIndex("HRM.SalaryJobTitle", "RankId");
            AddForeignKey("HRM.SalaryJobTitle", "StepId", "HRM.Step", "StepId", cascadeDelete: true);
            AddForeignKey("HRM.SalaryJobTitle", "RankId", "HRM.Rank", "RankId", cascadeDelete: true);
            AddForeignKey("HRM.SalaryJobTitle", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
        }
    }
}

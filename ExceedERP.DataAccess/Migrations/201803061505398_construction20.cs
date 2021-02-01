namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction20 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.ProjectMaterialExpense", "GLPeriodId", "EXCEED.GLPeriod");
            DropIndex("Construction.ProjectMaterialExpense", new[] { "GLPeriodId" });
        }
        
        public override void Down()
        {
            CreateIndex("Construction.ProjectMaterialExpense", "GLPeriodId");
            AddForeignKey("Construction.ProjectMaterialExpense", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
        }
    }
}

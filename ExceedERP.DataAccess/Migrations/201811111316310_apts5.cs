namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apts5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.HealthRankAndValue", "GLPeriodId", "EXCEED.GLPeriod");
            DropIndex("EXCEED.HealthRankAndValue", new[] { "GLPeriodId" });
            AddColumn("APTS.DrugStores", "Name", c => c.String());
            AddColumn("APTS.DrugStores", "NameWithCode", c => c.String());
            AlterColumn("EXCEED.EmployeeAccident", "AccidentDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.EmployeeAccident", "AccidentDate", c => c.DateTime());
            DropColumn("APTS.DrugStores", "NameWithCode");
            DropColumn("APTS.DrugStores", "Name");
            CreateIndex("EXCEED.HealthRankAndValue", "GLPeriodId");
            AddForeignKey("EXCEED.HealthRankAndValue", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
        }
    }
}

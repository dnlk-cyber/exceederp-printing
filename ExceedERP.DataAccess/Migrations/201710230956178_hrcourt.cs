namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrcourt : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.DisciplineMeasuresDetail", "MeasuresId", "HRM.DisciplineMeasures");
            DropIndex("HRM.DisciplineMeasuresDetail", new[] { "MeasuresId" });
            AddColumn("HRM.Leave", "Hours", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.Leave", "Hours");
            CreateIndex("HRM.DisciplineMeasuresDetail", "MeasuresId");
            AddForeignKey("HRM.DisciplineMeasuresDetail", "MeasuresId", "HRM.DisciplineMeasures", "MeasuresId", cascadeDelete: true);
        }
    }
}

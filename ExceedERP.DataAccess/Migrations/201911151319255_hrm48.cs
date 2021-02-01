namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm48 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.EmpDisciplineMeasure", "DetailMeasuresId", "HRM.DisciplineMeasuresDetail");
            DropIndex("HRM.EmpDisciplineMeasure", new[] { "DetailMeasuresId" });
            AddColumn("HRM.EmpDisciplineMeasure", "Month", c => c.Int(nullable: false));
            AlterColumn("HRM.EmpDisciplineMeasure", "DetailMeasuresId", c => c.Int());
            CreateIndex("HRM.EmpDisciplineMeasure", "DetailMeasuresId");
            AddForeignKey("HRM.EmpDisciplineMeasure", "DetailMeasuresId", "HRM.DisciplineMeasuresDetail", "DetailMeasuresId");
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.EmpDisciplineMeasure", "DetailMeasuresId", "HRM.DisciplineMeasuresDetail");
            DropIndex("HRM.EmpDisciplineMeasure", new[] { "DetailMeasuresId" });
            AlterColumn("HRM.EmpDisciplineMeasure", "DetailMeasuresId", c => c.Int(nullable: false));
            DropColumn("HRM.EmpDisciplineMeasure", "Month");
            CreateIndex("HRM.EmpDisciplineMeasure", "DetailMeasuresId");
            AddForeignKey("HRM.EmpDisciplineMeasure", "DetailMeasuresId", "HRM.DisciplineMeasuresDetail", "DetailMeasuresId", cascadeDelete: true);
        }
    }
}

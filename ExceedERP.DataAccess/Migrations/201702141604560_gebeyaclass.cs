namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gebeyaclass : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "EXCEED.OverTimeJobResult", newSchema: "Payroll");
            DropForeignKey("HRM.ExprianceRequest", "EmployeeId", "HRM.Employee");
            DropIndex("HRM.ExprianceRequest", new[] { "EmployeeId" });
            CreateTable(
                "HRM.ExperienceRequest",
                c => new
                    {
                        ExperienceRequestId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        ApplicationDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ExperienceRequestId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "Payroll.PerformanceRank",
                c => new
                    {
                        PerformanceRankId = c.Int(nullable: false, identity: true),
                        Minimum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Maximum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(maxLength: 100),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PerformanceRankId);
            
            DropTable("HRM.ExprianceRequest");
        }
        
        public override void Down()
        {
            CreateTable(
                "HRM.ExprianceRequest",
                c => new
                    {
                        ExprianceRequestId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        ApplicationDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ExprianceRequestId);
            
            DropForeignKey("HRM.ExperienceRequest", "EmployeeId", "HRM.Employee");
            DropIndex("HRM.ExperienceRequest", new[] { "EmployeeId" });
            DropTable("Payroll.PerformanceRank");
            DropTable("HRM.ExperienceRequest");
            CreateIndex("HRM.ExprianceRequest", "EmployeeId");
            AddForeignKey("HRM.ExprianceRequest", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
            MoveTable(name: "Payroll.OverTimeJobResult", newSchema: "EXCEED");
        }
    }
}

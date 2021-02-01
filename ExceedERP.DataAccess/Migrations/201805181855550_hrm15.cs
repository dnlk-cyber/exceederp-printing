namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm15 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Performance.EmployeeMeasurement", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Performance.EmployeeMeasurementDefinition", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Performance.EmployeePerformanceAction", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Health.PatientCardDetail", "PatientCard_PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.PatientsLeave", "PatientCard_PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.Prescription", "PatientCard_PatientCardId", "Health.PatientCard");
            DropForeignKey("Health.ReferralSheet", "PatientCard_PatientCardId", "Health.PatientCard");
            DropIndex("Performance.EmployeeMeasurement", new[] { "GLPeriodId" });
            DropIndex("Performance.EmployeeMeasurementDefinition", new[] { "GLPeriodId" });
            DropIndex("Performance.EmployeePerformanceAction", new[] { "GLPeriodId" });
            DropIndex("Health.PatientCardDetail", new[] { "PatientCard_PatientCardId" });
            DropIndex("Health.PatientsLeave", new[] { "PatientCard_PatientCardId" });
            DropIndex("Health.Prescription", new[] { "PatientCard_PatientCardId" });
            DropIndex("Health.ReferralSheet", new[] { "PatientCard_PatientCardId" });
            DropColumn("Health.PatientCardDetail", "PatientId");
            DropColumn("Health.PatientsLeave", "PatientId");
            DropColumn("Health.Prescription", "PatientId");
            DropColumn("Health.ReferralSheet", "PatientId");
            RenameColumn(table: "Health.PatientCardDetail", name: "PatientCard_PatientCardId", newName: "PatientId");
            RenameColumn(table: "Health.PatientsLeave", name: "PatientCard_PatientCardId", newName: "PatientId");
            RenameColumn(table: "Health.Prescription", name: "PatientCard_PatientCardId", newName: "PatientId");
            RenameColumn(table: "Health.ReferralSheet", name: "PatientCard_PatientCardId", newName: "PatientId");
            DropPrimaryKey("Health.PatientCard");
            CreateTable(
                "HRM.Quarter",
                c => new
                    {
                        QuarterId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        AmharicName = c.String(nullable: false),
                        Sequence = c.Int(nullable: false),
                        Remark = c.String(),
                        QuarterLengthId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QuarterId)
                .ForeignKey("HRM.QuarterLength", t => t.QuarterLengthId, cascadeDelete: true)
                .Index(t => t.QuarterLengthId);
            
            CreateTable(
                "HRM.QuarterLength",
                c => new
                    {
                        QuarterLengthId = c.Int(nullable: false, identity: true),
                        Length = c.Int(nullable: false),
                        Remark = c.String(),
                        GlFiscalYearId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QuarterLengthId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId);
            
            AddColumn("Performance.EmployeeMeasurement", "QuarterId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeeMeasurementDefinition", "QuarterId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeeMeasurementDefinition", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformanceAction", "QuarterId", c => c.Int(nullable: false));
            AlterColumn("Health.PatientCard", "PatientCardId", c => c.String());
            AlterColumn("Health.PatientCard", "PatientId", c => c.Int(nullable: false, identity: true));
            AlterColumn("Health.PatientCardDetail", "PatientId", c => c.Int(nullable: false));
            AlterColumn("Health.PatientsLeave", "PatientId", c => c.Int(nullable: false));
            AlterColumn("Health.Prescription", "PatientId", c => c.Int(nullable: false));
            AlterColumn("Health.ReferralSheet", "PatientId", c => c.Int(nullable: false));
            AddPrimaryKey("Health.PatientCard", "PatientId");
            CreateIndex("Performance.EmployeeMeasurement", "QuarterId");
            CreateIndex("Performance.EmployeeMeasurementDefinition", "QuarterId");
            CreateIndex("Performance.EmployeePerformanceAction", "QuarterId");
            CreateIndex("Health.PatientCardDetail", "PatientId");
            CreateIndex("Health.PatientsLeave", "PatientId");
            CreateIndex("Health.Prescription", "PatientId");
            CreateIndex("Health.ReferralSheet", "PatientId");
            AddForeignKey("Performance.EmployeeMeasurement", "QuarterId", "HRM.Quarter", "QuarterId", cascadeDelete: true);
            AddForeignKey("Performance.EmployeeMeasurementDefinition", "QuarterId", "HRM.Quarter", "QuarterId", cascadeDelete: true);
            AddForeignKey("Performance.EmployeePerformanceAction", "QuarterId", "HRM.Quarter", "QuarterId", cascadeDelete: true);
            AddForeignKey("Health.PatientCardDetail", "PatientId", "Health.PatientCard", "PatientId", cascadeDelete: true);
            AddForeignKey("Health.PatientsLeave", "PatientId", "Health.PatientCard", "PatientId", cascadeDelete: true);
            AddForeignKey("Health.Prescription", "PatientId", "Health.PatientCard", "PatientId", cascadeDelete: true);
            AddForeignKey("Health.ReferralSheet", "PatientId", "Health.PatientCard", "PatientId", cascadeDelete: true);
            DropColumn("Performance.EmployeeMeasurement", "GLPeriodId");
            DropColumn("Performance.EmployeeMeasurementDefinition", "GLPeriodId");
            DropColumn("Performance.EmployeePerformanceAction", "GLPeriodId");
        }
        
        public override void Down()
        {
            AddColumn("Performance.EmployeePerformanceAction", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeeMeasurementDefinition", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeeMeasurement", "GLPeriodId", c => c.Int(nullable: false));
            DropForeignKey("Health.ReferralSheet", "PatientId", "Health.PatientCard");
            DropForeignKey("Health.Prescription", "PatientId", "Health.PatientCard");
            DropForeignKey("Health.PatientsLeave", "PatientId", "Health.PatientCard");
            DropForeignKey("Health.PatientCardDetail", "PatientId", "Health.PatientCard");
            DropForeignKey("Performance.EmployeePerformanceAction", "QuarterId", "HRM.Quarter");
            DropForeignKey("Performance.EmployeeMeasurementDefinition", "QuarterId", "HRM.Quarter");
            DropForeignKey("Performance.EmployeeMeasurement", "QuarterId", "HRM.Quarter");
            DropForeignKey("HRM.Quarter", "QuarterLengthId", "HRM.QuarterLength");
            DropForeignKey("HRM.QuarterLength", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropIndex("Health.ReferralSheet", new[] { "PatientId" });
            DropIndex("Health.Prescription", new[] { "PatientId" });
            DropIndex("Health.PatientsLeave", new[] { "PatientId" });
            DropIndex("Health.PatientCardDetail", new[] { "PatientId" });
            DropIndex("Performance.EmployeePerformanceAction", new[] { "QuarterId" });
            DropIndex("Performance.EmployeeMeasurementDefinition", new[] { "QuarterId" });
            DropIndex("HRM.QuarterLength", new[] { "GlFiscalYearId" });
            DropIndex("HRM.Quarter", new[] { "QuarterLengthId" });
            DropIndex("Performance.EmployeeMeasurement", new[] { "QuarterId" });
            DropPrimaryKey("Health.PatientCard");
            AlterColumn("Health.ReferralSheet", "PatientId", c => c.String(maxLength: 128));
            AlterColumn("Health.Prescription", "PatientId", c => c.String(maxLength: 128));
            AlterColumn("Health.PatientsLeave", "PatientId", c => c.String(maxLength: 128));
            AlterColumn("Health.PatientCardDetail", "PatientId", c => c.String(maxLength: 128));
            AlterColumn("Health.PatientCard", "PatientId", c => c.Int(nullable: false));
            AlterColumn("Health.PatientCard", "PatientCardId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("Performance.EmployeePerformanceAction", "QuarterId");
            DropColumn("Performance.EmployeeMeasurementDefinition", "IsApproved");
            DropColumn("Performance.EmployeeMeasurementDefinition", "QuarterId");
            DropColumn("Performance.EmployeeMeasurement", "QuarterId");
            DropTable("HRM.QuarterLength");
            DropTable("HRM.Quarter");
            AddPrimaryKey("Health.PatientCard", "PatientCardId");
            RenameColumn(table: "Health.ReferralSheet", name: "PatientId", newName: "PatientCard_PatientCardId");
            RenameColumn(table: "Health.Prescription", name: "PatientId", newName: "PatientCard_PatientCardId");
            RenameColumn(table: "Health.PatientsLeave", name: "PatientId", newName: "PatientCard_PatientCardId");
            RenameColumn(table: "Health.PatientCardDetail", name: "PatientId", newName: "PatientCard_PatientCardId");
            AddColumn("Health.ReferralSheet", "PatientId", c => c.Int(nullable: false));
            AddColumn("Health.Prescription", "PatientId", c => c.Int(nullable: false));
            AddColumn("Health.PatientsLeave", "PatientId", c => c.Int(nullable: false));
            AddColumn("Health.PatientCardDetail", "PatientId", c => c.Int(nullable: false));
            CreateIndex("Health.ReferralSheet", "PatientCard_PatientCardId");
            CreateIndex("Health.Prescription", "PatientCard_PatientCardId");
            CreateIndex("Health.PatientsLeave", "PatientCard_PatientCardId");
            CreateIndex("Health.PatientCardDetail", "PatientCard_PatientCardId");
            CreateIndex("Performance.EmployeePerformanceAction", "GLPeriodId");
            CreateIndex("Performance.EmployeeMeasurementDefinition", "GLPeriodId");
            CreateIndex("Performance.EmployeeMeasurement", "GLPeriodId");
            AddForeignKey("Health.ReferralSheet", "PatientCard_PatientCardId", "Health.PatientCard", "PatientCardId");
            AddForeignKey("Health.Prescription", "PatientCard_PatientCardId", "Health.PatientCard", "PatientCardId");
            AddForeignKey("Health.PatientsLeave", "PatientCard_PatientCardId", "Health.PatientCard", "PatientCardId");
            AddForeignKey("Health.PatientCardDetail", "PatientCard_PatientCardId", "Health.PatientCard", "PatientCardId");
            AddForeignKey("Performance.EmployeePerformanceAction", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Performance.EmployeeMeasurementDefinition", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Performance.EmployeeMeasurement", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm45 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.ComplainCC",
                c => new
                    {
                        ComplainCCId = c.Int(nullable: false, identity: true),
                        ComplainId = c.Int(nullable: false),
                        CC = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ComplainCCId);
            
            CreateTable(
                "HRM.ComplainDetail",
                c => new
                    {
                        ComplainDetailId = c.Int(nullable: false, identity: true),
                        ComplainId = c.Int(nullable: false),
                        Response = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ComplainDetailId);
            
            CreateTable(
                "HRM.ProbationPerformanceDetail",
                c => new
                    {
                        ProbationPerformanceDetailId = c.Int(nullable: false, identity: true),
                        ProbationPerformanceId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Measurement = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProbationPerformanceDetailId);
            
            CreateTable(
                "HRM.ProbationPerformance",
                c => new
                    {
                        ProbationPerformanceId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        HireDate = c.DateTime(),
                        HireDateAm = c.String(),
                        ProbationEndDate = c.DateTime(),
                        ProbationEndDateAm = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProbationPerformanceId);
            
            AddColumn("HRM.ComplainResponse", "Remark", c => c.String());
            DropColumn("HRM.Complain", "CC");
        }
        
        public override void Down()
        {
            AddColumn("HRM.Complain", "CC", c => c.Int(nullable: false));
            DropColumn("HRM.ComplainResponse", "Remark");
            DropTable("HRM.ProbationPerformance");
            DropTable("HRM.ProbationPerformanceDetail");
            DropTable("HRM.ComplainDetail");
            DropTable("HRM.ComplainCC");
        }
    }
}

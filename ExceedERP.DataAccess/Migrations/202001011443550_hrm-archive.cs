namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmarchive : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Archive.InstitutionArchiveNo", "InstitutionTypeId", "Archive.InstitutionType");
            DropIndex("Archive.InstitutionArchiveNo", new[] { "InstitutionTypeId" });
            CreateTable(
                "HRM.ProbationPerformanceSetting",
                c => new
                    {
                        ProbationPerformanceSettingId = c.Int(nullable: false, identity: true),
                        IntialWeight = c.Int(nullable: false),
                        EndWeight = c.Int(nullable: false),
                        ProbationMeasurement = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProbationPerformanceSettingId);
            
            CreateTable(
                "Archive.SubjectType",
                c => new
                    {
                        SubjectTypeId = c.Int(nullable: false, identity: true),
                        SubjectTypeName = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.SubjectTypeId);
            
            AddColumn("Archive.IncomingLetter", "InstitutionName", c => c.String());
            AddColumn("Archive.IncomingLetter", "OutgoingInstitutionName", c => c.String());
            AddColumn("Archive.OutgoingLetter", "OutgoingInstitutionName", c => c.String());
            AddColumn("HRM.ProbationPerformanceDetail", "Score", c => c.Int(nullable: false));
            AlterColumn("Archive.InstitutionArchiveNo", "InstitutionTypeId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("Archive.InstitutionArchiveNo", "InstitutionTypeId", c => c.Int(nullable: false));
            DropColumn("HRM.ProbationPerformanceDetail", "Score");
            DropColumn("Archive.OutgoingLetter", "OutgoingInstitutionName");
            DropColumn("Archive.IncomingLetter", "OutgoingInstitutionName");
            DropColumn("Archive.IncomingLetter", "InstitutionName");
            DropTable("Archive.SubjectType");
            DropTable("HRM.ProbationPerformanceSetting");
            CreateIndex("Archive.InstitutionArchiveNo", "InstitutionTypeId");
            AddForeignKey("Archive.InstitutionArchiveNo", "InstitutionTypeId", "Archive.InstitutionType", "InstitutionTypeId", cascadeDelete: true);
        }
    }
}

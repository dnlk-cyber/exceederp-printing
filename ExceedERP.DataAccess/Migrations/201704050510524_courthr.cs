namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class courthr : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.CommitteMinute",
                c => new
                    {
                        CommitteMinuteId = c.Int(nullable: false, identity: true),
                        FileNo = c.String(),
                        Agenda = c.String(),
                        PlaceOfMetting = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CommitteMinuteId);
            
            AddColumn("common.Attachment", "FileName", c => c.String());
            AddColumn("HRM.Commitment", "EmployeeCode", c => c.String());
            AddColumn("HRM.Experience", "EmployeeCode", c => c.String());
            AddColumn("HRM.LeaveRequest", "LeaveBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.Placement", "Index", c => c.Int(nullable: false));
            AddColumn("HRM.Step", "StepSequence", c => c.Int(nullable: false));
            AlterColumn("HRM.Committee", "CommitteeType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("HRM.Committee", "CommitteeType", c => c.String(nullable: false));
            DropColumn("HRM.Step", "StepSequence");
            DropColumn("HRM.Placement", "Index");
            DropColumn("HRM.LeaveRequest", "LeaveBalance");
            DropColumn("HRM.Experience", "EmployeeCode");
            DropColumn("HRM.Commitment", "EmployeeCode");
            DropColumn("common.Attachment", "FileName");
            DropTable("HRM.CommitteMinute");
        }
    }
}

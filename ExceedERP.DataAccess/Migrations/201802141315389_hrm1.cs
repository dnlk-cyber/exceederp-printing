namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.Committee", "EmployeeId", "HRM.Employee");
            DropIndex("HRM.Committee", new[] { "EmployeeId" });
            CreateTable(
                "HRM.CommitteeMember",
                c => new
                    {
                        CommitteeMemberId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Role = c.String(nullable: false),
                        CommitteeId = c.Int(nullable: false),
                        CommitteeType = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateAmharic = c.String(),
                        CreatedBy = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CommitteeMemberId)
                .ForeignKey("HRM.Committee", t => t.CommitteeId, cascadeDelete: true)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.CommitteeId);
            
            AddColumn("HRM.Committee", "Description", c => c.String());
            CreateIndex("HRM.CommitteMinute", "CommitteeId");
            AddForeignKey("HRM.CommitteMinute", "CommitteeId", "HRM.Committee", "CommitteeId", cascadeDelete: true);
            DropColumn("HRM.Committee", "EmployeeId");
            DropColumn("HRM.Committee", "Role");
        }
        
        public override void Down()
        {
            AddColumn("HRM.Committee", "Role", c => c.String(nullable: false));
            AddColumn("HRM.Committee", "EmployeeId", c => c.Int(nullable: false));
            DropForeignKey("HRM.CommitteeMember", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.CommitteeMember", "CommitteeId", "HRM.Committee");
            DropForeignKey("HRM.CommitteMinute", "CommitteeId", "HRM.Committee");
            DropIndex("HRM.CommitteMinute", new[] { "CommitteeId" });
            DropIndex("HRM.CommitteeMember", new[] { "CommitteeId" });
            DropIndex("HRM.CommitteeMember", new[] { "EmployeeId" });
            DropColumn("HRM.Committee", "Description");
            DropTable("HRM.CommitteeMember");
            CreateIndex("HRM.Committee", "EmployeeId");
            AddForeignKey("HRM.Committee", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
        }
    }
}

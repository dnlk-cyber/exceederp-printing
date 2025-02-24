namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class weekendexceptions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.WeekendExceptions",
                c => new
                    {
                        WeekendExceptionId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        OrgStructureId = c.Int(),
                        Remark = c.String(),
                        WeekendId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WeekendExceptionId)
                .ForeignKey("HRM.Weekend", t => t.WeekendId, cascadeDelete: true)
                .Index(t => t.WeekendId);
            
            AddColumn("HRM.Weekend", "IsFullDay", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.WeekendExceptions", "WeekendId", "HRM.Weekend");
            DropIndex("HRM.WeekendExceptions", new[] { "WeekendId" });
            DropColumn("HRM.Weekend", "IsFullDay");
            DropTable("HRM.WeekendExceptions");
        }
    }
}

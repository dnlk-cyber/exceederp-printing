namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm20 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.CommitteMinute", "DisciplinaryCaseId", c => c.Int(nullable: false));
            CreateIndex("HRM.CommitteMinute", "DisciplinaryCaseId");
            AddForeignKey("HRM.CommitteMinute", "DisciplinaryCaseId", "HRM.DisciplinaryCase", "DisciplinaryCaseId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.CommitteMinute", "DisciplinaryCaseId", "HRM.DisciplinaryCase");
            DropIndex("HRM.CommitteMinute", new[] { "DisciplinaryCaseId" });
            DropColumn("HRM.CommitteMinute", "DisciplinaryCaseId");
        }
    }
}

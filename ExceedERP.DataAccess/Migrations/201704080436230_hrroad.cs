namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrroad : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Employee", "FirstNameEnglish", c => c.String(nullable: false));
            AddColumn("HRM.Employee", "MiddleNameEnglish", c => c.String(nullable: false));
            AddColumn("HRM.Employee", "LastNameEnglish", c => c.String(nullable: false));
            AddColumn("common.Attachment", "MINUTE", c => c.String());
            AddColumn("HRM.CommitteMinute", "Date", c => c.DateTime(nullable: false));
            AddColumn("Health.PatientsLeave", "PeriodId", c => c.Int(nullable: false));
            AddColumn("Health.PatientsLeave", "From", c => c.DateTime(nullable: false));
            AddColumn("Health.PatientsLeave", "To", c => c.DateTime(nullable: false));
            CreateIndex("Health.PatientsLeave", "PeriodId");
            AddForeignKey("Health.PatientsLeave", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("Health.PatientsLeave", "PeriodId", "common.Period");
            DropIndex("Health.PatientsLeave", new[] { "PeriodId" });
            DropColumn("Health.PatientsLeave", "To");
            DropColumn("Health.PatientsLeave", "From");
            DropColumn("Health.PatientsLeave", "PeriodId");
            DropColumn("HRM.CommitteMinute", "Date");
            DropColumn("common.Attachment", "MINUTE");
            DropColumn("HRM.Employee", "LastNameEnglish");
            DropColumn("HRM.Employee", "MiddleNameEnglish");
            DropColumn("HRM.Employee", "FirstNameEnglish");
        }
    }
}

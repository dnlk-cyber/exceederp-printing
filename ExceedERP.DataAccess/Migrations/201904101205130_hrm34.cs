namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm34 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Archive.EmployeeIncommingLetter", "RecivedTime", c => c.String());
            AlterColumn("Archive.EmployeeIncommingLetter", "OutgoingTime", c => c.String());
            AlterColumn("Archive.IncomingLetter", "RecivedTime", c => c.String());
            AlterColumn("Archive.IncomingLetter", "OutgoingTime", c => c.String());
            AlterColumn("Archive.OutgoingLetter", "OutgoingTime", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("Archive.OutgoingLetter", "OutgoingTime", c => c.DateTime(nullable: false));
            AlterColumn("Archive.IncomingLetter", "OutgoingTime", c => c.DateTime(nullable: false));
            AlterColumn("Archive.IncomingLetter", "RecivedTime", c => c.DateTime(nullable: false));
            AlterColumn("Archive.EmployeeIncommingLetter", "OutgoingTime", c => c.DateTime(nullable: false));
            AlterColumn("Archive.EmployeeIncommingLetter", "RecivedTime", c => c.DateTime(nullable: false));
        }
    }
}

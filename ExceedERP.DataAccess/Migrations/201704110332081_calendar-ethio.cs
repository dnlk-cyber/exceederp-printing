namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class calendarethio : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Employee", "DateOfBirthAm", c => c.String());
            AddColumn("HRM.Employee", "HiredDateAmharic", c => c.String());
            AddColumn("common.Period", "AmharicStartDate", c => c.String());
            AddColumn("common.Period", "AmharicEndDate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("common.Period", "AmharicEndDate");
            DropColumn("common.Period", "AmharicStartDate");
            DropColumn("HRM.Employee", "HiredDateAmharic");
            DropColumn("HRM.Employee", "DateOfBirthAm");
        }
    }
}

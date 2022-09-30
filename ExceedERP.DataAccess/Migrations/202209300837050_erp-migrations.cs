namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class erpmigrations : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Employee", "IsReHired", c => c.Boolean(nullable: false));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "SupportGroupId", c => c.Int(nullable: false));
            AlterColumn("HRM.Employee", "EmployeeTypeId", c => c.Int());
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "TicketGroup");
        }
        
        public override void Down()
        {
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "TicketGroup", c => c.String());
            AlterColumn("HRM.Employee", "EmployeeTypeId", c => c.Int(nullable: false));
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "SupportGroupId");
            DropColumn("HRM.Employee", "IsReHired");
        }
    }
}

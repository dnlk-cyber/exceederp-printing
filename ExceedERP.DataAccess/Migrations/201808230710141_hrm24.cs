namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm24 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Payroll.OverTimeTransfer", "IsSendForApproval", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Payroll.OverTimeTransfer", "IsSendForApproval");
        }
    }
}

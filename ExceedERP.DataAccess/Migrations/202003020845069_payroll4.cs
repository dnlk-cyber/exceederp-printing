namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payroll4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Payroll.RegularContribution", "NetGross", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Payroll.RegularContribution", "NetGross");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrpayroll1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Branch", "LocationAccount", c => c.String());
            AddColumn("Payroll.PayrollDistribution", "LocationCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Payroll.PayrollDistribution", "LocationCode");
            DropColumn("HRM.Branch", "LocationAccount");
        }
    }
}

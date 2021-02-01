namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Payroll.PayrollEmployeeAccount", "SubAccCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Payroll.PayrollEmployeeAccount", "SubAccCode");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mixed10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetReceive", "IsManual", c => c.Boolean(nullable: false));
            AlterColumn("Payroll.EmpPayrollNoDaysWorked", "Days", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("Payroll.EmpPayrollNoDaysWorked", "Days", c => c.Int(nullable: false));
            DropColumn("EXCEED.FixedAssetReceive", "IsManual");
        }
    }
}

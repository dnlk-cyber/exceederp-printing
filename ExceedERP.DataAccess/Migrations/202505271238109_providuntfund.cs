namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class providuntfund : DbMigration
    {
        public override void Up()
        {
            AddColumn("Payroll.ProvidentFund", "ApplyPension", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Payroll.ProvidentFund", "ApplyPension");
        }
    }
}

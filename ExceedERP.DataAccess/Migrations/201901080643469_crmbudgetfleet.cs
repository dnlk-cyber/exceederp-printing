namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class crmbudgetfleet : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.BudgetAccountSheduleCategory", "SheduleCategoryType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.BudgetAccountSheduleCategory", "SheduleCategoryType");
        }
    }
}

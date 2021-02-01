namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class budget : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Budget.BudgetCategory", "BudgetControl", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("Budget.BudgetCategory", "BudgetControl", c => c.String(nullable: false));
        }
    }
}

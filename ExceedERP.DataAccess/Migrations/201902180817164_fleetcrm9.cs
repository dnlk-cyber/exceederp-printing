namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetcrm9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Budget.BudgetRequestApproval", "IsTransfer", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Budget.BudgetRequestApproval", "IsTransfer");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finsetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FinanceSetting", "BankServiceChargeExpense", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FinanceSetting", "BankServiceChargeExpense");
        }
    }
}

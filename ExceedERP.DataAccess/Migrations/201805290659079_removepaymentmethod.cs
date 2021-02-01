namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removepaymentmethod : DbMigration
    {
        public override void Up()
        {
            DropColumn("EXCEED.ApTravelExpenseClaim", "PaymentMethod");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ApTravelExpenseClaim", "PaymentMethod", c => c.String());
        }
    }
}

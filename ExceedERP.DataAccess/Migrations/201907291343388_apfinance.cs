namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apfinance : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApTravelExpenseClaim", "Paid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "Paid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "Paid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PaymentRequest", "Paid");
            DropColumn("EXCEED.ApTravelOrder", "Paid");
            DropColumn("EXCEED.ApTravelExpenseClaim", "Paid");
        }
    }
}

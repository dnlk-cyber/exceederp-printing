namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class travelclaim : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ApTravelExpenseClaim", "PaymentMethod", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ApTravelExpenseClaim", "PaymentMethod", c => c.String(nullable: false));
        }
    }
}

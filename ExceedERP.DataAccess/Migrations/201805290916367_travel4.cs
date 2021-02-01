namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class travel4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApTravelOrderAdvancePayment", "PaymentMethod", c => c.String(nullable: false));
            DropColumn("EXCEED.ApTravelOrder", "PaymentMethod");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ApTravelOrder", "PaymentMethod", c => c.String(nullable: false));
            DropColumn("EXCEED.ApTravelOrderAdvancePayment", "PaymentMethod");
        }
    }
}

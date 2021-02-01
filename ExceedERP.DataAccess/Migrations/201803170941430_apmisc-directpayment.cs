namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apmiscdirectpayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApMiscellaneousPayment", "IsDirectPayment", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ApMiscellaneousPayment", "IsDirectPayment");
        }
    }
}

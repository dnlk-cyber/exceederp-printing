namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class discountaccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FinanceSetting", "DiscountArAccount", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FinanceSetting", "DiscountArAccount");
        }
    }
}

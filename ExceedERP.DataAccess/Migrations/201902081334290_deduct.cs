namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApDirectPaymentLine", "DeductAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ApDirectPaymentLine", "DeductAmount");
        }
    }
}

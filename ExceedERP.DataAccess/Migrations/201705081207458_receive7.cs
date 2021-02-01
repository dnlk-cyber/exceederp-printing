namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class receive7 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.PropertyCountItem", "PreviousBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PropertyCountItem", "PreviousBalance", c => c.Int(nullable: false));
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class print : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PropertyCount", "CountDate", c => c.DateTime(nullable: false));
            AlterColumn("EXCEED.PropertyCountItem", "Count", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PropertyCountItem", "Count", c => c.Int(nullable: false));
            DropColumn("EXCEED.PropertyCount", "CountDate");
        }
    }
}

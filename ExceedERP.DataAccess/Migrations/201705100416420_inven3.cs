namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inven3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.IssueFuelItem", "AdditionalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.IssueFuelItem", "AdditionalCost");
        }
    }
}

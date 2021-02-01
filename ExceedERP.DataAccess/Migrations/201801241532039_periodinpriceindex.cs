namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class periodinpriceindex : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ItemPriceIndex", "GLPeriodId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ItemPriceIndex", "GLPeriodId");
        }
    }
}

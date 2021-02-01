namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stockcosttypes : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ItemCategory", "CostType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ItemCategory", "CostType");
        }
    }
}

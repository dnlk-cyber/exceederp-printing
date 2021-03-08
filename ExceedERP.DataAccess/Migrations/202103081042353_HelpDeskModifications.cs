namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HelpDeskModifications : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.MaterialsUsed", "Quantity", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.MaterialsUsed", "Quantity");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetcrm3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("CRM.InventoryCateoryMap", "InventoryCategoryId", c => c.String(nullable: false));
            AlterColumn("CRM.InventoryCateoryMap", "Category", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("CRM.InventoryCateoryMap", "Category", c => c.String());
            AlterColumn("CRM.InventoryCateoryMap", "InventoryCategoryId", c => c.String());
        }
    }
}

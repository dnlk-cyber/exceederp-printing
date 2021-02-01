namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetcrm : DbMigration
    {
        public override void Up()
        {
            AddColumn("FleetManagement.CheckList", "IsMaintenace", c => c.Boolean(nullable: false));
            AlterColumn("FleetManagement.ExternalMaterial", "MaterialName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("FleetManagement.ExternalMaterial", "MaterialName", c => c.String());
            DropColumn("FleetManagement.CheckList", "IsMaintenace");
        }
    }
}

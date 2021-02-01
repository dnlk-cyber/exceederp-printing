namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class workunitonreturn : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreReturn", "OrgStructureId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StoreReturn", "OrgStructureId");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class returnfromother : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreReturn", "IsForOther", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreReturn", "ForBranchId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StoreReturn", "ForBranchId");
            DropColumn("EXCEED.StoreReturn", "IsForOther");
        }
    }
}

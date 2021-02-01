namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reqforother : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisition", "IsForOther", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "ForBranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StoreRequisition", "ForBranchId");
            DropColumn("EXCEED.StoreRequisition", "IsForOther");
        }
    }
}

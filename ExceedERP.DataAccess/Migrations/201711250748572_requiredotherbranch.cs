namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requiredotherbranch : DbMigration
    {
        public override void Up()
        {
            DropColumn("EXCEED.StoreRequisition", "ForBranchId");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.StoreRequisition", "ForBranchId", c => c.Int(nullable: false));
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class branchreqother : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisition", "ForBranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StoreRequisition", "ForBranchId");
        }
    }
}

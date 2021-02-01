namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class issue1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisition", "BranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StoreRequisition", "BranchId");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invento4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisitionItem", "GeneratePurchaseReq", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Issue", "IsFuel", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.Issue", "IsFuel");
            DropColumn("EXCEED.StoreRequisitionItem", "GeneratePurchaseReq");
        }
    }
}

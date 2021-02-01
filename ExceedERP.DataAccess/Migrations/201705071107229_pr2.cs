namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pr2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseOrder", "ProformaPurchaseID", c => c.Int(nullable: false));
            DropColumn("EXCEED.PurchaseOrder", "PONumber");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.PurchaseOrder", "PONumber", c => c.String());
            DropColumn("EXCEED.PurchaseOrder", "ProformaPurchaseID");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class purchaseorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseOrder", "ForwardedFrom", c => c.String());
            AddColumn("EXCEED.PurchaseOrder", "ForwardedTo", c => c.String());
            AddColumn("EXCEED.PurchaseOrder", "IsFinalApproval", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseOrder", "IsFinalApproval");
            DropColumn("EXCEED.PurchaseOrder", "ForwardedTo");
            DropColumn("EXCEED.PurchaseOrder", "ForwardedFrom");
        }
    }
}

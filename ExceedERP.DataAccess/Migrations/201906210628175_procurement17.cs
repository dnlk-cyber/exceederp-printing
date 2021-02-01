namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractManagement", "IsFullfilled", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractManagement", "IsPaid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseOrder", "IsPaid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseOrder", "IsPaid");
            DropColumn("EXCEED.ContractManagement", "IsPaid");
            DropColumn("EXCEED.ContractManagement", "IsFullfilled");
        }
    }
}

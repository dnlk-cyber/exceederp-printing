namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contract2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractManagement", "IsTransferredToPayment", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractManagement", "IsTransferredToFollowUp", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseOrder", "IsTransferredToPayment", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseOrder", "IsTransferredToPayment");
            DropColumn("EXCEED.ContractManagement", "IsTransferredToFollowUp");
            DropColumn("EXCEED.ContractManagement", "IsTransferredToPayment");
        }
    }
}

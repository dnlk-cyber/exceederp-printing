namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contractmanagement : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractManagement", "ForwardedFrom", c => c.String());
            AddColumn("EXCEED.ContractManagement", "ForwardedTo", c => c.String());
            AddColumn("EXCEED.ContractManagement", "IsFinalApproval", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ContractManagement", "IsFinalApproval");
            DropColumn("EXCEED.ContractManagement", "ForwardedTo");
            DropColumn("EXCEED.ContractManagement", "ForwardedFrom");
        }
    }
}

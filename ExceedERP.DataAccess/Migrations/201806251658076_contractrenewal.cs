namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contractrenewal : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractRenewal", "ForwardedFrom", c => c.String());
            AddColumn("EXCEED.ContractRenewal", "ForwardedTo", c => c.String());
            AddColumn("EXCEED.ContractRenewal", "IsFinalApproval", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ContractRenewal", "IsFinalApproval");
            DropColumn("EXCEED.ContractRenewal", "ForwardedTo");
            DropColumn("EXCEED.ContractRenewal", "ForwardedFrom");
        }
    }
}

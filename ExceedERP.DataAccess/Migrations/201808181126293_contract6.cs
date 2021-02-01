namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contract6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractManagement", "ContractStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ContractManagement", "ContractStatus");
        }
    }
}

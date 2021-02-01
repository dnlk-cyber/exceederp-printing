namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contractman : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractManagement", "ProcurementRefNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ContractManagement", "ProcurementRefNo");
        }
    }
}

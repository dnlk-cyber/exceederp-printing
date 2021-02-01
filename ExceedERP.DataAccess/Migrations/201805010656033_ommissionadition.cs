namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ommissionadition : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractManagementItem", "Addition", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ContractManagementItem", "Omission", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ContractManagementItem", "Omission");
            DropColumn("EXCEED.ContractManagementItem", "Addition");
        }
    }
}

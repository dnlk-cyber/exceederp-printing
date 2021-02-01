namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contract5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractManagement", "DeliveryType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ContractManagement", "DeliveryType");
        }
    }
}

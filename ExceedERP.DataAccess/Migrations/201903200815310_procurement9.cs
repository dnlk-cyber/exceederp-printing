namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractManagementItem", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ContractManagementItem", "Name");
        }
    }
}

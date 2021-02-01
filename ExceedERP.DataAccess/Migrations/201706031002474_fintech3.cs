namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fintech3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArCustomerType", "Enable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ArCustomerType", "Enable");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class arstandardcoll : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArStandardCollection", "IsNewCustomer", c => c.Boolean(nullable: false));
            AlterColumn("EXCEED.ArStandardCollection", "CustomerId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ArStandardCollection", "CustomerId", c => c.String(nullable: false));
            DropColumn("EXCEED.ArStandardCollection", "IsNewCustomer");
        }
    }
}

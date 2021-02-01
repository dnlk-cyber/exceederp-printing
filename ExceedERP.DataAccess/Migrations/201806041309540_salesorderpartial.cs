namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesorderpartial : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ArSalesOrderPartial", "ItemName", c => c.String());
            AlterColumn("EXCEED.ArSalesOrderPartial", "AccountCode", c => c.String());
            AlterColumn("EXCEED.ArSalesOrderPartial", "UoM", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ArSalesOrderPartial", "UoM", c => c.String(nullable: false));
            AlterColumn("EXCEED.ArSalesOrderPartial", "AccountCode", c => c.String(nullable: false));
            AlterColumn("EXCEED.ArSalesOrderPartial", "ItemName", c => c.String(nullable: false));
        }
    }
}

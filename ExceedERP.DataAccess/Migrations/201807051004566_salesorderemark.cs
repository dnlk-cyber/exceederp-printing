namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesorderemark : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArSalesOrderLine", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ArSalesOrderLine", "Remark");
        }
    }
}

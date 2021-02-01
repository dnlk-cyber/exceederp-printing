namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesorder2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArSalesOrder", "Product", c => c.String());
            AddColumn("EXCEED.ArSalesOrder", "Length", c => c.String());
            AddColumn("EXCEED.ArSalesOrder", "Repeatition", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ArSalesOrder", "Repeatition");
            DropColumn("EXCEED.ArSalesOrder", "Length");
            DropColumn("EXCEED.ArSalesOrder", "Product");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class arcustomer : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ArInvoice", "CustomerName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ArInvoice", "CustomerName", c => c.String(nullable: false));
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class arinvoiceref : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArInvoice", "Reference", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ArInvoice", "Reference");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class creditdebitmemo : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArInvoice", "SourceInvoiceId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ArInvoice", "SourceInvoiceId");
        }
    }
}

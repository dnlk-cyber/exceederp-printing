namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class creditdebitmemo1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ArInvoice", "SourceInvoiceId", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ArInvoice", "SourceInvoiceId", c => c.String());
        }
    }
}

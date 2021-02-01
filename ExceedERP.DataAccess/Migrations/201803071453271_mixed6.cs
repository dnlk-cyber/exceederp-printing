namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mixed6 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ApPayment", "PaymentReference", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ApPayment", "PaymentReference", c => c.String());
        }
    }
}

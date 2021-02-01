namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class followup1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ForeignPurchase", "ProcurementMethod", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ForeignPurchase", "ProcurementMethod", c => c.Int(nullable: false));
        }
    }
}

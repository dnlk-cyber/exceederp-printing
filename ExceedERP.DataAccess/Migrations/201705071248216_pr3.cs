namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pr3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.PurchaseOrderValidation", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PurchaseOrderValidation", "Status", c => c.String());
        }
    }
}

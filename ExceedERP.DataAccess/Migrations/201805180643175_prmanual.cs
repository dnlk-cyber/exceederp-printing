namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prmanual : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseRequisition", "IsManual", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseRequisition", "IsManual");
        }
    }
}

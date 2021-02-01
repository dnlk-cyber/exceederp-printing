namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class po4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.PurchaseOrder", "ProformaPurchaseID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PurchaseOrder", "ProformaPurchaseID", c => c.Int(nullable: false));
        }
    }
}

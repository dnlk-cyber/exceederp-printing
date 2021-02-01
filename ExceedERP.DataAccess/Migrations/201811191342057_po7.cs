namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class po7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseOrder", "IsTransferredToFollowUp", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseOrder", "IsTransferredToFollowUp");
        }
    }
}

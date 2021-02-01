namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tender1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Tender", "PurchaseCategory", c => c.Int(nullable: false));
            DropColumn("EXCEED.Tender", "IsICB");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.Tender", "IsICB", c => c.Boolean(nullable: false));
            DropColumn("EXCEED.Tender", "PurchaseCategory");
        }
    }
}

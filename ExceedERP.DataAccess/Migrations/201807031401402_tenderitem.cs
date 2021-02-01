namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tenderitem : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.PassFailResult", "ItemName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PassFailResult", "ItemName", c => c.String(nullable: false));
        }
    }
}

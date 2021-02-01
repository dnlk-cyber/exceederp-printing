namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tend1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.PassFailResult", "ItemCode", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PassFailResult", "ItemCode", c => c.String());
        }
    }
}

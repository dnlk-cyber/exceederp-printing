namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class receive2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.GoodReceiveValidation", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.GoodReceiveValidation", "Status", c => c.String());
        }
    }
}

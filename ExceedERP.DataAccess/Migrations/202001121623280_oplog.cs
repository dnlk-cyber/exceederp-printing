namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class oplog : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GlobalOperationLog", "Module", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GlobalOperationLog", "Module");
        }
    }
}

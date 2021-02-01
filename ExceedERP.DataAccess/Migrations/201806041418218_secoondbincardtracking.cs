namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class secoondbincardtracking : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.BinCardLineSecondary", "BinCardLineID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.BinCardLineSecondary", "BinCardLineID");
        }
    }
}

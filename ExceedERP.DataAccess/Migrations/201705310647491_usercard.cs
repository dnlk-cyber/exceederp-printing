namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usercard : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.UserCardItems", "GRNDate", c => c.DateTime());
            AddColumn("EXCEED.UserCardItems", "GRNNo", c => c.String());
            DropColumn("EXCEED.UserCardItems", "ReturnDate");
            DropColumn("EXCEED.UserCardItems", "SRNNo");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.UserCardItems", "SRNNo", c => c.String());
            AddColumn("EXCEED.UserCardItems", "ReturnDate", c => c.DateTime());
            DropColumn("EXCEED.UserCardItems", "GRNNo");
            DropColumn("EXCEED.UserCardItems", "GRNDate");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usercard1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.UserCardItems", "Tag", c => c.String());
            AddColumn("EXCEED.UserCardItems", "SerialNo", c => c.String());
            AddColumn("EXCEED.UserCardItems", "GRN", c => c.String());
            DropColumn("EXCEED.UserCardItems", "TagNumber");
            DropColumn("EXCEED.UserCardItems", "SerialPartNo");
            DropColumn("EXCEED.UserCardItems", "GRNNo");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.UserCardItems", "GRNNo", c => c.String());
            AddColumn("EXCEED.UserCardItems", "SerialPartNo", c => c.String());
            AddColumn("EXCEED.UserCardItems", "TagNumber", c => c.String());
            DropColumn("EXCEED.UserCardItems", "GRN");
            DropColumn("EXCEED.UserCardItems", "SerialNo");
            DropColumn("EXCEED.UserCardItems", "Tag");
        }
    }
}

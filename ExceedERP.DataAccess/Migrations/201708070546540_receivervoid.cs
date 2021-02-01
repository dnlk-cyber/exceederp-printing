namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class receivervoid : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ItemTransfer", "IsSecondVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemTransfer", "SecondVoidBy", c => c.String());
            AddColumn("EXCEED.ItemTransfer", "SecondVoidTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ItemTransfer", "SecondVoidTime");
            DropColumn("EXCEED.ItemTransfer", "SecondVoidBy");
            DropColumn("EXCEED.ItemTransfer", "IsSecondVoid");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itemnameevaluation : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PassFailResult", "CatCode", c => c.String());
            AddColumn("EXCEED.PassFailResult", "CategoryName", c => c.String());
            AddColumn("EXCEED.PassFailResult", "ItemName", c => c.String(nullable: false));
            AlterColumn("EXCEED.PassFailResult", "ItmCode", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PassFailResult", "ItmCode", c => c.String(nullable: false));
            DropColumn("EXCEED.PassFailResult", "ItemName");
            DropColumn("EXCEED.PassFailResult", "CategoryName");
            DropColumn("EXCEED.PassFailResult", "CatCode");
        }
    }
}

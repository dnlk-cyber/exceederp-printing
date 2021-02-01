namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tenderextension2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("EXCEED.TenderExtension", "FloatDate");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.TenderExtension", "FloatDate", c => c.DateTime(nullable: false));
        }
    }
}

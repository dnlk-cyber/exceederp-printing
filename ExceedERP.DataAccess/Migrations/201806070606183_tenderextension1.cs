namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tenderextension1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("EXCEED.TenderExtension", new[] { "TenderId" });
            CreateIndex("EXCEED.TenderExtension", "TenderID");
        }
        
        public override void Down()
        {
            DropIndex("EXCEED.TenderExtension", new[] { "TenderID" });
            CreateIndex("EXCEED.TenderExtension", "TenderId");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm41 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ComplainResponse",
                c => new
                    {
                        ComplainResponseId = c.Int(nullable: false, identity: true),
                        Response = c.String(),
                        ComplainId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ComplainResponseId);
            
            AddColumn("HRM.Complain", "RefersTo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.Complain", "RefersTo");
            DropTable("EXCEED.ComplainResponse");
        }
    }
}

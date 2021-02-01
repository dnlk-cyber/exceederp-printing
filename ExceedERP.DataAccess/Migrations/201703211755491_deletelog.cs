namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deletelog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GlobalDeleteLog",
                c => new
                    {
                        GlobalDeleteLogId = c.Int(nullable: false, identity: true),
                        IpAddress = c.String(),
                        UserName = c.String(),
                        TimeAccessed = c.DateTime(nullable: false),
                        Type = c.String(),
                        Status = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.GlobalDeleteLogId);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.GlobalDeleteLog");
        }
    }
}

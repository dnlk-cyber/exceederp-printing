namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notification1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.Notification",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Details = c.String(),
                        Title = c.String(),
                        DetailsUrl = c.String(),
                        SentTo = c.String(),
                        Date = c.DateTime(nullable: false),
                        IsRead = c.String(),
                        IsDeleted = c.String(),
                        IsReminder = c.String(),
                        Code = c.String(),
                        NotificationType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.Notification");
        }
    }
}

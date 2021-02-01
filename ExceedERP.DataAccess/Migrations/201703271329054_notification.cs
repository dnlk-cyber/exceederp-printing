namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notification : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GlobalNotification",
                c => new
                    {
                        GlobalNotificationId = c.Int(nullable: false, identity: true),
                        IpAddress = c.String(),
                        ResponsibleRoles = c.String(),
                        ResponsibleUserName = c.String(),
                        AlarmGeneratedTime = c.DateTime(nullable: false),
                        DeadLine = c.DateTime(nullable: false),
                        Status = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.GlobalNotificationId);
            
            DropTable("EXCEED.GlobalOrgInformation");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.GlobalOrgInformation",
                c => new
                    {
                        GlobalOrgInformationID = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Name = c.String(nullable: false),
                        Abbreviation = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        Email = c.String(),
                        Fax = c.String(),
                        Address = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.GlobalOrgInformationID);
            
            DropTable("EXCEED.GlobalNotification");
        }
    }
}

namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newone : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Archive.LetterSender",
                c => new
                    {
                        LetterSenderId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Receiver = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        Sender = c.String(),
                        BookNo = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.LetterSenderId)
                .ForeignKey("Archive.IncommingLetter", t => t.BookNo)
                .Index(t => t.BookNo);
            
            AddColumn("Archive.LetterStatus", "Date", c => c.DateTime(nullable: false));
            DropColumn("Performance.EmployeeMeasurementDefinition", "Role");
        }
        
        public override void Down()
        {
            AddColumn("Performance.EmployeeMeasurementDefinition", "Role", c => c.Int(nullable: false));
            DropForeignKey("Archive.LetterSender", "BookNo", "Archive.IncommingLetter");
            DropIndex("Archive.LetterSender", new[] { "BookNo" });
            DropColumn("Archive.LetterStatus", "Date");
            DropTable("Archive.LetterSender");
        }
    }
}

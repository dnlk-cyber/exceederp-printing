namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tencercalcel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.TenderCancellationUploadDbModel",
                c => new
                    {
                        TenderCancellationUploadDbModelId = c.Int(nullable: false, identity: true),
                        TenderCancellationId = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.TenderCancellationUploadDbModelId)
                .ForeignKey("EXCEED.TenderCancellation", t => t.TenderCancellationId, cascadeDelete: true)
                .Index(t => t.TenderCancellationId);
            
            AddColumn("EXCEED.TenderCancellation", "Date", c => c.DateTime(nullable: false));
            DropColumn("EXCEED.TenderCancellation", "FileName");
            DropColumn("EXCEED.TenderCancellation", "FilePath");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.TenderCancellation", "FilePath", c => c.String());
            AddColumn("EXCEED.TenderCancellation", "FileName", c => c.String());
            DropForeignKey("EXCEED.TenderCancellationUploadDbModel", "TenderCancellationId", "EXCEED.TenderCancellation");
            DropIndex("EXCEED.TenderCancellationUploadDbModel", new[] { "TenderCancellationId" });
            DropColumn("EXCEED.TenderCancellation", "Date");
            DropTable("EXCEED.TenderCancellationUploadDbModel");
        }
    }
}

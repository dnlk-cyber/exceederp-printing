namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class archive1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.EmployeeIncommingUploadDbModel", "BookNo", "Archive.EmployeeIncommingLetter");
            DropIndex("EXCEED.EmployeeIncommingUploadDbModel", new[] { "BookNo" });
            AddColumn("Archive.LetterSender", "SenderDepartment", c => c.Int(nullable: false));
            DropTable("EXCEED.EmployeeIncommingUploadDbModel");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.EmployeeIncommingUploadDbModel",
                c => new
                    {
                        EmployeeIncommingUploadDbModelId = c.Int(nullable: false, identity: true),
                        BookNo = c.String(maxLength: 128),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeIncommingUploadDbModelId);
            
            DropColumn("Archive.LetterSender", "SenderDepartment");
            CreateIndex("EXCEED.EmployeeIncommingUploadDbModel", "BookNo");
            AddForeignKey("EXCEED.EmployeeIncommingUploadDbModel", "BookNo", "Archive.EmployeeIncommingLetter", "BookNo");
        }
    }
}

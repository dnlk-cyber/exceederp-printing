namespace ExceedERP.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class hrmerge7 : DbMigration
    {
        public override void Up()
        {
            //MoveTable(name: "HRM.JobTitleMappingDetailTemp", newSchema: "EXCEED");
            DropForeignKey("HRM.JobTitleMappingMain", "JobTitleMappingId", "HRM.JobTitleMapping");
            DropForeignKey("HRM.JobTitleMappingMianTemp", "JobTitleMapping_JobTitleMappingTempId", "HRM.JobTitleMappingTemp");
            DropForeignKey("HRM.JobTitleMappingDetailTemp", "JobTitleMapping_JobTitleMappingTempId", "HRM.JobTitleMappingTemp");
            DropIndex("HRM.JobTitleMappingDetailTemp", new[] { "JobTitleMapping_JobTitleMappingTempId" });
            DropIndex("HRM.JobTitleMappingMain", new[] { "JobTitleMappingId" });
            DropIndex("HRM.JobTitleMappingMianTemp", new[] { "JobTitleMapping_JobTitleMappingTempId" });
            DropColumn("HRM.JobTitleMappingDetailTemp", "JobTitleMappingId");
            RenameColumn(table: "HRM.JobTitleMappingDetailTemp", name: "JobTitleMapping_JobTitleMappingTempId", newName: "JobTitleMappingId");
            DropPrimaryKey("HRM.JobTitleMappingTemp");
            CreateTable(
                "Archive.EmployeeIncommingLetter",
                c => new
                    {
                        BookNo = c.String(nullable: false, maxLength: 128),
                        LetterDate = c.DateTime(nullable: false),
                        LetterAmharic = c.String(),
                        LetterReferenceNo = c.String(nullable: false),
                        RecivedDate = c.DateTime(nullable: false),
                        RecivedDateAmharic = c.String(),
                        OwnerName = c.String(nullable: false),
                        InstitutionTypeId = c.Int(),
                        Subject = c.String(nullable: false),
                        Shelve = c.String(nullable: false),
                        Row = c.String(nullable: false),
                        FolderNo = c.String(nullable: false),
                        FileNo = c.String(nullable: false),
                        NoOfAttachment = c.Int(nullable: false),
                        RefersTo = c.String(),
                        RefernceType = c.String(),
                        Remark = c.String(),
                        OutgoingLetterDate = c.DateTime(),
                        OutgoingLetterDateAm = c.String(),
                        OutgoingLetterReferenceNo = c.String(),
                        OutgoingOwnerName = c.String(),
                        OutgoingInstitutionTypeId = c.Int(),
                        OutgoingSubject = c.String(),
                        OutgoingShelve = c.String(),
                        OutgoingRow = c.String(),
                        OutgoingFolderNo = c.String(),
                        OutgoingFileNo = c.String(),
                        OutgoingNoOfAttachment = c.Int(nullable: false),
                        OutgoingSerialNo = c.String(),
                        OutgoingRefernceType = c.String(),
                        OutgoingRemark = c.String(),
                        VolumeNo = c.String(),
                        OutgoingVolumeNo = c.String(),
                        EnableDisbale = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.BookNo);

            CreateTable(
                "Archive.EmployeeLetterSender",
                c => new
                    {
                        LetterSenderId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        DateAmharic = c.String(),
                        Receiver = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        Sender = c.String(),
                        BookNo = c.String(maxLength: 128),
                        OwnerName = c.String(),
                        FileName = c.String(),
                        StatusName = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LetterSenderId)
                .ForeignKey("Archive.EmployeeIncommingLetter", t => t.BookNo)
                .Index(t => t.BookNo);

            //AddColumn("HRM.JobTitleMappingTemp", "JobTitleMappingId", c => c.Int(nullable: false, identity: true));
            AddColumn("Archive.IncomingLetter", "EnableDisbale", c => c.Boolean(nullable: false));
            AlterColumn("HRM.JobTitleMappingDetailTemp", "JobTitleMappingId", c => c.Int(nullable: false));
            AlterColumn("Archive.LetterSender", "Receiver", c => c.Int(nullable: false));
            //AddPrimaryKey("HRM.JobTitleMappingTemp", "JobTitleMappingId");
            CreateIndex("HRM.JobTitleMappingDetailTemp", "JobTitleMappingId");
            //AddForeignKey("HRM.JobTitleMappingDetailTemp", "JobTitleMappingId", "HRM.JobTitleMappingTemp", "JobTitleMappingId", cascadeDelete: true);
            DropColumn("HRM.JobTitleMappingTemp", "JobTitleMappingTempId");
            DropTable("HRM.JobTitleMappingMain");
            DropTable("HRM.JobTitleMappingMianTemp");
        }

        public override void Down()
        {
            CreateTable(
                "HRM.JobTitleMappingMianTemp",
                c => new
                    {
                        JobTitleMappingMianId = c.Int(nullable: false, identity: true),
                        MainFunction = c.String(),
                        JobTitleMappingId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        JobTitleMapping_JobTitleMappingTempId = c.Int(),
                    })
                .PrimaryKey(t => t.JobTitleMappingMianId);

            CreateTable(
                "HRM.JobTitleMappingMain",
                c => new
                    {
                        JobTitleMappingMianId = c.Int(nullable: false, identity: true),
                        MainFunction = c.String(),
                        JobTitleMappingId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobTitleMappingMianId);

            AddColumn("HRM.JobTitleMappingTemp", "JobTitleMappingTempId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("EXCEED.JobTitleMappingDetailTemp", "JobTitleMappingId", "HRM.JobTitleMappingTemp");
            DropForeignKey("Archive.EmployeeLetterSender", "BookNo", "Archive.EmployeeIncommingLetter");
            DropIndex("EXCEED.JobTitleMappingDetailTemp", new[] { "JobTitleMappingId" });
            DropIndex("Archive.EmployeeLetterSender", new[] { "BookNo" });
            DropPrimaryKey("HRM.JobTitleMappingTemp");
            AlterColumn("Archive.LetterSender", "Receiver", c => c.String(nullable: false));
            AlterColumn("EXCEED.JobTitleMappingDetailTemp", "JobTitleMappingId", c => c.Int());
            DropColumn("Archive.IncomingLetter", "EnableDisbale");
            DropColumn("HRM.JobTitleMappingTemp", "JobTitleMappingId");
            DropTable("Archive.EmployeeLetterSender");
            DropTable("Archive.EmployeeIncommingLetter");
            AddPrimaryKey("HRM.JobTitleMappingTemp", "JobTitleMappingTempId");
            RenameColumn(table: "EXCEED.JobTitleMappingDetailTemp", name: "JobTitleMappingId", newName: "JobTitleMapping_JobTitleMappingTempId");
            AddColumn("EXCEED.JobTitleMappingDetailTemp", "JobTitleMappingId", c => c.Int(nullable: false));
            CreateIndex("HRM.JobTitleMappingMianTemp", "JobTitleMapping_JobTitleMappingTempId");
            CreateIndex("HRM.JobTitleMappingMain", "JobTitleMappingId");
            CreateIndex("EXCEED.JobTitleMappingDetailTemp", "JobTitleMapping_JobTitleMappingTempId");
            AddForeignKey("HRM.JobTitleMappingDetailTemp", "JobTitleMapping_JobTitleMappingTempId", "HRM.JobTitleMappingTemp", "JobTitleMappingTempId");
            AddForeignKey("HRM.JobTitleMappingMianTemp", "JobTitleMapping_JobTitleMappingTempId", "HRM.JobTitleMappingTemp", "JobTitleMappingTempId");
            AddForeignKey("HRM.JobTitleMappingMain", "JobTitleMappingId", "HRM.JobTitleMapping", "JobTitleMappingId", cascadeDelete: true);
            MoveTable(name: "EXCEED.JobTitleMappingDetailTemp", newSchema: "HRM");
        }
    }
}

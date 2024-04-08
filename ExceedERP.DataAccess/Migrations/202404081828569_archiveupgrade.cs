namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class archiveupgrade : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "EXCEED.EmployeeFolderAttachment", newSchema: "Archive");
            DropForeignKey("Archive.EmployeeLetterSender", "BookNo", "Archive.EmployeeIncommingLetter");
            DropForeignKey("Archive.IncomingLetter", "InstitutionTypeId", "Archive.InstitutionType");
            DropForeignKey("Archive.IncomingLetter", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("Archive.LetterSender", "BookNo", "Archive.IncomingLetter");
            DropForeignKey("Archive.RecivedLetters", "BookNo", "Archive.IncomingLetter");
            DropIndex("Archive.EmployeeLetterSender", new[] { "BookNo" });
            DropIndex("Archive.LetterSender", new[] { "BookNo" });
            DropIndex("Archive.IncomingLetter", new[] { "InstitutionTypeId" });
            DropIndex("Archive.IncomingLetter", new[] { "OrgStructureId" });
            DropIndex("Archive.RecivedLetters", new[] { "BookNo" });
            DropPrimaryKey("Archive.EmployeeIncommingLetter");
            CreateTable(
                "Archive.Answer_DepartmentToStaff",
                c => new
                    {
                        Answer_DepartmentToStaffId = c.Int(nullable: false, identity: true),
                        OutGoing_DepartmentToStaffId = c.Int(nullable: false),
                        Answer = c.String(),
                        EmployeeId = c.Int(nullable: false),
                        IsAnswered = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateEt = c.String(),
                        Time = c.String(),
                    })
                .PrimaryKey(t => t.Answer_DepartmentToStaffId);
            
            CreateTable(
                "Archive.ArchiveAttachement",
                c => new
                    {
                        ArchiveAttachementId = c.Int(nullable: false, identity: true),
                        InstitutionIncomingLetterId = c.Int(nullable: false),
                        ArchiveCategory = c.Int(nullable: false),
                        LetterType = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ArchiveAttachementId);
            
            CreateTable(
                "Archive.AttachedLettersForStaff",
                c => new
                    {
                        AttachedLettersForStaffId = c.Int(nullable: false, identity: true),
                        LetterForMultipleStaffId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.AttachedLettersForStaffId);
            
            CreateTable(
                "Archive.Attachment_Answer_DepartmentToDepartment",
                c => new
                    {
                        Attachment_Answer_DepartmentToDepartmentId = c.Int(nullable: false, identity: true),
                        OutGoing_DepartmentToDepartment_AnswerId = c.Int(nullable: false),
                        MasterId = c.String(),
                        FilePath = c.String(),
                        ShortPath = c.String(),
                        FileName = c.String(),
                        FileExtension = c.String(),
                    })
                .PrimaryKey(t => t.Attachment_Answer_DepartmentToDepartmentId);
            
            CreateTable(
                "Archive.Attachment_Answer_DepartmentToStaff",
                c => new
                    {
                        Attachment_Answer_DepartmentToStaffId = c.Int(nullable: false, identity: true),
                        Answer_DepartmentToStaffId = c.Int(nullable: false),
                        FilePath = c.String(),
                        ShortPath = c.String(),
                        FileName = c.String(),
                        FileExtension = c.String(),
                    })
                .PrimaryKey(t => t.Attachment_Answer_DepartmentToStaffId);
            
            CreateTable(
                "Archive.Attachment_Answer_InstitutionIncoming",
                c => new
                    {
                        Attachment_Answer_InstitutionIncomingId = c.Int(nullable: false, identity: true),
                        InstitutionIncomingAnswerId = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                    })
                .PrimaryKey(t => t.Attachment_Answer_InstitutionIncomingId);
            
            CreateTable(
                "Archive.Attachment_Answer_LetterSentForEmployee",
                c => new
                    {
                        Attachment_Answer_LetterSentForEmployeeId = c.Int(nullable: false, identity: true),
                        EmployeeIncomingAnswerId = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                    })
                .PrimaryKey(t => t.Attachment_Answer_LetterSentForEmployeeId);
            
            CreateTable(
                "Archive.Attachment_DepartmentToDepartment",
                c => new
                    {
                        Attachment_DepartmentToDepartmentId = c.Int(nullable: false, identity: true),
                        OutGoing_DepartmentToDepartmentId = c.Int(nullable: false),
                        FilePath = c.String(),
                        ShortPath = c.String(),
                        FileName = c.String(),
                        FileExtension = c.String(),
                    })
                .PrimaryKey(t => t.Attachment_DepartmentToDepartmentId);
            
            CreateTable(
                "Archive.Attachment_DepartmentToStaff",
                c => new
                    {
                        Attachment_DepartmentToStaffId = c.Int(nullable: false, identity: true),
                        OutGoing_DepartmentToStaffId = c.Int(nullable: false),
                        FilePath = c.String(),
                        ShortPath = c.String(),
                        FileName = c.String(),
                        FileExtension = c.String(),
                    })
                .PrimaryKey(t => t.Attachment_DepartmentToStaffId);
            
            CreateTable(
                "Archive.CompanyDocument",
                c => new
                    {
                        CompanyDocumentId = c.Int(nullable: false, identity: true),
                        CompanyDocumentTypeId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.CompanyDocumentId);
            
            CreateTable(
                "Archive.CompanyDocumentType",
                c => new
                    {
                        CompanyDocumentTypeId = c.Int(nullable: false, identity: true),
                        DocumentType = c.String(),
                    })
                .PrimaryKey(t => t.CompanyDocumentTypeId);
            
            CreateTable(
                "Archive.DepartmentFolder",
                c => new
                    {
                        DepartmentFolderId = c.Int(nullable: false, identity: true),
                        Owner_OrgStructureId = c.Int(nullable: false),
                        OrgStructureId = c.Int(),
                        Department_SubjectTypeId = c.Int(),
                        Room = c.String(),
                        Shelve = c.String(),
                        Row = c.String(),
                        Folder = c.String(),
                        Volume = c.String(),
                        FileNo = c.String(),
                    })
                .PrimaryKey(t => t.DepartmentFolderId);
            
            CreateTable(
                "Archive.DepartmentOrSubjectTypeSelectionForFolderCreation",
                c => new
                    {
                        DepartmentOrSubjectTypeSelectionForFolderCreationId = c.Int(nullable: false, identity: true),
                        Owner_OrgStructureId = c.Int(nullable: false),
                        DepartmentOrSubjectType = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.DepartmentOrSubjectTypeSelectionForFolderCreationId);
            
            CreateTable(
                "Archive.EmployeeIncomingAnswer",
                c => new
                    {
                        EmployeeIncomingAnswerId = c.Int(nullable: false, identity: true),
                        EmployeeIncomingLetterId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        Answer = c.String(),
                        IsAnswered = c.Boolean(nullable: false),
                        Date = c.String(),
                        Time = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeIncomingAnswerId);
            
            CreateTable(
                "Archive.EmployeeOutgoingAnswer",
                c => new
                    {
                        EmployeeOutgoingAnswerId = c.Int(nullable: false, identity: true),
                        EmployeeOutgoingId = c.Int(nullable: false),
                        Answer = c.String(),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        IsAnswered = c.Boolean(nullable: false),
                        Date = c.String(),
                        Time = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeOutgoingAnswerId);
            
            CreateTable(
                "Archive.EmployeeOutgoingLetter",
                c => new
                    {
                        EmployeeOutgoingLetterId = c.Int(nullable: false, identity: true),
                        LetterReferenceNo = c.String(),
                        SenderOrgId = c.Int(nullable: false),
                        ReceiverEmpId = c.Int(nullable: false),
                        Subject = c.String(),
                        RoomNo = c.String(),
                        Shelve = c.String(),
                        Row = c.String(),
                        FolderNo = c.String(),
                        Volume = c.String(),
                        FileNo = c.String(),
                        OutgoingLetterDate = c.DateTime(nullable: false),
                        OutgoingLetterDateAm = c.String(),
                        Time = c.String(),
                        IsTransfered = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeOutgoingLetterId);
            
            CreateTable(
                "Archive.InstitutionIncomingAnswer",
                c => new
                    {
                        InstitutionIncomingAnswerId = c.Int(nullable: false, identity: true),
                        InstitutionIncomingId = c.Int(nullable: false),
                        LetterSenderId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        Answer = c.String(),
                        IsAnswered = c.Boolean(nullable: false),
                        Date = c.String(),
                        Time = c.String(),
                    })
                .PrimaryKey(t => t.InstitutionIncomingAnswerId);
            
            CreateTable(
                "Archive.InstitutionIncomingLetter",
                c => new
                    {
                        InstitutionIncomingLetterId = c.Int(nullable: false, identity: true),
                        BookNo = c.String(),
                        LetterDate = c.DateTime(nullable: false),
                        LetterDateAmh = c.String(),
                        LetterReferenceNo = c.String(),
                        RecivedDate = c.DateTime(nullable: false),
                        RecivedDateAmh = c.String(),
                        SenderName = c.String(),
                        InstitutionTypeId = c.Int(nullable: false),
                        Subject = c.String(),
                        Shelve = c.String(),
                        Row = c.String(),
                        FolderNo = c.String(),
                        FileNo = c.String(),
                        Remark = c.String(),
                        VolumeNo = c.String(),
                        RecivedTime = c.String(),
                        SubjectTypeId = c.Int(nullable: false),
                        IsTransfered = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InstitutionIncomingLetterId);
            
            CreateTable(
                "Archive.InstitutionOutgoingLetter",
                c => new
                    {
                        InstitutionOutgoingLetterId = c.Int(nullable: false, identity: true),
                        BookNo = c.String(),
                        LetterReferenceNo = c.String(),
                        OwnerName = c.String(),
                        InstitutionTypeId = c.Int(nullable: false),
                        Subject = c.String(),
                        Shelve = c.String(),
                        Row = c.String(),
                        FolderNo = c.String(),
                        FileNo = c.String(),
                        NoOfAttachment = c.Int(nullable: false),
                        SerialNo = c.String(),
                        RefernceType = c.String(),
                        Remark = c.String(),
                        OutgoingLetterDate = c.DateTime(nullable: false),
                        OutgoingLetterDateAm = c.String(),
                        OutgoingTime = c.String(),
                        SubjectTypeId = c.Int(nullable: false),
                        RefersTo = c.Int(),
                    })
                .PrimaryKey(t => t.InstitutionOutgoingLetterId);
            
            CreateTable(
                "Archive.LetterForMultipleStaff",
                c => new
                    {
                        LetterForMultipleStaffId = c.Int(nullable: false, identity: true),
                        SendingDate = c.DateTime(nullable: false),
                        SendingDateAm = c.String(),
                        Title = c.String(),
                        IsTransfered = c.Boolean(nullable: false),
                        IsForAllEmployee = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.LetterForMultipleStaffId);
            
            CreateTable(
                "Archive.LetterReceiver",
                c => new
                    {
                        LetterReceiverId = c.Int(nullable: false, identity: true),
                        LetterForMultipleStaffId = c.Int(nullable: false),
                        EmployeeId = c.Int(),
                        IsForAllEmployee = c.Boolean(nullable: false),
                        IsTransfered = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.LetterReceiverId);
            
            CreateTable(
                "Archive.OutGoing_DepartmentToDepartment_Answer",
                c => new
                    {
                        OutGoing_DepartmentToDepartment_AnswerId = c.Int(nullable: false, identity: true),
                        OutGoing_DepartmentToDepartmentId = c.Int(nullable: false),
                        Answer = c.String(),
                        OrgStructureId = c.Int(nullable: false),
                        IsAnswered = c.Boolean(nullable: false),
                        DateAm = c.String(),
                        Time = c.String(),
                    })
                .PrimaryKey(t => t.OutGoing_DepartmentToDepartment_AnswerId);
            
            CreateTable(
                "Archive.OutGoing_DepartmentToDepartment",
                c => new
                    {
                        OutGoing_DepartmentToDepartmentId = c.Int(nullable: false, identity: true),
                        OrgstructureId = c.Int(nullable: false),
                        Message = c.String(),
                        IsForAll = c.Boolean(nullable: false),
                        IsSent = c.Boolean(nullable: false),
                        Date = c.DateTime(),
                        DateEt = c.String(),
                        Time = c.String(),
                    })
                .PrimaryKey(t => t.OutGoing_DepartmentToDepartmentId);
            
            CreateTable(
                "Archive.OutGoing_DepartmentToStaff",
                c => new
                    {
                        OutGoing_DepartmentToStaffId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(nullable: false),
                        Message = c.String(),
                        IsForAll = c.Boolean(nullable: false),
                        IsSent = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateEt = c.String(),
                        Time = c.String(),
                    })
                .PrimaryKey(t => t.OutGoing_DepartmentToStaffId);
            
            CreateTable(
                "Archive.Receiver_DepartmentToDepartment",
                c => new
                    {
                        Receiver_DepartmentToDepartmentId = c.Int(nullable: false, identity: true),
                        OutGoing_DepartmentToDepartmentId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        Date = c.DateTime(),
                        DateEt = c.String(),
                        Time = c.String(),
                        IsSent = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Receiver_DepartmentToDepartmentId);
            
            CreateTable(
                "Archive.Receiver_DepartmentToStaff",
                c => new
                    {
                        Receiver_DepartmentToStaffId = c.Int(nullable: false, identity: true),
                        OutGoing_DepartmentToStaffId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        Date = c.DateTime(),
                        DateEt = c.String(),
                        Time = c.String(),
                        IsSent = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Receiver_DepartmentToStaffId);
            
            CreateTable(
                "Archive.SubjectTypeForDepartmentLevel",
                c => new
                    {
                        SubjectTypeForDepartmentLevelId = c.Int(nullable: false, identity: true),
                        Owner_OrgStructureId = c.Int(nullable: false),
                        Name = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.SubjectTypeForDepartmentLevelId);
            
            AddColumn("Archive.EmployeeIncommingLetter", "EmployeeIncomingLetterId", c => c.Int(nullable: false, identity: true));
            AddColumn("Archive.EmployeeIncommingLetter", "Sender", c => c.Int(nullable: false));
            AddColumn("Archive.EmployeeIncommingLetter", "RoomNo", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "IsTransfered", c => c.Boolean(nullable: false));
            AddColumn("Archive.EmployeeLetterSender", "EmployeeIncomingLetterId", c => c.Int(nullable: false));
            AddColumn("Archive.EmployeeLetterSender", "Time", c => c.String());
            AddColumn("Archive.EmployeeLetterSender", "IsTransfered", c => c.Boolean(nullable: false));
            AddColumn("Archive.InstitutionArchiveNo", "Volume", c => c.String());
            AddColumn("Archive.InstitutionArchiveNo", "SubjectTypeId", c => c.Int(nullable: false));
            AddColumn("Archive.LetterSender", "Time", c => c.String());
            AddColumn("Archive.LetterSender", "InstitutionIncomingLetterId", c => c.Int(nullable: false));
            AddColumn("Archive.LetterSender", "IsTransfered", c => c.Boolean(nullable: false));
            AddColumn("Archive.LetterSender", "ShortPath", c => c.String());
            AddColumn("Archive.LetterSender", "FileExtension", c => c.String());
            AddColumn("Archive.PersonalArchiveNo", "Volume", c => c.String());
            AddColumn("Archive.SubjectType", "SubjectCode", c => c.String());
            AlterColumn("Archive.EmployeeIncommingLetter", "BookNo", c => c.String());
            AlterColumn("Archive.EmployeeIncommingLetter", "LetterReferenceNo", c => c.String());
            AlterColumn("Archive.EmployeeIncommingLetter", "Subject", c => c.String());
            AlterColumn("Archive.EmployeeLetterSender", "Receiver", c => c.Int(nullable: false));
            AlterColumn("Archive.EmployeeLetterSender", "Description", c => c.String());
            AlterColumn("Archive.InstitutionArchiveNo", "FileNo", c => c.String());
            AlterColumn("Archive.InstitutionArchiveNo", "Shelve", c => c.String());
            AlterColumn("Archive.InstitutionArchiveNo", "Row", c => c.String());
            AlterColumn("Archive.InstitutionArchiveNo", "Folder", c => c.String());
            AlterColumn("Archive.LetterSender", "Description", c => c.String());
            AlterColumn("Archive.PersonalArchiveNo", "Folder", c => c.String());
            AlterColumn("Archive.PersonalArchiveNo", "Shelve", c => c.String());
            AlterColumn("Archive.PersonalArchiveNo", "Row", c => c.String());
            AlterColumn("Archive.PersonalArchiveNo", "FileNo", c => c.String());
            AddPrimaryKey("Archive.EmployeeIncommingLetter", "EmployeeIncomingLetterId");
            DropColumn("Archive.EmployeeIncommingLetter", "OwnerName");
            DropColumn("Archive.EmployeeIncommingLetter", "NoOfAttachment");
            DropColumn("Archive.EmployeeIncommingLetter", "RefersTo");
            DropColumn("Archive.EmployeeIncommingLetter", "RefernceType");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingLetterDate");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingLetterDateAm");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingLetterReferenceNo");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingOwnerName");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingSubject");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingShelve");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingRow");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingFolderNo");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingFileNo");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingNoOfAttachment");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingSerialNo");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingRefernceType");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingRemark");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingVolumeNo");
            DropColumn("Archive.EmployeeIncommingLetter", "EnableDisbale");
            DropColumn("Archive.EmployeeIncommingLetter", "OutgoingTime");
            DropColumn("Archive.EmployeeLetterSender", "Sender");
            DropColumn("Archive.EmployeeLetterSender", "BookNo");
            DropColumn("Archive.EmployeeLetterSender", "OwnerName");
            DropColumn("Archive.EmployeeLetterSender", "FileName");
            DropColumn("Archive.EmployeeLetterSender", "File");
            DropColumn("Archive.EmployeeLetterSender", "FileType");
            DropColumn("Archive.EmployeeLetterSender", "SenderDepartment");
            DropColumn("Archive.EmployeeLetterSender", "TransferDescription");
            DropColumn("Archive.InstitutionArchiveNo", "InstitutionName");
            DropColumn("Archive.LetterSender", "Sender");
            DropColumn("Archive.LetterSender", "BookNo");
            DropColumn("Archive.LetterSender", "OwnerName");
            DropColumn("Archive.LetterSender", "FileName");
            DropColumn("Archive.LetterSender", "File");
            DropColumn("Archive.LetterSender", "FileType");
            DropColumn("Archive.LetterSender", "TransferDescription");
            DropTable("EXCEED.AttachmentArchive");
            DropTable("Archive.IncomingLetter");
            DropTable("Archive.OutgoingLetter");
            DropTable("Archive.RecivedLetters");
            DropTable("Archive.Zone");
        }
        
        public override void Down()
        {
            CreateTable(
                "Archive.Zone",
                c => new
                    {
                        ZoneId = c.Int(nullable: false, identity: true),
                        ZoneName = c.String(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ZoneId);
            
            CreateTable(
                "Archive.RecivedLetters",
                c => new
                    {
                        RecivedLetterId = c.Int(nullable: false, identity: true),
                        RecivedDate = c.DateTime(nullable: false),
                        InstnPerson = c.String(),
                        Receiver = c.Int(nullable: false),
                        Sender = c.String(),
                        Description = c.String(),
                        FileName = c.String(),
                        LetterStatus = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        BookNo = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.RecivedLetterId);
            
            CreateTable(
                "Archive.OutgoingLetter",
                c => new
                    {
                        NewOutgoingLetterLetterId = c.Int(nullable: false, identity: true),
                        BookNo = c.String(nullable: false),
                        LetterReferenceNo = c.String(nullable: false),
                        OwnerName = c.String(nullable: false),
                        InstitutionTypeId = c.Int(nullable: false),
                        Subject = c.String(nullable: false),
                        Shelve = c.String(nullable: false),
                        Row = c.String(nullable: false),
                        FolderNo = c.String(nullable: false),
                        FileNo = c.String(nullable: false),
                        NoOfAttachment = c.Int(nullable: false),
                        SerialNo = c.String(),
                        RefernceType = c.String(),
                        Remark = c.String(),
                        OutgoingLetterDate = c.DateTime(nullable: false),
                        OutgoingLetterDateAm = c.String(),
                        OutgoingTime = c.String(),
                        OutgoingInstitutionName = c.String(),
                        RefersTo = c.Int(),
                    })
                .PrimaryKey(t => t.NewOutgoingLetterLetterId);
            
            CreateTable(
                "Archive.IncomingLetter",
                c => new
                    {
                        BookNo = c.String(nullable: false, maxLength: 128),
                        LetterDate = c.DateTime(nullable: false),
                        LetterAmharic = c.String(),
                        LetterReferenceNo = c.String(nullable: false),
                        RecivedDate = c.DateTime(nullable: false),
                        RecivedDateAmharic = c.String(),
                        OwnerName = c.String(nullable: false),
                        InstitutionTypeId = c.Int(nullable: false),
                        Subject = c.String(nullable: false),
                        Shelve = c.String(nullable: false),
                        Row = c.String(nullable: false),
                        FolderNo = c.String(nullable: false),
                        FileNo = c.String(nullable: false),
                        NoOfAttachment = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        RefernceType = c.String(),
                        Remark = c.String(),
                        OutgoingLetterDate = c.DateTime(),
                        OutgoingLetterDateAm = c.String(),
                        OutgoingLetterReferenceNo = c.String(),
                        OutgoingOwnerName = c.String(),
                        OutgoingInstitutionTypeId = c.Int(nullable: false),
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
                        Status = c.Int(nullable: false),
                        OutgoingTime = c.String(),
                        RecivedTime = c.String(),
                        InstitutionName = c.String(),
                        OutgoingInstitutionName = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BookNo);
            
            CreateTable(
                "EXCEED.AttachmentArchive",
                c => new
                    {
                        AttachmentArchiveId = c.Int(nullable: false, identity: true),
                        ReferenceType = c.Int(nullable: false),
                        Reference = c.String(),
                        Description = c.String(),
                        URL = c.String(),
                        BinaryFile = c.Binary(),
                        FileType = c.String(),
                        Index = c.Double(nullable: false),
                        FileName = c.String(),
                        Remark = c.String(),
                        MINUTE = c.String(),
                    })
                .PrimaryKey(t => t.AttachmentArchiveId);
            
            AddColumn("Archive.LetterSender", "TransferDescription", c => c.String());
            AddColumn("Archive.LetterSender", "FileType", c => c.String());
            AddColumn("Archive.LetterSender", "File", c => c.Binary());
            AddColumn("Archive.LetterSender", "FileName", c => c.String());
            AddColumn("Archive.LetterSender", "OwnerName", c => c.String());
            AddColumn("Archive.LetterSender", "BookNo", c => c.String(maxLength: 128));
            AddColumn("Archive.LetterSender", "Sender", c => c.String());
            AddColumn("Archive.InstitutionArchiveNo", "InstitutionName", c => c.String(nullable: false));
            AddColumn("Archive.EmployeeLetterSender", "TransferDescription", c => c.String());
            AddColumn("Archive.EmployeeLetterSender", "SenderDepartment", c => c.Int(nullable: false));
            AddColumn("Archive.EmployeeLetterSender", "FileType", c => c.String());
            AddColumn("Archive.EmployeeLetterSender", "File", c => c.Binary());
            AddColumn("Archive.EmployeeLetterSender", "FileName", c => c.String());
            AddColumn("Archive.EmployeeLetterSender", "OwnerName", c => c.String());
            AddColumn("Archive.EmployeeLetterSender", "BookNo", c => c.String(maxLength: 128));
            AddColumn("Archive.EmployeeLetterSender", "Sender", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingTime", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "EnableDisbale", c => c.Boolean(nullable: false));
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingVolumeNo", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingRemark", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingRefernceType", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingSerialNo", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingNoOfAttachment", c => c.Int());
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingFileNo", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingFolderNo", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingRow", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingShelve", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingSubject", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingOwnerName", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingLetterReferenceNo", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingLetterDateAm", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "OutgoingLetterDate", c => c.DateTime());
            AddColumn("Archive.EmployeeIncommingLetter", "RefernceType", c => c.String());
            AddColumn("Archive.EmployeeIncommingLetter", "RefersTo", c => c.Int(nullable: false));
            AddColumn("Archive.EmployeeIncommingLetter", "NoOfAttachment", c => c.Int(nullable: false));
            AddColumn("Archive.EmployeeIncommingLetter", "OwnerName", c => c.String(nullable: false));
            DropPrimaryKey("Archive.EmployeeIncommingLetter");
            AlterColumn("Archive.PersonalArchiveNo", "FileNo", c => c.String(nullable: false));
            AlterColumn("Archive.PersonalArchiveNo", "Row", c => c.String(nullable: false));
            AlterColumn("Archive.PersonalArchiveNo", "Shelve", c => c.String(nullable: false));
            AlterColumn("Archive.PersonalArchiveNo", "Folder", c => c.String(nullable: false));
            AlterColumn("Archive.LetterSender", "Description", c => c.String(nullable: false));
            AlterColumn("Archive.InstitutionArchiveNo", "Folder", c => c.String(nullable: false));
            AlterColumn("Archive.InstitutionArchiveNo", "Row", c => c.String(nullable: false));
            AlterColumn("Archive.InstitutionArchiveNo", "Shelve", c => c.String(nullable: false));
            AlterColumn("Archive.InstitutionArchiveNo", "FileNo", c => c.String(nullable: false));
            AlterColumn("Archive.EmployeeLetterSender", "Description", c => c.String(nullable: false));
            AlterColumn("Archive.EmployeeLetterSender", "Receiver", c => c.String(nullable: false));
            AlterColumn("Archive.EmployeeIncommingLetter", "Subject", c => c.String(nullable: false));
            AlterColumn("Archive.EmployeeIncommingLetter", "LetterReferenceNo", c => c.String(nullable: false));
            AlterColumn("Archive.EmployeeIncommingLetter", "BookNo", c => c.String(nullable: false, maxLength: 128));
            DropColumn("Archive.SubjectType", "SubjectCode");
            DropColumn("Archive.PersonalArchiveNo", "Volume");
            DropColumn("Archive.LetterSender", "FileExtension");
            DropColumn("Archive.LetterSender", "ShortPath");
            DropColumn("Archive.LetterSender", "IsTransfered");
            DropColumn("Archive.LetterSender", "InstitutionIncomingLetterId");
            DropColumn("Archive.LetterSender", "Time");
            DropColumn("Archive.InstitutionArchiveNo", "SubjectTypeId");
            DropColumn("Archive.InstitutionArchiveNo", "Volume");
            DropColumn("Archive.EmployeeLetterSender", "IsTransfered");
            DropColumn("Archive.EmployeeLetterSender", "Time");
            DropColumn("Archive.EmployeeLetterSender", "EmployeeIncomingLetterId");
            DropColumn("Archive.EmployeeIncommingLetter", "IsTransfered");
            DropColumn("Archive.EmployeeIncommingLetter", "RoomNo");
            DropColumn("Archive.EmployeeIncommingLetter", "Sender");
            DropColumn("Archive.EmployeeIncommingLetter", "EmployeeIncomingLetterId");
            DropTable("Archive.SubjectTypeForDepartmentLevel");
            DropTable("Archive.Receiver_DepartmentToStaff");
            DropTable("Archive.Receiver_DepartmentToDepartment");
            DropTable("Archive.OutGoing_DepartmentToStaff");
            DropTable("Archive.OutGoing_DepartmentToDepartment");
            DropTable("Archive.OutGoing_DepartmentToDepartment_Answer");
            DropTable("Archive.LetterReceiver");
            DropTable("Archive.LetterForMultipleStaff");
            DropTable("Archive.InstitutionOutgoingLetter");
            DropTable("Archive.InstitutionIncomingLetter");
            DropTable("Archive.InstitutionIncomingAnswer");
            DropTable("Archive.EmployeeOutgoingLetter");
            DropTable("Archive.EmployeeOutgoingAnswer");
            DropTable("Archive.EmployeeIncomingAnswer");
            DropTable("Archive.DepartmentOrSubjectTypeSelectionForFolderCreation");
            DropTable("Archive.DepartmentFolder");
            DropTable("Archive.CompanyDocumentType");
            DropTable("Archive.CompanyDocument");
            DropTable("Archive.Attachment_DepartmentToStaff");
            DropTable("Archive.Attachment_DepartmentToDepartment");
            DropTable("Archive.Attachment_Answer_LetterSentForEmployee");
            DropTable("Archive.Attachment_Answer_InstitutionIncoming");
            DropTable("Archive.Attachment_Answer_DepartmentToStaff");
            DropTable("Archive.Attachment_Answer_DepartmentToDepartment");
            DropTable("Archive.AttachedLettersForStaff");
            DropTable("Archive.ArchiveAttachement");
            DropTable("Archive.Answer_DepartmentToStaff");
            AddPrimaryKey("Archive.EmployeeIncommingLetter", "BookNo");
            CreateIndex("Archive.RecivedLetters", "BookNo");
            CreateIndex("Archive.IncomingLetter", "OrgStructureId");
            CreateIndex("Archive.IncomingLetter", "InstitutionTypeId");
            CreateIndex("Archive.LetterSender", "BookNo");
            CreateIndex("Archive.EmployeeLetterSender", "BookNo");
            AddForeignKey("Archive.RecivedLetters", "BookNo", "Archive.IncomingLetter", "BookNo");
            AddForeignKey("Archive.LetterSender", "BookNo", "Archive.IncomingLetter", "BookNo");
            AddForeignKey("Archive.IncomingLetter", "OrgStructureId", "HRM.MainCompanyStructure", "OrgStructureId", cascadeDelete: true);
            AddForeignKey("Archive.IncomingLetter", "InstitutionTypeId", "Archive.InstitutionType", "InstitutionTypeId", cascadeDelete: true);
            AddForeignKey("Archive.EmployeeLetterSender", "BookNo", "Archive.EmployeeIncommingLetter", "BookNo");
            MoveTable(name: "Archive.EmployeeFolderAttachment", newSchema: "EXCEED");
        }
    }
}

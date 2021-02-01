namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cmshelpdesk : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "InfoDesk.AssignTicket_HelpDesk",
                c => new
                    {
                        AssignTicket_HelpDeskId = c.Int(nullable: false, identity: true),
                        CustomerTicketId = c.Int(nullable: false),
                        AssinedTo = c.Int(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        AssignedDate = c.DateTime(nullable: false),
                        ClosedDate = c.DateTime(),
                        Age = c.String(),
                        Remarked = c.String(),
                        IsAssigned = c.Boolean(nullable: false),
                        IsSendForClosing = c.Boolean(nullable: false),
                        IsSolved = c.Boolean(nullable: false),
                        Solution = c.String(),
                        Role = c.String(),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AssignTicket_HelpDeskId);
            
            CreateTable(
                "CMS.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        Region = c.Int(nullable: false),
                        Name = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CityId);
            
            CreateTable(
                "CMS.CMSCourses",
                c => new
                    {
                        CMSCourseId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(),
                        CreditHours = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CMSCourseId);
            
            CreateTable(
                "CMS.CMSPrograms",
                c => new
                    {
                        CMSProgramId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CMSProgramId);
            
            CreateTable(
                "CMS.CmsTrainees",
                c => new
                    {
                        CmsTraineeId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        RegistrationDate = c.DateTime(nullable: false),
                        RegistrationNumber = c.String(),
                        CMSProgramId = c.Int(nullable: false),
                        CMSCourseId = c.Int(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        MiddleName = c.String(),
                        FullName = c.String(),
                        Nationality = c.String(),
                        CompanyName = c.String(),
                        Region = c.Int(nullable: false),
                        Address = c.String(),
                        CityId = c.Int(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        PhoneNumber = c.String(),
                        MobilePhoneNumber = c.String(),
                        Email = c.String(),
                        PreferedDate = c.DateTime(),
                        TrainingVenue = c.String(),
                        ProfilePicFileName = c.String(),
                        Status = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CmsTraineeId);
            
            CreateTable(
                "CMS.CompanyEvaluations",
                c => new
                    {
                        CompanyEvaluationId = c.Int(nullable: false, identity: true),
                        CmsTraineeId = c.Int(nullable: false),
                        ISOChecklistId = c.Int(nullable: false),
                        Value = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CompanyEvaluationId)
                .ForeignKey("CMS.CmsTrainees", t => t.CmsTraineeId, cascadeDelete: true)
                .Index(t => t.CmsTraineeId);
            
            CreateTable(
                "InfoDesk.CustomerTicket_HelpDesk",
                c => new
                    {
                        CustomerTicket_HelpDeskId = c.Int(nullable: false, identity: true),
                        SpecificTicketTypeId = c.Int(),
                        Description = c.String(),
                        AssignedTo = c.String(),
                        Ticket = c.String(),
                        PriorityColor = c.String(),
                        Status = c.Int(nullable: false),
                        Priority = c.String(),
                        TicketGroup = c.String(),
                        DueDate = c.DateTime(),
                        ClosedDate = c.DateTime(),
                        SolutionGiven = c.String(),
                        EmployeeId = c.Int(),
                        Remark = c.String(),
                        AgeOfTicket = c.String(),
                        IsClosed = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CustomerTicket_HelpDeskId);
            
            CreateTable(
                "CMS.ISOChecklist",
                c => new
                    {
                        ISOChecklistId = c.Int(nullable: false, identity: true),
                        ISOTypeId = c.Int(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        ValueType = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ISOChecklistId)
                .ForeignKey("CMS.ISOTypes", t => t.ISOTypeId, cascadeDelete: true)
                .Index(t => t.ISOTypeId);
            
            CreateTable(
                "CMS.ISOTypes",
                c => new
                    {
                        ISOTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsDefault = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ISOTypeId);
            
            CreateTable(
                "InfoDesk.PriorityType_HelpDesk",
                c => new
                    {
                        PriorityType_HelpDeskId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Color = c.String(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PriorityType_HelpDeskId);
            
            CreateTable(
                "CMS.RegistrationNumberPrefixes",
                c => new
                    {
                        RegistrationNumberPrefixId = c.Int(nullable: false, identity: true),
                        PrefixValue = c.String(nullable: false),
                        Description = c.String(),
                        IsDefault = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RegistrationNumberPrefixId);
            
            CreateTable(
                "InfoDesk.StatusType_HelpDesk",
                c => new
                    {
                        StatusType_HelpDeskId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Color = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StatusType_HelpDeskId);
            
            CreateTable(
                "InfoDesk.SupportGroup_HelpDesk",
                c => new
                    {
                        SupportGroup_HelpDeskId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SupportGroup_HelpDeskId);
            
            CreateTable(
                "EXCEED.SupportGroupDetail_HelpDesk",
                c => new
                    {
                        SupportGroupDetail_HelpDeskId = c.Int(nullable: false, identity: true),
                        SupportGroupId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        SupportGroup_SupportGroup_HelpDeskId = c.Int(),
                    })
                .PrimaryKey(t => t.SupportGroupDetail_HelpDeskId)
                .ForeignKey("InfoDesk.SupportGroup_HelpDesk", t => t.SupportGroup_SupportGroup_HelpDeskId)
                .Index(t => t.SupportGroup_SupportGroup_HelpDeskId);
            
            CreateTable(
                "InfoDesk.TicketActivityDefinition_HelpDesk",
                c => new
                    {
                        TicketActivityDefinition_HelpDeskId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        TicketActivityStatus = c.Int(nullable: false),
                        Index = c.Int(nullable: false),
                        CanFinalize = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TicketActivityDefinition_HelpDeskId);
            
            CreateTable(
                "InfoDesk.TicketStatusLog_HelpDesk",
                c => new
                    {
                        TicketStatusLog_HelpDeskId = c.Int(nullable: false, identity: true),
                        TicketId = c.Int(nullable: false),
                        User = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TicketStatusLog_HelpDeskId);
            
            CreateTable(
                "InfoDesk.TicketType_HelpDesk",
                c => new
                    {
                        TicketType_HelpDeskId = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TicketType_HelpDeskId);
            
            CreateTable(
                "CMS.TraineeIdNumberComponents",
                c => new
                    {
                        TraineeIdNumberComponentId = c.Int(nullable: false, identity: true),
                        RegistrationNumberPrefixId = c.Int(nullable: false),
                        ISOTypeId = c.Int(nullable: false),
                        CmsTraineeId = c.Int(nullable: false),
                        PrefixValue = c.String(),
                        ISOType = c.String(),
                        SystemId = c.String(),
                        SequenceNumber = c.String(),
                        CombinedIdNumber = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TraineeIdNumberComponentId)
                .ForeignKey("CMS.CmsTrainees", t => t.CmsTraineeId, cascadeDelete: true)
                .Index(t => t.CmsTraineeId);
            
            CreateTable(
                "CMS.TraineePrograms",
                c => new
                    {
                        TraineeProgramId = c.Int(nullable: false, identity: true),
                        ProgramId = c.Int(nullable: false),
                        CmsTraineeId = c.Int(nullable: false),
                        TheoreticalExamResult = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PracticalExamResult = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TrainingStatus = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TraineeProgramId)
                .ForeignKey("CMS.CmsTrainees", t => t.CmsTraineeId, cascadeDelete: true)
                .Index(t => t.CmsTraineeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("CMS.TraineePrograms", "CmsTraineeId", "CMS.CmsTrainees");
            DropForeignKey("CMS.TraineeIdNumberComponents", "CmsTraineeId", "CMS.CmsTrainees");
            DropForeignKey("EXCEED.SupportGroupDetail_HelpDesk", "SupportGroup_SupportGroup_HelpDeskId", "InfoDesk.SupportGroup_HelpDesk");
            DropForeignKey("CMS.ISOChecklist", "ISOTypeId", "CMS.ISOTypes");
            DropForeignKey("CMS.CompanyEvaluations", "CmsTraineeId", "CMS.CmsTrainees");
            DropIndex("CMS.TraineePrograms", new[] { "CmsTraineeId" });
            DropIndex("CMS.TraineeIdNumberComponents", new[] { "CmsTraineeId" });
            DropIndex("EXCEED.SupportGroupDetail_HelpDesk", new[] { "SupportGroup_SupportGroup_HelpDeskId" });
            DropIndex("CMS.ISOChecklist", new[] { "ISOTypeId" });
            DropIndex("CMS.CompanyEvaluations", new[] { "CmsTraineeId" });
            DropTable("CMS.TraineePrograms");
            DropTable("CMS.TraineeIdNumberComponents");
            DropTable("InfoDesk.TicketType_HelpDesk");
            DropTable("InfoDesk.TicketStatusLog_HelpDesk");
            DropTable("InfoDesk.TicketActivityDefinition_HelpDesk");
            DropTable("EXCEED.SupportGroupDetail_HelpDesk");
            DropTable("InfoDesk.SupportGroup_HelpDesk");
            DropTable("InfoDesk.StatusType_HelpDesk");
            DropTable("CMS.RegistrationNumberPrefixes");
            DropTable("InfoDesk.PriorityType_HelpDesk");
            DropTable("CMS.ISOTypes");
            DropTable("CMS.ISOChecklist");
            DropTable("InfoDesk.CustomerTicket_HelpDesk");
            DropTable("CMS.CompanyEvaluations");
            DropTable("CMS.CmsTrainees");
            DropTable("CMS.CMSPrograms");
            DropTable("CMS.CMSCourses");
            DropTable("CMS.Cities");
            DropTable("InfoDesk.AssignTicket_HelpDesk");
        }
    }
}

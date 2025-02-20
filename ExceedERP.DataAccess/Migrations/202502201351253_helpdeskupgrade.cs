namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class helpdeskupgrade : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "InfoDesk.AssignedUsers",
                c => new
                    {
                        AssignedUserId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        UserName = c.String(),
                        Remark = c.String(),
                        SoftwareLicenseId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AssignedUserId)
                .ForeignKey("InfoDesk.SoftwareLicenses", t => t.SoftwareLicenseId, cascadeDelete: true)
                .Index(t => t.SoftwareLicenseId);
            
            CreateTable(
                "InfoDesk.SoftwareLicenses",
                c => new
                    {
                        SoftwareLicenseId = c.Int(nullable: false, identity: true),
                        LicenseId = c.String(),
                        SoftwareName = c.String(nullable: false),
                        Version = c.String(),
                        Vendor = c.String(),
                        LicenseType = c.Int(nullable: false),
                        LicenseKey = c.String(),
                        PurchaseDate = c.DateTime(nullable: false),
                        LicenseExpiryOrRenewalDate = c.DateTime(nullable: false),
                        NumberofLicenses = c.Int(nullable: false),
                        NumberofUsers = c.Int(nullable: false),
                        CostperLicense = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgStructureId = c.Int(nullable: false),
                        AssignedUsers = c.Int(nullable: false),
                        LicenseStatus = c.Int(nullable: false),
                        UsageLimitations = c.String(),
                        RenewalNotificationDate = c.DateTime(),
                        IsRenewalNotified = c.Boolean(nullable: false),
                        SupportContact = c.String(),
                        CommentsOrNotes = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SoftwareLicenseId);
            
            CreateTable(
                "InfoDesk.CustomerTicketAttachments",
                c => new
                    {
                        CustomerTicketAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        CustomerTicket_HelpDeskId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerTicketAttachmentId)
                .ForeignKey("InfoDesk.CustomerTicket_HelpDesk", t => t.CustomerTicket_HelpDeskId, cascadeDelete: true)
                .Index(t => t.CustomerTicket_HelpDeskId);
            
            CreateTable(
                "InfoDesk.InfoDeskKnowledgeBases",
                c => new
                    {
                        InfoDeskKnowledgeBaseId = c.Int(nullable: false, identity: true),
                        Problem = c.String(nullable: false),
                        Solution = c.String(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InfoDeskKnowledgeBaseId);
            
            CreateTable(
                "InfoDesk.KnowledgeBaseAttachments",
                c => new
                    {
                        KnowledgeBaseAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        InfoDeskKnowledgeBaseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.KnowledgeBaseAttachmentId)
                .ForeignKey("InfoDesk.InfoDeskKnowledgeBases", t => t.InfoDeskKnowledgeBaseId, cascadeDelete: true)
                .Index(t => t.InfoDeskKnowledgeBaseId);
            
            AddColumn("InfoDesk.AssignTicket_HelpDesk", "TicketPriority", c => c.Int(nullable: false));
            AddColumn("InfoDesk.AssignTicket_HelpDesk", "RootCause", c => c.String());
            AddColumn("InfoDesk.AssignTicket_HelpDesk", "PhoneNumber", c => c.String());
            AddColumn("InfoDesk.AssignTicket_HelpDesk", "Address", c => c.String());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "TicketType_HelpDeskId", c => c.Int(nullable: false));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "TicketPriority", c => c.Int(nullable: false));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "SupportGroup_HelpDeskId", c => c.Int(nullable: false));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "TicketStatus", c => c.Int(nullable: false));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "BranchId", c => c.Int());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "BusinessUnitId", c => c.Int());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "OrgStructureId", c => c.Int());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "Role", c => c.String());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "AssignedDate", c => c.DateTime());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "MaterialItemsId", c => c.Int());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "Quantity", c => c.Int());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "OverDue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "RootCuase", c => c.String());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "IsAssigned", c => c.Boolean(nullable: false));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "SendApprovalTime", c => c.DateTime());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "SendApprovalBy", c => c.String());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "IsSendForClosing", c => c.Boolean(nullable: false));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "IsMultipleAssign", c => c.Boolean(nullable: false));
            AddColumn("InfoDesk.SupportGroup_HelpDesk", "BranchId", c => c.Int(nullable: false));
            AddColumn("InfoDesk.SupportGroup_HelpDesk", "BusinessUnitId", c => c.Int());
            AddColumn("InfoDesk.TicketType_HelpDesk", "TicketCode", c => c.String());
            AddColumn("InfoDesk.TicketType_HelpDesk", "DueDays", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("InfoDesk.TicketType_HelpDesk", "TicketPriority", c => c.Int(nullable: false));
            AddColumn("InfoDesk.TicketType_HelpDesk", "TicketCategory", c => c.Int(nullable: false));
            AlterColumn("InfoDesk.AssignTicket_HelpDesk", "MaterialItemsId", c => c.Int());
            AlterColumn("InfoDesk.CustomerTicket_HelpDesk", "AssignedTo", c => c.Int());
            CreateIndex("InfoDesk.CustomerTicket_HelpDesk", "TicketType_HelpDeskId");
            CreateIndex("InfoDesk.CustomerTicket_HelpDesk", "SupportGroup_HelpDeskId");
            CreateIndex("InfoDesk.CustomerTicket_HelpDesk", "MaterialItemsId");
            CreateIndex("InfoDesk.CustomerTicket_HelpDesk", "EmployeeId");
            CreateIndex("EXCEED.MaterialsUsed", "MaterialItemsId");
            AddForeignKey("InfoDesk.CustomerTicket_HelpDesk", "MaterialItemsId", "EXCEED.MaterialItems", "MaterialItemsId");
            AddForeignKey("InfoDesk.CustomerTicket_HelpDesk", "EmployeeId", "HRM.Employee", "EmployeeId");
            AddForeignKey("InfoDesk.CustomerTicket_HelpDesk", "SupportGroup_HelpDeskId", "InfoDesk.SupportGroup_HelpDesk", "SupportGroup_HelpDeskId", cascadeDelete: true);
            AddForeignKey("InfoDesk.CustomerTicket_HelpDesk", "TicketType_HelpDeskId", "InfoDesk.TicketType_HelpDesk", "TicketType_HelpDeskId", cascadeDelete: true);
            AddForeignKey("EXCEED.MaterialsUsed", "MaterialItemsId", "EXCEED.MaterialItems", "MaterialItemsId", cascadeDelete: true);
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "SpecificTicketTypeId");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "Ticket");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "Status");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "Priority");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "SupportGroupId");
        }
        
        public override void Down()
        {
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "SupportGroupId", c => c.Int(nullable: false));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "Priority", c => c.String());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "Status", c => c.Int(nullable: false));
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "Ticket", c => c.String());
            AddColumn("InfoDesk.CustomerTicket_HelpDesk", "SpecificTicketTypeId", c => c.Int());
            DropForeignKey("EXCEED.MaterialsUsed", "MaterialItemsId", "EXCEED.MaterialItems");
            DropForeignKey("InfoDesk.KnowledgeBaseAttachments", "InfoDeskKnowledgeBaseId", "InfoDesk.InfoDeskKnowledgeBases");
            DropForeignKey("InfoDesk.CustomerTicketAttachments", "CustomerTicket_HelpDeskId", "InfoDesk.CustomerTicket_HelpDesk");
            DropForeignKey("InfoDesk.CustomerTicket_HelpDesk", "TicketType_HelpDeskId", "InfoDesk.TicketType_HelpDesk");
            DropForeignKey("InfoDesk.CustomerTicket_HelpDesk", "SupportGroup_HelpDeskId", "InfoDesk.SupportGroup_HelpDesk");
            DropForeignKey("InfoDesk.CustomerTicket_HelpDesk", "EmployeeId", "HRM.Employee");
            DropForeignKey("InfoDesk.CustomerTicket_HelpDesk", "MaterialItemsId", "EXCEED.MaterialItems");
            DropForeignKey("InfoDesk.AssignedUsers", "SoftwareLicenseId", "InfoDesk.SoftwareLicenses");
            DropIndex("EXCEED.MaterialsUsed", new[] { "MaterialItemsId" });
            DropIndex("InfoDesk.KnowledgeBaseAttachments", new[] { "InfoDeskKnowledgeBaseId" });
            DropIndex("InfoDesk.CustomerTicketAttachments", new[] { "CustomerTicket_HelpDeskId" });
            DropIndex("InfoDesk.CustomerTicket_HelpDesk", new[] { "EmployeeId" });
            DropIndex("InfoDesk.CustomerTicket_HelpDesk", new[] { "MaterialItemsId" });
            DropIndex("InfoDesk.CustomerTicket_HelpDesk", new[] { "SupportGroup_HelpDeskId" });
            DropIndex("InfoDesk.CustomerTicket_HelpDesk", new[] { "TicketType_HelpDeskId" });
            DropIndex("InfoDesk.AssignedUsers", new[] { "SoftwareLicenseId" });
            AlterColumn("InfoDesk.CustomerTicket_HelpDesk", "AssignedTo", c => c.String());
            AlterColumn("InfoDesk.AssignTicket_HelpDesk", "MaterialItemsId", c => c.Int(nullable: false));
            DropColumn("InfoDesk.TicketType_HelpDesk", "TicketCategory");
            DropColumn("InfoDesk.TicketType_HelpDesk", "TicketPriority");
            DropColumn("InfoDesk.TicketType_HelpDesk", "DueDays");
            DropColumn("InfoDesk.TicketType_HelpDesk", "TicketCode");
            DropColumn("InfoDesk.SupportGroup_HelpDesk", "BusinessUnitId");
            DropColumn("InfoDesk.SupportGroup_HelpDesk", "BranchId");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "IsMultipleAssign");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "IsSendForClosing");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "SendApprovalBy");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "SendApprovalTime");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "IsSendForApproval");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "IsAssigned");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "RootCuase");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "OverDue");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "Quantity");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "MaterialItemsId");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "AssignedDate");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "Role");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "OrgStructureId");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "BusinessUnitId");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "BranchId");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "TicketStatus");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "SupportGroup_HelpDeskId");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "TicketPriority");
            DropColumn("InfoDesk.CustomerTicket_HelpDesk", "TicketType_HelpDeskId");
            DropColumn("InfoDesk.AssignTicket_HelpDesk", "Address");
            DropColumn("InfoDesk.AssignTicket_HelpDesk", "PhoneNumber");
            DropColumn("InfoDesk.AssignTicket_HelpDesk", "RootCause");
            DropColumn("InfoDesk.AssignTicket_HelpDesk", "TicketPriority");
            DropTable("InfoDesk.KnowledgeBaseAttachments");
            DropTable("InfoDesk.InfoDeskKnowledgeBases");
            DropTable("InfoDesk.CustomerTicketAttachments");
            DropTable("InfoDesk.SoftwareLicenses");
            DropTable("InfoDesk.AssignedUsers");
        }
    }
}

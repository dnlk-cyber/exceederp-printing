using ExceedERP.Core.Domain.InfoDesk;
using ExceedERP.Core.Domain.InfoDesk.License;
using ExceedERP.Core.Domain.InfoDesk.Support;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.DataAccess.Context
{
    public class InfoDeskEntityConfiguration
    {
        public class SupportGroupEntityTypeConfiguration : EntityTypeConfiguration<SupportGroup_HelpDesk>
        {

            public SupportGroupEntityTypeConfiguration()
            {
                this.ToTable("SupportGroup_HelpDesk", "InfoDesk");
                this.HasKey<int>(t => t.SupportGroup_HelpDeskId);

            }
        }


        public class CustomerTicketEntityTypeConfiguration : EntityTypeConfiguration<CustomerTicket_HelpDesk>
        {

            public CustomerTicketEntityTypeConfiguration()
            {
                this.ToTable("CustomerTicket_HelpDesk", "InfoDesk");
                this.HasKey<int>(o => o.CustomerTicket_HelpDeskId);
            }
        }
        public class TicketActivityDefinitionEntityTypeConfiguration : EntityTypeConfiguration<TicketActivityDefinition_HelpDesk>
        {

            public TicketActivityDefinitionEntityTypeConfiguration()
            {
                this.ToTable("TicketActivityDefinition_HelpDesk", "InfoDesk");
                this.HasKey<int>(o => o.TicketActivityDefinition_HelpDeskId);
            }
        }

        public class TicketStatusLogEntityTypeConfiguration : EntityTypeConfiguration<TicketStatusLog_HelpDesk>
        {

            public TicketStatusLogEntityTypeConfiguration()
            {
                this.ToTable("TicketStatusLog_HelpDesk", "InfoDesk");
                this.HasKey<int>(t => t.TicketStatusLog_HelpDeskId);

            }
        }
        public class SupportGroupDetailEntityTypeConfiguration : EntityTypeConfiguration<SupportGroupDetail_HelpDesk>
        {

            public SupportGroupDetailEntityTypeConfiguration()
            {
                this.ToTable("SupportGroupDetail_HelpDesk", "InfoDesk");
                this.HasKey<int>(t => t.SupportGroupDetail_HelpDeskId);

            }
        }
        public class StatusTypeEntityTypeConfiguration : EntityTypeConfiguration<StatusType_HelpDesk>
        {

            public StatusTypeEntityTypeConfiguration()
            {
                this.ToTable("StatusType_HelpDesk", "InfoDesk");
                this.HasKey<int>(t => t.StatusType_HelpDeskId);

            }
        }
        public class PriorityTypeEntityTypeConfiguration : EntityTypeConfiguration<PriorityType_HelpDesk>
        {

            public PriorityTypeEntityTypeConfiguration()
            {
                this.ToTable("PriorityType_HelpDesk", "InfoDesk");
                this.HasKey<int>(t => t.PriorityType_HelpDeskId);

            }
        }
        public class AssignTicketEntityTypeConfiguration : EntityTypeConfiguration<AssignTicket_HelpDesk>
        {

            public AssignTicketEntityTypeConfiguration()
            {
                this.ToTable("AssignTicket_HelpDesk", "InfoDesk");
                this.HasKey<int>(t => t.AssignTicket_HelpDeskId);

            }
        }
       


        public class TicketTypeEntityTypeConfiguration : EntityTypeConfiguration<TicketType_HelpDesk>
        {

            public TicketTypeEntityTypeConfiguration()
            {
                this.ToTable("TicketType_HelpDesk", "InfoDesk");
                this.HasKey<int>(t => t.TicketType_HelpDeskId);

            }
        }    
        public class CustomerTicketAttachmentEntityTypeConfiguration : EntityTypeConfiguration<CustomerTicketAttachment>
        {

            public CustomerTicketAttachmentEntityTypeConfiguration()
            {
                this.ToTable("CustomerTicketAttachments", "InfoDesk");
                this.HasKey<int>(t => t.CustomerTicketAttachmentId);

            }
        }    
        public class InfoDeskKnowledgeBaseEntityTypeConfiguration : EntityTypeConfiguration<InfoDeskKnowledgeBase>
        {

            public InfoDeskKnowledgeBaseEntityTypeConfiguration()
            {
                this.ToTable("InfoDeskKnowledgeBases", "InfoDesk");
                this.HasKey<int>(t => t.InfoDeskKnowledgeBaseId);

            }
        }    
        public class KnowledgeBaseAttachmentEntityTypeConfiguration : EntityTypeConfiguration<KnowledgeBaseAttachment>
        {

            public KnowledgeBaseAttachmentEntityTypeConfiguration()
            {
                this.ToTable("KnowledgeBaseAttachments", "InfoDesk");
                this.HasKey<int>(t => t.KnowledgeBaseAttachmentId);

            }
        }
        public class SoftwareLicenseEntityTypeConfiguration : EntityTypeConfiguration<SoftwareLicense>
        {

            public SoftwareLicenseEntityTypeConfiguration()
            {
                this.ToTable("SoftwareLicenses", "InfoDesk");
                this.HasKey<int>(t => t.SoftwareLicenseId);

            }
        }

        public class AssignedUserEntityTypeConfiguration : EntityTypeConfiguration<AssignedUser>
        {

            public AssignedUserEntityTypeConfiguration()
            {
                this.ToTable("AssignedUsers", "InfoDesk");
                this.HasKey<int>(t => t.AssignedUserId);

            }
        }    


    }
}

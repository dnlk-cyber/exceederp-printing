using ExceedERP.Core.Domain.Archive;
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.Core.Domain.ArchiveManagement.DepartmentToDepartment;
using ExceedERP.Core.Domain.ArchiveManagement.DepartmentToStaff;
using ExceedERP.Core.Domain.ArchiveManagement.Employee;
using ExceedERP.Core.Domain.ArchiveManagement.Institution;
using ExceedERP.Core.Domain.ArchiveManagement.Setting;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.DataAccess.Context
{
   public class ArchiveEntityConfiguration
    {
    }

    public class EmployeeIncommingLetterEntityTypeConfiguration : EntityTypeConfiguration<EmployeeIncomingLetter>
    {
        public EmployeeIncommingLetterEntityTypeConfiguration()
        {
            this.ToTable("EmployeeIncommingLetter", "Archive");
            this.HasKey<int>(o => o.EmployeeIncomingLetterId);
        }
    }
    public class PersonalArchiveNoEntityTypeConfiguration : EntityTypeConfiguration<PersonalArchiveNo>
    {
        public PersonalArchiveNoEntityTypeConfiguration()
        {
            this.ToTable("PersonalArchiveNo", "Archive");
            this.HasKey<int>(o => o.PersonalArchiveNoId);
        }
    }
    public class InstitutionOutgoingLetterEntityTypeConfiguration : EntityTypeConfiguration<InstitutionOutgoingLetter>
    {

        public InstitutionOutgoingLetterEntityTypeConfiguration()
        {
            this.ToTable("InstitutionOutgoingLetter", "Archive");
            this.HasKey<int>(o => o.InstitutionOutgoingLetterId);
        }
    }
    public class InstitutionTypeEntityTypeConfiguration : EntityTypeConfiguration<InstitutionType>
    {

        public InstitutionTypeEntityTypeConfiguration()
        {
            this.ToTable("InstitutionType", "Archive");
            this.HasKey<int>(o => o.InstitutionTypeId);
        }
    }
    //public class ZoneEntityTypeConfiguration : EntityTypeConfiguration<Zone>
    //{

    //    public ZoneEntityTypeConfiguration()
    //    {
    //        this.ToTable("Zone", "Archive");
    //        this.HasKey<int>(o => o.ZoneId);
    //    }
    //}
    //public class PersonalArchiveNoEntityTypeConfiguration : EntityTypeConfiguration<PersonalArchiveNo>
    //{

    //    public PersonalArchiveNoEntityTypeConfiguration()
    //    {
    //        this.ToTable("PersonalArchiveNo", "Archive");
    //        this.HasKey<int>(o => o.PersonalArchiveNoId);
    //    }
    //}
    public class InstitutionArchiveNoEntityTypeConfiguration : EntityTypeConfiguration<InstitutionArchiveNo>
    {

        public InstitutionArchiveNoEntityTypeConfiguration()
        {
            this.ToTable("InstitutionArchiveNo", "Archive");
            this.HasKey<int>(o => o.InstiArchiveNoId);
        }
    }

  
    public class LetterStatusEntityTypeConfiguration : EntityTypeConfiguration<LetterStatus>
    {

        public LetterStatusEntityTypeConfiguration()
        {
            this.ToTable("LetterStatus", "Archive");
            this.HasKey<int>(o => o.LetterStatusId);
        }
    }
    public class LetterSenderEntityTypeConfiguration : EntityTypeConfiguration<LetterSender>
    {

        public LetterSenderEntityTypeConfiguration()
        {
            this.ToTable("LetterSender", "Archive");
            this.HasKey<int>(o => o.LetterSenderId);
        }
    }
    public class EmployeeLetterSenderEntityTypeConfiguration : EntityTypeConfiguration<EmployeeLetterSender>
    {

        public EmployeeLetterSenderEntityTypeConfiguration()
        {
            this.ToTable("EmployeeLetterSender", "Archive");
            this.HasKey<int>(o => o.LetterSenderId);
        }
    }
    //public class RecivedLettersEntityTypeConfiguration : EntityTypeConfiguration<RecivedLetters>
    //{
    //    public RecivedLettersEntityTypeConfiguration()
    //    {
    //        this.ToTable("RecivedLetters", "Archive");
    //        this.HasKey<int>(o => o.RecivedLetterId);
    //    }
    //}
    public class InstitutionIncomingEntityTypeConfiguration : EntityTypeConfiguration<InstitutionIncomingLetter>
    {
        public InstitutionIncomingEntityTypeConfiguration()
        {
            this.ToTable("InstitutionIncoming", "Archive");
            this.HasKey<string>(o => o.BookNo);
        }
    }

    public class SubjectTypeEntityTypeConfiguration : EntityTypeConfiguration<SubjectType>
    {
        public SubjectTypeEntityTypeConfiguration()
        {
            this.ToTable("SubjectType", "Archive");
            this.HasKey<int>(o => o.SubjectTypeId);
        }
    }

    public class InstitutionIncomingLetterEntityTypeConfiguration : EntityTypeConfiguration<Core.Domain.ArchiveManagement.InstitutionIncomingLetter>
    {
        public InstitutionIncomingLetterEntityTypeConfiguration()
        {
            this.ToTable("InstitutionIncomingLetter", "Archive");
            this.HasKey<int>(o => o.InstitutionIncomingLetterId);
        }
    }
    public class ArchiveAttachementEntityTypeConfiguration : EntityTypeConfiguration<ArchiveAttachement>
    {
        public ArchiveAttachementEntityTypeConfiguration()
        {
            this.ToTable("ArchiveAttachement", "Archive");
            this.HasKey<int>(o => o.ArchiveAttachementId);
        }
    }

    public class InstitutionIncomingAnswerEntityTypeConfiguration : EntityTypeConfiguration<InstitutionIncomingAnswer>
    {
        public InstitutionIncomingAnswerEntityTypeConfiguration()
        {
            this.ToTable("InstitutionIncomingAnswer", "Archive");
            this.HasKey<int>(o => o.InstitutionIncomingAnswerId);
        }
    }

    public class EmployeeFolderAttachmentEntityTypeConfiguration : EntityTypeConfiguration<EmployeeFolderAttachment>
    {
        public EmployeeFolderAttachmentEntityTypeConfiguration()
        {
            this.ToTable("EmployeeFolderAttachment", "Archive");
            this.HasKey<int>(o => o.EmployeeFolderAttachmentId);
        }
    }

    public class EmployeeIncomingAnswerEntityTypeConfiguration : EntityTypeConfiguration<EmployeeIncomingAnswer>
    {
        public EmployeeIncomingAnswerEntityTypeConfiguration()
        {
            this.ToTable("EmployeeIncomingAnswer", "Archive");
            this.HasKey<int>(o => o.EmployeeIncomingAnswerId);
        }
    }
    public class EmployeeOutgoingLetterEntityTypeConfiguration : EntityTypeConfiguration<EmployeeOutgoingLetter>
    {
        public EmployeeOutgoingLetterEntityTypeConfiguration()
        {
            this.ToTable("EmployeeOutgoingLetter", "Archive");
            this.HasKey<int>(o => o.EmployeeOutgoingLetterId);
        }
    }

    public class EmployeeOutgoingAnswerEntityTypeConfiguration : EntityTypeConfiguration<EmployeeOutgoingAnswer>
    {
        public EmployeeOutgoingAnswerEntityTypeConfiguration()
        {
            this.ToTable("EmployeeOutgoingAnswer", "Archive");
            this.HasKey<int>(o => o.EmployeeOutgoingAnswerId);
        }
    }

    public class CompanyDocumentTypeEntityTypeConfiguration : EntityTypeConfiguration<CompanyDocumentType>
    {
        public CompanyDocumentTypeEntityTypeConfiguration()
        {
            this.ToTable("CompanyDocumentType", "Archive");
            this.HasKey<int>(o => o.CompanyDocumentTypeId);
        }
    }

    public class CompanyDocumentEntityTypeConfiguration : EntityTypeConfiguration<CompanyDocument>
    {
        public CompanyDocumentEntityTypeConfiguration()
        {
            this.ToTable("CompanyDocument", "Archive");
            this.HasKey<int>(o => o.CompanyDocumentId);
        }
    }

    public class LetterReceiverEntityTypeConfiguration : EntityTypeConfiguration<LetterReceiver>
    {
        public LetterReceiverEntityTypeConfiguration()
        {
            this.ToTable("LetterReceiver", "Archive");
            this.HasKey<int>(o => o.LetterReceiverId);
        }
    }

    public class AttachedLettersForStaffEntityTypeConfiguration : EntityTypeConfiguration<AttachedLettersForStaff>
    {
        public AttachedLettersForStaffEntityTypeConfiguration()
        {
            this.ToTable("AttachedLettersForStaff", "Archive");
            this.HasKey<int>(o => o.AttachedLettersForStaffId);
        }
    }

    public class LetterForMultipleStaffEntityTypeConfiguration : EntityTypeConfiguration<LetterForMultipleStaff>
    {
        public LetterForMultipleStaffEntityTypeConfiguration()
        {
            this.ToTable("LetterForMultipleStaff", "Archive");
            this.HasKey<int>(o => o.LetterForMultipleStaffId);
        }
    }

    public class OutGoing_DepartmentToDepartmentEntityTypeConfiguration : EntityTypeConfiguration<OutGoing_DepartmentToDepartment>
    {
        public OutGoing_DepartmentToDepartmentEntityTypeConfiguration()
        {
            this.ToTable("OutGoing_DepartmentToDepartment", "Archive");
            this.HasKey<int>(o => o.OutGoing_DepartmentToDepartmentId);
        }
    }

    public class Receiver_DepartmentToDepartmentEntityTypeConfiguration : EntityTypeConfiguration<Receiver_DepartmentToDepartment>
    {
        public Receiver_DepartmentToDepartmentEntityTypeConfiguration()
        {
            this.ToTable("Receiver_DepartmentToDepartment", "Archive");
            this.HasKey<int>(o => o.Receiver_DepartmentToDepartmentId);
        }
    }
    public class Attachment_DepartmentToDepartmentEntityTypeConfiguration : EntityTypeConfiguration<Attachment_DepartmentToDepartment>
    {
        public Attachment_DepartmentToDepartmentEntityTypeConfiguration()
        {
            this.ToTable("Attachment_DepartmentToDepartment", "Archive");
            this.HasKey<int>(o => o.Attachment_DepartmentToDepartmentId);
        }
    }
    public class OutGoing_DepartmentToDepartment_AnswerEntityTypeConfiguration : EntityTypeConfiguration<OutGoing_DepartmentToDepartment_Answer>
    {
        public OutGoing_DepartmentToDepartment_AnswerEntityTypeConfiguration()
        {
            this.ToTable("OutGoing_DepartmentToDepartment_Answer", "Archive");
            this.HasKey<int>(o => o.OutGoing_DepartmentToDepartment_AnswerId);
        }
    }
    public class DepartmentFolderEntityTypeConfiguration : EntityTypeConfiguration<DepartmentFolder>
    {        public DepartmentFolderEntityTypeConfiguration()
        {
            this.ToTable("DepartmentFolder", "Archive");
            this.HasKey<int>(o => o.DepartmentFolderId);
        }
    }

    public class OutGoing_DepartmentToStaffEntityTypeConfiguration : EntityTypeConfiguration<OutGoing_DepartmentToStaff>
    {
        public OutGoing_DepartmentToStaffEntityTypeConfiguration()
        {
            this.ToTable("OutGoing_DepartmentToStaff", "Archive");
            this.HasKey<int>(o => o.OutGoing_DepartmentToStaffId);
        }
    }

    public class Attachment_DepartmentToStaffEntityTypeConfiguration : EntityTypeConfiguration<Attachment_DepartmentToStaff>
    {
        public Attachment_DepartmentToStaffEntityTypeConfiguration()
        {
            this.ToTable("Attachment_DepartmentToStaff", "Archive");
            this.HasKey<int>(o => o.Attachment_DepartmentToStaffId);
        }
    }

    public class Receiver_DepartmentToStaffEntityTypeConfiguration : EntityTypeConfiguration<Receiver_DepartmentToStaff>
    {
        public Receiver_DepartmentToStaffEntityTypeConfiguration()
        {
            this.ToTable("Receiver_DepartmentToStaff", "Archive");
            this.HasKey<int>(o => o.Receiver_DepartmentToStaffId);
        }
    }

    public class Answer_DepartmentToStaffEntityTypeConfiguration : EntityTypeConfiguration<Answer_DepartmentToStaff>
    {
        public Answer_DepartmentToStaffEntityTypeConfiguration()
        {
            this.ToTable("Answer_DepartmentToStaff", "Archive");
            this.HasKey<int>(o => o.Answer_DepartmentToStaffId);
        }
    }

    public class Attachment_Answer_DepartmentToDepartmentEntityTypeConfiguration : EntityTypeConfiguration<Attachment_Answer_DepartmentToDepartment>
    {
        public Attachment_Answer_DepartmentToDepartmentEntityTypeConfiguration()
        {
            this.ToTable("Attachment_Answer_DepartmentToDepartment", "Archive");
            this.HasKey<int>(o => o.Attachment_Answer_DepartmentToDepartmentId);
        }
    }

    public class Attachment_Answer_DepartmentToStaffEntityTypeConfiguration : EntityTypeConfiguration<Attachment_Answer_DepartmentToStaff>
    {
        public Attachment_Answer_DepartmentToStaffEntityTypeConfiguration()
        {
            this.ToTable("Attachment_Answer_DepartmentToStaff", "Archive");
            this.HasKey<int>(o => o.Attachment_Answer_DepartmentToStaffId);
        }
    }

    public class Attachment_Answer_InstitutionIncomingEntityTypeConfiguration : EntityTypeConfiguration<Attachment_Answer_InstitutionIncoming>
    {
        public Attachment_Answer_InstitutionIncomingEntityTypeConfiguration()
        {
            this.ToTable("Attachment_Answer_InstitutionIncoming", "Archive");
            this.HasKey<int>(o => o.Attachment_Answer_InstitutionIncomingId);
        }
    }

    public class Attachment_Answer_LetterSentForEmployeeEntityTypeConfiguration : EntityTypeConfiguration<Attachment_Answer_LetterSentForEmployee>
    {
        public Attachment_Answer_LetterSentForEmployeeEntityTypeConfiguration()
        {
            this.ToTable("Attachment_Answer_LetterSentForEmployee", "Archive");
            this.HasKey<int>(o => o.Attachment_Answer_LetterSentForEmployeeId);
        }
    }

    public class SubjectTypeForDepartmentLevelEntityTypeConfiguration : EntityTypeConfiguration<SubjectTypeForDepartmentLevel>
    {
        public SubjectTypeForDepartmentLevelEntityTypeConfiguration()
        {
            this.ToTable("SubjectTypeForDepartmentLevel", "Archive");
            this.HasKey<int>(o => o.SubjectTypeForDepartmentLevelId);
        }
    }

    public class DepartmentOrSubjectTypeSelectionForFolderCreationEntityTypeConfiguration : EntityTypeConfiguration<DepartmentOrSubjectTypeSelectionForFolderCreation>
    {
        public DepartmentOrSubjectTypeSelectionForFolderCreationEntityTypeConfiguration()
        {
            this.ToTable("DepartmentOrSubjectTypeSelectionForFolderCreation", "Archive");
            this.HasKey<int>(o => o.DepartmentOrSubjectTypeSelectionForFolderCreationId);
        }
    }
}

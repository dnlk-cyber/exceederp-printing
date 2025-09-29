using System.Data.Entity.ModelConfiguration;
using ExceedERP.Core.Domain.Health;
using ExceedERP.Core.Domain.APTS;

namespace ExceedERP.DataAccess
{

    #region Health 
       
        public class HealthRankAndValueValidationEntityTypeConfiguration : EntityTypeConfiguration<HealthRankAndValueValidation>
        {
            public HealthRankAndValueValidationEntityTypeConfiguration()
            {
                this.ToTable("HealthRankAndValueValidation", "Health");
                this.HasKey<int>(o => o.HealthRankAndValueValidationId);
            }
        }

        public class HealthRankAndValueFinalValidationEntityTypeConfiguration : EntityTypeConfiguration<HealthRankAndValueFinalValidation>
        {
            public HealthRankAndValueFinalValidationEntityTypeConfiguration()
            {
                this.ToTable("HealthRankAndValueFinalValidation", "Health");
                this.HasKey<int>(o => o.HealthRankAndValueFinalValidationId);
            }
        }
        public class HealthRankAndValueEntityTypeConfiguration : EntityTypeConfiguration<HealthRankAndValue>
        {
            public HealthRankAndValueEntityTypeConfiguration()
            {
                this.ToTable("HealthRankAndValues", "Health");
                this.HasKey<int>(o => o.HealthRankAndValueId);
            }
        }
        public class HealthRankAndValueSettingEntityTypeConfiguration : EntityTypeConfiguration<HealthRankAndValueSetting>
        {
            public HealthRankAndValueSettingEntityTypeConfiguration()
            {
                this.ToTable("HealthRankAndValueSettings", "Health");
                this.HasKey<int>(o => o.HealthRankAndValueSettingId);
            }
        }
        public class HealthResponsiblityLevelValueSettingEntityTypeConfiguration : EntityTypeConfiguration<HealthResponsiblityLevelValueSetting>
        {
            public HealthResponsiblityLevelValueSettingEntityTypeConfiguration()
            {
                this.ToTable("HealthResponsiblityLevelValueSettings", "Health");
                this.HasKey<int>(o => o.HealthResponsiblityLevelValueSettingId);
            }
        }
        public class PaymentCoverageSettingEntityTypeConfiguration : EntityTypeConfiguration<PaymentCoverageSetting>
        {
            public PaymentCoverageSettingEntityTypeConfiguration()
            {
                this.ToTable("PaymentCoverageSettings", "Health");
                this.HasKey<int>(o => o.PaymentCoverageSettingId);
            }
        }
        public class HealthBenefitSettingEntityTypeConfiguration : EntityTypeConfiguration<HealthBenefitSetting>
        {
            public HealthBenefitSettingEntityTypeConfiguration()
            {
                this.ToTable("HealthBenefitSettings", "Health");
                this.HasKey<int>(o => o.HealthBenefitSettingId);
            }
        }
       public class HealthApprovalSettingEntityTypeConfiguration : EntityTypeConfiguration<HealthApprovalSetting>
        {
            public HealthApprovalSettingEntityTypeConfiguration()
            {
                this.ToTable("HealthApprovalSettings", "Health");
                this.HasKey<int>(o => o.HealthApprovalSettingId);
            }
        }
        public class HealthExpenseAndTaxSettingEntityTypeConfiguration : EntityTypeConfiguration<HealthExpenseAndTaxSetting>
        {
            public HealthExpenseAndTaxSettingEntityTypeConfiguration()
            {
                this.ToTable("HealthExpenseAndTaxSettings", "Health");
                this.HasKey<int>(o => o.HealthExpenseAndTaxSettingId);
            }
        }
    public class HealthAttachmentEntityTypeConfiguration : EntityTypeConfiguration<HealthAttachment>
    {

        public HealthAttachmentEntityTypeConfiguration()
        {
                this.ToTable("HealthAttachments", "Health");
                this.HasKey<int>(o => o.HealthAttachmentId);

        }

    }
       

        public class PatientResultEntityTypeConfiguration : EntityTypeConfiguration<PatientResult>
        {
            public PatientResultEntityTypeConfiguration()
            {
                this.ToTable("PatientResult", "Health");
                this.HasKey<int>(o => o.PatientResultId);
            }
        }
        public class PatientResultDetailEntityTypeConfiguration : EntityTypeConfiguration<PatientResultDetail>
        {
            public PatientResultDetailEntityTypeConfiguration()
            {
                this.ToTable("PatientResultDetail", "Health");
                this.HasKey<int>(o => o.PatientResultDetailId);
            }
        }
        public class PatientDrugOrderEntityTypeConfiguration : EntityTypeConfiguration<PatientDrugOrder>
        {
            public PatientDrugOrderEntityTypeConfiguration()
            {
                this.ToTable("PatientDrugOrder", "Health");
                this.HasKey<int>(o => o.PatientDrugOrderId);
            }
        }
        public class DiagnosisEntityTypeConfiguration : EntityTypeConfiguration<Diagnosis>
        {
            public DiagnosisEntityTypeConfiguration()
            {
                this.ToTable("Diagnosis", "Health");
                this.HasKey<int>(o => o.DiagnosisId);
            }
        }
      
    
        public class ReferralFormEntityTypeConfiguration : EntityTypeConfiguration<ReferralForm>
        {
            public ReferralFormEntityTypeConfiguration()
            {
                this.ToTable("ReferralForm", "Health");
                this.HasKey<int>(o => o.ReferralFormId);
            }
        }
        public class ReferralSheetEntityTypeConfiguration : EntityTypeConfiguration<ReferralSheet>
        {
            public ReferralSheetEntityTypeConfiguration()
            {
                this.ToTable("ReferralSheet", "Health");
                this.HasKey<int>(o => o.ReferralSheetId);
            }
        }
        public class PatientsLeaveEntityTypeConfiguration : EntityTypeConfiguration<PatientsLeave>
        {
            public PatientsLeaveEntityTypeConfiguration()
            {
                this.ToTable("PatientsLeave", "Health");
                this.HasKey<int>(o => o.PatientsLeaveId);
            }
        }
        public class PrescriptionEntityTypeConfiguration : EntityTypeConfiguration<Prescription>
        {
            public PrescriptionEntityTypeConfiguration()
            {
                this.ToTable("Prescription", "Health");
                this.HasKey<int>(o => o.PrescriptionId);
            }
        }
        public class TreatmentEntityTypeConfiguration : EntityTypeConfiguration<Treatment>
        {
            public TreatmentEntityTypeConfiguration()
            {
                this.ToTable("Treatment", "Health");
                this.HasKey<int>(o => o.TreatmentId);
            }
        }
        public class PatientCardEntityTypeConfiguration : EntityTypeConfiguration<PatientCard>
        {
            public PatientCardEntityTypeConfiguration()
            {
                this.ToTable("PatientCard", "Health");
                //this.HasKey<string>(o => o. PatientCardId);
                this.HasKey<int>(o => o.PatientId);
            }
        }
        public class PatientCardDetailEntityTypeConfiguration : EntityTypeConfiguration<PatientCardDetail>
        {
            public PatientCardDetailEntityTypeConfiguration()
            {
                this.ToTable("PatientCardDetail", "Health");
                this.HasKey<int>(o => o.PatientCardDetailId);
            }
        }
        public class LaboratoryCatagoryEntityTypeConfiguration : EntityTypeConfiguration<LaboratoryCatagory>
        {
            public LaboratoryCatagoryEntityTypeConfiguration()
            {
                this.ToTable("LaboratoryCatagoryies", "Health");
                this.HasKey<int>(o => o.LaboratoryCatagoryId);
            }
        }
        public class LaboratoryTestsEntityTypeConfiguration : EntityTypeConfiguration<LaboratoryTests>
        {
            public LaboratoryTestsEntityTypeConfiguration()
            {
                this.ToTable("LaboratoryTests", "Health");
                this.HasKey<int>(o => o.LaboratoryTestsId);
            }
        }
        public class LaboratoryTestPatientCardEntityTypeConfiguration : EntityTypeConfiguration<LaboratoryTestPatientCard>
        {
            public LaboratoryTestPatientCardEntityTypeConfiguration()
            {
                this.ToTable("LaboratoryTestPatientCards", "Health");
                this.HasKey<int>(o => o.LaboratoryTestPatientCardId);
            }
        }
        public class LaboratoryCatagoryPatientCardEntityTypeConfiguration : EntityTypeConfiguration<LaboratoryCatagoryPatientCard>
        {
            public LaboratoryCatagoryPatientCardEntityTypeConfiguration()
            {
                this.ToTable("LaboratoryCatagoryPatientCards", "Health");
                this.HasKey<int>(o => o.LaboratoryCatagoryPatientCardId);
            }
        }
        public class MedicalRequestEntityTypeConfiguration : EntityTypeConfiguration<MedicalRequest>
        {
            public MedicalRequestEntityTypeConfiguration()
            {
                this.ToTable("MedicalRequests", "Health");
                this.HasKey<int>(o => o.MedicalRequestId);
            }
        }
       public class MedicalAttachmentEntityTypeConfiguration : EntityTypeConfiguration<MedicalAttachment>
        {
            public MedicalAttachmentEntityTypeConfiguration()
            {
                this.ToTable("MedicalAttachments", "Health");
                this.HasKey<int>(o => o.MedicalAttachmentId);
            }
        }

    #endregion
}

using System.Data.Entity.ModelConfiguration;
using ExceedERP.Core.Domain.Insurance;
using ExceedERP.Core.Domain.APTS;
using ExceedERP.Core.Domain.Insurance.InsuranceNew;
using ExceedERP.Core.Domain.Insurance.Setting;

namespace ExceedERP.DataAccess
{

#region Insurance

    #region New Insurances
   
        public class EmployeeMedicalExpenseRefundEntityTypeConfiguration : EntityTypeConfiguration<EmployeeMedicalExpenseRefund>
        {
            public EmployeeMedicalExpenseRefundEntityTypeConfiguration()
            {
                this.ToTable("EmployeeMedicalExpenseRefunds", "Insurance");
                this.HasKey<int>(x => x.EmployeeMedicalExpenseRefundId);
            }

        }
        public class MedicalExpenseRefundAttachmentEntityTypeConfiguration : EntityTypeConfiguration<MedicalExpenseRefundAttachment>
        {
            public MedicalExpenseRefundAttachmentEntityTypeConfiguration()
            {
                this.ToTable("MedicalExpenseRefundAttachments", "Insurance");
                this.HasKey<int>(x => x.MedicalExpenseRefundAttachmentId);
            }
        }
        public class MedicalExpenseRefundDepartmentApprovalEntityTypeConfiguration : EntityTypeConfiguration<MedicalExpenseRefundDepartmentApproval>
        {
            public MedicalExpenseRefundDepartmentApprovalEntityTypeConfiguration()
            {
                this.ToTable("MedicalExpenseRefundDepartmentApprovals", "Insurance");
                this.HasKey<int>(x => x.MedicalExpenseRefundDepartmentApprovalId);
            }
        }
        public class MedicalExpenseRefundApprovalEntityTypeConfiguration : EntityTypeConfiguration<MedicalExpenseRefundApproval>
        {
            public MedicalExpenseRefundApprovalEntityTypeConfiguration()
            {
                this.ToTable("MedicalExpenseRefundApprovals", "Insurance");
                this.HasKey<int>(x => x.MedicalExpenseRefundApprovalId);
            }
        }

        public class EmployeeAccidentInsuranceClaimEntityTypeConfiguration : EntityTypeConfiguration<EmployeeAccidentInsuranceClaim>
        {
                public EmployeeAccidentInsuranceClaimEntityTypeConfiguration()
                {
                    this.ToTable("EmployeeAccidentInsuranceClaims", "Insurance");
                    this.HasKey<int>(x => x.EmployeeAccidentInsuranceClaimId);
                }


        }

        public class AccidentInsuranceClaimAttachmentEntityTypeConfiguration : EntityTypeConfiguration<AccidentInsuranceClaimAttachment>
        {
                public AccidentInsuranceClaimAttachmentEntityTypeConfiguration()
                {
                    this.ToTable("AccidentInsuranceClaimAttachments", "Insurance");
                    this.HasKey<int>(x => x.AccidentInsuranceClaimAttachmentId);
                }


        }

        public class AccidentInsuranceClaimDepartmentApprovalEntityTypeConfiguration : EntityTypeConfiguration<AccidentInsuranceClaimDepartmentApproval>
        {
                public AccidentInsuranceClaimDepartmentApprovalEntityTypeConfiguration()
                {
                    this.ToTable("AccidentInsuranceClaimDepartmentApprovals", "Insurance");
                    this.HasKey<int>(x => x.AccidentInsuranceClaimDepartmentApprovalId);
                }


        }
        public class AccidentInsuranceClaimApprovalEntityTypeConfiguration : EntityTypeConfiguration<AccidentInsuranceClaimApproval>
        {
            public AccidentInsuranceClaimApprovalEntityTypeConfiguration()
            {
                this.ToTable("AccidentInsuranceClaimApprovals", "Insurance");
                this.HasKey<int>(x => x.AccidentInsuranceClaimApprovalId);
            }

        }
        public class EmployeeAccidentCreditServiceEntityTypeConfiguration : EntityTypeConfiguration<EmployeeAccidentCreditService>
        {
            public EmployeeAccidentCreditServiceEntityTypeConfiguration()
            {
                this.ToTable("EmployeeAccidentCreditServices", "Insurance");
                this.HasKey<int>(x => x.EmployeeAccidentCreditServiceId);
            }

        }
        public class AccidentCreditServiceAttachmentEntityTypeConfiguration : EntityTypeConfiguration<AccidentCreditServiceAttachment>
        {
            public AccidentCreditServiceAttachmentEntityTypeConfiguration()
            {
                this.ToTable("AccidentCreditServiceAttachments", "Insurance");
                this.HasKey<int>(x => x.AccidentCreditServiceAttachmentId);
            }

        }
        public class AccidentCreditServiceDepartmentApprovalEntityTypeConfiguration : EntityTypeConfiguration<AccidentCreditServiceDepartmentApproval>
        {
            public AccidentCreditServiceDepartmentApprovalEntityTypeConfiguration()
            {
                this.ToTable("AccidentCreditServiceDepartmentApprovals", "Insurance");
                this.HasKey<int>(x => x.AccidentCreditServiceDepartmentApprovalId);
            }

        }
        public class AccidentCreditServiceApprovalEntityTypeConfiguration : EntityTypeConfiguration<AccidentCreditServiceApproval>
        {
            public AccidentCreditServiceApprovalEntityTypeConfiguration()
            {
                this.ToTable("AccidentCreditServiceApprovals", "Insurance");
                this.HasKey<int>(x => x.AccidentCreditServiceApprovalId);
            }

        }
        public class InsurancePolicyRenewalEntityTypeConfiguration : EntityTypeConfiguration<InsurancePolicyRenewal>
        {
            public InsurancePolicyRenewalEntityTypeConfiguration()
            {
                this.ToTable("InsurancePolicyRenewals", "Insurance");
                this.HasKey<int>(x => x.InsurancePolicyRenewalId);
            }

        }
        public class PolicyRenewalAttachmentEntityTypeConfiguration : EntityTypeConfiguration<PolicyRenewalAttachment>
        {
            public PolicyRenewalAttachmentEntityTypeConfiguration()
            {
                this.ToTable("PolicyRenewalAttachments", "Insurance");
                this.HasKey<int>(x => x.PolicyRenewalAttachmentId);
            }

        }
        public class PolicyRenewalDepartmentApprovalEntityTypeConfiguration : EntityTypeConfiguration<PolicyRenewalDepartmentApproval>
        {
            public PolicyRenewalDepartmentApprovalEntityTypeConfiguration()
            {
                this.ToTable("PolicyRenewalDepartmentApprovals", "Insurance");
                this.HasKey<int>(x => x.PolicyRenewalDepartmentApprovalId);
            }

        }
        public class PolicyRenewalApprovalEntityTypeConfiguration : EntityTypeConfiguration<PolicyRenewalApproval>
        {
            public PolicyRenewalApprovalEntityTypeConfiguration()
            {
                this.ToTable("PolicyRenewalApprovals", "Insurance");
                this.HasKey<int>(x => x.PolicyRenewalApprovalId);
            }

        }
        public class EquipmentAccidentClaimEntityTypeConfiguration : EntityTypeConfiguration<EquipmentAccidentClaim>
        {
            public EquipmentAccidentClaimEntityTypeConfiguration()
            {
                this.ToTable("EquipmentAccidentClaims", "Insurance");
                this.HasKey<int>(x => x.EquipmentAccidentClaimId);
            }

        }
        public class EquipmentAccidentClaimAttachmentEntityTypeConfiguration : EntityTypeConfiguration<EquipmentAccidentClaimAttachment>
        {
            public EquipmentAccidentClaimAttachmentEntityTypeConfiguration()
            {
                this.ToTable("EquipmentAccidentClaimAttachment", "Insurance");
                this.HasKey<int>(x => x.EquipmentAccidentClaimAttachmentId);
            }

        }
        public class AccidentClaimDepartmentApprovalEntityTypeConfiguration : EntityTypeConfiguration<AccidentClaimDepartmentApproval>
        {
            public AccidentClaimDepartmentApprovalEntityTypeConfiguration()
            {
                this.ToTable("AccidentClaimDepartmentApprovals", "Insurance");
                this.HasKey<int>(x => x.AccidentClaimDepartmentApprovalId);
            }

        }
        public class AccidentClaimApprovalEntityTypeConfiguration : EntityTypeConfiguration<AccidentClaimApproval>
        {
                public AccidentClaimApprovalEntityTypeConfiguration()
                {
                    this.ToTable("AccidentClaimApprovals", "Insurance");
                    this.HasKey<int>(x => x.AccidentClaimApprovalId);
                }


        }
        public class FireAndLightningInsuranceEntityTypeConfiguration : EntityTypeConfiguration<FireAndLightningInsurance>
        {
                public FireAndLightningInsuranceEntityTypeConfiguration()
                {
                    this.ToTable("FireAndLightningInsurance", "Insurance");
                    this.HasKey<int>(x => x.FireAndLightningInsuranceId);
                }


        }
        public class FireAndLightningInsuranceDetailEntityTypeConfiguration : EntityTypeConfiguration<FireAndLightningInsuranceDetail>
        {
                public FireAndLightningInsuranceDetailEntityTypeConfiguration()
                {
                    this.ToTable("FireAndLightningInsuranceDetails", "Insurance");
                    this.HasKey<int>(x => x.FireAndLightningInsuranceDetailId);
                }


        }
        public class FireAndLightningInsuranceAttachmentEntityTypeConfiguration : EntityTypeConfiguration<FireAndLightningInsuranceAttachment>
        {
                public FireAndLightningInsuranceAttachmentEntityTypeConfiguration()
                {
                    this.ToTable("FireAndLightningInsuranceAttachment", "Insurance");
                    this.HasKey<int>(x => x.FireAndLightningInsuranceAttachmentId);
                }


        }
        public class FireAndLightningInsuranceDepartmentApprovalEntityTypeConfiguration : EntityTypeConfiguration<FireAndLightningInsuranceDepartmentApproval>
        {
                public FireAndLightningInsuranceDepartmentApprovalEntityTypeConfiguration()
                {
                    this.ToTable("FireAndLightningInsuranceDepartmentApprovals", "Insurance");
                    this.HasKey<int>(x => x.FireAndLightningInsuranceDepartmentApprovalId);
                }


        }
        public class FireAndLightningInsuranceApprovalEntityTypeConfiguration : EntityTypeConfiguration<FireAndLightningInsuranceApproval>
        {
                public FireAndLightningInsuranceApprovalEntityTypeConfiguration()
                {
                    this.ToTable("FireAndLightningInsuranceApprovals", "Insurance");
                    this.HasKey<int>(x => x.FireAndLightningInsuranceApprovalId);
                }


        }
        public class InsurancePaymentSettingEntityTypeConfiguration : EntityTypeConfiguration<InsurancePaymentSetting>
        {
            public InsurancePaymentSettingEntityTypeConfiguration()
            {
                this.ToTable("InsurancePaymentSettings", "Insurance");
                this.HasKey<int>(x => x.InsurancePaymentSettingId);
            }


        }
    
    #endregion
    #region old Insurance

        public class InsuranceGroupEntityTypeConfiguration : EntityTypeConfiguration<InsuranceGroup>
        {
            public InsuranceGroupEntityTypeConfiguration()
            {
                this.ToTable("InsuranceGroup", "Insurance");
                this.HasKey<int>(o => o.InsuranceGroupId);
            }
        }
        public class AccidentRecordEntityTypeConfiguration : EntityTypeConfiguration<AccidentRecord>
        {
            public AccidentRecordEntityTypeConfiguration()
            {
                this.ToTable("AccidentRecord", "Insurance");
                this.HasKey<int>(o => o.AccidentRecordId);
            }
        }
        public class AccidentMedicalRecordEntityTypeConfiguration : EntityTypeConfiguration<AccidentMedicalRecord>
        {
            public AccidentMedicalRecordEntityTypeConfiguration()
            {
                this.ToTable("AccidentMedicalRecord", "Insurance");
                this.HasKey<int>(o => o.AccidentMedicalRecordId);
            }
        }
        public class RestitutionEntityTypeConfiguration : EntityTypeConfiguration<Restitution>
        {
            public RestitutionEntityTypeConfiguration()
            {
                this.ToTable("Restitution", "Insurance");
                this.HasKey<int>(o => o.RestitutionId);
            }
        }
        public class InsuranceEntityTypeConfiguration : EntityTypeConfiguration<Insurance>
        {
            public InsuranceEntityTypeConfiguration()
            {
                this.ToTable("Insurance", "Insurance");
                this.HasKey<int>(o => o.InsuranceId);
            }
        }

        public class CourtInsuranceEntityTypeConfiguration : EntityTypeConfiguration<CourtInsurance>
        {
            public CourtInsuranceEntityTypeConfiguration()
            {
                this.ToTable("CourtInsurance", "Insurance");
                this.HasKey<int>(o => o.CourtInsuranceId);
            }
        }
        public class InsuranceCategoryEntityTypeConfiguration : EntityTypeConfiguration<InsuranceCategory>
        {
            public InsuranceCategoryEntityTypeConfiguration()
            {
                this.ToTable("InsuranceCategory", "Insurance");
                this.HasKey<int>(o => o.InsuranceCategoryId);
            }
        }
        public class InsuranceTypeEntityTypeConfiguration : EntityTypeConfiguration<InsuranceType>
        {
            public InsuranceTypeEntityTypeConfiguration()
            {
                this.ToTable("InsuranceType", "Insurance");
                this.HasKey<int>(o => o.InsuranceTypeId);
            }
        }
        public class OtherInsuranceEntityTypeConfiguration : EntityTypeConfiguration<OtherInsurance>
        {
            public OtherInsuranceEntityTypeConfiguration()
            {
                this.ToTable("OtherInsurance", "Insurance");
                this.HasKey<int>(o => o.OtherInsuranceId);
            }
        }

        public class VehicleAccidentDetailEntityTypeConfiguration : EntityTypeConfiguration<VehicleAccidentDetail>
        {
            public VehicleAccidentDetailEntityTypeConfiguration()
            {
                this.ToTable("VehicleAccidentDetail", "Insurance");
                this.HasKey<int>(o => o.VehicleAccidentDetailId);
            }
        }

    #endregion
    
#endregion
}

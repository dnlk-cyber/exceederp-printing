using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.Core.Domain.CRM;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Configuration;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.HRM.Letters;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.Core.Domain.HRM.Prohibition;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.Core.Domain.APTS;

namespace ExceedERP.DataAccess
{


    public class DrugToTransferTemporaryListsEntityTypeConfiguration : EntityTypeConfiguration<DrugToTransferTemporaryLists>
    {

        public DrugToTransferTemporaryListsEntityTypeConfiguration()
        {
            this.ToTable("DrugToTransferTemporaryLists", "APTS");
            this.HasKey<int>(o => o.DrugToTransferTemporaryListsId);

        }
    }



    public class DrugStoreTransferEntityTypeConfiguration : EntityTypeConfiguration<DrugStoreTransfer>
    {

        public DrugStoreTransferEntityTypeConfiguration()
        {
            this.ToTable("DrugStoreTransfer", "APTS");
            this.HasKey<int>(o => o.DrugStoreTransferId);

        }
    }

    public class DrugListsEntityTypeConfiguration : EntityTypeConfiguration<DrugLists>
    {

        public DrugListsEntityTypeConfiguration()
        {
            this.ToTable("DrugLists", "APTS");
            this.HasKey<int>(o => o.DrugListsId);

        }
    }
    public class LaboratoryCatagoryEntityTypeConfiguration : EntityTypeConfiguration<LaboratoryCatagory>
    {

        public LaboratoryCatagoryEntityTypeConfiguration()
        {
            this.ToTable("LaboratoryCatagory", "APTS");
            this.HasKey<int>(o => o.LaboratoryCatagoryId);

        }
    }
    public class LaboratoryTestsEntityTypeConfiguration : EntityTypeConfiguration<LaboratoryTests>
    {

        public LaboratoryTestsEntityTypeConfiguration()
        {
            this.ToTable("LaboratoryTests", "APTS");
            this.HasKey<int>(o => o.LaboratoryTestsId);

        }
    }
    public class LaboratoryCatagoryPatientCardEntityTypeConfiguration : EntityTypeConfiguration<LaboratoryCatagoryPatientCard>
    {

        public LaboratoryCatagoryPatientCardEntityTypeConfiguration()
        {
            this.ToTable("LaboratoryCatagoryPatientCard", "APTS");
            this.HasKey<int>(o => o.LaboratoryCatagoryPatientCardId);

        }
    }
    public class LaboratoryTestPatientCardEntityTypeConfiguration : EntityTypeConfiguration<LaboratoryTestPatientCard>
    {

        public LaboratoryTestPatientCardEntityTypeConfiguration()
        {
            this.ToTable("LaboratoryTestPatientCard", "APTS");
            this.HasKey<int>(o => o.LaboratoryTestPatientCardId);

        }
    }

    public class APTSRequestStatusEntityTypeConfiguration : EntityTypeConfiguration<APTSRequestStatus>
    {

        public APTSRequestStatusEntityTypeConfiguration()
        {
            this.ToTable("APTSRequestStatus", "APTS");
            this.HasKey<int>(o => o.APTSRequestStatusId);

        }
    }
    public class EmergencyDrugRequestEntityTypeConfiguration : EntityTypeConfiguration<EmergencyDrugRequest>
    {

        public EmergencyDrugRequestEntityTypeConfiguration()
        {
            this.ToTable("EmergencyDrugRequest", "APTS");
            this.HasKey<int>(o => o.EmergencyDrugRequestId);

        }
    }
    public class EmergencyDrugRequestItemEntityTypeConfiguration : EntityTypeConfiguration<EmergencyDrugRequestItem>
    {

        public EmergencyDrugRequestItemEntityTypeConfiguration()
        {
            this.ToTable("EmergencyDrugRequestItem", "APTS");
            this.HasKey<int>(o => o.EmergencyDrugRequestItemId);

        }
    }
    public class LaboratoryDrugRequestEntityTypeConfiguration : EntityTypeConfiguration<LaboratoryDrugRequest>
    {

        public LaboratoryDrugRequestEntityTypeConfiguration()
        {
            this.ToTable("LaboratoryDrugRequest", "APTS");
            this.HasKey<int>(o => o.LaboratoryDrugRequestId);

        }
    }
    public class LaboratoryDrugRequestItemEntityTypeConfiguration : EntityTypeConfiguration<LaboratoryDrugRequestItem>
    {

        public LaboratoryDrugRequestItemEntityTypeConfiguration()
        {
            this.ToTable("LaboratoryDrugRequestItem", "APTS");
            this.HasKey<int>(o => o.LaboratoryDrugRequestItemId);

        }
    }
    public class DrugPurchaseRequisitionEntityTypeConfiguration : EntityTypeConfiguration<DrugPurchaseRequisition>
    {

        public DrugPurchaseRequisitionEntityTypeConfiguration()
        {
            this.ToTable("DrugPurchaseRequisitions", "APTS");
            this.HasKey<int>(o => o.DrugPurchaseRequisitionId);

        }
    }
    public class DespensaryStoreEntityTypeConfiguration : EntityTypeConfiguration<DespensaryStore>
    {

        public DespensaryStoreEntityTypeConfiguration()
        {
            this.ToTable("DespensaryStores", "APTS");
            this.HasKey<int>(o => o.DespensaryStoreId);

        }
    }

    public class DrugStoreEntityTypeConfiguration : EntityTypeConfiguration<DrugStore>
    {

        public DrugStoreEntityTypeConfiguration()
        {
            this.ToTable("DrugStores", "APTS");
            this.HasKey<int>(o => o.DrugStoreId);

        }
    }
    public class DrugStoreItemEntityTypeConfiguration : EntityTypeConfiguration<DrugStoreItem>
    {

        public DrugStoreItemEntityTypeConfiguration()
        {
            this.ToTable("DrugStoreItems", "APTS");
            this.HasKey<int>(o => o.DrugStoreItemId);

        }
    }
    public class DrugReceivalModel19EntityTypeConfiguration : EntityTypeConfiguration<DrugReceivalModel19>
    {

        public DrugReceivalModel19EntityTypeConfiguration()
        {
            this.ToTable("DrugReceivalModel19", "APTS");
            this.HasKey<int>(o => o.DrugReceivalModel19Id);

        }
    }
   

    public class Model22EntityTypeConfiguration : EntityTypeConfiguration<Model22>
    {

        public Model22EntityTypeConfiguration()
        {
            this.ToTable("Model22", "APTS");
            this.HasKey<int>(o => o.Model22Id);

        }
    }
    public class PatientRegisterEntityTypeConfiguration : EntityTypeConfiguration<PatientRegister>
    {

        public PatientRegisterEntityTypeConfiguration()
        {
            this.ToTable("PatientRegister", "APTS");
            this.HasKey<int>(o => o.PatientRegisterId);

        }
    }
    public class PatientRegisterDetailEntityTypeConfiguration : EntityTypeConfiguration<PatientRegisterDetail>
    {

        public PatientRegisterDetailEntityTypeConfiguration()
        {
            this.ToTable("PatientRegisterDetail", "APTS");
            this.HasKey<int>(o => o.PatientRegisterDetailId);

        }
    }
    public class RequisitionModel20EntityTypeConfiguration : EntityTypeConfiguration<RequisitionModel20>
    {

        public RequisitionModel20EntityTypeConfiguration()
        {
            this.ToTable("RequisitionModel20", "APTS");
            this.HasKey<int>(o => o.RequisitionModel20Id);

        }
    }


    public class DrugRequestEntityTypeConfiguration : EntityTypeConfiguration<DrugRequest>
    {

        public DrugRequestEntityTypeConfiguration()
        {
            this.ToTable("DrugRequests", "APTS");
            this.HasKey<int>(o => o.DrugRequestId);

        }
    }

    public class DrugRequestItemEntityTypeConfiguration : EntityTypeConfiguration<DrugRequestItem>
    {

        public DrugRequestItemEntityTypeConfiguration()
        {
            this.ToTable("DrugRequestItems", "APTS");
            this.HasKey<int>(o => o.DrugRequestItemId);

        }
    }

    public class DrugUnitEntityTypeConfiguration : EntityTypeConfiguration<DrugUnit>
    {

        public DrugUnitEntityTypeConfiguration()
        {
            this.ToTable("DrugUnit", "APTS");
            this.HasKey<int>(o => o.DrugUnitId);

        }
    }
    public class DrugReceivalDetailEntityTypeConfiguration : EntityTypeConfiguration<DrugReceivalDetail>
    {

        public DrugReceivalDetailEntityTypeConfiguration()
        {
            this.ToTable("DrugReceivalDetail", "APTS");
            this.HasKey<int>(o => o.DrugReceivalDetailId);

        }
    }
    public class APTSValidationEntityTypeConfiguration : EntityTypeConfiguration<APTSValidation>
    {

        public APTSValidationEntityTypeConfiguration()
        {
            this.ToTable("APTSValidation", "APTS");
            this.HasKey<int>(o => o.APTSValidationId);

        }
    }
}




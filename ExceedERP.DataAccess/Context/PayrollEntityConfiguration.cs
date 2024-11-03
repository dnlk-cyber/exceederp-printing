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
using ExceedERP.Core.Domain.HRM.Payroll.Setting;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Core.Domain.HRM.Payroll.BackPays;
using ExceedERP.Core.Domain.HRM.Payroll.MissingPayrollBackPays;

namespace ExceedERP.DataAccess
{
    public class PayrollProcessViewModelEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessViewModel>
    {
        public PayrollProcessViewModelEntityTypeConfiguration()
        {
            this.ToTable("PayrollProcessViewModel", "Payroll");
            this.HasKey<int>(o => o.PayrollProcessViewModelId);
            this.Property(o => o.AbsentDeduction).HasPrecision(18, 9);
            this.Property(o => o.AssocationContribution).HasPrecision(18, 9);
            this.Property(o => o.BasicSalary).HasPrecision(18, 9);
            this.Property(o => o.DaysAbsent).HasPrecision(18, 9);
            this.Property(o => o.GrossDeduction).HasPrecision(18, 9);
            this.Property(o => o.GrossEarning).HasPrecision(18, 9);
            this.Property(o => o.GrossNonTaxableEarning).HasPrecision(18, 9);
            this.Property(o => o.GrossTaxableEarning).HasPrecision(18, 9);
            this.Property(o => o.IncomeTax).HasPrecision(18, 9);
            this.Property(o => o.LabourUnionContribution).HasPrecision(18, 9);
            this.Property(o => o.NetEarning).HasPrecision(18, 9);
            this.Property(o => o.NonTaxableAllowance).HasPrecision(18, 9);
            this.Property(o => o.NonTaxableHouseAllowance).HasPrecision(18, 9);
            this.Property(o => o.NonTaxablePositionAllowance).HasPrecision(18, 9);
            this.Property(o => o.NonTaxableProfessionalAllowance).HasPrecision(18, 9);
            this.Property(o => o.NonTaxableProjectAllowance).HasPrecision(18, 9);
            this.Property(o => o.NonTaxableResponsiblityAllowance).HasPrecision(18, 9);
            this.Property(o => o.NonTaxableTelephoneAllowance).HasPrecision(18, 9);
            this.Property(o => o.NonTaxableTopUpAllowance).HasPrecision(18, 9);
            this.Property(o => o.NonTaxableTransportAllowance).HasPrecision(18, 9);
            this.Property(o => o.PensionFromCompany).HasPrecision(18, 9);
            this.Property(o => o.PensionFromEmployee).HasPrecision(18, 9);
            this.Property(o => o.ServiceCharge).HasPrecision(18, 9);
            this.Property(o => o.SportFee).HasPrecision(18, 9);
            this.Property(o => o.TaxableAllowance).HasPrecision(18, 9);
            this.Property(o => o.TaxableHouseAllowance).HasPrecision(18, 9);
            this.Property(o => o.TaxablePositionAllowance).HasPrecision(18, 9);
            this.Property(o => o.TaxableProfessionalAllowance).HasPrecision(18, 9);
            this.Property(o => o.TaxableProjectAllowance).HasPrecision(18, 9);
            this.Property(o => o.TaxableResponsiblityAllowance).HasPrecision(18, 9);
            this.Property(o => o.TaxableTelephoneAllowance).HasPrecision(18, 9);
            this.Property(o => o.TaxableTopUpAllowance).HasPrecision(18, 9);
            this.Property(o => o.TaxableTransportAllowance).HasPrecision(18, 9);

            this.Property(o => o.TotalAdditions).HasPrecision(18, 9);
            this.Property(o => o.TotalAdvance).HasPrecision(18, 9);
            this.Property(o => o.TotalAllowance).HasPrecision(18, 9);
            this.Property(o => o.TotalDeductions).HasPrecision(18, 9);
            this.Property(o => o.TotalFixedContribution).HasPrecision(18, 9);
            this.Property(o => o.TotalHouseAllowance).HasPrecision(18, 9);
            this.Property(o => o.TotalLoan).HasPrecision(18, 9);
            this.Property(o => o.TotalLongTermDeductions).HasPrecision(18, 9);
            this.Property(o => o.TotalOvertime).HasPrecision(18, 9);
            this.Property(o => o.TotalPenality).HasPrecision(18, 9);
            this.Property(o => o.TotalPension).HasPrecision(18, 9);
            this.Property(o => o.TotalPositionAllowance).HasPrecision(18, 9);
            this.Property(o => o.TotalProfessionalAllowance).HasPrecision(18, 9);
            this.Property(o => o.TotalProjectAllowance).HasPrecision(18, 9);
            this.Property(o => o.TotalRegularContribution).HasPrecision(18, 9);
            this.Property(o => o.TotalResponsiblityAllowance).HasPrecision(18, 9);
            this.Property(o => o.TotalTelephoneAllowance).HasPrecision(18, 9);
            this.Property(o => o.TotalTopUpAllowance).HasPrecision(18, 9);
            this.Property(o => o.TotalTransportAllowance).HasPrecision(18, 9);

        }
    }

	public class PayrollCampAllowedBranchEntityTypeConfiguration : EntityTypeConfiguration<PayrollCampAllowedBranch>
	{
		public PayrollCampAllowedBranchEntityTypeConfiguration()
		{
			this.ToTable("PayrollCampAllowedBranch", "Payroll");
			this.HasKey<int>(o => o.PayrollCampAllowedBranchId);
		}
	}
	public class PayrollEmployeeHouseRentEntityTypeConfiguration : EntityTypeConfiguration<PayrollEmployeeHouseRent>
    {
        public PayrollEmployeeHouseRentEntityTypeConfiguration()
        {
            this.ToTable("PayrollEmployeeHouseRent", "Payroll");
            this.HasKey<int>(o => o.PayrollEmployeeHouseRentId);
        }
    }
    public class OverTimeEntityTypeConfiguration : EntityTypeConfiguration<OverTime>
    {
        public OverTimeEntityTypeConfiguration()
        {
            this.ToTable("OverTime", "Payroll");
            this.HasKey<int>(o => o.OverTimeId);
        }
    }
    public class PayrollProcessAllowancesEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessAllowances>
    {

        public PayrollProcessAllowancesEntityTypeConfiguration()
        {
            this.ToTable("PayrollProcessAllowances", "Payroll");
            this.HasKey<int>(o => o.PayrollProcessAllowancesId);

        }
    }
    public class PayrollPeriodEntityTypeConfiguration : EntityTypeConfiguration<PayrollPeriod>
    {

        public PayrollPeriodEntityTypeConfiguration()
        {
            this.ToTable("PayrollPeriod", "Payroll");
            this.HasKey<int>(o => o.GLPeriodId);

        }
    }

    public class BackPaymentEntityTypeConfiguration : EntityTypeConfiguration<BackPay>
    {

        public BackPaymentEntityTypeConfiguration()
        {
            this.ToTable("BackPay", "Payroll");
            this.HasKey<int>(o => o.BackPayId);

        }

    }
    public class BackPaymentLineEntityTypeConfiguration : EntityTypeConfiguration<BackPaymentLine>
    {

        public BackPaymentLineEntityTypeConfiguration()
        {
            this.ToTable("BackPaymentLine", "Payroll");
            this.HasKey<int>(o => o.BackPaymentLineId);
            this.Property(o => o.BasicSalary).HasPrecision(18, 9);
            this.Property(o => o.Allowances).HasPrecision(18, 9);
            this.Property(o => o.IncomeTax).HasPrecision(18, 9);
            this.Property(o => o.Pension).HasPrecision(18, 9);
            this.Property(o => o.Net).HasPrecision(18, 9);

        }

    }

    public class BackPaymentDetailEntityTypeConfiguration : EntityTypeConfiguration<BackPaymentDetail>
    {

        public BackPaymentDetailEntityTypeConfiguration()
        {
            this.ToTable("BackPaymentDetail", "Payroll");
            this.HasKey<int>(o => o.BackPaymentDetailId);
            this.Property(o => o.Amount).HasPrecision(18, 9);

        }

    }

    public class BackPaymentDistributionEntityTypeConfiguration : EntityTypeConfiguration<BackPaymentDistribution>
    {

        public BackPaymentDistributionEntityTypeConfiguration()
        {
            this.ToTable("BackPaymentDistribution", "Payroll");
            this.HasKey<int>(o => o.BackPaymentDistributionId);

        }

    }

    //missing payroll back
    public class MissingPayrollBackPayEntityTypeConfiguration : EntityTypeConfiguration<MissingPayrollBackPay>
    {

        public MissingPayrollBackPayEntityTypeConfiguration()
        {
            this.ToTable("MissingPayrollBackPay", "Payroll");
            this.HasKey<int>(o => o.MissingPayrollBackPayId);

        }

    }

    public class MissingPayrollBackPaymentLineEntityTypeConfiguration : EntityTypeConfiguration<MissingPayrollBackPaymentLine>
    {

        public MissingPayrollBackPaymentLineEntityTypeConfiguration()
        {
            this.ToTable("MissingPayrollBackPaymentLine", "Payroll");
            this.HasKey<int>(o => o.MissingPayrollBackPaymentLineId);

        }

    }

    public class MissingPayrollBackPaymentDetailPeriodEntityTypeConfiguration : EntityTypeConfiguration<MissingPayrollBackPaymentDetailPeriod>
    {

        public MissingPayrollBackPaymentDetailPeriodEntityTypeConfiguration()
        {
            this.ToTable("MissingPayrollBackPaymentDetailPeriod", "Payroll");
            this.HasKey<int>(o => o.MissingPayrollBackPaymentDetailPeriodId);

        }

    }

    public class MissingPayrollBackPaymentDetailEntityTypeConfiguration : EntityTypeConfiguration<MissingPayrollBackPaymentDetail>
    {

        public MissingPayrollBackPaymentDetailEntityTypeConfiguration()
        {
            this.ToTable("MissingPayrollBackPaymentDetail", "Payroll");
            this.HasKey<int>(o => o.MissingPayrollBackPaymentDetailId);

        }

    }

    public class MissingPayrollBackPaymentDistributionEntityTypeConfiguration : EntityTypeConfiguration<MissingPayrollBackPaymentDistribution>
    {

        public MissingPayrollBackPaymentDistributionEntityTypeConfiguration()
        {
            this.ToTable("MissingPayrollBackPaymentDistribution", "Payroll");
            this.HasKey<int>(o => o.MissingPayrollBackPaymentDistributionId);

        }

    }

    public class BonusEntityTypeConfiguration : EntityTypeConfiguration<Bonus>
    {

        public BonusEntityTypeConfiguration()
        {
            this.ToTable("Bonus", "Payroll");
            this.HasKey<int>(o => o.BonusId);

        }

    }

    public class ContributionTypeEntityTypeConfiguration : EntityTypeConfiguration<ContributionType>
    {

        public ContributionTypeEntityTypeConfiguration()
        {
            this.ToTable("ContributionType", "HRM");
            this.HasKey<int>(o => o.ContributionTypeId);
        }
    }

    public class PerdimeSettingEntityTypeConfiguration : EntityTypeConfiguration<PerdimeSetting>
    {

        public PerdimeSettingEntityTypeConfiguration()
        {
            this.ToTable("PerdimeSetting", "Payroll");
            this.HasKey<int>(o => o.PerdimeSettingId);
        }
    }

    public class PayrollAccountsEntityTypeConfiguration : EntityTypeConfiguration<PayrollAccounts>
    {

        public PayrollAccountsEntityTypeConfiguration()
        {
            this.ToTable("PayrollAccounts", "Payroll");
            this.HasKey<int>(o => o.PayrollAccountsId);
        }
    }

    public class PayrollEmployeeAccountEntityTypeConfiguration : EntityTypeConfiguration<PayrollEmployeeAccount>
    {
        public PayrollEmployeeAccountEntityTypeConfiguration()
        {
            this.ToTable("PayrollEmployeeAccount", "Payroll");
            this.HasKey<int>(o => o.PayrollEmployeeAccountId);
        }
    }

    public class PayrollDistributionEntityTypeConfiguration : EntityTypeConfiguration<PayrollDistribution>
    {
        public PayrollDistributionEntityTypeConfiguration()
        {
            this.ToTable("PayrollDistribution", "Payroll");
            this.HasKey<int>(o => o.PayrollDistributionId);
            this.Property(o => o.Credit).HasPrecision(18, 9);
            this.Property(o => o.Debit).HasPrecision(18, 9);
        }
    }

    public class PayrollProcessRegularContributionEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessRegularContribution>
    {
        public PayrollProcessRegularContributionEntityTypeConfiguration()
        {
            this.ToTable("PayrollProcessRegularContribution", "Payroll");
            this.HasKey<int>(o => o.PayrollProcessRegularContributionID);
            this.Property(o => o.Amount).HasPrecision(18, 9);

        }
    }

    public class PayrollEmployeeCostCenterEntityTypeConfiguration : EntityTypeConfiguration<PayrollEmployeeCostCenter>
    {
        public PayrollEmployeeCostCenterEntityTypeConfiguration()
        {
            this.ToTable("PayrollEmployeeCostCenter", "Payroll");
            this.HasKey<int>(o => o.PayrollEmployeeCostCenterId);
        }
    }

    public class PeriodStatusEntityTypeConfiguration : EntityTypeConfiguration<PeriodStatus>
    {
        public PeriodStatusEntityTypeConfiguration()
        {
            this.ToTable("PeriodStatus", "Payroll");
            this.HasKey<int>(o => o.PeriodStatusId);
        }
    }

    public class EmpPayrollServiceChargeViewModelEntityTypeConfiguration :
        EntityTypeConfiguration<EmpPayrollServiceChargeViewModel>
    {
        public EmpPayrollServiceChargeViewModelEntityTypeConfiguration()
        {
            this.ToTable("EmpPayrollServiceChargeViewModel", "Payroll");
            this.HasKey<int>(o => o.EmpPayrollServiceChargeViewModelId);
        }

    }

    public class ProvidentFundEntityTypeConfiguration : EntityTypeConfiguration<ProvidentFund>
    {

        public ProvidentFundEntityTypeConfiguration()
        {
            this.ToTable("ProvidentFund", "Payroll");
            this.HasKey<int>(o => o.ProvidentFundId);

        }
    }

    public class PayrollProcessLongTermDeductionsEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessLongTermDeductions>
    {
        public PayrollProcessLongTermDeductionsEntityTypeConfiguration()
        {
            this.ToTable("PayrollProcessLongTermDeductions", "Payroll");
            this.HasKey<int>(o => o.EmpPayrollLongTermDeductionId);
            this.Property(o => o.Amount).HasPrecision(18, 9);
        }
    }

    public class BonusSettingEntityTypeConfiguration : EntityTypeConfiguration<BonusSetting>
    {

        public BonusSettingEntityTypeConfiguration()
        {
            this.ToTable("BonusSetting", "Payroll");
            this.HasKey<int>(o => o.BonusSettingId);

        }

    }

    public class AdvanceEntityTypeConfiguration : EntityTypeConfiguration<Advance>
    {

        public AdvanceEntityTypeConfiguration()
        {
            this.ToTable("Advance", "Payroll");
            this.HasKey<int>(o => o.AdvanceId);
        }
    }

    public class AdditionEntityTypeConfiguration : EntityTypeConfiguration<Addition>
    {

        public AdditionEntityTypeConfiguration()
        {
            this.ToTable("Addition", "Payroll");
            this.HasKey<int>(o => o.AdditionId);

        }

    }

    public class AssociationEntityTypeConfiguration : EntityTypeConfiguration<Association>
    {

        public AssociationEntityTypeConfiguration()
        {
            this.ToTable("Association", "Payroll");
            this.HasKey<int>(o => o.AssociationId);
            this.Property(o => o.Amount).HasPrecision(18, 9);
            this.Property(o => o.MonthlyAmount).HasPrecision(18, 9);
            this.Property(o => o.AdditionalAmount).HasPrecision(18, 9);

        }
    }


    public class FixedContributionEntityTypeConfiguration : EntityTypeConfiguration<FixedContribution>
    {

        public FixedContributionEntityTypeConfiguration()
        {
            this.ToTable("FixedContribution", "Payroll");
            this.HasKey<int>(o => o.FixedContributionId);
            this.Property(o => o.Amount).HasPrecision(18, 9);
            this.Property(o => o.MonthlyAmount).HasPrecision(18, 9);
            this.Property(o => o.AmountPercent).HasPrecision(18, 9);
            this.Property(o => o.Balance).HasPrecision(18, 9);
        }
    }

    public class PayrollProcessAdditionsEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessAdditions>
    {
        public PayrollProcessAdditionsEntityTypeConfiguration()
        {
            this.ToTable("PayrollProcessAdditions", "Payroll");
            this.HasKey<int>(o => o.PayrollProcessAdditionsId);
            this.Property(o => o.Amount).HasPrecision(18, 9);

        }
    }
    public class EmpPayrollDeductionEntityTypeConfiguration : EntityTypeConfiguration<EmpPayrollDeduction>
    {
        public EmpPayrollDeductionEntityTypeConfiguration()
        {
            this.ToTable("EmpPayrollDeduction", "Payroll");
            this.HasKey<int>(o => o.EmpPayrollDeductionId);
        }
    }
    public class PayrollProcessDeductionsEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessDeductions>
    {
        public PayrollProcessDeductionsEntityTypeConfiguration()
        {
            this.ToTable("PayrollProcessDeductions", "Payroll");
            this.HasKey<int>(o => o.PayrollProcessDeductionsId);
            this.Property(o => o.Amount).HasPrecision(18, 9);
            this.Property(o => o.OutstandingAmount).HasPrecision(18, 9);
        }
    }
    public class EmpPayrollLongTermDeductionEntityTypeConfiguration : EntityTypeConfiguration<EmpPayrollLongTermDeduction>
    {
        public EmpPayrollLongTermDeductionEntityTypeConfiguration()
        {
            this.ToTable("EmpPayrollLongTermDeduction", "Payroll");
            this.HasKey<int>(o => o.EmpPayrollLongTermDeductionId);
        }
    }
    public class PayrollProcessLoansEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessLoans>
    {
        public PayrollProcessLoansEntityTypeConfiguration()
        {
            this.ToTable("PayrollProcessLoans", "Payroll");
            this.HasKey<int>(o => o.PayrollProcessLoansId);
            this.Property(o => o.Amount).HasPrecision(18, 9);
            this.Property(o => o.Outstanding).HasPrecision(18, 9);

        }
    }
    public class PayrollProcessAdvancesEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessAdvances>
    {
        public PayrollProcessAdvancesEntityTypeConfiguration()
        {
            this.ToTable("PayrollProcessAdvances", "Payroll");
            this.HasKey<int>(o => o.PayrollProcessAdvancesId);
            this.Property(o => o.Amount).HasPrecision(18, 9);
            this.Property(o => o.Outstanding).HasPrecision(18, 9);
        }
    }
    public class PayrollProcessFixedContributionEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessFixedContribution>
    {
        public PayrollProcessFixedContributionEntityTypeConfiguration()
        {
            this.ToTable("PayrollProcessFixedContribution", "Payroll");
            this.HasKey<int>(o => o.PayrollProcessFixedContributionID);
            this.Property(o => o.Amount).HasPrecision(18, 9);
            this.Property(o => o.Outstanding).HasPrecision(18, 9);
        }
    }

    public class RegularContributionEntityTypeConfiguration : EntityTypeConfiguration<RegularContribution>
    {

        public RegularContributionEntityTypeConfiguration()
        {
            this.ToTable("RegularContribution", "Payroll");
            this.HasKey<int>(o => o.RegContributionId);
            this.Property(o => o.Amount).HasPrecision(18, 9);
            this.Property(o => o.AdditionalAmount).HasPrecision(18, 9);
        }
    }

    public class LoanEntityTypeConfiguration : EntityTypeConfiguration<Loan>
    {

        public LoanEntityTypeConfiguration()
        {
            this.ToTable("Loan", "Payroll");
            this.HasKey<int>(o => o.LoanId);
        }
    }


    public class OverTimeTransferEntityTypeConfiguration : EntityTypeConfiguration<OverTimeTransfer>
    {

        public OverTimeTransferEntityTypeConfiguration()
        {
            this.ToTable("OverTimeTransfer", "Payroll");
            this.HasKey<int>(o => o.OverTimeTransferId);
        }
    }
    public class OverTimeTrasferDetailEntityTypeConfiguration : EntityTypeConfiguration<OverTimeTrasferDetail>
    {

        public OverTimeTrasferDetailEntityTypeConfiguration()
        {
            this.ToTable("OverTimeTrasferDetail", "Payroll");
            this.HasKey<int>(o => o.OverTimeTrasferDetailId);
        }
    }
    public class OverTimePerDayEntityTypeConfiguration : EntityTypeConfiguration<OverTimePerDay>
    {

        public OverTimePerDayEntityTypeConfiguration()
        {
            this.ToTable("OverTimePerDay", "Payroll");
            this.HasKey<string>(o => o.TicketNumber);
        }
    }


    public class PayrollEmployeeCampDayEntityTypeConfiguration : EntityTypeConfiguration<PayrollEmployeeCampDay>
    {

        public PayrollEmployeeCampDayEntityTypeConfiguration()
        {
            this.ToTable("PayrollEmployeeCampDay", "Payrol");
            this.HasKey<int>(o => o.PayrollEmployeeCampDayId);

        }
    }
    public class PayrollCampDailyRateEntityTypeConfiguration : EntityTypeConfiguration<PayrollCampDailyRate>
    {

        public PayrollCampDailyRateEntityTypeConfiguration()
        {
            this.ToTable("PayrollCampDailyRate", "Payrol");
            this.HasKey<int>(o => o.PayrollCampDailyRateId);

        }
    }

    public class PayrollEmployeeCampEntityTypeConfiguration : EntityTypeConfiguration<PayrollEmployeeCamp>
    {

        public PayrollEmployeeCampEntityTypeConfiguration()
        {
            this.ToTable("PayrollEmployeeCamp", "Payrol");
            this.HasKey<int>(o => o.PayrollEmployeeCampId);

        }
    }
}




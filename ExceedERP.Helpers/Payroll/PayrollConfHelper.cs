using ExceedERP.DataAccess.Context;
using System;
using System.Linq;

namespace ExceedERP.Helpers.Payroll
{
    public class PayrollConfHelper
    {
        public static bool ApplyDeductedSalaryForPension()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Payroll")
                    .Where(x => x.Reference == "APPLY_DEDUCTED_SALARY_PENSION")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }
        //public static bool ApplyPeriodDays()
        //{
        //    using (ExceedDbContext db = new ExceedDbContext())
        //    {
        //        var conf = db.Configurations
        //            .Where(x => x.Category == "Payroll")
        //            .Where(x => x.Reference == "APPLY_PERIOD_DAYS")
        //            .Where(x => x.Attribute == "TRUE")
        //            .FirstOrDefault();

        //        return conf != null;
        //    }
        //}

        public static Tuple<bool, string> ApplyPeriodDays()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations.FirstOrDefault(x =>
                    x.Category == "Payroll" &&
                    x.Reference == "APPLY_PERIOD_DAYS");

                if (conf == null)
                    return Tuple.Create(false, (string)null);

                return Tuple.Create(
                    string.Equals(conf.CurrentValue, "TRUE", StringComparison.OrdinalIgnoreCase),
                    conf.CurrentValue
                );
            }
        }

        public static bool ApplyDeductAllowance()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Payroll")
                    .Where(x => x.Reference == "DEDUCT_ALLOWANCE_BY_DAYS_ABSENT")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool AppllyProofSheet()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Payroll")
                    .Where(x => x.Reference == "APPLY_PROOF_SHEET")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool ApplyWorkingDaysToDesert()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Payroll")
                    .Where(x => x.Reference == "DESERT_APPLY_WORKING_DAYS")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static bool ApplyDelegationBenefit()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Payroll")
                    .Where(x => x.Reference == "DELEGATION_BENEFIT_ALLOWED")
                    .Where(x => x.Attribute == "TRUE")
                    .FirstOrDefault();

                return conf != null;
            }
        }

        public static int GetDelegationEligibilityDays()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Payroll")
                    .Where(x => x.Reference == "DELEGATION_BENEFIT_RULE")
                    .Where(x => x.Attribute == "ELIGIBILITY_DAYS")
                    .FirstOrDefault();

                int.TryParse(conf?.CurrentValue, out int days);
                return days;
            }
        }

        public static int GetDelegationBasicSalaryPercentage()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Payroll")
                    .Where(x => x.Reference == "DELEGATION_BENEFIT_RULE")
                    .Where(x => x.Attribute == "BASIC_SALARY_DIFFERENCE_PERCENT")
                    .FirstOrDefault();

                int.TryParse(conf?.CurrentValue, out int percent);
                return percent;
            }
        }

        public static int GetDelegationAllowancePercentage()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var conf = db.Configurations
                    .Where(x => x.Category == "Payroll")
                    .Where(x => x.Reference == "DELEGATION_BENEFIT_RULE")
                    .Where(x => x.Attribute == "ALLOWANCE_PERCENT")
                    .FirstOrDefault();

                int.TryParse(conf?.CurrentValue, out int percent);
                return percent;
            }
        }

        public static int GetValidDelegatorEmployeeId(int empId, int days, DateTime date)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var delegation = db.Delegations
                    .Where(x => x.EmployeeId == empId)
                    .Where(x => x.IsWithBenefit)
                    .FirstOrDefault();

                if (delegation != null && delegation.FromDateTime != null && delegation.FromEmployeeId != 0)
                {
                    int dDays = (date - (DateTime)delegation.FromDateTime).Days;
                    if (dDays >= days)
                        return delegation.FromEmployeeId;
                }

                return 0;
            }
        }

        public static bool HasGivenActiveDelegation(int empId, DateTime fromdate, DateTime todate)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var delegation = db.Delegations
                    .Where(x => x.FromEmployeeId == empId)
                    .Where(x => x.FromDateTime <= fromdate)
                    .Where(x => x.IsWithBenefit)
                    .Where(x => x.ToDateTime >= todate || x.ToDateTime == null)
                    .FirstOrDefault();

                return delegation != null;
            }
        }
    }

}

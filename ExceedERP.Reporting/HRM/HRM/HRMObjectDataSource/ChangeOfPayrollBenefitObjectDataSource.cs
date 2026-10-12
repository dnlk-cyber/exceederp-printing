using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
   public class ChangeOfPayrollBenefitObjectDataSource:IDisposable
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ChangeOfPayrollBenefitVM> GetBenefits(string id)
        {
            List<ChangeOfPayrollBenefitVM> changeOfPayrollBenefits = new List<ChangeOfPayrollBenefitVM>();
            if (!string.IsNullOrEmpty(id))
            {
                int did = int.Parse(id);
                var placement = db.Placements.Find(did);
                if (placement != null)
                {

                    var allowanceperEmployees = db.AllowancePerEmployees.Where(x => x.EmployeeId == placement.EmployeeId);
                    foreach (var emp in allowanceperEmployees)
                    {
                        var allowance = new ChangeOfPayrollBenefitVM();
                        var allowanceDefinition = db.AllowanceDefinations.Find(emp.AllowanceDefinationId);
                        if (allowanceDefinition != null)
                        {
                            allowance.BenfitType = allowanceDefinition.AllowanceType.ToString();
                            allowance.Name = allowanceDefinition.AllowanceName;
                            allowance.MeasureType = allowanceDefinition.AmtPercent.ToString();
                            allowance.Amount = allowanceDefinition.Amount;
                            changeOfPayrollBenefits.Add(allowance);

                        }
                    }
                }
            }
            return changeOfPayrollBenefits;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}

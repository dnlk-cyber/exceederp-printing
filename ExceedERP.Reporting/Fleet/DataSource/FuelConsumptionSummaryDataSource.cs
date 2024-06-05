using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
        public  class FuelConsumptionSummaryDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FuelConsumptionSummary> GetFuelConsumptionInfo(DateTime StartingDate, DateTime EndingDate)
        {
            List<FuelConsumptionSummary> FuelConsumptionList = new List<FuelConsumptionSummary>();
              GLPeriod Periods = new GLPeriod();
              List<Branch> Branchs = db.Branch.ToList();




              Periods =
                           db.GLPeriods.Where(x =>
                                   x.DateFrom >= StartingDate && x.DateTo <= EndingDate
                                   ).FirstOrDefault();
 
            foreach (var Branch in Branchs)
                {


                    decimal Workingcost = 0;
                    decimal Workingtime = 0;
                    string TypeOfWork = string.Empty;
                                      // filter by fuel type 
                    IQueryable<Issue> FuelIssues  =
                        db.Issues.Where(
                            x => x.IsFuel == true && x.BranchId == Branch.BranchId &&  x.DateCreated >= StartingDate && x.DateCreated <= EndingDate);

                               var    FuelConsumptionViewModel = new FuelConsumptionSummary();

                    FuelConsumptionViewModel.ProjectName = Branch.Name;
             //   FuelConsumptionViewModel.Amount = FuelIssues.Sum(x=>x.IssueItems.)

                }

            return FuelConsumptionList;

        }
    }
}


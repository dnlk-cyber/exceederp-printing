using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Insurance.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Insurance.DataSource
{
    [DataObject]
    class YearlyAccidentFromProjectObjectDataSource
    {

        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<YearlyAccidentFromProjectViewModel> GetProjectAccident(string year)
        {
            List<YearlyAccidentFromProjectViewModel> accidentList = new List<YearlyAccidentFromProjectViewModel>();

            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId.ToString() == year);
             if(fiscalYear != null)
            {
                var location = "";
                var accidents = db.Accident.Where(x => x.DateOfOccurrence >= fiscalYear.DateFrom && x.DateOfOccurrence <= fiscalYear.DateTo && x.IsSendForApproval).ToList();
                foreach (var accident in accidents)
                {
                    if (accident.AccidentLocation == location)
                    {
                        continue;
                    }
                    var accid = new YearlyAccidentFromProjectViewModel();                    
                   
                    accid.ProjectName = accident.AccidentLocation;
                     location = accid.ProjectName;
                
                    accid.YearlyAccidentFromProjectListViewModel = new List<YearlyAccidentFromProjectListViewModel>();
                    var acc = new YearlyAccidentFromProjectListViewModel();
                    foreach (var a in accidents.Where(x=>x.AccidentLocation == accident.AccidentLocation))
                    {
                       
                        acc.AccidentCarPlateNo = acc.AccidentCarPlateNo + "፤ " + a.PlateNo;
                        acc.NoOfAccident = accidents.Where(x => x.AccidentLocation == accident.AccidentLocation).ToList().Count();

                      
                    }
                    accid.YearlyAccidentFromProjectListViewModel.Add(acc);
                    accidentList.Add(accid);
                }
            }
            


            return accidentList;
        }
    }
}

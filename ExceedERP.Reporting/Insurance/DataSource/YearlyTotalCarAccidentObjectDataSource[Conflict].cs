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
    class YearlyTotalCarAccidentObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<YearlyTotalCarAccident> GetYearlyAccidnet(string year)
        {
            List<YearlyTotalCarAccident> accidentList = new List<YearlyTotalCarAccident>();

            var period = db.GLFiscalYears.FirstOrDefault(x=>x.GlFiscalYearId.ToString() == year);
            if(period != null)
            {
                var accidents = db.Accident.Where(x => x.IsSendForApproval && x.DateOfOccurrence >= period.DateFrom && x.DateOfOccurrence <= period.DateTo).ToList();
                foreach (var accident in accidents)
                {
                    var yearlyAccident = new YearlyTotalCarAccident
                    {
                        Equipment = accident.Equipment,
                        CauseOfAccident = accident.CauseOfAccident,
                        AccidentLevel = accident.LevelOfDamage,
                        ThirdPartyAccident = accident.ThirdPartyDamage
                    };
                    accidentList.Add(yearlyAccident);
                }
            }
           

            return accidentList;
        }
    }
}

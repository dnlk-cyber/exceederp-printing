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
    public class AccidentWithMoneyObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccidentWithMoneyViewModel> GetAccident(string year)
        {
            List<AccidentWithMoneyViewModel> accidentList = new List<AccidentWithMoneyViewModel>();
            var accid = new AccidentWithMoneyViewModel();


            var location = "";
            if (year != null)
            {
                var glyear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId.ToString() == year);
                if (glyear != null)
                {
                   var yr = db.GLFiscalYears.FirstOrDefault(x=>x.DateFrom.Year == (glyear.DateFrom.Year-1));
                    if(yr != null)
                    {
                        accid.PreviousYear = yr.Name;
                    }
                   
                    accid.CurrentYear = glyear.Name;
                    var accidentTotal = db.Accident.ToList();
                    var accidents = accidentTotal.Where(x => x.DateOfOccurrence >= glyear.DateFrom && x.DateOfOccurrence <= glyear.DateTo).ToList();
                    foreach (var accident in accidents)
                    {
                        if (accident.AccidentLocation == location)
                        {
                            continue;
                        }
                        accid = new AccidentWithMoneyViewModel();
                        location = accident.AccidentLocation;
                        accid.ProjectName = accident.AccidentLocation;
                        var accidentNo = accidents.Where(x => x.AccidentLocation == accident.AccidentLocation).Count();
                        var accidentCost = accidents.Where(x => x.AccidentLocation == accident.AccidentLocation).ToList();
                        int? cost = 0;
                        int? prevCost = 0;
                        foreach (var cos in accidentCost)
                        {
                            cost = cost + cos.CostcoveredbyCompany + cos.CostcoveredbyInsurance + cos.CostcoveredbyOperator + cos.CostcoveredbyOtherBodies;
                        }
                        accid.VehicleAccidentCurrentYear = accidentCost.Count();
                        accid.MoneyCurrent = Convert.ToDecimal(cost);
                        var previousAccidents = accidentTotal.Where(x => x.AccidentLocation == accident.AccidentLocation && x.DateOfOccurrence.Year == (glyear.DateFrom.Year - 1));
                        foreach (var prev in previousAccidents)
                        {
                            prevCost = prevCost + prev.CostcoveredbyCompany + prev.CostcoveredbyInsurance + prev.CostcoveredbyOperator + prev.CostcoveredbyOtherBodies;
                        }

                        accid.VehicleAccidentPreviosYear = previousAccidents.Count();
                        accid.MoneyPrevious = Convert.ToDecimal(prevCost);
                        if (accid.MoneyCurrent > accid.MoneyPrevious)
                        {
                            if (accid.VehicleAccidentCurrentYear > accid.VehicleAccidentPreviosYear)
                            {
                                accid.Difference = (accid.MoneyCurrent - accid.MoneyPrevious) + "(በገንዘብም በጉዳትም የጨመረ )";
                            }
                            else if (accid.VehicleAccidentCurrentYear < accid.VehicleAccidentPreviosYear)
                            {
                                accid.Difference = (accid.MoneyCurrent - accid.MoneyPrevious) + "(በገንዘብ የጨመረ በጉዳትም የቀነሰ )";
                            }
                            else
                            {
                                accid.Difference = (accid.MoneyCurrent - accid.MoneyPrevious) + "(በገንዘብ የጨመረ በጉዳትም እኩል የሆነ )";
                            }

                        }
                        if (accid.MoneyPrevious > accid.MoneyCurrent)
                        {
                            if (accid.VehicleAccidentPreviosYear > accid.VehicleAccidentCurrentYear)
                            {
                                accid.Difference = (accid.MoneyCurrent - accid.MoneyPrevious) + "(በገንዘብም በጉዳትም የቀነሰ  )";
                            }
                            else if (accid.VehicleAccidentCurrentYear > accid.VehicleAccidentPreviosYear)
                            {
                                accid.Difference = (accid.MoneyCurrent - accid.MoneyPrevious) + "(በገንዘብ የቀነሰ በጉዳትም የጨመረ )";
                            }
                            else
                            {
                                accid.Difference = (accid.MoneyCurrent - accid.MoneyPrevious) + "(በገንዘብ የቀነሰ በጉዳትም እኩል የሆነ )";
                            }

                        }
                        else
                        {
                            if (accid.VehicleAccidentPreviosYear > accid.VehicleAccidentCurrentYear)
                            {
                                accid.Difference = (accid.MoneyCurrent - accid.MoneyPrevious) + "(በገንዘብም እኩል የሆነ በጉዳትም የቀነሰ  )";
                            }
                            else if (accid.VehicleAccidentCurrentYear > accid.VehicleAccidentPreviosYear)
                            {
                                accid.Difference = (accid.MoneyCurrent - accid.MoneyPrevious) + "(በገንዘብ እኩል የሆነ በጉዳትም የጨመረ )";
                            }
                            else
                            {
                                accid.Difference = (accid.MoneyCurrent - accid.MoneyPrevious) + "(በገንዘብም በጉዳትም እኩል የሆነ )";
                            }
                        }
                        accidentList.Add(accid);
                    }
                }
            }

            return accidentList;
        }


    }
}

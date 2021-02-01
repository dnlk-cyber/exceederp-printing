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
    public class AccidentCoveredByInsuranceAndAskedForInsuranceObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccidentCoveredByInsuranceAndAskedForInsuranceViewModel> GetInsuredAndAskedforInsuranceAccidents(string year)
        {
            List<AccidentCoveredByInsuranceAndAskedForInsuranceViewModel> accidentInsuredList = new List<AccidentCoveredByInsuranceAndAskedForInsuranceViewModel>();
            if (year != null)
            {
                var equipments = db.EquipmentIdentity.ToList();

                var eqId = "";
                var glyear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId.ToString() == year);
                if (glyear != null)
                {
                    int count = 0;
                    int? cost = 0;
                    var totalAccident = db.Accident.ToList();
                    //var accidents = totalAccident.Where(x => x.DateOfOccurrence >= glyear.DateFrom && x.DateOfOccurrence <= glyear.DateTo).ToList();
                    foreach (var accidents in totalAccident)
                    {
                        var accidentInsured = new AccidentCoveredByInsuranceAndAskedForInsuranceViewModel();
                        var accident = totalAccident.FirstOrDefault(x => x.AccidentID == accidents.AccidentID && x.DateOfOccurrence >= glyear.DateFrom && x.DateOfOccurrence <= glyear.DateTo);
                        if (accident != null)
                        {
                          
                            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == accident.FleetsID);
                            if (fleet != null)
                            {
                                var equipment = equipments.FirstOrDefault(x => x.EquipmentIdentityID == fleet.EquipmentIdentityID);
                                if (equipment != null)
                                {
                                    if (equipment.EquipmentIdentityID.ToString() == eqId)
                                    {
                                        continue;
                                    }
                                    eqId = equipment.EquipmentIdentityID.ToString();
                                    var fleetWithCatagories = db.Fleets.Where(x => x.EquipmentIdentityID == fleet.EquipmentIdentityID);
                                    foreach (var fl in fleetWithCatagories)
                                    {
                                        var fleetAcc = totalAccident.FirstOrDefault(x => x.FleetsID == fl.FleetsID);
                                        if (fleetAcc != null)
                                        {
                                            count++;
                                            cost = cost + fleetAcc.CostcoveredbyCompany + fleetAcc.CostcoveredbyInsurance + fleetAcc.CostcoveredbyOperator + fleetAcc.CostcoveredbyOtherBodies;
                                        }
                                    }
                                    accidentInsured.InsuranceTypeCurrentYear = count + " " + equipment.EquipmentCategoryName;
                                    accidentInsured.PaymentCurrentYear = cost;
                                }

                            }
                        }
                        ////////////////////////////////////////////////////////////////////////
                        var accPrev = totalAccident.FirstOrDefault(x => x.AccidentID == accident.AccidentID && (x.DateOfOccurrence.Year == glyear.DateFrom.Year-1 || x.DateOfOccurrence.Year <= glyear.DateTo.Year-1));
                        var accInsured = new AccidentCoveredByInsuranceAndAskedForInsuranceViewModel();
                        var accFleet = db.Fleets.FirstOrDefault(x => x.FleetsID == accident.FleetsID);
                        if (accFleet != null)
                        {
                            var equipment = equipments.FirstOrDefault(x => x.EquipmentIdentityID == accFleet.EquipmentIdentityID);
                            if (equipment != null)
                            {
                                if (equipment.EquipmentIdentityID.ToString() == eqId)
                                {
                                    continue;
                                }
                                eqId = equipment.EquipmentIdentityID.ToString();
                                var accfleetWithCatagories = db.Fleets.Where(x => x.EquipmentIdentityID == accFleet.EquipmentIdentityID);
                                foreach (var fl in accfleetWithCatagories)
                                {
                                    var fleetAcc = totalAccident.FirstOrDefault(x => x.FleetsID == fl.FleetsID);
                                    if (fleetAcc != null)
                                    {
                                        count++;
                                        cost = cost + fleetAcc.CostcoveredbyCompany + fleetAcc.CostcoveredbyInsurance + fleetAcc.CostcoveredbyOperator + fleetAcc.CostcoveredbyOtherBodies;
                                    }
                                }
                                accidentInsured.InsuranceTypeCurrentYear = count + " " + equipment.EquipmentCategoryName;
                                accidentInsured.PaymentCurrentYear = cost;
                            }
                            if(accidentInsured.PaymentCurrentYear > accidentInsured.PaymentPreviousYear)
                            {
                                accidentInsured.Difference = (accidentInsured.PaymentCurrentYear - accidentInsured.PaymentPreviousYear).ToString();
                            }
                            else
                            {
                                accidentInsured.Difference = (accidentInsured.PaymentCurrentYear - accidentInsured.PaymentPreviousYear).ToString();
                            }
                        }


                        accidentInsuredList.Add(accidentInsured);
                    }
                }
            }
            return accidentInsuredList;
        }
    }
}

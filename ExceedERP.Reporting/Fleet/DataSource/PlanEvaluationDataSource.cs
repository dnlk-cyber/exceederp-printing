using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;
using ExceedERP.Reporting.SharedDataSources;
using System.Drawing;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class PlanEvaluationDataSource
    {



        ExceedDbContext db = new ExceedDbContext();

   [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PlanEvaluationViewModel> GetPlanEvaluation(string Identity, string category, string Manufacturer,
           DateTime startingDate)
        {
           List<PlanEvaluationViewModel> FleetList = new List<PlanEvaluationViewModel>();
            FleetViewModel FleetViewModel;
            List<Fleets> Fleets;
            List<EquipmentPlanandEvaluations> Issues;
            DateTime endingDate;
            var product = db.Products.ToList();





            string AmharicName = "";
            string Name = "";
            string Address = "";
            Image LogoFileName = null;

            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {

                        if (Address == null)
                        {
                            Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }



            if (!string.IsNullOrEmpty(Identity))
            {
                if (!string.IsNullOrEmpty(category))
                {
                    Fleets =
                        db.Fleets.Where(
                            so =>
                                so.EquipmentIdentityID.ToString() == Identity &&
                                so.EquipmentTypeID.ToString() == category)
                            .ToList();
                }
                else
                {
                    Fleets =
                        db.Fleets.Where(so => so.EquipmentTypeID.ToString() == category)
                            .ToList();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Manufacturer))
                {
                    Fleets = db.Fleets.Where(so => so.ManufacturerYear == Manufacturer).ToList();
                }
                else
                {
                    Fleets = db.Fleets.ToList();
                }
               
            }
            if (Fleets.Any())
            {
                List<Fleets> List = Fleets;

                foreach (Fleets sov in List)
                {

                    decimal actualDowntime = 0;
                    decimal actualFuelConsumption = 0;
                    int actualIdletime = 0;
                    int actualWorkingtime = 0;
                    endingDate = startingDate.AddYears(1);
                    List<Fuel> fuel = db.Fuel.Where(x => x.FleetsID == sov.FleetsID && x.DateCreated >= startingDate && x.DateCreated <= endingDate).ToList();
                    List<TimeSheet> timeSheet = db.TimeSheet.Where(x => x.FleetsID == sov.FleetsID && x.DateCreated >= startingDate && x.DateCreated <= endingDate).ToList();

                    if(fuel.Any())
                    {
                        actualFuelConsumption = fuel.Sum(x => x.FuelFilledAmount);

                    }
                    if (timeSheet.Any())
                    {
                        foreach(var ts in timeSheet )
                        {
                            actualWorkingtime = actualWorkingtime + (DateTime.Parse(ts.WorkingHoursMorningto.ToString()).Hour - DateTime.Parse(ts.WorkingHoursMorningFrom.ToString()).Hour) + (DateTime.Parse(ts.WorkingHoursAfternoonTo.ToString()).Hour - DateTime.Parse(ts.WorkingHoursAfternoonFrom.ToString()).Hour);// + (ts.WorkingHoursAfternoonTo.Value - ts.WorkingHoursAfternoonFrom.Value);
                        }
                        actualIdletime = timeSheet.Sum(x => x.IdleTime);
                        actualDowntime = timeSheet.Sum(x => x.DownTime);
                        actualWorkingtime = timeSheet.Sum(x => x.DownTime);

                    }

                    List<EquipmentPlanandEvaluations> plans = db.EquipmentPlanandEvaluation.Where(x => x.EquipmentCategoryId == sov.FleetsID && x.DateCreated > startingDate && x.DateCreated < endingDate).ToList();
                    if (plans.Any())
                    {
                        foreach (var p in plans)
                        {
                            var PlanEvaluationViewModel = new PlanEvaluationViewModel();

                            PlanEvaluationViewModel.Equipment = sov.PlateNumber + " - " + sov.EquipmentName;
                            PlanEvaluationViewModel.PlanDowntime = decimal.Parse(p.PlanDowntime.ToString());
                            PlanEvaluationViewModel.PlanEquipmentsPittyCashExpenditure = decimal.Parse(p.PlanEquipmentsPittyCashExpenditure.ToString());
                            PlanEvaluationViewModel.PlanFuelConsumption = decimal.Parse(p.PlanFuelConsumption.ToString());
                        
                            PlanEvaluationViewModel.PlanIdletime = decimal.Parse(p.PlanIdletime.ToString());
                            PlanEvaluationViewModel.Description = sov.PlateNumber + " - " + sov.EquipmentName;
                            PlanEvaluationViewModel.PlanIncome = decimal.Parse(p.PlanIncome.ToString());
                            PlanEvaluationViewModel.PlanWorkingtime = decimal.Parse(p.PlanWorkingtime.ToString());
                            PlanEvaluationViewModel.PlanTyreUtilization = decimal.Parse(p.PlanTyreUtilization.ToString());
                            PlanEvaluationViewModel.ActualFuelConsumption = actualFuelConsumption;
                            PlanEvaluationViewModel.ActualPlanDowntime = actualDowntime;
                            PlanEvaluationViewModel.ActualPlanIdletime = actualIdletime;
                            PlanEvaluationViewModel.ActualPlanWorkingtime = actualWorkingtime;
                            PlanEvaluationViewModel.UnitOfMeasurement = p.UnitOfMeasurement;


                            PlanEvaluationViewModel.AmharicName = AmharicName;
                            PlanEvaluationViewModel.Name = Name;
                            PlanEvaluationViewModel.LogoFileName = LogoFileName;
                            PlanEvaluationViewModel.Address = Address;

                            FleetList.Add(PlanEvaluationViewModel);
                        }
                    }
                }
            }


            return FleetList;     
        }
       
    }
}

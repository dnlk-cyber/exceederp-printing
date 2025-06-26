using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class FleetUtilizationSummaryOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        //public List<FleetUtilizationSummaryViewModel> GetUtilization(string glFiscalYearId, string glPeriodId, string week)
        public List<FleetUtilizationSummaryViewModel> GetUtilization(string glFiscalYearId, DateTime From,DateTime To)
        {
            List<FleetUtilizationSummaryViewModel> summaryList = new List<FleetUtilizationSummaryViewModel>();
            var Datefrom = From;
            var Dateto = To;
            if (int.TryParse(glFiscalYearId, out int fiscalYearId))
            {
                var plans = db.FleetAnnualPlans.Where(x => x.GlFiscalYearId == fiscalYearId).ToList();
                var planDetails = db.FleetAnnualPlanDetails.ToList();
                //planDetails = planDetails.Where(x => x.DateFrom== Datefrom && x.DateTo== Dateto).ToList();
                planDetails = planDetails
                        .Where(filteredPlan => db.GLPeriods
                        .Any(gl => gl.GLPeriodId == filteredPlan.GlPeriodId
                        && gl.DateFrom >= Datefrom
                        && gl.DateFrom <= Dateto))
                        .ToList();

                var joinedPlans = from plan in plans
                                  join detail in planDetails
                                  on plan.FleetAnnualPlanId equals detail.FleetAnnualPlanId
                                  select new
                                  {
                                      GlFiscalYearId = plan.GlFiscalYearId,
                                      EquipmentTypeId = plan.EquipmentTypeId,
                                      FleetId = plan.FleetId,
                                      PlanHour = detail.ExpectedEffectiveWorkingHrPerMonth,
                                      GlPeriodId = detail.GlPeriodId,
                                      Quantity = detail.NumberOfEquipmentInMonth

                                  };


                var groupedPlans = joinedPlans.GroupBy(g => new { g.EquipmentTypeId, g.GlPeriodId })
                    .Select(s => new
                    {
                        s.Key.EquipmentTypeId,
                        s.Key.GlPeriodId,
                        PlanedHour = s.Sum(x => (decimal?)x.PlanHour) ?? 0,
                        Quantity = s.Sum(x => (decimal?)x.Quantity) ?? 0
                        //Quantity = s.Select(x => x.FleetId).Distinct(),
                    }).ToList();

                foreach (var groupedPlan in groupedPlans)
                {
                    var equipmentType = db.EquipmentType.Find(groupedPlan.EquipmentTypeId);
                    //var month = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == groupedPlan.GlPeriodId)?.Name;
                    if (equipmentType != null)
                    {
                        var ut = new FleetUtilizationSummaryViewModel
                        {
                            EquipmentTypeName = equipmentType.EquipmentTypeName,
                            PlanQuantity = (int)groupedPlan.Quantity,
                            PlannedMonthlyHour = groupedPlan.PlanedHour / 4,
                            ActualMonthlyAvaliableQuantity = GetActualQuantity(groupedPlan.EquipmentTypeId, groupedPlan.GlPeriodId,Datefrom,Dateto),
                            ActualMonthlyOperationalHour = GetActualOperationalHour(groupedPlan.EquipmentTypeId, groupedPlan.GlPeriodId, Datefrom, Dateto),
                            ActualMonthlyDownHour = GetDownHour(groupedPlan.EquipmentTypeId, groupedPlan.GlPeriodId, Datefrom,Dateto),
                            ActualMonthlyIdleHour = GetIdleHour(groupedPlan.EquipmentTypeId, groupedPlan.GlPeriodId, Datefrom,Dateto),
                            PlanVsActualDownHourRatio = 0,
                            PlanVsActualHourRatio = 0,
                            PlanVsActualIdleHourRatio = 0
                        };
                        if (ut.PlannedMonthlyHour != 0)
                        {
                            ut.PlanVsActualHourRatio = (ut.ActualMonthlyOperationalHour * 100) / ut.PlannedMonthlyHour;
                            ut.PlanVsActualIdleHourRatio = (ut.ActualMonthlyIdleHour * 100) / ut.PlannedMonthlyHour;
                            ut.PlanVsActualDownHourRatio = (ut.ActualMonthlyDownHour * 100) / ut.PlannedMonthlyHour;
                        }
                        summaryList.Add(ut);
                    }
                }

            }
            return summaryList;

            }
            //public List<FleetUtilizationSummaryViewModel> GetUtilization(string glFiscalYearId, string glPeriodId, string week)
        //{
        //    List<FleetUtilizationSummaryViewModel> summaryList = new List<FleetUtilizationSummaryViewModel>();
        //    if (int.TryParse(glFiscalYearId, out int fiscalYearId) && int.TryParse(glPeriodId, out int periodId) && int.TryParse(week, out int weekVal))
        //    {
        //        var plans = db.FleetAnnualPlans.Where(x => x.GlFiscalYearId == fiscalYearId).ToList();
        //        var planDetails = db.FleetAnnualPlanDetails.ToList();

        //        planDetails = planDetails.Where(x => x.GlPeriodId == periodId).ToList();
        //        var joinedPlans = from plan in plans
        //                          join detail in planDetails
        //                          on plan.FleetAnnualPlanId equals detail.FleetAnnualPlanId
        //                          select new
        //                          {
        //                              GlFiscalYearId = plan.GlFiscalYearId,
        //                              EquipmentTypeId = plan.EquipmentTypeId,
        //                              FleetId = plan.FleetId,
        //                              PlanHour = detail.ExpectedEffectiveWorkingHrPerMonth,
        //                              GlPeriodId = detail.GlPeriodId,
        //                              Quantity = detail.NumberOfEquipmentInMonth

        //                          };


        //        var groupedPlans = joinedPlans.GroupBy(g => new { g.EquipmentTypeId, g.GlPeriodId })
        //            .Select(s => new
        //            {
        //                s.Key.EquipmentTypeId,
        //                s.Key.GlPeriodId,
        //                PlanedHour = s.Sum(x => (decimal?)x.PlanHour) ?? 0,
        //                Quantity = s.Sum(x => (decimal?)x.Quantity) ?? 0
        //                //Quantity = s.Select(x => x.FleetId).Distinct(),
        //            }).ToList();

        //        foreach (var groupedPlan in groupedPlans)
        //        {
        //            var equipmentType = db.EquipmentType.Find(groupedPlan.EquipmentTypeId);
        //            var month = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == groupedPlan.GlPeriodId)?.Name;
        //            if (equipmentType != null && month != null)
        //            {
        //                var ut = new FleetUtilizationSummaryViewModel
        //                {
        //                    EquipmentTypeName = equipmentType.EquipmentTypeName,
        //                    PlanQuantity = (int)groupedPlan.Quantity,
        //                    PlannedMonthlyHour = groupedPlan.PlanedHour / 4,
        //                    ActualMonthlyAvaliableQuantity = GetActualQuantity(groupedPlan.EquipmentTypeId, groupedPlan.GlPeriodId, weekVal),
        //                    ActualMonthlyOperationalHour = GetActualOperationalHour(groupedPlan.EquipmentTypeId, groupedPlan.GlPeriodId, weekVal),
        //                    ActualMonthlyDownHour = GetDownHour(groupedPlan.EquipmentTypeId, groupedPlan.GlPeriodId, weekVal),
        //                    ActualMonthlyIdleHour = GetIdleHour(groupedPlan.EquipmentTypeId, groupedPlan.GlPeriodId, weekVal),
        //                    PlanVsActualDownHourRatio = 0,
        //                    PlanVsActualHourRatio = 0,
        //                    PlanVsActualIdleHourRatio = 0
        //                };
        //                if (ut.PlannedMonthlyHour != 0)
        //                {
        //                    ut.PlanVsActualHourRatio = (ut.ActualMonthlyOperationalHour * 100) / ut.PlannedMonthlyHour;
        //                    ut.PlanVsActualIdleHourRatio = (ut.ActualMonthlyIdleHour * 100) / ut.PlannedMonthlyHour;
        //                    ut.PlanVsActualDownHourRatio = (ut.ActualMonthlyDownHour * 100) / ut.PlannedMonthlyHour;
        //                }
        //                summaryList.Add(ut);
        //            }
        //        }

        //    }
        //    return summaryList;

        //}

        #region GetIdleHour
        private decimal GetIdleHour(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            decimal totalDownHour = 0;
            var equipmentType = db.EquipmentType.Find(equipmentTypeId);
            if (equipmentType != null)
            {
                switch (equipmentType.TimeSheetType)
                {
                    case TimeSheetType.DryLoading:
                        totalDownHour = GetDryloadIdleHour(equipmentTypeId, glPeriodId, Datefrom,Dateto);
                        break;
                    case TimeSheetType.DumpTruck:
                        totalDownHour = GetDumpTruckIdleHour(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;
                    case TimeSheetType.LightVehicle:
                        totalDownHour = GetLightVehicleIdleHour(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;
                    case TimeSheetType.Machinery:
                        totalDownHour = GetMachinaryIdle(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;
                    case TimeSheetType.WagonDrill:
                        totalDownHour = GetWagonDrillIdleHour(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;

                }
            }
            return totalDownHour;
        }

        private decimal GetDryloadIdleHour(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            // Retrieve the GL period from the database
            //var glPeriod = db.GLPeriods.Find(glPeriodId);
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Join the filtered DryLoadingTimeSheets with the fleet IDs
            var dryLoad = db.DryLoadingTimeSheets.Where(x => x.TimeSheetApproved)
           .Where(x => fleetIds.Contains(x.FleetsID))
           .Where(x => DbFunctions.TruncateTime(x.TripStartDate) >= startDate && DbFunctions.TruncateTime(x.TripStartDate) <= endDate).ToList();

            switch (EndDateCompare)
            {
                case 0:
                    dryLoad = dryLoad.Where(x => x.TripStartDate.Date >= startDate && x.TripStartDate.Date < Dateto).ToList();
                    break;
                case 1:
                    dryLoad = dryLoad.Where(x => x.TripStartDate.Date >= Datefrom && x.TripStartDate < Dateto).ToList();
                    break;
                case 2:
                    dryLoad = dryLoad.Where(x => x.TripStartDate.Date >= startDate && x.TripStartDate < endDate).ToList();
                    break;
                case 3:
                    dryLoad = dryLoad.Where(x => x.TripStartDate >= startDate&& x.TripStartDate < Dateto).ToList();
                    break;
                case 4:
                    dryLoad = dryLoad.Where(x => x.TripStartDate >= startDate&& x.TripStartDate < Dateto).ToList();
                    break;
            }
            return dryLoad.Sum(s => (decimal?)s.IdleHour) ?? 0;
        }

        private decimal GetDumpTruckIdleHour(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }

            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range

            var dumpTruck = db.DumpTruckTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList();
               
            switch (EndDateCompare)
            {
                case 0:
                    dumpTruck = dumpTruck.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    dumpTruck = dumpTruck.Where(x => x.Date.Date >= Datefrom && x.Date <Dateto).ToList();
                    break;
                case 2:
                    dumpTruck = dumpTruck.Where(x => x.Date.Date >= startDate && x.Date < endDate).ToList();
                    break;
                case 3:
                    dumpTruck = dumpTruck.Where(x => x.Date >= startDate && x.Date < Dateto).ToList();
                    break;
                case 4:
                    dumpTruck = dumpTruck.Where(x => x.Date >= Datefrom && x.Date < endDate).ToList();
                    break;
            };

            return dumpTruck.Sum(s => (decimal?)s.IdleHour) ?? 0;
        }
        private decimal GetMachinaryIdle(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }


            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();


            var machine = db.MachineryWorkingTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(x => fleetIds.Contains(x.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList();
                
            switch (EndDateCompare)
            {
                case 0:
                    machine = machine.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    machine = machine.Where(x => x.Date.Date >= Datefrom && x.Date < Dateto).ToList();
                    break;
                case 2:
                    machine = machine.Where(x => x.Date.Date >= startDate && x.Date < Dateto).ToList();
                    break;
                case 3:
                    machine = machine.Where(x => x.Date >= startDate && x.Date < endDate).ToList();
                    break;
                case 4:
                    machine = machine.Where(x => x.Date >= Datefrom && x.Date < endDate).ToList();
                    break;
            };

            return machine.Sum(s => (decimal?)s.IdleHours) ?? 0;
        }
        private decimal GetLightVehicleIdleHour(Guid equipmentTypeId, int glPeriodId, DateTime Dateto,DateTime Datefrom)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }

            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range

            var lightVehicle = db.LightVehicleTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(x => fleetIds.Contains(x.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList();
                

            switch (EndDateCompare)
            {
                case 0:
                    lightVehicle = lightVehicle.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    lightVehicle = lightVehicle.Where(x => x.Date.Date >= Datefrom && x.Date < Dateto).ToList();
                    break;
                case 2:
                    lightVehicle = lightVehicle.Where(x => x.Date.Date >= startDate && x.Date < endDate).ToList();
                    break;
                case 3:
                    lightVehicle = lightVehicle.Where(x => x.Date >= startDate && x.Date < Dateto).ToList();
                    break;
                case 4:
                    lightVehicle = lightVehicle.Where(x => x.Date >= Datefrom && x.Date < endDate).ToList();
                    break;
            };
            return lightVehicle.Sum(s => (decimal?)s.IdleHours) ?? 0;
        }
        private decimal GetWagonDrillIdleHour(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }

            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range

            var wagon = db.WagonDrillTimesheets.Where(x => x.TimeSheetApproved)
                .Where(x => fleetIds.Contains(x.FleetsID))
                .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList();
                
            switch (EndDateCompare)
            {
                case 0:
                    wagon = wagon.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    wagon = wagon.Where(x => x.Date.Date >= Datefrom && x.Date < Dateto).ToList();
                    break;
                case 2:
                    wagon = wagon.Where(x => x.Date.Date >= startDate && x.Date < endDate).ToList();
                    break;
                case 3:
                    wagon = wagon.Where(x => x.Date >= startDate && x.Date < Dateto).ToList();
                    break;
                case 4:
                    wagon = wagon.Where(x => x.Date >= Datefrom && x.Date < endDate).ToList();
                    break;
            };
            return wagon.Sum(s => (decimal?)s.IdleHour) ?? 0;
        }
        #endregion

        #region GetDownHour
        private decimal GetDownHour(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            decimal totalDownHour = 0;
            var equipmentType = db.EquipmentType.Find(equipmentTypeId);

            if (equipmentType != null)
            {
                switch (equipmentType.TimeSheetType)
                {
                    case TimeSheetType.DryLoading:
                        totalDownHour = GetDryloadDownHour(equipmentTypeId, glPeriodId, Datefrom,Dateto);
                        break;
                    case TimeSheetType.DumpTruck:
                        totalDownHour = GetDumpTruckDownHour(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;
                    case TimeSheetType.LightVehicle:
                        totalDownHour = GetLightVehicleDownHour(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;
                    case TimeSheetType.Machinery:
                        totalDownHour = GetMachinaryDownHour(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;
                    case TimeSheetType.WagonDrill:
                        totalDownHour = GetWagonDrillDownHour(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;

                }
            }
            return totalDownHour;
        }

        private decimal GetDryloadDownHour(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            var dryload = db.DryLoadingTimeSheets.Where(x => x.TimeSheetApproved)
                  .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
                  .Where(dryLoad => DbFunctions.TruncateTime(dryLoad.TripStartDate) >= startDate && DbFunctions.TruncateTime(dryLoad.TripStartDate) <= endDate).ToList();
                
            switch (EndDateCompare)
            {
                case 0:
                    dryload = dryload.Where(x => x.TripStartDate.Date >= startDate && x.TripStartDate.Date < Dateto).ToList();
                    break;
                case 1:
                    dryload = dryload.Where(x => x.TripStartDate.Date >= Datefrom && x.TripStartDate.Date < Dateto).ToList();
                    break;
                case 2:
                    dryload = dryload.Where(x => x.TripStartDate.Date >= startDate && x.TripStartDate.Date < endDate).ToList();
                    break;
                case 3:
                    dryload = dryload.Where(x => x.TripStartDate >= startDate && x.TripStartDate.Date < Dateto).ToList();
                    break;
                case 4:
                    dryload = dryload.Where(x => x.TripStartDate >= Datefrom && x.TripStartDate.Date < endDate).ToList();
                    break;
            };
            return dryload.Sum(s => (decimal?)s.Downhour) ?? 0;

        }
        private decimal GetDumpTruckDownHour(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            var dump = db.DumpTruckTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
                .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList();
                
            switch (EndDateCompare)
            {
                case 0:
                    dump = dump.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    dump = dump.Where(x => x.Date.Date >= Datefrom && x.Date.Date < Dateto).ToList();
                    break;
                case 2:
                    dump = dump.Where(x => x.Date.Date >= startDate && x.Date.Date < endDate).ToList();
                    break;
                case 3:
                    dump = dump.Where(x => x.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 4:
                    dump = dump.Where(x => x.Date >= Datefrom && x.Date.Date < endDate).ToList();
                    break;
            };
            return dump.Sum(s => (decimal?)s.DownHour) ?? 0;
        }
        private decimal GetMachinaryDownHour(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            var machine = db.MachineryWorkingTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(x => fleetIds.Contains(x.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList();
                 
            switch (EndDateCompare)
            {
                case 0:
                    machine = machine.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    machine = machine.Where(x => x.Date.Date >= Datefrom && x.Date.Date < Dateto).ToList();
                    break;
                case 2:
                    machine = machine.Where(x => x.Date.Date >= startDate && x.Date.Date < endDate).ToList();
                    break;
                case 3:
                    machine = machine.Where(x => x.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 4:
                    machine = machine.Where(x => x.Date >= Datefrom && x.Date.Date < endDate).ToList();
                    break;
            };
            return machine.Sum(s => (decimal?)s.DownHours) ?? 0;
        }
        private decimal GetLightVehicleDownHour(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            var lVechile = db.LightVehicleTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => fleetIds.Contains(x.FleetsID))
                .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList();
                

            switch (EndDateCompare)
            {
                case 0:
                    lVechile = lVechile.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    lVechile = lVechile.Where(x => x.Date.Date >= Datefrom && x.Date.Date < Dateto).ToList();
                    break;
                case 2:
                    lVechile = lVechile.Where(x => x.Date.Date >= startDate && x.Date.Date <endDate).ToList();
                    break;
                case 3:
                    lVechile = lVechile.Where(x => x.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 4:
                    lVechile = lVechile.Where(x => x.Date >= Datefrom && x.Date.Date < endDate).ToList();
                    break;
            };
            return lVechile.Sum(s => (decimal?)s.DownHours) ?? 0;
        }
        private decimal GetWagonDrillDownHour(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            var wagon = db.WagonDrillTimesheets.Where(x => x.TimeSheetApproved)
                  .Where(x => fleetIds.Contains(x.FleetsID))
                  .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList();
                  

            switch (EndDateCompare)
            {
                case 0:
                    wagon = wagon.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    wagon = wagon.Where(x => x.Date.Date >= Datefrom && x.Date.Date < Dateto).ToList();
                    break;
                case 2:
                    wagon = wagon.Where(x => x.Date.Date >= startDate && x.Date.Date < endDate).ToList();
                    break;
                case 3:
                    wagon = wagon.Where(x => x.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 4:
                    wagon = wagon.Where(x => x.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
            };
            return wagon.Sum(s => (decimal?)s.DownTime) ?? 0;

        }
        #endregion

        #region GetActualQuantity
        private int GetActualQuantity(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            int quantity = 0;
            var equipmentType = db.EquipmentType.Find(equipmentTypeId);

            if (equipmentType != null)
            {
                switch (equipmentType.TimeSheetType)
                {
                    case TimeSheetType.DryLoading:
                        quantity = GetDryloadQuantity(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;
                    case TimeSheetType.DumpTruck:
                        quantity = GetDumpTruckQuantity(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;
                    case TimeSheetType.LightVehicle:
                        quantity = GetLightVehicleQuantity(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;
                    case TimeSheetType.Machinery:
                        quantity = GetMachinaryQuantity(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;
                    case TimeSheetType.WagonDrill:
                        quantity = GetWagonDrillQuantity(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;

                }
            }
            return quantity;
        }

        private int GetDryloadQuantity(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom, DateTime Dateto)
        {
            // Retrieve the GL period from the database
           
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare =0;
            if(startDate==Datefrom&&endDate>=Dateto)
            {
                EndDateCompare = 0;
                
            }
            else if(startDate<=Datefrom&&endDate>=Dateto)
            {
                EndDateCompare = 1;
                
            }
            else if(startDate>=Datefrom&&endDate<=Dateto)
            {
                EndDateCompare = 2;
            }
            else if(startDate<=Datefrom&&endDate<=Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            var dry = db.DryLoadingTimeSheets.Where(x => x.TimeSheetApproved)
               .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
               .Where(dryLoad => DbFunctions.TruncateTime(dryLoad.TripStartDate) >= startDate && DbFunctions.TruncateTime(dryLoad.TripStartDate) <= endDate)
               .ToList();
            switch (EndDateCompare)
            {
                case 0:
                    dry = dry.Where(x => x.TripStartDate.Date >= startDate && x.TripStartDate.Date < Dateto).ToList();
                    break;
                case 1:
                    dry = dry.Where(x => x.TripStartDate.Date >= Datefrom && x.TripStartDate.Date < Dateto).ToList();
                    break;
                case 2:
                    dry = dry.Where(x => x.TripStartDate.Date >= startDate && x.TripStartDate.Date < endDate ).ToList();
                    break;
                case 3:
                    dry = dry.Where(x => x.TripStartDate >= startDate && x.TripStartDate.Date <= Dateto).ToList();
                    break;
                case 4:
                    dry = dry.Where(x => x.TripStartDate >= Datefrom && x.TripStartDate.Date <= endDate).ToList();
                    break;
            };
            return dry.Select(dryLoad => dryLoad.FleetsID)
                .Distinct()
                .Count();
        }
        private int GetDumpTruckQuantity(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;
                
            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;
                
            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            var dump = db.DumpTruckTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList();
                 
            switch (EndDateCompare)
            {
                case 0:
                    dump = dump.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    dump = dump.Where(x => x.Date.Date >= Datefrom && x.Date.Date < Dateto).ToList();
                    break;
                case 2:
                    dump = dump.Where(x => x.Date.Date >= startDate && x.Date.Date < endDate).ToList();
                    break;
                case 3:
                    dump = dump.Where(x => x.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 4:
                    dump = dump.Where(x => x.Date >= Datefrom && x.Date.Date < endDate).ToList();
                    break;
            };
           
            return dump.Select(dryLoad => dryLoad.FleetsID)
                .Distinct()
                .Count();
        }
        private int GetMachinaryQuantity(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            var dump = db.MachineryWorkingTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(x => fleetIds.Contains(x.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList();
                 

            switch (EndDateCompare)
            {
                case 0:
                    dump = dump.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    dump = dump.Where(x => x.Date.Date >= Datefrom && x.Date.Date < Dateto).ToList();
                    break;
                case 2:
                    dump = dump.Where(x => x.Date.Date >= startDate && x.Date.Date < endDate).ToList();
                    break;
                case 3:
                    dump = dump.Where(x => x.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 4:
                    dump = dump.Where(x => x.Date >= Datefrom && x.Date.Date < endDate).ToList();
                    break;
            };
           
            return dump.Select(dryLoad => dryLoad.FleetsID)
                .Distinct()
                .Count();
        }
        private int GetLightVehicleQuantity(Guid equipmentTypeId, int glPeriodId,DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            var lv = db.LightVehicleTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => fleetIds.Contains(x.FleetsID))
                .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList();
                

            switch (EndDateCompare)
            {
                case 0:
                    lv = lv.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    lv = lv.Where(x => x.Date.Date >= Datefrom && x.Date.Date < Dateto).ToList();
                    break;
                case 2:
                    lv = lv.Where(x => x.Date.Date >= startDate && x.Date.Date < endDate).ToList();
                    break;
                case 3:
                    lv = lv.Where(x => x.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 4:
                    lv = lv.Where(x => x.Date >= Datefrom && x.Date.Date < endDate).ToList();
                    break;
            };
            return lv.Select(x => x.FleetsID)
                .Distinct()
                .Count();
        }
        private int GetWagonDrillQuantity(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            var lv = db.WagonDrillTimesheets.Where(x => x.TimeSheetApproved)
                 .Where(x => fleetIds.Contains(x.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList();
                 
            switch (EndDateCompare)
            {
                case 0:
                    lv = lv.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    lv = lv.Where(x => x.Date.Date >= Datefrom && x.Date.Date < Dateto).ToList();
                    break;
                case 2:
                    lv = lv.Where(x => x.Date.Date >= startDate && x.Date.Date < endDate).ToList();
                    break;
                case 3:
                    lv = lv.Where(x => x.Date >= startDate && x.Date.Date <Dateto).ToList();
                    break;
                case 4:
                    lv = lv.Where(x => x.Date >= Datefrom && x.Date.Date <endDate).ToList();
                    break;
            };
            return lv.Select(x => x.FleetsID)
                .Distinct()
                .Count();

        }
        #endregion


        #region OpHour
        private decimal GetActualOperationalHour(Guid equipmentTypeId, int glPeriodId,DateTime Datefrom,DateTime Dateto)
        {
            decimal opHour = 0;
            var equipmentType = db.EquipmentType.Find(equipmentTypeId);

            if (equipmentType != null)
            {
                switch (equipmentType.TimeSheetType)
                {
                    case TimeSheetType.DryLoading:
                        opHour = GetDryloadOpHour(equipmentTypeId, glPeriodId, Datefrom,Dateto);
                        break;
                    case TimeSheetType.DumpTruck:
                        opHour = GetDumpTruckOpHour(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;
                    case TimeSheetType.LightVehicle:
                        opHour = GetLightVehicleOpHour(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;
                    case TimeSheetType.Machinery:
                        opHour = GetMachinaryOpHour(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;
                    case TimeSheetType.WagonDrill:
                        opHour = GetWagonDrillOpHour(equipmentTypeId, glPeriodId, Datefrom, Dateto);
                        break;

                }
            }
            return opHour;
        }

        private decimal GetDryloadOpHour(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range and aggregate working hour
            var dry = db.DryLoadingTimeSheets.Where(x => x.TimeSheetApproved)
               .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
               .Where(dryLoad => DbFunctions.TruncateTime(dryLoad.TripStartDate) >= startDate && DbFunctions.TruncateTime(dryLoad.TripStartDate) <= endDate)
                          .ToList();       

            switch (EndDateCompare)
            {
                case 0:
                    dry = dry.Where(x => x.TripStartDate.Date >= startDate && x.TripStartDate.Date < Dateto).ToList();
                    break;
                case 1:
                    dry = dry.Where(x => x.TripStartDate.Date >= Datefrom && x.TripStartDate.Date < Dateto).ToList();
                    break;
                case 2:
                    dry = dry.Where(x => x.TripStartDate.Date >= startDate && x.TripStartDate.Date < endDate).ToList();
                    break;
                case 3:
                    dry = dry.Where(x => x.TripStartDate >= startDate && x.TripStartDate.Date < Dateto).ToList();
                    break;
                case 4:
                    dry = dry.Where(x => x.TripStartDate >= Datefrom && x.TripStartDate.Date < endDate).ToList();
                    break;
            };
            return dry.Sum(s => (decimal?)s.WorkingHour) ?? 0;
        }


        private decimal GetDumpTruckOpHour(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            var dry = db.DumpTruckTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                 .ToList();

            switch (EndDateCompare)
            {
                case 0:
                    dry = dry.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    dry = dry.Where(x => x.Date.Date >= Datefrom && x.Date.Date < Dateto).ToList();
                    break;
                case 2:
                    dry = dry.Where(x => x.Date.Date >= startDate&& x.Date.Date < endDate).ToList();
                    break;
                case 3:
                    dry = dry.Where(x => x.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 4:
                    dry = dry.Where(x => x.Date >= Datefrom && x.Date.Date < endDate).ToList();
                    break;
            };
            return dry.Sum(s => (decimal?)s.NetworkingHour) ?? 0;
        }
        private decimal GetMachinaryOpHour(Guid equipmentTypeId, int glPeriodId, DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            var dry = db.MachineryWorkingTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => fleetIds.Contains(x.FleetsID))
                .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList()
                .ToList();

            switch (EndDateCompare)
            {
                case 0:
                    dry = dry.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    dry = dry.Where(x => x.Date.Date >= Datefrom && x.Date.Date < Dateto).ToList();
                    break;
                case 2:
                    dry = dry.Where(x => x.Date.Date >= startDate.AddDays(14) && x.Date.Date < endDate).ToList();
                    break;
                case 3:
                    dry = dry.Where(x => x.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 4:
                    dry = dry.Where(x => x.Date >= Datefrom && x.Date.Date < endDate).ToList();
                    break;
            };
            return dry.Sum(s => CalculateMachineryNetWorkingHour(s));
        }
        private decimal GetLightVehicleOpHour(Guid equipmentTypeId, int glPeriodId,DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            var dry = db.LightVehicleTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(x => fleetIds.Contains(x.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList()
                 .ToList();

            switch (EndDateCompare)
            {
                case 1:
                    dry = dry.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 2:
                    dry = dry.Where(x => x.Date.Date >= Datefrom && x.Date.Date <Dateto).ToList();
                    break;
                case 3:
                    dry = dry.Where(x => x.Date.Date >= startDate && x.Date.Date < endDate).ToList();
                    break;
                case 4:
                    dry = dry.Where(x => x.Date >= Datefrom && x.Date.Date < endDate).ToList();
                    break;
            };
            return dry.Sum(s => CalculateLightVehicleNetWorkingHour(s));
        }
        private decimal GetWagonDrillOpHour(Guid equipmentTypeId, int glPeriodId,DateTime Datefrom,DateTime Dateto)
        {
            var glPeriod = db.GLPeriods.Find(glPeriodId);
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            var EndDateCompare = 0;
            if (startDate == Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 0;

            }
            else if (startDate <= Datefrom && endDate >= Dateto)
            {
                EndDateCompare = 1;

            }
            else if (startDate >= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 2;
            }
            else if (startDate <= Datefrom && endDate <= Dateto)
            {
                EndDateCompare = 4;
            }
            else
            {
                EndDateCompare = 3;
            }
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            var dry = db.WagonDrillTimesheets.Where(x => x.TimeSheetApproved)
                 .Where(x => fleetIds.Contains(x.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                 .ToList();

            switch (EndDateCompare)
            {
                case 0:
                    dry = dry.Where(x => x.Date.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 1:
                    dry = dry.Where(x => x.Date.Date >= Datefrom && x.Date.Date < Dateto).ToList();
                    break;
                case 2:
                    dry = dry.Where(x => x.Date.Date >= startDate && x.Date.Date < endDate).ToList();
                    break;
                case 3:
                    dry = dry.Where(x => x.Date >= startDate && x.Date.Date < Dateto).ToList();
                    break;
                case 4:
                    dry = dry.Where(x => x.Date >= Datefrom && x.Date.Date < endDate).ToList();
                    break;
            };
            return dry.Sum(s => (decimal?)s.OperationNetWorkingHour) ?? 0;
        }
        #endregion
        private decimal CalculateMachineryNetWorkingHour(MachineryWorkingTimeSheet machineryTimeSheet)
        {
            return Math.Round((decimal)((machineryTimeSheet.MorningTimeEnd - machineryTimeSheet.MorningTimeStart).TotalHours + (machineryTimeSheet.AfternoonTimeEnd - machineryTimeSheet.AfternoonTimeStart).TotalHours + (machineryTimeSheet.NightTimeEnd - machineryTimeSheet.NightTimeStart).TotalHours), 2);
        }
        private decimal CalculateLightVehicleNetWorkingHour(LightVehicleTimeSheet lightVehicle)
        {
            return Math.Round((decimal)((lightVehicle.MorningTimeEnd - lightVehicle.MorningTimeStart).TotalHours + (lightVehicle.AfternoonTimeEnd - lightVehicle.AfternoonTimeStart).TotalHours + (lightVehicle.NightTimeEnd - lightVehicle.NightTimeStart).TotalHours), 2);
        }
        private double CalculateHour(DateTime startDate, DateTime endDate)
        {
            TimeSpan difference = endDate - startDate;
            return difference.TotalHours;
        }
    }

    public class FleetUtilizationSummaryViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EquipmentTypeName { get; set; }
        public int PlanQuantity { get; set; }

        public int Quantity { get; set; }
        public double Hour { get; set; }
        public int ActualMonthlyAvaliableQuantity { get; set; }
        public decimal PlannedMonthlyHour { get; set; }
        public decimal ActualMonthlyOperationalHour { get; set; }
        public decimal PlanVsActualHourRatio { get; set; }
        public decimal ActualMonthlyIdleHour { get; set; }
        public decimal PlanVsActualIdleHourRatio { get; set; }
        public decimal ActualMonthlyDownHour { get; set; }
        public decimal PlanVsActualDownHourRatio { get; set; }

    }
}

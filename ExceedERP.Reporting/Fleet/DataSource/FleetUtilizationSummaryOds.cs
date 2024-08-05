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

        public List<FleetUtilizationSummaryViewModel> GetUtilization(string glFiscalYearId, string glPeriodId)
        {
            List<FleetUtilizationSummaryViewModel> summaryList = new List<FleetUtilizationSummaryViewModel>();
            if (int.TryParse(glFiscalYearId, out int fiscalYearId))
            {
                var plans = db.FleetAnnualPlans.Where(x => x.GlFiscalYearId == fiscalYearId).ToList();
                var planDetails = db.FleetAnnualPlanDetails.ToList();
                if (int.TryParse(glPeriodId, out int periodId))
                {
                    planDetails = planDetails.Where(x => x.GlPeriodId == periodId).ToList();
                }

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
                    var month = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == groupedPlan.GlPeriodId)?.Name;
                    if (equipmentType != null && month != null)
                    {
                        var ut = new FleetUtilizationSummaryViewModel
                        {
                            EquipmentTypeName = equipmentType.EquipmentTypeName,
                            PlanQuantity = (int)groupedPlan.Quantity/4,
                            PlannedMonthlyHour = groupedPlan.PlanedHour/4,
                            ActualMonthlyAvaliableQuantity = GetActualQuantity(groupedPlan.EquipmentTypeId, groupedPlan.GlPeriodId)/4,
                            ActualMonthlyOperationalHour = GetActualOperationalHour(groupedPlan.EquipmentTypeId, groupedPlan.GlPeriodId)/4,
                            ActualMonthlyDownHour = GetDownHour(groupedPlan.EquipmentTypeId, groupedPlan.GlPeriodId)/4,
                            ActualMonthlyIdleHour = GetIdleHour(groupedPlan.EquipmentTypeId, groupedPlan.GlPeriodId)/4,
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

        #region GetIdleHour
        private decimal GetIdleHour(Guid equipmentTypeId, int glPeriodId)
        {
            decimal totalDownHour = 0;
            var equipmentType = db.EquipmentType.Find(equipmentTypeId);

            if (equipmentType != null)
            {
                switch (equipmentType.TimeSheetType)
                {
                    case TimeSheetType.DryLoading:
                        totalDownHour = GetDryloadIdleHour(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.DumpTruck:
                        totalDownHour = GetDumpTruckIdleHour(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.LightVehicle:
                        totalDownHour = GetLightVehicleIdleHour(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.Machinery:
                        totalDownHour = GetMachinaryIdle(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.WagonDrill:
                        totalDownHour = GetWagonDrillIdleHour(equipmentTypeId, glPeriodId);
                        break;

                }
            }
            return totalDownHour;
        }

        private decimal GetDryloadIdleHour(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Get the start and end dates for the GL period
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;

            // Join the filtered DryLoadingTimeSheets with the fleet IDs
            return db.DryLoadingTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
                .Where(dryLoad => DbFunctions.TruncateTime(dryLoad.TripStartDate) >= startDate && DbFunctions.TruncateTime(dryLoad.TripStartDate) <= endDate)
                .Sum(s => (decimal?)s.IdleHour) ?? 0;
        }

        private decimal GetDumpTruckIdleHour(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            // Get the start and end dates for the GL period
            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;

            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range

            return db.DumpTruckTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
                .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
               .Sum(s => (decimal?)s.IdleHour) ?? 0;

        }
        private decimal GetMachinaryIdle(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;


            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();


            return db.MachineryWorkingTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => fleetIds.Contains(x.FleetsID))
                .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                .Sum(s => (decimal?)s.IdleHours) ?? 0;

        }
        private decimal GetLightVehicleIdleHour(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;

            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range

            return db.LightVehicleTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => fleetIds.Contains(x.FleetsID))
                .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                .Sum(s => (decimal?)s.IdleHours) ?? 0;

        }
        private decimal GetWagonDrillIdleHour(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;

            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range

            return db.WagonDrillTimesheets.Where(x => x.TimeSheetApproved)
                .Where(x => fleetIds.Contains(x.FleetsID))
                .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                .Sum(s => (decimal?)s.IdleHour) ?? 0;


        }
        #endregion

        #region GetDownHour
        private decimal GetDownHour(Guid equipmentTypeId, int glPeriodId)
        {
            decimal totalDownHour = 0;
            var equipmentType = db.EquipmentType.Find(equipmentTypeId);

            if (equipmentType != null)
            {
                switch (equipmentType.TimeSheetType)
                {
                    case TimeSheetType.DryLoading:
                        totalDownHour = GetDryloadDownHour(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.DumpTruck:
                        totalDownHour = GetDumpTruckDownHour(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.LightVehicle:
                        totalDownHour = GetLightVehicleDownHour(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.Machinery:
                        totalDownHour = GetMachinaryDownHour(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.WagonDrill:
                        totalDownHour = GetWagonDrillDownHour(equipmentTypeId, glPeriodId);
                        break;

                }
            }
            return totalDownHour;
        }

        private decimal GetDryloadDownHour(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.DryLoadingTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
                 .Where(dryLoad => DbFunctions.TruncateTime(dryLoad.TripStartDate) >= startDate && DbFunctions.TruncateTime(dryLoad.TripStartDate) <= endDate)
                .Sum(s => (decimal?)s.Downhour) ?? 0;


        }
        private decimal GetDumpTruckDownHour(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.DumpTruckTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
                .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                .Sum(s => (decimal?)s.DownHour) ?? 0;

        }
        private decimal GetMachinaryDownHour(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.MachineryWorkingTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(x => fleetIds.Contains(x.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                 .Sum(s => (decimal?)s.DownHours) ?? 0;

        }
        private decimal GetLightVehicleDownHour(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.LightVehicleTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => fleetIds.Contains(x.FleetsID))
                .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                .Sum(s => (decimal?)s.DownHours) ?? 0;

        }
        private decimal GetWagonDrillDownHour(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.WagonDrillTimesheets.Where(x => x.TimeSheetApproved)
                  .Where(x => fleetIds.Contains(x.FleetsID))
                  .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                  .Sum(s => (decimal?)s.DownTime) ?? 0;


        }
        #endregion

        #region GetActualQuantity
        private int GetActualQuantity(Guid equipmentTypeId, int glPeriodId)
        {
            int quantity = 0;
            var equipmentType = db.EquipmentType.Find(equipmentTypeId);

            if (equipmentType != null)
            {
                switch (equipmentType.TimeSheetType)
                {
                    case TimeSheetType.DryLoading:
                        quantity = GetDryloadQuantity(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.DumpTruck:
                        quantity = GetDumpTruckQuantity(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.LightVehicle:
                        quantity = GetLightVehicleQuantity(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.Machinery:
                        quantity = GetMachinaryQuantity(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.WagonDrill:
                        quantity = GetWagonDrillQuantity(equipmentTypeId, glPeriodId);
                        break;

                }
            }
            return quantity;
        }

        private int GetDryloadQuantity(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.DryLoadingTimeSheets.Where(x => x.TimeSheetApproved)
               .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
               .Where(dryLoad => DbFunctions.TruncateTime(dryLoad.TripStartDate) >= startDate && DbFunctions.TruncateTime(dryLoad.TripStartDate) <= endDate)
               .Select(dryLoad => dryLoad.FleetsID)
                .Distinct()
                .Count();

        }
        private int GetDumpTruckQuantity(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.DumpTruckTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                 .Select(dryLoad => dryLoad.FleetsID)
                .Distinct()
                .Count();

        }
        private int GetMachinaryQuantity(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.MachineryWorkingTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(x => fleetIds.Contains(x.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                 .Select(dryLoad => dryLoad.FleetsID)
                .Distinct()
                .Count();

        }
        private int GetLightVehicleQuantity(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.LightVehicleTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => fleetIds.Contains(x.FleetsID))
                .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                .Select(dryLoad => dryLoad.FleetsID)
                .Distinct()
                .Count();

        }
        private int GetWagonDrillQuantity(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.WagonDrillTimesheets.Where(x => x.TimeSheetApproved)
                 .Where(x => fleetIds.Contains(x.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                 .Select(dryLoad => dryLoad.FleetsID)
                .Distinct()
                .Count();

        }
        #endregion


        #region OpHour
        private decimal GetActualOperationalHour(Guid equipmentTypeId, int glPeriodId)
        {
            decimal opHour = 0;
            var equipmentType = db.EquipmentType.Find(equipmentTypeId);

            if (equipmentType != null)
            {
                switch (equipmentType.TimeSheetType)
                {
                    case TimeSheetType.DryLoading:
                        opHour = GetDryloadOpHour(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.DumpTruck:
                        opHour = GetDumpTruckOpHour(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.LightVehicle:
                        opHour = GetLightVehicleOpHour(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.Machinery:
                        opHour = GetMachinaryOpHour(equipmentTypeId, glPeriodId);
                        break;
                    case TimeSheetType.WagonDrill:
                        opHour = GetWagonDrillOpHour(equipmentTypeId, glPeriodId);
                        break;

                }
            }
            return opHour;
        }

        private decimal GetDryloadOpHour(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range and aggregate working hour
            return db.DryLoadingTimeSheets.Where(x => x.TimeSheetApproved)
               .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
               .Where(dryLoad => DbFunctions.TruncateTime(dryLoad.TripStartDate) >= startDate && DbFunctions.TruncateTime(dryLoad.TripStartDate) <= endDate)
                                 .Sum(s => (decimal?)s.WorkingHour) ?? 0;

        }


        private decimal GetDumpTruckOpHour(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.DumpTruckTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(dryLoad => fleetIds.Contains(dryLoad.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                 .Sum(s => (decimal?)s.NetworkingHour) ?? 0;

        }
        private decimal GetMachinaryOpHour(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.MachineryWorkingTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => fleetIds.Contains(x.FleetsID))
                .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList()
                .Sum(s => CalculateMachineryNetWorkingHour(s));

        }
        private decimal GetLightVehicleOpHour(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.LightVehicleTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(x => fleetIds.Contains(x.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate).ToList()
                 .Sum(s => CalculateLightVehicleNetWorkingHour(s));

        }
        private decimal GetWagonDrillOpHour(Guid equipmentTypeId, int glPeriodId)
        {
            // Retrieve the GL period from the database
            var glPeriod = db.GLPeriods.Find(glPeriodId);

            var startDate = glPeriod.DateFrom.Date;
            var endDate = glPeriod.DateTo.Date;
            // Retrieve the fleet IDs from the database that match the equipment type ID
            var fleetIds = db.Fleets
                .Where(x => x.EquipmentTypeID == equipmentTypeId)
                .Select(x => x.FleetsID)
                .ToList();

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.WagonDrillTimesheets.Where(x => x.TimeSheetApproved)
                 .Where(x => fleetIds.Contains(x.FleetsID))
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= startDate && DbFunctions.TruncateTime(x.Date) <= endDate)
                 .Sum(s => (decimal?)s.OperationNetWorkingHour) ?? 0;

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

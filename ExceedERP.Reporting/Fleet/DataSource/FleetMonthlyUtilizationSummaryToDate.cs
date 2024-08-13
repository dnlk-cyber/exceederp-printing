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
    public class FleetMonthlyUtilizationSummaryToDate
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MonthlyUtilizationSummaryViewModelToDate> UtilizationSummaryTest(string glFiscalYearId)
        {
            List<MonthlyUtilizationSummaryViewModelToDate> summaryList = new List<MonthlyUtilizationSummaryViewModelToDate>();
            if (int.TryParse(glFiscalYearId, out int fiscalYearId))
            {
                var plans = db.FleetAnnualPlans.Where(x => x.GlFiscalYearId == fiscalYearId).ToList();
                var planDetails = db.FleetAnnualPlanDetails.Where(x => DbFunctions.TruncateTime(x.DateFrom) <= DateTime.Today.Date).ToList();




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
                                      Quantity = detail.NumberOfEquipmentInMonth,
                                      DateFrom = detail.DateFrom
                                  };


                var groupedPlans = joinedPlans.GroupBy(g => new { g.EquipmentTypeId, g.GlFiscalYearId})
                    .Select(s => new
                    {
                        s.Key.EquipmentTypeId,
                        s.Key.GlFiscalYearId,
                        //s.Key.GlPeriodId,
                        PlanedHour = s.Sum(x => (decimal?)x.PlanHour) ?? 0,
                        //Quantity = s.Sum(x => (decimal?)x.Quantity) ?? 0
                        Quantity = s.OrderByDescending(x => x.DateFrom).First().Quantity
                        //Quantity = s.Select(x => x.FleetId).Distinct(),
                    }).ToList();

                foreach (var groupedPlan in groupedPlans)
                {
                    var equipmentType = db.EquipmentType.Find(groupedPlan.EquipmentTypeId);
                    //var month = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == groupedPlan.GlPeriodId)?.Name;
                    if (equipmentType != null)
                    {
                        var ut = new MonthlyUtilizationSummaryViewModelToDate
                        {
                            EquipmentTypeName = equipmentType.EquipmentTypeName,
                            PlanQuantity = (int)groupedPlan.Quantity,
                            PlannedMonthlyHour = groupedPlan.PlanedHour,
                            ActualMonthlyAvaliableQuantity = GetActualQuantity(groupedPlan.EquipmentTypeId, groupedPlan.GlFiscalYearId),
                            ActualMonthlyOperationalHour = GetActualOperationalHour(groupedPlan.EquipmentTypeId, groupedPlan.GlFiscalYearId),
                            ActualMonthlyDownHour = GetDownHour(groupedPlan.EquipmentTypeId, groupedPlan.GlFiscalYearId),
                            ActualMonthlyIdleHour = GetIdleHour(groupedPlan.EquipmentTypeId, groupedPlan.GlFiscalYearId),
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
        private decimal GetIdleHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            decimal totalDownHour = 0;
            var equipmentType = db.EquipmentType.Find(equipmentTypeId);

            if (equipmentType != null)
            {
                switch (equipmentType.TimeSheetType)
                {
                    case TimeSheetType.DryLoading:
                        totalDownHour = GetDryloadIdleHour(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.DumpTruck:
                        totalDownHour = GetDumpTruckIdleHour(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.LightVehicle:
                        totalDownHour = GetLightVehicleIdleHour(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.Machinery:
                        totalDownHour = GetMachinaryIdle(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.WagonDrill:
                        totalDownHour = GetWagonDrillIdleHour(equipmentTypeId, glFiscalYearId);
                        break;

                }
            }
            return totalDownHour;
        }

        private decimal GetDryloadIdleHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);
            return db.DryLoadingTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => x.EquipmentTypeId == equipmentTypeId)
                .Where(x => DbFunctions.TruncateTime(x.TripStartDate) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.TripStartDate) <= DateTime.Today.Date)
                .Sum(s => (decimal?)s.IdleHour) ?? 0;
        }

        private decimal GetDumpTruckIdleHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);

            return db.DumpTruckTimeSheets.Where(x => x.TimeSheetApproved)
               .Where(x => x.EquipmentTypeId == equipmentTypeId)
                .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date)
               .Sum(s => (decimal?)s.IdleHour) ?? 0;

        }
        private decimal GetMachinaryIdle(Guid equipmentTypeId, int glFiscalYearId)
        {

            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);

            return db.MachineryWorkingTimeSheets.Where(x => x.TimeSheetApproved)
               .Where(x => x.EquipmentTypeId == equipmentTypeId)
                .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date)
                .Sum(s => (decimal?)s.IdleHours) ?? 0;

        }
        private decimal GetLightVehicleIdleHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);

            return db.LightVehicleTimeSheets.Where(x => x.TimeSheetApproved)
               .Where(x => x.EquipmentTypeId == equipmentTypeId)
                .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date)
                .Sum(s => (decimal?)s.IdleHours) ?? 0;

        }
        private decimal GetWagonDrillIdleHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);
            return db.WagonDrillTimesheets.Where(x => x.TimeSheetApproved)
               .Where(x => x.EquipmentTypeId == equipmentTypeId)
                .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date)
                .Sum(s => (decimal?)s.IdleHour) ?? 0;


        }
        #endregion

        #region GetDownHour
        private decimal GetDownHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            decimal totalDownHour = 0;
            var equipmentType = db.EquipmentType.Find(equipmentTypeId);

            if (equipmentType != null)
            {
                switch (equipmentType.TimeSheetType)
                {
                    case TimeSheetType.DryLoading:
                        totalDownHour = GetDryloadDownHour(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.DumpTruck:
                        totalDownHour = GetDumpTruckDownHour(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.LightVehicle:
                        totalDownHour = GetLightVehicleDownHour(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.Machinery:
                        totalDownHour = GetMachinaryDownHour(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.WagonDrill:
                        totalDownHour = GetWagonDrillDownHour(equipmentTypeId, glFiscalYearId);
                        break;

                }
            }
            return totalDownHour;
        }

        private decimal GetDryloadDownHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);
            return db.DryLoadingTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(x => x.EquipmentTypeId == equipmentTypeId)
                 .Where(x => DbFunctions.TruncateTime(x.TripStartDate) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.TripStartDate) <= DateTime.Today.Date)
                .Sum(s => (decimal?)s.Downhour) ?? 0;


        }
        private decimal GetDumpTruckDownHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);
            return db.DumpTruckTimeSheets.Where(x => x.TimeSheetApproved)
               .Where(x => x.EquipmentTypeId == equipmentTypeId)
                .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date)
                .Sum(s => (decimal?)s.DownHour) ?? 0;

        }
        private decimal GetMachinaryDownHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);
            return db.MachineryWorkingTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => x.EquipmentTypeId == equipmentTypeId)
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date)
                 .Sum(s => (decimal?)s.DownHours) ?? 0;

        }
        private decimal GetLightVehicleDownHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);
            return db.LightVehicleTimeSheets.Where(x => x.TimeSheetApproved)
               .Where(x => x.EquipmentTypeId == equipmentTypeId)
                .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date)
                .Sum(s => (decimal?)s.DownHours) ?? 0;

        }
        private decimal GetWagonDrillDownHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);
            return db.WagonDrillTimesheets.Where(x => x.TimeSheetApproved)
                 .Where(x => x.EquipmentTypeId == equipmentTypeId)
                  .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date)
                  .Sum(s => (decimal?)s.DownTime) ?? 0;


        }
        #endregion

        #region GetActualQuantity
        private int GetActualQuantity(Guid equipmentTypeId, int glFiscalYearId)
        {
            int quantity = 0;
            var equipmentType = db.EquipmentType.Find(equipmentTypeId);

            if (equipmentType != null)
            {
                switch (equipmentType.TimeSheetType)
                {
                    case TimeSheetType.DryLoading:
                        quantity = GetDryloadQuantity(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.DumpTruck:
                        quantity = GetDumpTruckQuantity(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.LightVehicle:
                        quantity = GetLightVehicleQuantity(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.Machinery:
                        quantity = GetMachinaryQuantity(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.WagonDrill:
                        quantity = GetWagonDrillQuantity(equipmentTypeId, glFiscalYearId);
                        break;

                }
            }
            return quantity;
        }

        private int GetDryloadQuantity(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);

            return db.DryLoadingTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => DbFunctions.TruncateTime(x.TripStartDate) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.TripStartDate) <= DateTime.Today.Date)
                .Where(x => x.EquipmentTypeId == equipmentTypeId)
                 .Select(dryLoad => dryLoad.FleetsID)
                .Distinct()
                .Count();            
        }
        private int GetDumpTruckQuantity(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);

            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.DumpTruckTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(x => x.EquipmentTypeId == equipmentTypeId)
                .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date)
                 .Select(dryLoad => dryLoad.FleetsID)
                 .Distinct()
                 .Count();

        }
        private int GetMachinaryQuantity(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);
            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.MachineryWorkingTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(x => x.EquipmentTypeId == equipmentTypeId)
                .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date)
                 .Select(dryLoad => dryLoad.FleetsID)
                .Distinct()
                .Count();

        }
        private int GetLightVehicleQuantity(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);
            // Join the DryLoadingTimeSheets with the fleet IDs within the GL period date range
            return db.LightVehicleTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => x.EquipmentTypeId == equipmentTypeId)
                .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date)
                .Select(dryLoad => dryLoad.FleetsID)
                .Distinct()
                .Count();

        }
        private int GetWagonDrillQuantity(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);


            return db.WagonDrillTimesheets.Where(x => x.TimeSheetApproved)
                 .Where(x => x.EquipmentTypeId == equipmentTypeId)
                .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date)
                 .Select(dryLoad => dryLoad.FleetsID)
                .Distinct()
                .Count();

        }
        #endregion


        #region OpHour
        private decimal GetActualOperationalHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            decimal opHour = 0;
            var equipmentType = db.EquipmentType.Find(equipmentTypeId);

            if (equipmentType != null)
            {
                switch (equipmentType.TimeSheetType)
                {
                    case TimeSheetType.DryLoading:
                        opHour = GetDryloadOpHour(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.DumpTruck:
                        opHour = GetDumpTruckOpHour(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.LightVehicle:
                        opHour = GetLightVehicleOpHour(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.Machinery:
                        opHour = GetMachinaryOpHour(equipmentTypeId, glFiscalYearId);
                        break;
                    case TimeSheetType.WagonDrill:
                        opHour = GetWagonDrillOpHour(equipmentTypeId, glFiscalYearId);
                        break;

                }
            }
            return opHour;
        }

        private decimal GetDryloadOpHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);
            return db.DryLoadingTimeSheets.Where(x => x.TimeSheetApproved)
               .Where(x => x.EquipmentTypeId == equipmentTypeId)
               .Where(dryLoad => DbFunctions.TruncateTime(dryLoad.TripStartDate) <= DateTime.Today.Date)
                                 .Sum(s => (decimal?)s.WorkingHour) ?? 0;

        }


        private decimal GetDumpTruckOpHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);
            return db.DumpTruckTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(x => x.EquipmentTypeId == equipmentTypeId)
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date)
                 .Sum(s => (decimal?)s.NetworkingHour) ?? 0;

        }
        private decimal GetMachinaryOpHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);
            return db.MachineryWorkingTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => x.EquipmentTypeId == equipmentTypeId)
                .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date).ToList()
                .Sum(s => CalculateMachineryNetWorkingHour(s));

        }
        private decimal GetLightVehicleOpHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);
            return db.LightVehicleTimeSheets.Where(x => x.TimeSheetApproved)
                 .Where(x => x.EquipmentTypeId == equipmentTypeId)
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date).ToList()
                 .Sum(s => CalculateLightVehicleNetWorkingHour(s));

        }
        private decimal GetWagonDrillOpHour(Guid equipmentTypeId, int glFiscalYearId)
        {
            var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalYearId);
            return db.WagonDrillTimesheets.Where(x => x.TimeSheetApproved)
                 .Where(x => x.EquipmentTypeId == equipmentTypeId)
                 .Where(x => DbFunctions.TruncateTime(x.Date) >= glFiscalYear.DateFrom.Date && DbFunctions.TruncateTime(x.Date) <= DateTime.Today.Date)
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

    public class MonthlyUtilizationSummaryViewModelToDate
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

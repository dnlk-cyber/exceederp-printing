using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Reporting.Fleet.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.DataSource
{
     [DataObject]
   public class PlanVsAccomplishmentDataSource
    {
         private static readonly ExceedDbContext Db = new ExceedDbContext();
        ExceedDbContext db = new ExceedDbContext();
        public static string GetYearPeriod(DateTime startdate, DateTime enddate)
        {

            var period = Db.GLPeriods.Where(x => x.DateFrom >= startdate && x.DateTo <= enddate).FirstOrDefault();
            if(period!= null)
            {
                return period.Name;
            }
            return "";
        }

        public static string GetEquipmentNameDetail(string plateno)
        {

            var fleet = Db.Fleets.Where(x => x.PlateNumber == plateno || x.UnitNum == plateno).FirstOrDefault();
            if (fleet != null)
            {
                return fleet.EquipmentName;
            }
            return "";
        }
        public static string GetEquipmentLocationDetail(string plateno)
        {

            var fleet = Db.Fleets.Where(x => x.PlateNumber == plateno || x.UnitNum == plateno).FirstOrDefault();
            if (fleet != null)
            {
                return fleet.Location;
            }
            return "";
        }
        public static string GetEquipmentSerialNoDetail(string plateno)
        {

            var fleet = Db.Fleets.Where(x => x.PlateNumber == plateno || x.UnitNum == plateno).FirstOrDefault();
            if (fleet != null)
            {
                return fleet.SerialNo;
            }
            return "";
        }
        public static string GetEquipmentChasisNumberDetail(string plateno)
        {

            var fleet = Db.Fleets.Where(x => x.PlateNumber == plateno || x.UnitNum == plateno).FirstOrDefault();
            if (fleet != null)
            {
                return fleet.ChasisNumber;
            }
            return "";
        }
        public static string GetEquipmentManufacturerDetail(string plateno)
        {

            var fleet = Db.Fleets.Where(x => x.PlateNumber == plateno || x.UnitNum == plateno).FirstOrDefault();
            if (fleet != null)
            {
                return fleet.Manufacturer;
            }
            return "";
        }
        public static string GetEquipmentModelDetail(string plateno)
        {

            var fleet = Db.Fleets.Where(x => x.PlateNumber == plateno || x.UnitNum == plateno).FirstOrDefault();
            if (fleet != null)
            {
                return fleet.Model;
            }
            return "";
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PlanVsAccomplishmentViewModel> GetPlanEvaluation(DateTime StartingDate, DateTime EndingDate, string Location, string Type, string category, string EquipmentName, string SerialNo, string status, string PlateNo, string Model, string ChasisNo)
        {
            List<PlanVsAccomplishmentViewModel> EquipmentPlanEvaluations = new List<PlanVsAccomplishmentViewModel>();
            List<PlanVsAccomplishmentViewModel> result = new List<PlanVsAccomplishmentViewModel>();
             var PlanEvaluations = db.EquipmentPlanandEvaluation.Where(x => x.DateCreated >= StartingDate && x.DateCreated <= EndingDate).ToList();

               var Job = db.JobOrderDetail.Where(x => x.DateCreated > StartingDate && x.DateCreated < EndingDate).ToList();
            try{
            foreach (var PlanEvaluation in PlanEvaluations)
            {
                decimal actualDowntime = 0;
                  int actualIdletime = 0;
                int actualWorkingtime = 0;

                var type = db.EquipmentIdentity.Where(x => x.EquipmentIdentityID == PlanEvaluation.EquipmentTypeId).FirstOrDefault();
                var obj = new PlanVsAccomplishmentViewModel();
                if (type != null)
                {
                    obj.EquipmentType = type.EquipmentCategoryName;
                    obj.EquipmentTypeId = PlanEvaluation.EquipmentCategoryId;
                }
                List<TimeSheet> timeSheet = db.TimeSheet.Where(x => x.FleetsID == PlanEvaluation.FleetId && x.DateCreated >= StartingDate && x.DateCreated <= EndingDate).ToList();
                if(!string.IsNullOrWhiteSpace(Location))
                {
                    timeSheet= timeSheet.Where(x => x.CustomerName == Location ).ToList();
                }
               
                if (timeSheet.Any())
                {
                    foreach (var ts in timeSheet)
                    {
                        actualWorkingtime = actualWorkingtime + (DateTime.Parse(ts.WorkingHoursMorningto.ToString()).Hour - DateTime.Parse(ts.WorkingHoursMorningFrom.ToString()).Hour) + (DateTime.Parse(ts.WorkingHoursAfternoonTo.ToString()).Hour - DateTime.Parse(ts.WorkingHoursAfternoonFrom.ToString()).Hour);// + (ts.WorkingHoursAfternoonTo.Value - ts.WorkingHoursAfternoonFrom.Value);
                    }
                    actualIdletime = timeSheet.Sum(x => x.IdleTime);
                    actualDowntime = timeSheet.Sum(x => x.DownTime);
                    actualWorkingtime = timeSheet.Sum(x => x.DownTime);
                
                    obj.PlanNo = 1;
                    obj.PlanKmHr = decimal.Parse(PlanEvaluation.PlanWorkingtime.ToString());
                    obj.AvailableKmHr = actualWorkingtime;
                    obj.AvailableNo = 1;
                    obj.UtilizedNo = 1;
                    obj.UtilizedNormalKmHr = timeSheet.Sum(x => x.FinalKMHrReading) - timeSheet.Sum(x => x.KMHrReading);
                    obj.UtilizedOverKmHr = timeSheet.Sum(x => x.IdleTime) + timeSheet.Sum(x => x.DownTime);
                }


                EquipmentPlanEvaluations.Add(obj);

            }
            foreach (var EquipmentPlanEvaluation in EquipmentPlanEvaluations.DistinctByField(x=>x.EquipmentType))
            {
                var obj = new PlanVsAccomplishmentViewModel();
                List<TimeSheet> externaltimeSheet = db.TimeSheet.Where(x => x.CustomerName == Location && x.FleetCategoryID == EquipmentPlanEvaluation.EquipmentTypeId && x.IsExternal && x.DateCreated >= StartingDate && x.DateCreated <= EndingDate).ToList();
             
                decimal totalkmhr =  EquipmentPlanEvaluations.Where(x => x.EquipmentType == EquipmentPlanEvaluation.EquipmentType).Sum(x => x.UtilizedNormalKmHr) + EquipmentPlanEvaluations.Where(x => x.EquipmentType == EquipmentPlanEvaluation.EquipmentType).Sum(x => x.UtilizedOverKmHr)+ externaltimeSheet.Sum(x => x.FinalKMHrReading) - externaltimeSheet.Sum(x => x.KMHrReading);
                obj.EquipmentType = EquipmentPlanEvaluation.EquipmentType;
                obj.PlanNo = EquipmentPlanEvaluations.Where(x => x.EquipmentType == EquipmentPlanEvaluation.EquipmentType).Count();
                obj.PlanKmHr = EquipmentPlanEvaluations.Where(x => x.EquipmentType == EquipmentPlanEvaluation.EquipmentType).Sum(x => x.PlanKmHr);
                obj.AvailableKmHr = EquipmentPlanEvaluations.Where(x => x.EquipmentType == EquipmentPlanEvaluation.EquipmentType).Sum(x => x.AvailableKmHr); ;
                obj.AvailableNo = EquipmentPlanEvaluations.Where(x => x.EquipmentType == EquipmentPlanEvaluation.EquipmentType).Count();
                obj.UtilizedNo = EquipmentPlanEvaluations.Where(x => x.EquipmentType == EquipmentPlanEvaluation.EquipmentType).Count();
                obj.UtilizedNormalKmHr = EquipmentPlanEvaluations.Where(x => x.EquipmentType == EquipmentPlanEvaluation.EquipmentType).Sum(x => x.UtilizedNormalKmHr);
                obj.UtilizedOverKmHr = EquipmentPlanEvaluations.Where(x => x.EquipmentType == EquipmentPlanEvaluation.EquipmentType).Sum(x => x.UtilizedOverKmHr);
                obj.DiffrenceUnassignedHr = externaltimeSheet.Sum(x => x.FinalKMHrReading) - externaltimeSheet.Sum(x => x.KMHrReading);
                obj.DiffrenceUnassignedNo = externaltimeSheet.Count;
                 obj.DiffrenceRentassignedHr = externaltimeSheet.Sum(x => x.FinalKMHrReading) - externaltimeSheet.Sum(x => x.KMHrReading);
                obj.DiffrenceRentassignedNo = externaltimeSheet.Count;
                obj.DiffrenceOtherassignedHr =0;
                obj.EnterprisePercent = (EquipmentPlanEvaluations.Where(x => x.EquipmentType == EquipmentPlanEvaluation.EquipmentType).Sum(x => x.UtilizedNormalKmHr) + EquipmentPlanEvaluations.Where(x => x.EquipmentType == EquipmentPlanEvaluation.EquipmentType).Sum(x => x.UtilizedOverKmHr)) / totalkmhr.DivideByZeroHelper();
                obj.RentAssigned = (externaltimeSheet.Sum(x => x.FinalKMHrReading) - externaltimeSheet.Sum(x => x.KMHrReading)) / totalkmhr.DivideByZeroHelper();
                result.Add(obj);
            }
        }
            catch(Exception e)
            {
                  var obj = new PlanVsAccomplishmentViewModel();
                  result.Add(obj);
            }
            return result;
        }
    }
}

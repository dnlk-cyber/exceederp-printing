using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Reporting.Fleet.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Helpers.Common;
namespace ExceedERP.Reporting.Fleet.DataSource
{

    [DataObject]
    public class EquipmentPlanEvaluationDataSource
    {


        ExceedDbContext db = new ExceedDbContext(); 

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentPlanEvaluationViewModel> GetPlanEvaluation(DateTime StartingDate, DateTime EndingDate, string Location, string Type, string category, string EquipmentName, string SerialNo, string status, string PlateNo, string Model, string ChasisNo)
        {
            List<EquipmentPlanEvaluationViewModel> EquipmentPlanEvaluations = new List<EquipmentPlanEvaluationViewModel>();

            FleetCostHelper FleetCostHelper = new Helpers.Fleet.FleetCostHelper();
            var PlanEvaluations = db.PMSchedules.Where(x=> x.DateCreated >= StartingDate && x.DateCreated <= EndingDate).ToList();
            var Job = db.JobOrderDetail.Where(x => x.DateCreated > StartingDate && x.DateCreated < EndingDate).ToList();

            foreach (var PlanEvaluation in PlanEvaluations.DistinctByField(x => x.EquipmentTypeId))
            {
                var type = db.EquipmentType.Where(x => x.EquipmentTypeID == PlanEvaluation.EquipmentTypeId).FirstOrDefault();
                var obj = new EquipmentPlanEvaluationViewModel();
                obj.EquipmentType = type.EquipmentTypeName;
                obj.Type = PlanEvaluation.Type;
                    var PMScheduleDetails = db.PMScheduleDetails.Where(x => x.PMScheduleId == PlanEvaluation.PMScheduleId).ToList();
                    foreach (var PMScheduleDetail in PMScheduleDetails)
                    {
                        if (FleetGenericHelper.FilterSingleFleet(PMScheduleDetail.FleetsId, category, Type, PlateNo, ChasisNo, SerialNo, string.Empty, Location, Model))
                        {
                       
                        obj.Plan++; 
                        obj.FleetSize++; 
                           obj.Actual += decimal.Parse((Job.Where(x => x.ServiceID == PMScheduleDetail.ServiceId).Count()).ToString());

                      }
                    }

                    EquipmentPlanEvaluations.Add(obj);
               
                }
            
            return EquipmentPlanEvaluations;
        }
    }
}

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
            var fleetCostHelper = new Helpers.Fleet.FleetCostHelper();

            // Filter PM Schedules by date range
            var planEvaluationsQuery = db.PMSchedules
                .Where(x => x.DateCreated >= StartingDate && x.DateCreated <= EndingDate);

            // Filter JobOrderDetails by date range
            var jobQuery = db.JobOrderDetail
                .Where(x => x.DateCreated > StartingDate && x.DateCreated < EndingDate)
                .Select(x => new { x.ServiceID });

            var jobList = jobQuery.ToList();

            // Get distinct equipment type IDs
            var distinctEquipmentTypeIds = planEvaluationsQuery
                .Select(x => x.EquipmentTypeId)
                .Distinct()
                .ToList();

            // Preload equipment types
            var equipmentTypesDict = db.EquipmentType
                .Where(x => distinctEquipmentTypeIds.Contains(x.EquipmentTypeID))
                .ToDictionary(x => x.EquipmentTypeID, x => x.EquipmentTypeName);

            // Materialize PlanEvaluations after DB-side filtering
            var planEvaluations = planEvaluationsQuery.ToList();

            var result = new List<EquipmentPlanEvaluationViewModel>();

            foreach (var planEvaluationGroup in planEvaluations.GroupBy(x => x.EquipmentTypeId))
            {
                var firstPlan = planEvaluationGroup.First(); // representative for the group
                var obj = new EquipmentPlanEvaluationViewModel
                {
                    EquipmentType = equipmentTypesDict.ContainsKey(firstPlan.EquipmentTypeId)
                                    ? equipmentTypesDict[firstPlan.EquipmentTypeId]
                                    : string.Empty,
                    Type = firstPlan.Type
                };

                // Gather all PMScheduleIds for this type group
                var pmScheduleIds = planEvaluationGroup.Select(x => x.PMScheduleId).ToList();

                // Get all relevant PMScheduleDetails
                var pmDetails = db.PMScheduleDetails
                    .Where(x => pmScheduleIds.Contains(x.PMScheduleId))
                    .Select(x => new { x.FleetsId, x.ServiceId })
                    .ToList();

                foreach (var detail in pmDetails)
                {
                    if (FleetGenericHelper.FilterSingleFleet(detail.FleetsId, category, Type, PlateNo, ChasisNo, SerialNo, string.Empty, Location, Model))
                    {
                        obj.Plan++;
                        obj.FleetSize++;
                        obj.Actual += jobList.Count(j => j.ServiceID == detail.ServiceId);
                    }
                }

                result.Add(obj);
            }

            return result;
        }
    }
}

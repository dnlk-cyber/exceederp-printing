using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
   public class MVDailyMaintenanceReportODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MVDailyMaintenanceReportVM> getMVReport(DateTime datetime,string mCenter)
        {
            List<MVDailyMaintenanceReportVM> list = new List<MVDailyMaintenanceReportVM>();
            var Today = datetime;
            var MaintenanceCenter = Guid.TryParse(mCenter, out Guid maintenanceCenterId);
            var ClosedJob = db.JobOrder.AsNoTracking()
      //.Where(x =>
      //    ((x.SecondOnlineApprovedTime.HasValue&& DbFunctions.TruncateTime(x.SecondOnlineApprovedTime).Value == Today && x.Status == MaintenanceConst.Closed)
      //    ||
      //    ((x.OnlineApprovedTime.HasValue && DbFunctions.TruncateTime(x.OnlineApprovedTime).Value == Today.Date) && x.Status == MaintenanceConst.Open) ||
      //    (x.IsSentToWorkShop == true && DbFunctions.TruncateTime(x.IsSentToWorkShopTime) == Today) ||
      //    (x.IsSubmittedToServiceCenter == true && DbFunctions.TruncateTime(x.OnlineSubmittedTime) == Today) ||
      //    (x.OnlineAssignedTime == Today))&&x.MaintenanceCenterId== maintenanceCenterId)
      .Where(x =>
         ((x.SecondOnlineApprovedTime.HasValue && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime).Value == Today && (x.Status == MaintenanceConst.Closed||x.Status==MaintenanceConst.Delivered))
         ||
         (x.Status == MaintenanceConst.Open)) && x.MaintenanceCenterId == maintenanceCenterId&&!x.IsVoid)
     .ToList();
            if (!ClosedJob.Any())
            {
                list.Add(new MVDailyMaintenanceReportVM
                {
                    Category = string.Empty,
                    EquipmentType = string.Empty,
                    OperatorName = string.Empty,
                    DateIn = null,
                    
                    DateClosed = null,
                    Location = string.Empty,
                    Remark = string.Empty,
                    PlateNumber = string.Empty,
                    Problems = string.Empty,
                    PhoneNumber = string.Empty
                });

            }
          
            var jobList = new List<Guid>();
            foreach (var closedjob in ClosedJob)
            {

                if (jobList.Contains(closedjob.JobOrderID)) 
                {
                    continue; // Skip duplicate
                }
                
                    MVDailyMaintenanceReportVM MVDMR = new MVDailyMaintenanceReportVM();
                    var getCategory = db.Fleets.FirstOrDefault(x => x.FleetsID == closedjob.FleetsID);
                    if (getCategory != null)
                    {
                        var categoryName = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == getCategory.EquipmentTypeID);
                        if (categoryName != null)
                        {
                            MVDMR.Category = db.EquipmentIdentity
                                .FirstOrDefault(x => x.EquipmentIdentityID == categoryName.EquipmentIdentityID)?
                                .EquipmentCategoryName ?? string.Empty;
                            MVDMR.EquipmentType = categoryName.EquipmentTypeName;
                        }
                    }
                var fetchRequest = db.JobRequest.FirstOrDefault(x => x.Num == closedjob.Referance);
                if(fetchRequest!=null)
                {
                    var fetchEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == fetchRequest.RequesterId);
                        if(fetchEmployee!=null)
                    {
                        MVDMR.OperatorName = fetchEmployee.FirstNameEnglish + " " + fetchEmployee.MiddleNameEnglish + " " + fetchEmployee.LastNameEnglish;
                        MVDMR.PhoneNumber = fetchEmployee.PhoneNo ?? "";
                    }
                }
                    if(closedjob.Status == MaintenanceConst.Open)
                    {
                    MVDMR.Status = "Under Repair";
                    }
                else
                {
                    MVDMR.Status="Closed";
                }
                var DateI = closedjob.DateCreated;
                MVDMR.DateIn = DateI.HasValue ? DateI.Value.ToString("yyyy/MM/dd") : string.Empty;
                var DateClose = closedjob.SecondOnlineApprovedTime;
                    MVDMR.Location = closedjob.MaintenanceCenter;
                    MVDMR.Remark = closedjob.Status;
                    MVDMR.PlateNumber = closedjob.LicensePlate;
                MVDMR.Num = closedjob.Num;
                MVDMR.DateClosed = DateClose.HasValue ? DateClose.Value.ToString("yyyy/MM/dd") : string.Empty;
                var fetchrequestId = db.JobRequest.FirstOrDefault(x => x.Num == closedjob.Referance);
                    if (fetchrequestId != null)
                    {
                    var jobCause = db.JobCauses.Where(x => x.OrderId == fetchrequestId.JobRequestId).ToList();
                    string xyz = null;
                    foreach (var cause in jobCause)
                    {
                        xyz = xyz + " " + cause.Description;
                    }

                    MVDMR.Problems = xyz;
                    }
                    list.Add(MVDMR);
                jobList.Add(closedjob.JobOrderID);


            }
           

            return list;
        }
        //var jobRequests = db.JobRequest.AsNoTracking().ToList();
        //var JobRequest = jobRequests
        //    .Where(x => x.DateCreated.HasValue && x.DateCreated == Today)
        //    .ToList();

        //foreach (var request in JobRequest)
        //{
        //    var checkIfClosed = db.JobOrder
        //        .FirstOrDefault(x => x.SecondOnlineApprovedTime.HasValue &&
        //                             x.SecondOnlineApprovedTime == Today &&
        //                             x.Referance == request.Num);

        //    if (checkIfClosed == null)
        //    {
        //        MVDailyMaintenanceReportVM MVDMR = new MVDailyMaintenanceReportVM();
        //        var getCategory = db.Fleets.FirstOrDefault(x => x.PlateNumber == request.LicensePlate);
        //        if (getCategory != null)
        //        {
        //            var categoryName = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == getCategory.EquipmentTypeID);
        //            if (categoryName != null)
        //            {
        //                MVDMR.Category = db.EquipmentIdentity
        //                    .FirstOrDefault(x => x.EquipmentIdentityID == categoryName.EquipmentIdentityID)?
        //                    .EquipmentCategoryName ?? string.Empty;
        //                MVDMR.EquipmentType = categoryName.EquipmentTypeName;
        //            }
        //        }
        //        MVDMR.Location = request.MaintenanceCenter;
        //        MVDMR.OperatorName = db.Person_Employee
        //            .FirstOrDefault(x => x.EmployeeId == request.RequesterId)?
        //            .FullNameaEnglish ?? string.Empty;
        //        MVDMR.DateIn = request.DateCreated ?? DateTime.MinValue;
        //        MVDMR.DateClosed = null;
        //        MVDMR.Remark = request.Remark;
        //        MVDMR.PlateNumber = request.LicensePlate;
        //        MVDMR.Problems = db.JobCauses
        //            .FirstOrDefault(x => x.OrderId == request.JobRequestId)?
        //            .Name ?? string.Empty;
        //        MVDMR.PhoneNumber = string.Empty;
        //        list.Add(MVDMR);
        //    }
        //}

        


    }

    public class MVDailyMaintenanceReportVM
    {
        public string Category { get; set; }
        public string EquipmentType { get; set; }
        public string PlateNumber { get; set; }
        public string DateIn { get; set; }
        public string DateClosed { get; set; }
        public string Problems { get; set; }
        public string Status { get; set; }
        public int Num { get; set; }
        public string Location { get; set; }
        public string OperatorName { get; set; }
        public string PhoneNumber { get; set; }
        public string Remark { get; set; }

    }
}

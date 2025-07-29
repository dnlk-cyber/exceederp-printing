//using ExceedERP.DataAccess.Context;
//using ExceedERP.Helpers.Common;  
//using Kendo.Mvc.UI;
//using Kendo.Mvc.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;     

//using System.Data.Entity;
//using ExceedERP.Core.Domain.Maintenance;
//namespace ExceedERP.Web.Areas.Maintenance.Controllers
//{
//    public class MaintenanceCostController : Controller
//    {


//        ExceedDbContext db = new ExceedDbContext();
 
//        public ActionResult MaintenanceCost_Read([DataSourceRequest] DataSourceRequest request , Guid Id)
//        {
//            List<MaintenanceCostDTO> list = new List<MaintenanceCostDTO>();
//           IQueryable<ServiceCrewWorkLog> FetchServiceCrewWorkLogs = db.ServiceCrewWorkLogs.AsQueryable();
//            foreach(var FetchedServiceCrew in FetchServiceCrewWorkLogs)
//            {
//                MaintenanceCostDTO MaintenanceCost = new MaintenanceCostDTO();
//                var FetchParent = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId == FetchedServiceCrew.JobId);
//                if (FetchParent != null)
//                {
//                    MaintenanceCost.EquipmentType = FetchParent.EquipmentName;
//                    MaintenanceCost.ServiceGroup =  FetchedServiceCrew.ServiceGroup;
//                    var serviceName = db.JobOrderDetail
//                     .Select(s => new
//                     {
//                         Name = s.RepairType.Name + "-" + s.JobCauses.Name
//                     })
//                     .FirstOrDefault(); 

//                     MaintenanceCost.Service = serviceName?.Name; // Handle null case
//                    MaintenanceCost.WorkShop = FetchedServiceCrew.WorkShopType;
//                    var FetchSparePart = db.StoreRequisitions.FirstOrDefault(x => x.ServiceJobOrderNo == FetchParent.ServiceCenterJobId.ToString());
//                    if(FetchSparePart!=null)
//                    {
//                        var FetchItems = db.StoreRequisitionItems.Where(x => x.StoreRequisitionID == FetchSparePart.StoreRequisitionID);
//                       if(FetchItems.Any())
//                        {
//                            foreach(var Item in FetchItems)
//                            {

//                               MaintenanceCost.SparePartCost=
//                            }
//                        }
//                    }
//                    MaintenanceCost.SparePartCost=
//                    }
//                      }
//            return Json(JobCauses.ToDataSourceResult(request));
//        }

       

//    }
//    public class MaintenanceCostDTO
//    {
//    public string EquipmentType { get; set; }
//    public string WorkCategory { get; set; }
//    public ServiceGroup ServiceGroup { get; set; }
//    public string Service { get; set; }
//    public WorkShopType WorkShop { get; set; }
//    public decimal LabCost { get; set; }
//    public decimal SparePartCost { get; set; }
//    public decimal LubricantCost { get; set; }
//    public decimal OtherCost { get; set; }
//    public decimal TotalCost { get; set; }
    
//    }
//}

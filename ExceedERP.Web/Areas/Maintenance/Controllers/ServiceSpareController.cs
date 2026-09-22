using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    [AuthorizeRoles(MaintenanceRoles.ServiceUser)]
    public class ServiceSpareController : Controller
    {
        private IServiceSpareService _ServiceSpareService;
        private ExceedDbContext db = new ExceedDbContext();
        public ServiceSpareController()
        {
            _ServiceSpareService = new ServiceSpareService();

        }

        //
        // GET: /FleetManagement/ServiceSpare/


        public ActionResult ServiceSpare_Read([DataSourceRequest] DataSourceRequest request)
        {
            var ServiceSpare = _ServiceSpareService.GetAll().Select(i => new ServiceSpare

            {
                ServiceSpareID = i.ServiceSpareID,
                ServiceID = i.ServiceID,
                Date = i.Date,
                ApprovalStatus = i.ApprovalStatus,
                ServiceName = i.ServiceName,
                SpareMaterialCategoryId = i.SpareMaterialCategoryId.ToString(),
                //sparepartname = i.sparepart.sparepartname,
                //category = i.SparePart.SpareMaterialCategory.Description,
                SparePartID = i.SparePartID.ToString(),
                Quantity = i.Quantity

            }

            );
            return Json(ServiceSpare.ToDataSourceResult(request));
        }




        public JsonResult ServiceSpare_Create(string sserviceID, string Type, [DataSourceRequest] DataSourceRequest request, ServiceSpare ServiceSpare)
        {
            ModelState.Remove("ServiceID");
            ModelState.Remove("ServiceSpareID");
            ModelState.Remove("InventoryItemId");
            //var Item = db.InventoryItems.Where(x => x.ItemCode == ServiceSpare.ServiceName).FirstOrDefault();
            //if (Item != null)
            //{
            List<ServiceSpare> Services = _ServiceSpareService.GetAll().Where(f => f.SparePartID == ServiceSpare.SparePartID && f.ServiceID == Guid.Parse(sserviceID)).ToList();
            if (Services.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", "  SparePart/Material Already Exists in Service ");
            }
            else
            {


                if (ServiceSpare != null)
                {
                    ServiceSpare.ServiceSpareID = Guid.NewGuid();
                    ServiceSpare.SpareMaterialCategoryId = ServiceSpare.SpareMaterialCategoryId;
                    ServiceSpare.SparePartID = ServiceSpare.SparePartID;
                    ServiceSpare.ServiceName = ServiceSpare.ServiceName;
                    ServiceSpare.ServiceID = Guid.Parse(sserviceID);
                    //ServiceSpare.Type = SpareType.Spare;
                    ServiceSpare.Quantity = ServiceSpare.Quantity;
                    ServiceSpare.CreatedBy = User.Identity.Name;
                    ServiceSpare.DateCreated = DateTime.Now;
                    ServiceSpare.ApprovalStatus = ApprovalStatuses.Pending;

                    ServiceSpare.TotalPrice = ServiceSpare.Quantity * ServiceSpare.UnitPrice;

                    _ServiceSpareService.Insert(ServiceSpare);
                }
                //}
            }
            return Json(new[] { ServiceSpare }.ToDataSourceResult(request, ModelState));
        }


        //public JsonResult ServiceBatterySpare_Create(string sserviceID, string Type, [DataSourceRequest] DataSourceRequest request, ServiceSpare ServiceSpare)
        //{
        //    ModelState.Remove("ServiceID");
        //    ModelState.Remove("ServiceSpareID");
        //    ModelState.Remove("InventoryItemId");
        //    var Item = db.InventoryItems.Where(x => x.ItemCode == ServiceSpare.ServiceName).FirstOrDefault();
        //    if (Item != null)
        //    {
        //        List<ServiceSpare> Services = _ServiceSpareService.GetAll().Where(f => f.InventoryItemId == ServiceSpare.ServiceName && f.ServiceID == Guid.Parse(sserviceID)).ToList();
        //        if (Services.Any())
        //        {
        //            ModelState.Clear();
        //            ModelState.AddModelError("ExistingName", "  Battery Already Exists in Service ");
        //        }
        //        else
        //        {


        //            if (ServiceSpare != null)
        //            {
        //                ServiceSpare.ServiceSpareID = Guid.NewGuid();
        //                ServiceSpare.InventoryItemId = Item.ItemCode;
        //                ServiceSpare.ServiceName = Item.Name;
        //                ServiceSpare.ServiceID = Guid.Parse(sserviceID);
        //                ServiceSpare.Type = SpareType.Battery;
        //                ServiceSpare.Quantity = ServiceSpare.Quantity;
        //                ServiceSpare.CreatedBy = User.Identity.Name;
        //                ServiceSpare.DateCreated = DateTime.Now;
        //                ServiceSpare.ApprovalStatus = ApprovalStatuses.Pending;

        //                _ServiceSpareService.Insert(ServiceSpare);
        //            }
        //        }
        //    }
        //    return Json(new[] { ServiceSpare }.ToDataSourceResult(request, ModelState));
        //}

        //public JsonResult ServiceTyreSpare_Create(string sserviceID, string Type, [DataSourceRequest] DataSourceRequest request, ServiceSpare ServiceSpare)
        //{
        //    ModelState.Remove("ServiceID");
        //    ModelState.Remove("ServiceSpareID");
        //    ModelState.Remove("InventoryItemId");
        //    var Item = db.InventoryItems.Where(x => x.ItemCode == ServiceSpare.ServiceName).FirstOrDefault();
        //    if (Item != null)
        //    {
        //        List<ServiceSpare> Services = _ServiceSpareService.GetAll().Where(f => f.InventoryItemId == ServiceSpare.ServiceName && f.ServiceID == Guid.Parse(sserviceID)).ToList();
        //        if (Services.Any())
        //        {
        //            ModelState.Clear();
        //            ModelState.AddModelError("ExistingName", "  SparePart Already Exists in Service ");
        //        }
        //        else
        //        {


        //            if (ServiceSpare != null)
        //            {
        //                ServiceSpare.ServiceSpareID = Guid.NewGuid();
        //                ServiceSpare.InventoryItemId = Item.ItemCode;
        //                ServiceSpare.ServiceName = Item.Name;
        //                ServiceSpare.ServiceID = Guid.Parse(sserviceID);
        //                ServiceSpare.Type = SpareType.Tyre;
        //                ServiceSpare.Quantity = ServiceSpare.Quantity;
        //                ServiceSpare.CreatedBy = User.Identity.Name;
        //                ServiceSpare.DateCreated = DateTime.Now;
        //                ServiceSpare.ApprovalStatus = ApprovalStatuses.Pending;

        //                _ServiceSpareService.Insert(ServiceSpare);
        //            }
        //        }
        //    }
        //    return Json(new[] { ServiceSpare }.ToDataSourceResult(request, ModelState));
        //}

        //public JsonResult ServiceOilLubricantSpare_Create(string sserviceID, string Type, [DataSourceRequest] DataSourceRequest request, ServiceSpare ServiceSpare)
        //{
        //    ModelState.Remove("ServiceID");
        //    ModelState.Remove("ServiceSpareID");
        //    ModelState.Remove("InventoryItemId");
        //    var Item = db.InventoryItems.Where(x => x.ItemCode == ServiceSpare.ServiceName).FirstOrDefault();
        //    if (Item != null)
        //    {
        //        List<ServiceSpare> Services = _ServiceSpareService.GetAll().Where(f => f.InventoryItemId == ServiceSpare.ServiceName && f.ServiceID == Guid.Parse(sserviceID)).ToList();
        //        if (Services.Any())
        //        {
        //            ModelState.Clear();
        //            ModelState.AddModelError("ExistingName", "  SparePart Already Exists in Service ");
        //        }
        //        else
        //        {


        //            if (ServiceSpare != null)
        //            {
        //                ServiceSpare.ServiceSpareID = Guid.NewGuid();
        //                ServiceSpare.InventoryItemId = Item.ItemCode;
        //                ServiceSpare.ServiceName = Item.Name;
        //                ServiceSpare.ServiceID = Guid.Parse(sserviceID);
        //                ServiceSpare.Type = SpareType.Oil_Lubricant;
        //                ServiceSpare.Quantity = ServiceSpare.Quantity;
        //                ServiceSpare.CreatedBy = User.Identity.Name;
        //                ServiceSpare.DateCreated = DateTime.Now;
        //                ServiceSpare.ApprovalStatus = ApprovalStatuses.Pending;

        //                _ServiceSpareService.Insert(ServiceSpare);
        //            }
        //        }
        //    }
        //    return Json(new[] { ServiceSpare }.ToDataSourceResult(request, ModelState));
        //}

        //public JsonResult ServiceOtherSpare_Create(string sserviceID, string Type, [DataSourceRequest] DataSourceRequest request, ServiceSpare ServiceSpare)
        //{
        //    ModelState.Remove("ServiceID");
        //    ModelState.Remove("ServiceSpareID");
        //    ModelState.Remove("InventoryItemId");
        //    var Item = db.InventoryItems.Where(x => x.ItemCode == ServiceSpare.ServiceName).FirstOrDefault();
        //    if (Item != null)
        //    {
        //        List<ServiceSpare> Services = _ServiceSpareService.GetAll().Where(f => f.InventoryItemId == ServiceSpare.ServiceName && f.ServiceID == Guid.Parse(sserviceID)).ToList();
        //        if (Services.Any())
        //        {
        //            ModelState.Clear();
        //            ModelState.AddModelError("ExistingName", "  SparePart Already Exists in Service ");
        //        }
        //        else
        //        {


        //            if (ServiceSpare != null)
        //            {
        //                ServiceSpare.ServiceSpareID = Guid.NewGuid();
        //                ServiceSpare.InventoryItemId = Item.ItemCode;
        //                ServiceSpare.ServiceName = Item.Name;
        //                ServiceSpare.ServiceID = Guid.Parse(sserviceID);
        //                ServiceSpare.Type = SpareType.Other;
        //                ServiceSpare.Quantity = ServiceSpare.Quantity;
        //                ServiceSpare.CreatedBy = User.Identity.Name;
        //                ServiceSpare.DateCreated = DateTime.Now;
        //                ServiceSpare.ApprovalStatus = ApprovalStatuses.Pending;

        //                _ServiceSpareService.Insert(ServiceSpare);
        //            }
        //        }
        //    }
        //    return Json(new[] { ServiceSpare }.ToDataSourceResult(request, ModelState));
        //}

        //public JsonResult OutSourceServiceSpare_Create(string sserviceID, string Type, [DataSourceRequest] DataSourceRequest request, ServiceSpare ServiceSpare)
        //{
        //    ModelState.Remove("ServiceID");
        //    ModelState.Remove("ServiceSpareID");
        //    ModelState.Remove("InventoryItemId");
        //    var Item = db.InventoryItems.Where(x => x.ItemCode == ServiceSpare.ServiceName).FirstOrDefault();
        //    if (Item != null)
        //    {
        //        List<ServiceSpare> Services = _ServiceSpareService.GetAll().Where(f => f.InventoryItemId == ServiceSpare.ServiceName && f.ServiceID == Guid.Parse(sserviceID)).ToList();
        //        if (Services.Any())
        //        {
        //            ModelState.Clear();
        //            ModelState.AddModelError("ExistingName", "  SparePart Already Exists in Service ");
        //        }
        //        else
        //        {


        //            if (ServiceSpare != null)
        //            {
        //                ServiceSpare.ServiceSpareID = Guid.NewGuid();
        //                ServiceSpare.InventoryItemId = Item.ItemCode;
        //                ServiceSpare.ServiceName = Item.Name;
        //                ServiceSpare.ServiceID = Guid.Parse(sserviceID);
        //                ServiceSpare.Type = SpareType.Other;
        //                ServiceSpare.Quantity = ServiceSpare.Quantity;
        //                ServiceSpare.CreatedBy = User.Identity.Name;
        //                ServiceSpare.DateCreated = DateTime.Now;
        //                ServiceSpare.ApprovalStatus = ApprovalStatuses.Pending;

        //                _ServiceSpareService.Insert(ServiceSpare);
        //            }
        //        }
        //    }
        //    return Json(new[] { ServiceSpare }.ToDataSourceResult(request, ModelState));
        //}
        public JsonResult ExternalServiceSpare_Create(string sserviceID, [DataSourceRequest] DataSourceRequest request, ExternalMaterial ServiceSpare)
        {
            ModelState.Remove("ExternalMaterialID");
            ModelState.Remove("JobId");
            ISparePartService PartService = new SparePartService();
            Guid JobId = Guid.Parse(sserviceID);
            List<ExternalMaterial> Services = db.ExternalMaterial.Where(f => f.MaterialName == ServiceSpare.MaterialName && f.JobId == JobId).ToList();
            if (Services.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", "  SparePart Already Exists in Service ");
            }
            else
            {


                if (ServiceSpare != null)
                {
                    ServiceSpare.ExternalMaterialID = Guid.NewGuid();

                    ServiceSpare.JobId = Guid.Parse(sserviceID);
                    ServiceSpare.CreatedBy = User.Identity.Name;
                    ServiceSpare.DateCreated = DateTime.Now;
                    ServiceSpare.TotalAmount = ServiceSpare.Quantity * ServiceSpare.UnitPrice;
                    ServiceSpare.ApprovalStatuses = ApprovalStatuses.Pending;
                    db.ExternalMaterial.Add(ServiceSpare);
                    db.SaveChanges();
                }
            }
            return Json(new[] { ServiceSpare }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalServiceSpare_Approve([DataSourceRequest] DataSourceRequest request, ExternalMaterial ServiceSpare)
        {
            if (ServiceSpare != null)
            {
                if (ServiceSpare.ApprovalStatuses == ApprovalStatuses.Approve)
                {
                    ServiceSpare.IsOnlineApproved = true;
                    ServiceSpare.OnlineApprovedTime = DateTime.Now;
                    ServiceSpare.OnlineApprovedBy = User.Identity.Name;
                }

                db.ExternalMaterial.Attach(ServiceSpare);
                db.Entry(ServiceSpare).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ServiceSpare }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceSpare_Approve([DataSourceRequest] DataSourceRequest request, ServiceSpare ServiceSpare)
        {

            if (ServiceSpare != null)
            {

                if (ServiceSpare.ApprovalStatus == ApprovalStatuses.Approve)
                {
                    ServiceSpare.IsOnlineApproved = true;
                    ServiceSpare.OnlineApprovedTime = DateTime.Now;
                    ServiceSpare.OnlineApprovedBy = User.Identity.Name;

                    if (ServiceSpare.Type == SpareType.Battery)
                    {

                    }

                    if (ServiceSpare.Type == SpareType.Tyre)
                    {

                    }
                }
                _ServiceSpareService.Update(ServiceSpare);
            }

            return Json(new[] { ServiceSpare }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceSpare_Update([DataSourceRequest] DataSourceRequest request, ServiceSpare serviceSpare)
        {
            ModelState.Remove("Type");
            ModelState.Remove("UsedQuantity");
            ModelState.Remove("UnitPrice");
            ModelState.Remove("DateCreated");
            ModelState.Clear();
            //ModelState.Remove("UnitPrice");
            //var Spare = _ServiceSpareService.GetById(ServiceSpare.ServiceSpareID);

            //if (ServiceSpare.Quantity > Spare.Quantity)
            //{
            //    ModelState.AddModelError("InvalidQunatity", " Used Quantity must be Less Than Actual Quantity");
            //    return Json(new[] { ServiceSpare }.ToDataSourceResult(request, ModelState));
            //}
            //else
            //{
            _ServiceSpareService = new ServiceSpareService();

            var sparePartFromContext = _ServiceSpareService.GetById(serviceSpare.ServiceSpareID);
            if (sparePartFromContext != null)
            {
                sparePartFromContext.SpareMaterialCategoryId = serviceSpare.SpareMaterialCategoryId;
                sparePartFromContext.SparePartID = serviceSpare.SparePartID;
                sparePartFromContext.Quantity = serviceSpare.Quantity;
                sparePartFromContext.UnitPrice = serviceSpare.UnitPrice;

                sparePartFromContext.ModifiedBy = User.Identity.Name;
                sparePartFromContext.LastModified = DateTime.Now;
                //ServiceSpare.ApprovalStatus = ServiceSpare.ApprovalStatus;

                sparePartFromContext.TotalPrice = serviceSpare.Quantity * serviceSpare.UnitPrice;
            }
            //if (ServiceSpare != null)
            //{



            _ServiceSpareService.Update(sparePartFromContext);
            //}
            //}

            return Json(new[] { sparePartFromContext }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExServiceSpare_Approve([DataSourceRequest] DataSourceRequest request, ExternalMaterial ExternalMaterial)
        {

            if (ExternalMaterial != null)
            {

                if (ExternalMaterial.ApprovalStatuses == ApprovalStatuses.Approve)
                {
                    ExternalMaterial.IsOnlineApproved = true;
                    ExternalMaterial.OnlineApprovedTime = DateTime.Now;
                    ExternalMaterial.OnlineApprovedBy = User.Identity.Name;

                    //if (ExternalMaterial.Type == SpareType.Battery)
                    //{

                    //}

                    //if (ExternalMaterial.Type == SpareType.Tyre)
                    //{

                    //}
                }

                db.ExternalMaterial.Attach(ExternalMaterial);
                db.Entry(ExternalMaterial).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ExternalMaterial }.ToDataSourceResult(request));
        }

        //public JsonResult OutSourceServiceSpareByService(string sserviceID, [DataSourceRequest] DataSourceRequest request)
        //{
        //    _ServiceSpareService = new ServiceSpareService();
        //    return Json(_ServiceSpareService.GetAll().Where(s => s.ServiceID.ToString() == sserviceID).Select(f => new { ServiceSpareID = f.ServiceSpareID, ServiceID = f.ServiceID, InventoryItemId = f.InventoryItemId, ServiceName = f.ServiceName, Quantity = f.Quantity, Date = f.Date, ApprovalStatus = f.ApprovalStatus, Remark = f.Remark, CreatedBy = f.CreatedBy, DateCreated = f.DateCreated }).ToDataSourceResult(request));
        //}
        public JsonResult ServiceSpareByService(string sserviceID, [DataSourceRequest] DataSourceRequest request)
        {
            _ServiceSpareService = new ServiceSpareService();
            //List<ServiceSpare> IssueSpares = new List<ServiceSpare>();
            //FleetReportHelper fleetReportHelper   = new FleetReportHelper();
            List<ServiceSpare> Services =
                _ServiceSpareService.GetAll().Where(f => f.ServiceID == Guid.Parse(sserviceID)).ToList();

            //var JobOrder = db.JobOrder.Where(x => x.Num.ToString() == sserviceID).FirstOrDefault();
            //if (JobOrder != null)
            //{
            //    var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == JobOrder.Num.ToString() || x.JobOrderNo == JobOrder.JobOrderID.ToString());
            //    //foreach (var StoreRequestion in StoreRequestions)
            //    //{
            //    //    var Requistions = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID).ToList();
            //    //    var SpareMap = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Spare).ToList();
            //    //    foreach (var Map in SpareMap)
            //    //    {
            //    //        foreach (var Requistion in Requistions.Where(x => x.InventoryItem.ItemCategory.ItemCategoryCode == Map.InventoryCategoryId))
            //    //        {
            //    //            IssueSpares.Add(new ServiceSpare { ServiceSpareID = Guid.NewGuid(), Type = SpareType.Spare, InventoryItemId = Requistion.ItemCode + " - " + Requistion.Name, UsedQuantity = Requistion.ApprovedQuantity, Quantity = Requistion.ApprovedQuantity, Date = Requistion.DateCreated, Status = Requistion.Issue.Status, Remark = Requistion.Issue.Remark, CreatedBy = Requistion.CreatedBy, DateCreated = Requistion.DateCreated });


            //    //        }
            //    //    }
            //    //}
            //}

            //return Json(IssueSpares.ToDataSourceResult(request));
            return Json(Services.ToDataSourceResult(request));
        }

        //     public JsonResult ServiceSpareTyreByService(string sserviceID, [DataSourceRequest] DataSourceRequest request)
        //     {
        //         _ServiceSpareService = new ServiceSpareService();
        //         List<ServiceSpare> IssueSpares = new List<ServiceSpare>();
        //         FleetReportHelper fleetReportHelper = new FleetReportHelper();
        //         var JobOrder = db.JobOrder.Where(x => x.Num.ToString() == sserviceID).FirstOrDefault();
        //         if (JobOrder != null)
        //         {
        //             var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == JobOrder.Num.ToString() || x.JobOrderNo == JobOrder.JobOrderID.ToString());
        //             foreach (var StoreRequestion in StoreRequestions)
        //             {
        //                 var Requistions = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID).ToList();
        //                 var SpareMap = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Tyre).ToList();
        //                 foreach (var Map in SpareMap)
        //                 {
        //                     foreach (var Requistion in Requistions.Where(x => x.InventoryItem.ItemCategory.ItemCategoryCode == Map.InventoryCategoryId))
        //                     {
        //                         IssueSpares.Add(new ServiceSpare { ServiceSpareID = Guid.NewGuid(), Type = SpareType.Tyre, InventoryItemId = Requistion.ItemCode + " - " + Requistion.Name, UsedQuantity = Requistion.ApprovedQuantity, Quantity = Requistion.ApprovedQuantity, Date = Requistion.DateCreated, Status = Requistion.Issue.Status, Remark = Requistion.Issue.Remark, CreatedBy = Requistion.CreatedBy, DateCreated = Requistion.DateCreated });


        //                     }
        //                 }
        //             }
        //         }


        //         return Json(IssueSpares.ToDataSourceResult(request));
        //     }
        //     public JsonResult ServiceSpareBatteryByService(string sserviceID, [DataSourceRequest] DataSourceRequest request)
        //     {
        //         _ServiceSpareService = new ServiceSpareService();
        //         List<ServiceSpare> IssueSpares = new List<ServiceSpare>();
        //         FleetReportHelper fleetReportHelper = new FleetReportHelper();
        //         var JobOrder = db.JobOrder.Where(x => x.Num.ToString() == sserviceID).FirstOrDefault();
        //         if (JobOrder != null)
        //         {
        //             var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == JobOrder.Num.ToString() || x.JobOrderNo == JobOrder.JobOrderID.ToString());
        //             foreach (var StoreRequestion in StoreRequestions)
        //             {
        //                 var Requistions = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID).ToList();
        //                 var SpareMap = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Battery).ToList();
        //                 foreach (var Map in SpareMap)
        //                 {
        //                     foreach (var Requistion in Requistions.Where(x => x.InventoryItem.ItemCategory.ItemCategoryCode == Map.InventoryCategoryId))
        //                     {
        //                         IssueSpares.Add(new ServiceSpare { ServiceSpareID = Guid.NewGuid(), Type = SpareType.Battery, InventoryItemId = Requistion.ItemCode + " - " + Requistion.Name, UsedQuantity = Requistion.ApprovedQuantity, Quantity = Requistion.ApprovedQuantity, Date = Requistion.DateCreated, Status = Requistion.Issue.Status, Remark = Requistion.Issue.Remark, CreatedBy = Requistion.CreatedBy, DateCreated = Requistion.DateCreated });


        //                     }
        //                 }
        //             }
        //         }


        //         return Json(IssueSpares.ToDataSourceResult(request));
        //     }
        //     public JsonResult ServiceSpareOilLubricantByService(string sserviceID, [DataSourceRequest] DataSourceRequest request)
        //     {
        //         _ServiceSpareService = new ServiceSpareService();
        //         List<ServiceSpare> IssueSpares = new List<ServiceSpare>();
        //         FleetReportHelper fleetReportHelper = new FleetReportHelper();
        //         var JobOrder = db.JobOrder.Where(x => x.Num.ToString() == sserviceID).FirstOrDefault();
        //         if (JobOrder != null)
        //         {
        //             var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == JobOrder.Num.ToString() || x.JobOrderNo == JobOrder.JobOrderID.ToString());
        //             foreach (var StoreRequestion in StoreRequestions)
        //             {
        //                 var Requistions = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID).ToList();
        //                 var SpareMap = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Oil_Lubricant).ToList();
        //                 foreach (var Map in SpareMap)
        //                 {
        //                     foreach (var Requistion in Requistions.Where(x => x.InventoryItem.ItemCategory.ItemCategoryCode == Map.InventoryCategoryId))
        //                     {

        //IssueSpares.Add(new ServiceSpare { ServiceSpareID = Guid.NewGuid(), Type = SpareType.Oil_Lubricant, InventoryItemId = Requistion.ItemCode + " - " + Requistion.Name, UsedQuantity = Requistion.ApprovedQuantity, Quantity = Requistion.ApprovedQuantity, Date = Requistion.DateCreated, Status = Requistion.Issue.Status, Remark = Requistion.Issue.Remark, CreatedBy = Requistion.CreatedBy, DateCreated = Requistion.DateCreated });


        //                     }
        //                 }
        //             }

        //         }
        //         return Json(IssueSpares.ToDataSourceResult(request));
        //     }
        //     public JsonResult ServiceSpareOtherByService(string sserviceID, [DataSourceRequest] DataSourceRequest request)
        //     {
        //         _ServiceSpareService = new ServiceSpareService();
        //         List<ServiceSpare> IssueSpares = new List<ServiceSpare>();
        //         FleetReportHelper fleetReportHelper = new FleetReportHelper();
        //         var JobOrder = db.JobOrder.Where(x => x.Num.ToString() == sserviceID).FirstOrDefault();
        //         if (JobOrder != null)
        //         {
        //             var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == JobOrder.Num.ToString() || x.JobOrderNo == JobOrder.JobOrderID.ToString());
        //             foreach (var StoreRequestion in StoreRequestions)
        //             {
        //                 var Requistions = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID).ToList();
        //                 var SpareMap = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Other).ToList();
        //                 foreach (var Map in SpareMap)
        //                 {
        //                     foreach (var Requistion in Requistions.Where(x => x.InventoryItem.ItemCategory.ItemCategoryCode == Map.InventoryCategoryId))
        //                     {


        //                         IssueSpares.Add(new ServiceSpare { ServiceSpareID = Guid.NewGuid(), Type = SpareType.Other, InventoryItemId = Requistion.ItemCode + " - " + Requistion.Name, UsedQuantity = Requistion.ApprovedQuantity, Quantity = Requistion.ApprovedQuantity, Date = Requistion.DateCreated, Status = Requistion.Issue.Status, Remark = Requistion.Issue.Remark, CreatedBy = Requistion.CreatedBy, DateCreated = Requistion.DateCreated });


        //                     }
        //                 }
        //             }
        //             }


        //         return Json(IssueSpares.ToDataSourceResult(request));
        //     }

        //     public JsonResult OutSourceInternalSpareByService(string sserviceID, [DataSourceRequest] DataSourceRequest request)
        //     {
        //         _ServiceSpareService = new ServiceSpareService();
        //         return Json(_ServiceSpareService.GetAll().Where(s => s.ServiceID.ToString() == sserviceID).Select(f => new { ServiceSpareID = f.ServiceSpareID, ServiceID = f.ServiceID, Type = f.Type, InventoryItemId = f.InventoryItemId, ServiceName = f.ServiceName, Quantity = f.Quantity, Date = f.Date, ApprovalStatus = f.ApprovalStatus, Remark = f.Remark, CreatedBy = f.CreatedBy, DateCreated = f.DateCreated }).ToDataSourceResult(request));
        //     }
        public JsonResult ExternalServiceSpareByService(string RserviceID, [DataSourceRequest] DataSourceRequest request)
        {

            _ServiceSpareService = new ServiceSpareService();
            Guid Id = Guid.Parse(RserviceID.ToString());
            return Json(db.ExternalMaterial.Where(s => s.JobId == Id).Select(f => new { ExternalMaterialID = f.ExternalMaterialID, JobId = f.JobId, MaterialName = f.MaterialName, TotalAmount = f.TotalAmount, Type = f.Type, UnitPrice = f.UnitPrice, Quantity = f.Quantity, ApprovalStatuses = f.ApprovalStatuses, Remark = f.Remark, CreatedBy = f.CreatedBy, DateCreated = f.DateCreated }).ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceSpare_Destroy([DataSourceRequest] DataSourceRequest request, ServiceSpare serviceSpare)
        {
            var ServiceSpare = _ServiceSpareService.GetById(serviceSpare.ServiceSpareID);
            if (ServiceSpare != null)
            {
                _ServiceSpareService.Delete(ServiceSpare);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " Delete Service Spare number " + serviceSpare.ServiceSpareID);
            }

            return Json(new[] { ServiceSpare }.ToDataSourceResult(request));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

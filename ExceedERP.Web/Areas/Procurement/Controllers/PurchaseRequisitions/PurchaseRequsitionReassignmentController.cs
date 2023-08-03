using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PurchaseRequisitions
    {
     public class PurchaseRequsitionReassignmentController : Controller
        {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementTeamLeader, ProcurementRoles.ProcurementApprover)]
        public ActionResult Index()
            {
            return View();
            }

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementTeamLeader, ProcurementRoles.ProcurementApprover)]
        public ActionResult PurchaseRequisitionAssignments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PurchaseRequisitionAssignment> purchaseRequisitionAssignments = _db.PurchaseRequisitionAssignments.Where(pri => pri.PurchaseRequisitionID == id);
            DataSourceResult result = purchaseRequisitionAssignments.ToDataSourceResult(request, purchaseRequisitionAssignment => new {
                PurchaseRequisitionAssignmentId = purchaseRequisitionAssignment.PurchaseRequisitionAssignmentId,
                PurchaseRequisitionID = purchaseRequisitionAssignment.PurchaseRequisitionID,
                ProcurementOfficer = purchaseRequisitionAssignment.ProcurementOfficer,
                ProcurementMethod = purchaseRequisitionAssignment.ProcurementMethod,
                PurchaseCategory = purchaseRequisitionAssignment.PurchaseCategory,
                TenderType = purchaseRequisitionAssignment.TenderType,

                DateCreated = purchaseRequisitionAssignment.DateCreated,
                CreatedBy = purchaseRequisitionAssignment.CreatedBy,
                LastModified = purchaseRequisitionAssignment.LastModified,
                ModifiedBy = purchaseRequisitionAssignment.ModifiedBy,

                IsVoid = purchaseRequisitionAssignment.IsVoid,
                VoidBy = purchaseRequisitionAssignment.VoidBy,
                VoidTime = purchaseRequisitionAssignment.VoidTime,
                IsOnlineApproved = purchaseRequisitionAssignment.IsOnlineApproved,
                OnlineApprovedBy = purchaseRequisitionAssignment.OnlineApprovedBy,
                OnlineApprovedTime = purchaseRequisitionAssignment.OnlineApprovedTime,
            });

            return Json(result);
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementTeamLeader)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionAssignments_Create([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionAssignment purchaseRequisitionAssignment, int id)
        {
            if (purchaseRequisitionAssignment.ProcurementMethod == ProcurementMethod.Tender && purchaseRequisitionAssignment.TenderType == TenderType.NA)
                ModelState.AddModelError("tendertype", "Please choose correct tender type");

            if (purchaseRequisitionAssignment.ProcurementMethod != ProcurementMethod.Tender && purchaseRequisitionAssignment.TenderType != TenderType.NA)
                ModelState.AddModelError("tendertype", "Please choose correct tender type");

            var purchaseReq =_db.PurchaseRequisitions.Find(purchaseRequisitionAssignment.PurchaseRequisitionID);

            if (purchaseReq != null&& (purchaseReq.ProcurementMethod != ProcurementMethod.DirectPurchase && purchaseReq.FromPreviousPurchase))
                ModelState.AddModelError("notallowed","Only direct purchase is allowed because it is made to be from previous purchase");

                                  
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionAssignment
                {
                    PurchaseRequisitionID = id,
                    ProcurementOfficer = purchaseRequisitionAssignment.ProcurementOfficer,
                    ProcurementMethod = purchaseRequisitionAssignment.ProcurementMethod,
                    PurchaseCategory = purchaseRequisitionAssignment.PurchaseCategory,
                    TenderType = purchaseRequisitionAssignment.TenderType,

                    CreatedBy = User.Identity.Name,
                    DateCreated=DateTime.Now
                };

                _db.PurchaseRequisitionAssignments.Add(entity);
                _db.SaveChanges();
                purchaseRequisitionAssignment.PurchaseRequisitionAssignmentId = entity.PurchaseRequisitionAssignmentId;
                GetPrItems(entity);
                #region notification
                var record = _db.PurchaseRequisitions.Find(id);
                if (record != null)
                {
                    NotifModels notif = new NotifModels
                    {
                        UserId = UserInformationHelper.GetEmployeeUserId(record.ProcurementOfficer),
                        Title="Purchase requisition reassignment",
                        Message = $" {UserInformationHelper.GetUserFullName(User.Identity.Name)} has assigned you a a purchaser for document purchase requisition with ID {record.PurchaseRequisitionID}  with method  {purchaseRequisitionAssignment.ProcurementMethod.ToString()} "
                    };

                    Notify.NotifyUser(notif);
                }

                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition reassignment", "Success", User.Identity.Name, $"created reassignemnt {entity.PurchaseRequisitionAssignmentId} under record {entity.PurchaseRequisitionID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseRequisitionAssignment }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementTeamLeader)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionAssignments_Update([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionAssignment purchaseRequisitionAssignment)
        {
            if(purchaseRequisitionAssignment.ProcurementMethod==ProcurementMethod.Tender && purchaseRequisitionAssignment.TenderType==TenderType.NA)
                ModelState.AddModelError("tendertype","Please choose correct tender type");

            if (purchaseRequisitionAssignment.ProcurementMethod != ProcurementMethod.Tender && purchaseRequisitionAssignment.TenderType != TenderType.NA)
                ModelState.AddModelError("tendertype", "Please choose correct tender type");

            var purchaseReq = _db.PurchaseRequisitions.Find(purchaseRequisitionAssignment.PurchaseRequisitionID);

            if (purchaseReq != null && (purchaseReq.ProcurementMethod != ProcurementMethod.DirectPurchase && purchaseReq.FromPreviousPurchase))
                ModelState.AddModelError("notallowed", "Only direct purchase is allowed because it is made to be from previous purchase");

            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionAssignment
                {
                    PurchaseRequisitionID = purchaseRequisitionAssignment.PurchaseRequisitionID,
                    PurchaseRequisitionAssignmentId = purchaseRequisitionAssignment.PurchaseRequisitionAssignmentId,
                    ProcurementOfficer = purchaseRequisitionAssignment.ProcurementOfficer,
                    ProcurementMethod = purchaseRequisitionAssignment.ProcurementMethod,
                    PurchaseCategory = purchaseRequisitionAssignment.PurchaseCategory,
                    TenderType = purchaseRequisitionAssignment.TenderType,
                    CreatedBy = purchaseRequisitionAssignment.CreatedBy,
                    DateCreated = purchaseRequisitionAssignment.DateCreated,

                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today,
                };

                _db.PurchaseRequisitionAssignments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region notification
                var record = _db.PurchaseRequisitions.Find(purchaseRequisitionAssignment.PurchaseRequisitionID);
                if (record != null)
                {
                    NotifModels notif = new NotifModels
                    {
                        UserId = UserInformationHelper.GetEmployeeUserId(record.ProcurementOfficer),
                        Title = "Purchase requisition reassignment",
                        Message = $" {UserInformationHelper.GetUserFullName(User.Identity.Name)} has assigned you a a purchaser for document purchase requisition with ID {record.PurchaseRequisitionID}  with method  {purchaseRequisitionAssignment.ProcurementMethod.ToString()} "
                    };

                    Notify.NotifyUser(notif);
                }

                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition reassignment", "Success", User.Identity.Name, $"updated reassignemnt {entity.PurchaseRequisitionAssignmentId} under record {entity.PurchaseRequisitionID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseRequisitionAssignment }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementTeamLeader)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionAssignments_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionAssignment purchaseRequisitionAssignment)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionAssignment
                {
                    PurchaseRequisitionAssignmentId = purchaseRequisitionAssignment.PurchaseRequisitionAssignmentId,
                    ProcurementOfficer = purchaseRequisitionAssignment.ProcurementOfficer,
                    ProcurementMethod = purchaseRequisitionAssignment.ProcurementMethod,
                    PurchaseCategory = purchaseRequisitionAssignment.PurchaseCategory,
                };

                _db.PurchaseRequisitionAssignments.Attach(entity);
                _db.PurchaseRequisitionAssignments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { purchaseRequisitionAssignment }.ToDataSourceResult(request, ModelState));
        }

        private void GetPrItems(PurchaseRequisitionAssignment record)
            {
                var purchaseRequisitionItems = _db.PurchaseRequisitionItems.Where(sri => sri.PurchaseRequisitionID == record.PurchaseRequisitionID).ToList();
                foreach (var requestedItem in purchaseRequisitionItems)
                {
                    var purchaseRequisitionItem = new PurchaseRequisitionReassignmentItem
                    {
                        CatCode = requestedItem.CatCode,
                        CategoryName = requestedItem.CategoryName,
                        ItmCode = requestedItem.ItmCode,
                        ItemName = requestedItem.ItemName,
                        Name = requestedItem.Name,
                        PartNo = requestedItem.PartNo,
                        ModelNo = requestedItem.ModelNo,
                        SerialNo = requestedItem.SerialNo,
                        ChasisNo = requestedItem.ChasisNo,
                        PurchaseRequisitionAssignmentId = record.PurchaseRequisitionAssignmentId,
                        Quantity = requestedItem.Quantity,
                        ItemType = requestedItem.ItemType,
                        UoM = requestedItem.UoM,
                        ItemSpecification=requestedItem.ItemSpecification,

                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                    };


                    _db.PurchaseRequisitionReassignmentItems.Add(purchaseRequisitionItem);
                    _db.SaveChanges();
                }
            }
        protected override void Dispose(bool disposing)
            {
            _db.Dispose();
            base.Dispose(disposing);
            }
        }
    }
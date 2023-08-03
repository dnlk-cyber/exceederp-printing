using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Proforma
{


    public class ProformaPurchaseController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            //var categories = db.ItemCategorys;
            var items = _db.ProformaItems;
            var fixedAsset = _db.AssetRegistrations;
            var branches = _db.Branch;
            var costCenter = _db.BranchCostCenter;
            var tax = _db.GLTaxRates;

            var participatingSuppliers = _db.ParticipatingSuppliers;

            ViewData["suppliers"] = participatingSuppliers;

            ViewData["tax"] = tax;
            ViewData["taxes"] = tax;
            ViewData["costCenters"] = costCenter;
            ViewData["Branches"] = branches;

            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItmCode + "-" + i.ItemName + "-" + i.Name,
                    ItmCode = i.ItmCode
                }).AsQueryable();

            ViewData["Items"] = selecteditems;

            var lotitems = items.Select(
                i => new
                {
                    Name = i.ItmCode + "_" + i.ItemName + "_" + i.Name,
                    ProformaItemID = i.ProformaItemID
                }).AsQueryable();

            ViewData["ProformaItems"] = lotitems;

            var selectedcatedories = items.Select(
                i => new
                {
                    Name = i.CatCode + "-" + i.CategoryName,
                    CatCode = i.CatCode
                }).AsQueryable();
            ViewData["Category"] = selectedcatedories;

            ViewData["fixedAsset"] = fixedAsset;

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

        }


        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult ProformaPurchases_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProformaPurchase> proformapurchases = _db.ProformaPurchases;

            if (System.Web.HttpContext.Current.User.IsInRole(ProcurementRoles.ProcurementTenderApprover))
            {
            }
            else if (System.Web.HttpContext.Current.User.IsInRole(ProcurementRoles.ProcurementTenderUser))
            {
                int empid = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
                proformapurchases = proformapurchases.Where(x => x.ProcurementOfficer == empid && empid != 0 || x.ForwardedTo == User.Identity.Name);
            }

            

            DataSourceResult result = proformapurchases.ToDataSourceResult(request, proformaPurchase => new
            {
                ProformaPurchaseID = proformaPurchase.ProformaPurchaseID,
                ProcurementOfficer = proformaPurchase.ProcurementOfficer,
                BranchId = proformaPurchase.BranchId,
                BusinessUnitId = proformaPurchase.BusinessUnitId,
                PurchaseRequisitionAssignmentId = proformaPurchase.PurchaseRequisitionAssignmentId,
                PurchaseRequisitionID = proformaPurchase.PurchaseRequisitionID,
                StoreRequisitionID = proformaPurchase.StoreRequisitionID,
                ProformaReference = proformaPurchase.ProformaReference,
                Description = proformaPurchase.Description,
                FloatDate = proformaPurchase.FloatDate,
                ClosingDate = proformaPurchase.ClosingDate,
                EvaluationDate = proformaPurchase.EvaluationDate,
                PurchaseCategory = proformaPurchase.PurchaseCategory,
                Status = proformaPurchase.Status,
                PurchaseStatus = proformaPurchase.PurchaseStatus,
                EvaluateTotalFinance = proformaPurchase.EvaluateTotalFinance,

                ForwardedFrom = proformaPurchase.ForwardedFrom,
                ForwardedTo = proformaPurchase.ForwardedTo,
                IsFinalApproval = proformaPurchase.IsFinalApproval,

                IsOnlineApproved = proformaPurchase.IsOnlineApproved,
                OnlineApprovedBy = proformaPurchase.OnlineApprovedBy,
                OnlineApprovedTime = proformaPurchase.OnlineApprovedTime,
                IsSendForApproval = proformaPurchase.IsSendForApproval,
                SendForApprovalBy = proformaPurchase.SendForApprovalBy,
                SendForApprovalTime = proformaPurchase.SendForApprovalTime,
                IsVoid = proformaPurchase.IsVoid,
                VoidBy = proformaPurchase.VoidBy,
                VoidTime = proformaPurchase.VoidTime,
                OrgBranchName = proformaPurchase.OrgBranchName,
                DateCreated = proformaPurchase.DateCreated,
                LastModified = proformaPurchase.LastModified,
                CreatedBy = proformaPurchase.CreatedBy,
                ModifiedBy = proformaPurchase.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaPurchases_Create([DataSourceRequest]DataSourceRequest request, ProformaPurchase proformaPurchase)
        {
           

            #region validation

            if (proformaPurchase.FloatDate > proformaPurchase.ClosingDate)
                ModelState.AddModelError("float", "Proforma Float date should be before closing date");
            if (proformaPurchase.ClosingDate >= proformaPurchase.EvaluationDate)
                ModelState.AddModelError("float", "Proforma closing date should be before evaluation date");

            //var exists = _db.ProformaPurchases.Where(x => x.PurchaseRequisitionID == proformaPurchase.PurchaseRequisitionID).ToList();
            //if (exists.Any())
            //{
            //    ModelState.AddModelError("exist","Purchase requisision no. "+proformaPurchase.PurchaseRequisitionID +" is already used in other proforma.");
            //}

            #endregion

            var assignment = _db.PurchaseRequisitionAssignments.Find(proformaPurchase.PurchaseRequisitionAssignmentId);
            if (assignment != null)
            {
                var pr = _db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);
                if (pr != null)
                {
                    proformaPurchase.StoreRequisitionID = pr.StoreRequisitionID;
                    proformaPurchase.PurchaseRequisitionID = pr.PurchaseRequisitionID;
                    proformaPurchase.BranchId = pr.BranchId;
                    proformaPurchase.BusinessUnitId = pr.BusinessUnitId;
                    proformaPurchase.ProcurementOfficer = pr.ProcurementOfficer;
                }

                else
                {
                    proformaPurchase.BranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
                    proformaPurchase.BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                    proformaPurchase.ProcurementOfficer = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
                }
            }

            if (ModelState.IsValid)
            {
               
                var entity = new ProformaPurchase
                {
                    PurchaseRequisitionAssignmentId = proformaPurchase.PurchaseRequisitionAssignmentId,
                    StoreRequisitionID = proformaPurchase.StoreRequisitionID,
                    ProformaReference = proformaPurchase.ProformaReference,
                    ProcurementOfficer = proformaPurchase.ProcurementOfficer,
                    BranchId = proformaPurchase.BranchId,
                    BusinessUnitId = proformaPurchase.BusinessUnitId,
                    Description = proformaPurchase.Description,
                    FloatDate = proformaPurchase.FloatDate,
                    PurchaseRequisitionID = proformaPurchase.PurchaseRequisitionID,
                    ClosingDate = proformaPurchase.ClosingDate,
                    EvaluationDate = proformaPurchase.EvaluationDate,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    PurchaseStatus = proformaPurchase.PurchaseStatus,
                    EvaluateTotalFinance = proformaPurchase.EvaluateTotalFinance,

                    ForwardedFrom = User.Identity.Name,
                    ForwardedTo = User.Identity.Name,
                    IsFinalApproval = proformaPurchase.IsFinalApproval,

                    PurchaseCategory = proformaPurchase.PurchaseCategory,
                    IsOnlineApproved = proformaPurchase.IsOnlineApproved,
                    OnlineApprovedBy = proformaPurchase.OnlineApprovedBy,
                    OnlineApprovedTime = proformaPurchase.OnlineApprovedTime,
                    IsVoid = proformaPurchase.IsVoid,
                    VoidBy = proformaPurchase.VoidBy,
                    VoidTime = proformaPurchase.VoidTime,
                    OrgBranchName = proformaPurchase.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = proformaPurchase.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = proformaPurchase.ModifiedBy
                };

                _db.ProformaPurchases.Add(entity);
                _db.SaveChanges();
                proformaPurchase.ProformaPurchaseID = entity.ProformaPurchaseID;
               
                PurchaseRequistionItems(entity.PurchaseRequisitionAssignmentId, entity.ProformaPurchaseID);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma", "Success", User.Identity.Name, $"created proforma {entity.ProformaPurchaseID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { proformaPurchase }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaPurchases_Update([DataSourceRequest]DataSourceRequest request, ProformaPurchase proformaPurchase)
        {
            #region validation

            if (proformaPurchase.FloatDate > proformaPurchase.ClosingDate)
                ModelState.AddModelError("float", "Proforma Float date should be before closing date");
            if (proformaPurchase.ClosingDate >= proformaPurchase.EvaluationDate)
                ModelState.AddModelError("float", "Proforma closing date should be before evaluation date");

            //var exists = _db.ProformaPurchases
            //    .Where(x => x.ProformaPurchaseID != proformaPurchase.ProformaPurchaseID)
            //    .Where(x => x.PurchaseRequisitionID == proformaPurchase.PurchaseRequisitionID).ToList();
            //if (exists.Any())
            //{
            //    ModelState.AddModelError("exist", "Purchase requisision no. " + proformaPurchase.PurchaseRequisitionID + " is already used in other proforma.");
            //}

            #endregion

            var assignment = _db.PurchaseRequisitionAssignments.Find(proformaPurchase.PurchaseRequisitionAssignmentId);
            if (assignment != null)
            {
                var pr = _db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);
                if (pr != null)
                {
                    proformaPurchase.StoreRequisitionID = pr.StoreRequisitionID;
                    proformaPurchase.PurchaseRequisitionID = pr.PurchaseRequisitionID;
                    proformaPurchase.BranchId = pr.BranchId;
                    proformaPurchase.BusinessUnitId = pr.BusinessUnitId;
                    proformaPurchase.ProcurementOfficer = pr.ProcurementOfficer;
                }

                else
                {
                    proformaPurchase.BranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
                    proformaPurchase.BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
                    proformaPurchase.ProcurementOfficer = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
                }
            }

            if (ModelState.IsValid)
            {
               
                var entity = new ProformaPurchase
                {
                    PurchaseRequisitionAssignmentId = proformaPurchase.PurchaseRequisitionAssignmentId,
                    PurchaseRequisitionID = proformaPurchase.PurchaseRequisitionID,
                    StoreRequisitionID = proformaPurchase.StoreRequisitionID,
                    ProcurementOfficer = proformaPurchase.ProcurementOfficer,
                    BranchId = proformaPurchase.BranchId,
                    BusinessUnitId = proformaPurchase.BusinessUnitId,
                    ProformaPurchaseID = proformaPurchase.ProformaPurchaseID,
                    ProformaReference = proformaPurchase.ProformaReference,
                    Description = proformaPurchase.Description,
                    FloatDate = proformaPurchase.FloatDate,
                    ClosingDate = proformaPurchase.ClosingDate,
                    EvaluationDate = proformaPurchase.EvaluationDate,
                    PurchaseCategory = proformaPurchase.PurchaseCategory,
                    PurchaseStatus = proformaPurchase.PurchaseStatus,
                    EvaluateTotalFinance = proformaPurchase.EvaluateTotalFinance,

                    ForwardedFrom = proformaPurchase.ForwardedFrom,
                    ForwardedTo = proformaPurchase.ForwardedTo,
                    IsFinalApproval = proformaPurchase.IsFinalApproval,

                    Status = proformaPurchase.Status,
                    IsOnlineApproved = proformaPurchase.IsOnlineApproved,
                    OnlineApprovedBy = proformaPurchase.OnlineApprovedBy,
                    OnlineApprovedTime = proformaPurchase.OnlineApprovedTime,
                    IsVoid = proformaPurchase.IsVoid,
                    VoidBy = proformaPurchase.VoidBy,
                    VoidTime = proformaPurchase.VoidTime,
                    OrgBranchName = proformaPurchase.OrgBranchName,
                    DateCreated = proformaPurchase.DateCreated,
                    LastModified = proformaPurchase.LastModified,
                    CreatedBy = proformaPurchase.CreatedBy,
                    ModifiedBy = proformaPurchase.ModifiedBy
                };

                _db.ProformaPurchases.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma", "Success", User.Identity.Name, $"updated proforma {entity.ProformaPurchaseID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { proformaPurchase }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaPurchases_Destroy([DataSourceRequest]DataSourceRequest request, ProformaPurchase proformaPurchase)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformaPurchase
                {
                    PurchaseRequisitionAssignmentId = proformaPurchase.PurchaseRequisitionAssignmentId,
                    PurchaseRequisitionID = proformaPurchase.PurchaseRequisitionID,
                    ProcurementOfficer = proformaPurchase.ProcurementOfficer,
                    BranchId = proformaPurchase.BranchId,
                    BusinessUnitId = proformaPurchase.BusinessUnitId,
                    ProformaPurchaseID = proformaPurchase.ProformaPurchaseID,
                    ProformaReference = proformaPurchase.ProformaReference,
                    Description = proformaPurchase.Description,
                    FloatDate = proformaPurchase.FloatDate,
                    ClosingDate = proformaPurchase.ClosingDate,
                    EvaluationDate = proformaPurchase.EvaluationDate,
                    PurchaseCategory = proformaPurchase.PurchaseCategory,
                    Status = proformaPurchase.Status,
                    IsOnlineApproved = proformaPurchase.IsOnlineApproved,
                    OnlineApprovedBy = proformaPurchase.OnlineApprovedBy,
                    OnlineApprovedTime = proformaPurchase.OnlineApprovedTime,
                    IsVoid = proformaPurchase.IsVoid,
                    VoidBy = proformaPurchase.VoidBy,
                    VoidTime = proformaPurchase.VoidTime,
                    OrgBranchName = proformaPurchase.OrgBranchName,
                    DateCreated = proformaPurchase.DateCreated,
                    LastModified = proformaPurchase.LastModified,
                    CreatedBy = proformaPurchase.CreatedBy,
                    ModifiedBy = proformaPurchase.ModifiedBy
                };

                _db.ProformaPurchases.Attach(entity);
                _db.ProformaPurchases.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { proformaPurchase }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        private void PurchaseRequistionItems(int  assignmentid, int id)
        {
            var assignment = _db.PurchaseRequisitionAssignments.Find(assignmentid);
            if (assignment != null)
            {
                foreach (var item in assignment.PurchaseRequisitionReassignmentItems)
                {
                    ProformaItem proformaItem = new ProformaItem
                    {
                        ProformaPurchaseID = id,
                        CatCode = item.CatCode,
                        CategoryName = item.CategoryName,
                        ItmCode = item.ItmCode,
                        ItemName = item.ItemName,
                        PartNo = item.PartNo,
                        UoM = item.UoM,
                        Name=item.Name,
                        ModelNo = item.ModelNo,                        
                        ItemSpecification = item.ItemSpecification,
                        ItemType = item.ItemType,
                        Quantity = item.Quantity
                    };
                    _db.ProformaItems.Add(proformaItem);
                    _db.SaveChanges();
                }

                ProformaPurchaseRequisition purchaseRequisition = new ProformaPurchaseRequisition
                {
                    ProformaPurchaseID = id,
                    PurchaseRequisitionID = assignment.PurchaseRequisitionID,
                    StoreRequisitionID = assignment.PurchaseRequisition.StoreRequisitionID,
                    PurchaseRequisitionAssignmentId=assignmentid
                };
                _db.ProformaPurchaseRequisitions.Add(purchaseRequisition);
            }

            _db.SaveChanges();
        }
        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.ProformaPurchases.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.ProformaItem.Count > 0)
            {
                #region validate price and tax of winning supplier

                var winningSuppliers = _db.WinningSuppliers.Where(x => x.ProformaPurchaseID == record.ProformaPurchaseID).ToList();
                if(!winningSuppliers.Any())
                {
                    var formattedData2 = new
                    {
                        Message = "Please add atleast one winner.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                foreach (var item in winningSuppliers.SelectMany(x=>x.WinningItems))
                {
                    if (item.UPrice <= 0 || item.GLTaxRateID == 0 || String.IsNullOrWhiteSpace(item.WinningSupplier.SupplierName))
                    {
                        var formattedData3 = new
                        {
                            Message = "Please fill supplier,unit price and tax information for all items in proforma winner.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData3, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.ProformaPurchases.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.ProformaItem.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        public JsonResult SendToChecker(int id, string user)
        {
            var record = _db.ProformaPurchases.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.ProformaItem.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 13);
                record.ForwardedTo = user;
                record.ForwardedFrom = User.Identity.Name;
                _db.ProformaPurchases.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.ProformaItem.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        

        [AuthorizeRoles(ProcurementRoles.ProcurementVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.ProformaPurchases.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid)
            {
                #region validate payment

                var purchaseOrder = _db.PurchaseOrders
                    .Where(x => x.IsVoid == false)
                    .Where(x => x.IsOnlineApproved)
                    .FirstOrDefault(x => x.ProformaPurchaseID == id);

                if (purchaseOrder != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This proforma purchase can not be void since it is referred in purchase order {purchaseOrder.PurchaseOrderID}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ProformaPurchases.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }

        public JsonResult GenerateResult(string id)
        {
            int proformaid = int.Parse(id);
            var proformaPurchase = _db.ProformaPurchases.Find(proformaid);
            if (proformaPurchase != null)
            {
                _db.ProformaEvaluationResults.RemoveRange(proformaPurchase.ProformaEvaluationResult);
                _db.SaveChanges();
                var evaluations = _db.ProformaEvaluationCriterias.Where(x=>x.ProformaPurchaseID==proformaid).ToList();

                //var techcriteria = evaluations.Where(x => x.Type == CriteriaType.Technical).ToList();
                //var fincriteria = evaluations.Where(x => x.Type == CriteriaType.Financial).ToList();

                //if (techcriteria.Any() && !fincriteria.Any())
                //{
                //    var formattedData11 = new
                //    {
                //        Message = "Please add financial criteria.",
                //        Status = "NOT_OK"
                //    };
                //    return Json(formattedData11, JsonRequestBehavior.AllowGet);
                //}

                #region validation

                //var allParticipantItems = _db.ParticipatingSupplierItems
                //    .Where(x => x.ParticipatingSupplier.ProformaPurchaseID == proformaid).Where(x => x.IsOutOfStock == false).Where(x => x.TPrice == 0);

                //if (allParticipantItems.Any())
                //{
                //    var formattedData3 = new
                //    {
                //        Message = "Please fill price for all items.",
                //        Status = "NOT_OK"
                //    };
                //    return Json(formattedData3, JsonRequestBehavior.AllowGet);
                //}


                #endregion


                var participants = _db.ParticipatingSuppliers.Where(x => x.ProformaPurchaseID == proformaid).ToList();
                #region for financial computation

                List<int> supplierIds = participants.Select(x => x.ParticipatingSupplierID).ToList();
                var allparticipantsItems = _db.ParticipatingSupplierItems
                    .Where(x => x.IsOutOfStock == false)
                    .Where(x => supplierIds.Contains(x.ParticipatingSupplierID)).ToList();
                    
                #endregion
                foreach (var supplier in participants)
                {
                    var participantsItems = _db.ParticipatingSupplierItems
                        .Where(x=>x.ParticipatingSupplierID==supplier.ParticipatingSupplierID)
                        .Where(x => x.IsOutOfStock == false).ToList();

                    var evaluationResult = new ProformaEvaluationResult
                    {
                        ProformaPurchaseID = proformaPurchase.ProformaPurchaseID,
                        Financial = 0,
                        Technical = 0,
                        Pass = PassFailStatus.NA,
                        Total = 0,
                        SupplierId = supplier.SupplierId,
                        IsNewSupplier = supplier.IsNewSupplier,
                        SupplierName = supplier.SupplierName
                    };
                    _db.ProformaEvaluationResults.Add(evaluationResult);
                    _db.SaveChanges();

                    foreach (var participantsItem in participantsItems)
                    {
                        var evaluationItemResult = new ProformaEvaluationItemResult
                        {
                            ProformaEvaluationResult = evaluationResult,
                            CatCode = participantsItem.CatCode,
                            CategoryName = participantsItem.CategoryName,
                            ItmCode = participantsItem.ItmCode,
                            ItemName = participantsItem.ItemName,
                            ItemSpecification = participantsItem.ItemSpecification,
                            Name = participantsItem.Name,
                            ProformaItemID = participantsItem.ProformaItemID,
                        };

                        #region pass fail

                        bool isPassFail = false;
                        var passfailCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Pass_Fail);
                        if (passfailCriteria != null)
                        {
                            isPassFail = true;
                            if (participantsItem.ItemType == PurchaseItemType.FixedAsset)
                            {
                                var proformPassFail = _db.ProformPassFails
                                    .Where(x => x.ProformaPurchaseID == proformaid)
                                    .Where(x => x.ProformaPurchaseID == proformaid)
                                    .Where(x => x.ItmCode == participantsItem.ItmCode)
                                    .Where(x => x.Name == participantsItem.Name)
                                    .Where(x => x.ModelNo == participantsItem.ModelNo)
                                    .FirstOrDefault(x => x.SupplierName == supplier.SupplierName);

                                if (proformPassFail != null) evaluationItemResult.Pass = proformPassFail.Result;

                            }
                            else
                            {
                                var proformPassFail = _db.ProformPassFails
                                    .Where(x => x.ProformaPurchaseID == proformaid)
                                    .Where(x => x.ProformaPurchaseID == proformaid)
                                    .Where(x => x.ItmCode == participantsItem.ItmCode)
                                    .Where(x => x.ItemSpecification == participantsItem.ItemSpecification)//added
                                    .FirstOrDefault(x => x.SupplierName == supplier.SupplierName);

                                if (proformPassFail != null) evaluationItemResult.Pass = proformPassFail.Result;

                            }

                        }

                        #endregion

                        #region technical

                        var technicalCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Technical);
                        if (technicalCriteria != null)
                        {
                            if (participantsItem.ItemType == PurchaseItemType.FixedAsset)
                            {
                                var proformaTechnical = _db.ProformTechnicals
                                    .Where(x => x.ProformaPurchaseID == proformaid)
                                    .Where(x => x.ItmCode == participantsItem.ItmCode)
                                    .Where(x => x.Name == participantsItem.Name)
                                    .Where(x => x.ModelNo == participantsItem.ModelNo)
                                    .FirstOrDefault(x => x.SupplierName == supplier.SupplierName);

                                if (proformaTechnical != null) evaluationItemResult.Technical = proformaTechnical.Result;
                            }
                            else
                            {
                                var proformaTechnical = _db.ProformTechnicals
                                    .Where(x => x.ProformaPurchaseID == proformaid)
                                    .Where(x => x.ItmCode == participantsItem.ItmCode)
                                    .Where(x => x.ItemSpecification == participantsItem.ItemSpecification)//added
                                    .FirstOrDefault(x => x.SupplierName == supplier.SupplierName);

                                if (proformaTechnical != null) evaluationItemResult.Technical = proformaTechnical.Result;
                            }
                        }

                        #endregion

                        if ((passfailCriteria == null) && (technicalCriteria == null))
                            isPassFail = true;

                        #region financial

                        var financialCriteria = evaluations.FirstOrDefault(x => x.Type == CriteriaType.Financial);
                        int criteriaid = 0;
                        if (financialCriteria != null)
                            criteriaid = financialCriteria.ProformaEvaluationCriteriaID;

                        evaluationItemResult.Financial =
                            ComputeFinancial(participantsItem, criteriaid, allparticipantsItems, isPassFail);

                        #endregion


                        evaluationItemResult.Total = evaluationItemResult.Financial + evaluationItemResult.Technical;
                        _db.ProformaEvaluationItemResults.Add(evaluationItemResult);
                        _db.SaveChanges();
                    }
                }

                GenerateSummaryResult(proformaid);

                var formattedData = new
                {
                    Message = "Complete",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var formattedData1 = new
            {
                Message = "The record is not found.",
                Status = "NOT_OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateWinner(string id)
        {
            int proformaid = int.Parse(id);
            var proformaPurchase = _db.ProformaPurchases.Find(proformaid);
            if (proformaPurchase != null)
            {
                if (proformaPurchase.EvaluateTotalFinance)
                {
                    #region evaluate by total financial result
                    var awards = _db.WinningSuppliers.Where(x => x.ProformaPurchaseID == proformaid).ToList();
                    var winnerCanProvide = awards.Where(x => x.AwardGiven).ToList();
                    var winnerCantProvide =
                        awards.Where(x => x.AwardGiven == false).Select(x => x.SupplierName).ToList();
                    _db.WinningSuppliers.RemoveRange(winnerCanProvide);
                    _db.SaveChanges();

                    var proformaEvaluationResults = _db.ProformaEvaluationResults
                        .Where(x => x.Pass != PassFailStatus.Fail)
                        .Where(x => x.ProformaPurchaseID == proformaid).ToList();
                    if (winnerCantProvide.Any())
                        proformaEvaluationResults = proformaEvaluationResults
                            .Where(x => winnerCantProvide.Contains(x.SupplierName) == false).ToList();

                    var topResult = proformaEvaluationResults.OrderByDescending(x => x.Total).FirstOrDefault();

                    if (topResult != null)
                    {
                        var winner = _db.WinningSuppliers.Where(x => x.ProformaPurchaseID == proformaid).FirstOrDefault(x => x.SupplierName == topResult.SupplierName);
                        var resultItems = _db.ProformaEvaluationItemResults.Where(x =>
                            x.ProformaEvaluationResultID == topResult.ProformaEvaluationResultID).ToList();
                        if (winner == null)
                        {
                            #region header

                            var winningSupplier = new WinningSupplier
                            {
                                ProformaPurchaseID = proformaPurchase.ProformaPurchaseID,
                                AwardDate = DateTime.Today,
                                DeliveryDate = DateTime.Today,
                                SupplierId = topResult.SupplierId,
                                IsNewSupplier = topResult.IsNewSupplier,
                                SupplierName = topResult.SupplierName,
                                AwardGiven = true
                            };
                            _db.WinningSuppliers.Add(winningSupplier);
                            _db.SaveChanges();

                            #endregion

                            foreach (var item in resultItems)
                            {
                                #region line


                                var participantitem = _db.ParticipatingSupplierItems.Where(x => x.ParticipatingSupplier.ProformaPurchaseID == proformaid)
                                    .Where(x => x.ProformaItemID == item.ProformaItemID)
                                    .FirstOrDefault(x => x.ParticipatingSupplier.SupplierName == topResult.SupplierName);
                                if (participantitem != null)
                                {
                                    var supplierItem = new WinningSupplierItem
                                    {
                                        WinningSupplier = winningSupplier,
                                        ProformaItemID = item.ProformaItemID,
                                        ItmCode = item.ItmCode,
                                        ItemName = item.ItemSpecification,
                                        Name = item.Name,
                                        ModelNo = item.ModelNo,
                                        CatCode = item.CatCode,
                                        CategoryName = item.CategoryName,
                                        ItemType = participantitem.ItemType,
                                        PartNo = participantitem.PartNo,
                                        ItemSpecification = participantitem.ItemSpecification,
                                        UoM = participantitem.UoM,
                                        Quanity = participantitem.Quantity,
                                        UPrice = participantitem.UPrice,
                                        GLTaxRateID = participantitem.GLTaxRateID,
                                        TaxAmount = participantitem.TaxAmount,
                                        TPrice = participantitem.TPrice
                                    };
                                    _db.WinningSupplierItems.Add(supplierItem);
                                }

                                _db.SaveChanges();

                                #endregion
                            }

                        }
                        else
                        {
                            foreach (var item in resultItems)
                            {
                                #region line
                                var participantitem = _db.ParticipatingSupplierItems.Where(x => x.ParticipatingSupplier.ProformaPurchaseID == proformaid)
                                    .Where(x => x.ProformaItemID == item.ProformaItemID)
                                    .FirstOrDefault(x => x.ParticipatingSupplier.SupplierName == topResult.SupplierName);
                                if (participantitem != null)
                                {
                                    var supplierItem = new WinningSupplierItem
                                    {
                                        WinningSupplier = winner,
                                        ProformaItemID = item.ProformaItemID,
                                        ItmCode = item.ItmCode,
                                        ItemName = item.ItemSpecification,
                                        Name = item.Name,
                                        ModelNo = item.ModelNo,
                                        CatCode = item.CatCode,
                                        CategoryName = item.CategoryName,
                                        ItemType = participantitem.ItemType,
                                        PartNo = participantitem.PartNo,
                                        ItemSpecification = participantitem.ItemSpecification,
                                        UoM = participantitem.UoM,
                                        Quanity = participantitem.Quantity,
                                        UPrice = participantitem.UPrice,
                                        GLTaxRateID = participantitem.GLTaxRateID,
                                        TaxAmount = participantitem.TaxAmount,
                                        TPrice = participantitem.TPrice
                                    };
                                    _db.WinningSupplierItems.Add(supplierItem);
                                }

                                _db.SaveChanges();

                                #endregion
                            }
                        }
                    }



                    #endregion
                }
                else
                {
                    #region evaluate by individual item financial result

                    var winners = _db.WinningSuppliers.Where(x => x.ProformaPurchaseID == proformaid).ToList();
                    var winnerCanProvide = winners.Where(x => x.AwardGiven).ToList();

                    var winnerCantProvide =
                        winners.Where(x => x.AwardGiven == false).Select(x => x.SupplierName).ToList();
                    _db.WinningSuppliers.RemoveRange(winnerCanProvide);
                    _db.SaveChanges();

                    var proformaitems = _db.ProformaItems.Where(x => x.ProformaPurchaseID == proformaid).ToList();

                    foreach (var proformaItem in proformaitems)
                    {
                        var proformaResults = _db.ProformaEvaluationItemResults.Where(x => x.Pass != PassFailStatus.Fail)
                            .Where(x => x.ProformaEvaluationResult.ProformaPurchaseID == proformaid).ToList();
                        if (winnerCantProvide.Any())
                            proformaResults = proformaResults
                                .Where(x => winnerCantProvide.Contains(x.ProformaEvaluationResult.SupplierName) == false).ToList();

                        var topResult = proformaResults.Where(x => x.Pass != PassFailStatus.Fail)
                            .Where(x => x.ProformaEvaluationResult.ProformaPurchaseID == proformaid)
                            .Where(x => x.ProformaItemID == proformaItem.ProformaItemID).OrderByDescending(x => x.Total)
                            .FirstOrDefault();

                        if (topResult != null)
                        {
                            ParticipatingSupplierItem participantitem;
                            if (proformaItem.ItemType == PurchaseItemType.FixedAsset)
                            {
                                participantitem = _db.ParticipatingSupplierItems
                                    .Where(x => x.ParticipatingSupplier.ProformaPurchaseID == proformaid)
                                    .Where(x => x.ItmCode == proformaItem.ItmCode)
                                    .Where(x => x.Name == proformaItem.Name)
                                    .Where(x => x.ModelNo == proformaItem.ModelNo)
                                    .FirstOrDefault(x =>
                                        x.ParticipatingSupplier.SupplierName == topResult.ProformaEvaluationResult.SupplierName);
                            }
                            else
                            {
                                participantitem = _db.ParticipatingSupplierItems
                                    .Where(x => x.ParticipatingSupplier.ProformaPurchaseID == proformaid)
                                    .Where(x => x.ItmCode == proformaItem.ItmCode)
                                     .Where(x => x.ItemSpecification == proformaItem.ItemSpecification)//added
                                    .FirstOrDefault(x =>
                                        x.ParticipatingSupplier.SupplierName == topResult.ProformaEvaluationResult.SupplierName);
                            }

                            if (participantitem != null)
                            {
                                var winner = _db.WinningSuppliers.Where(x => x.ProformaPurchaseID == proformaid)
                                    .FirstOrDefault(x => x.SupplierName == topResult.ProformaEvaluationResult.SupplierName);
                                if (winner == null)
                                {
                                    #region header

                                    var winningSupplier = new WinningSupplier
                                    {
                                        ProformaPurchaseID = proformaPurchase.ProformaPurchaseID,
                                        AwardDate = DateTime.Today,
                                        DeliveryDate = DateTime.Today,
                                        SupplierId = topResult.ProformaEvaluationResult.SupplierId,
                                        IsNewSupplier = topResult.ProformaEvaluationResult.IsNewSupplier,
                                        SupplierName = topResult.ProformaEvaluationResult.SupplierName,
                                        AwardGiven = true
                                    };
                                    _db.WinningSuppliers.Add(winningSupplier);
                                    _db.SaveChanges();

                                    #endregion

                                    #region line

                                    var winningitem = new WinningSupplierItem
                                    {
                                        WinningSupplier = winningSupplier,
                                        ProformaItemID = proformaItem.ProformaItemID,
                                        ItmCode = proformaItem.ItmCode,
                                        ItemName = proformaItem.ItemName,
                                        Name = proformaItem.Name,
                                        ModelNo = proformaItem.ModelNo,
                                        CatCode = proformaItem.CatCode,
                                        CategoryName = proformaItem.CategoryName,
                                        ItemType = proformaItem.ItemType,
                                        PartNo = proformaItem.PartNo,
                                        IsApproved = proformaItem.IsApproved,
                                        ItemSpecification = proformaItem.ItemSpecification,
                                        UoM = proformaItem.UoM,
                                        Quanity = proformaItem.Quantity,
                                        UPrice = participantitem.UPrice,
                                        GLTaxRateID = participantitem.GLTaxRateID,
                                        TaxAmount = participantitem.TaxAmount,
                                        TPrice = participantitem.TPrice
                                    };
                                    _db.WinningSupplierItems.Add(winningitem);
                                    _db.SaveChanges();

                                    #endregion
                                }
                                else
                                {
                                    #region line

                                    var winningitem = new WinningSupplierItem
                                    {
                                        WinningSupplier = winner,
                                        ProformaItemID = proformaItem.ProformaItemID,
                                        ItmCode = proformaItem.ItmCode,
                                        ItemName = proformaItem.ItemName,
                                        Name = proformaItem.Name,
                                        ModelNo = proformaItem.ModelNo,
                                        CatCode = proformaItem.CatCode,
                                        CategoryName = proformaItem.CategoryName,
                                        ItemType = proformaItem.ItemType,
                                        PartNo = proformaItem.PartNo,
                                        IsApproved = proformaItem.IsApproved,
                                        ItemSpecification = proformaItem.ItemSpecification,
                                        UoM = proformaItem.UoM,
                                        Quanity = proformaItem.Quantity,
                                        UPrice = participantitem.UPrice,
                                        GLTaxRateID = participantitem.GLTaxRateID,
                                        TaxAmount = participantitem.TaxAmount,
                                        TPrice = participantitem.TPrice
                                    };
                                    _db.WinningSupplierItems.Add(winningitem);
                                    _db.SaveChanges();

                                    #endregion
                                }
                            }

                        }
                    }

                    #endregion
                }
                var formattedData = new
                {
                    Message = "Complete",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var formattedData1 = new
            {
                Message = "The record is not found.",
                Status = "NOT_OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }

        private float ComputeFinancial(ParticipatingSupplierItem participatingItem, int criteriaId, List<ParticipatingSupplierItem> participantsItems, bool passfail)
        {
            int supId = participatingItem.ParticipatingSupplierID;
            string itemcode = participatingItem.ItmCode;
            string spec = participatingItem.ItemSpecification;
            string name = participatingItem.Name;
            string model = participatingItem.ModelNo;
            float result = 0;
            float lowestPrice = 0;
            ParticipatingSupplierItem participantsItem;
            if (participatingItem.ItemType == PurchaseItemType.FixedAsset)
            {
                participantsItem = participantsItems
                    .Where(x => x.ItmCode == itemcode)
                    .Where(x => x.Name == name)
                    .Where(x => x.ModelNo == model)
                    .FirstOrDefault(x => x.ParticipatingSupplierID == supId);
                lowestPrice = (float)participantsItems.Where(x => x.ItmCode == itemcode).Where(x => x.Name == name).Where(x => x.ModelNo == model).Min(x => x.TPrice);
            }
            else
            {
                participantsItem = participantsItems
                    .Where(x => x.ItmCode == itemcode)
                     .Where(x => x.ItemSpecification == spec)//added
                    .FirstOrDefault(x => x.ParticipatingSupplierID == supId);

                lowestPrice = (float)participantsItems.Where(x => x.ItmCode == itemcode).Min(x => x.TPrice);
            }

            if (participantsItem != null)
            {
                float price = (float)participantsItem.TPrice;
                if (passfail)
                {
                    result = lowestPrice * 100 / price;
                }
                else
                {
                    var criteria = _db.ProformaEvaluationCriterias.Find(criteriaId);
                    if (criteria != null) result = lowestPrice * criteria.Weight / price;
                }
            }
            return result;
        }

        private void GenerateSummaryResult(int proformaid)
        {
           
            var proformaEvaluationResults = _db.ProformaEvaluationResults.Where(x => x.ProformaPurchaseID == proformaid).ToList();

            decimal minFinancial = 0;

            foreach (var result in proformaEvaluationResults)
            {
                decimal sum = _db.ParticipatingSupplierItems
                    .Where(x=>x.IsOutOfStock==false)
                    .Where(x => x.ParticipatingSupplier.ProformaPurchaseID == proformaid)
                    .Where(x => x.ParticipatingSupplier.SupplierName == result.SupplierName).Sum(x => x.TPrice);
                if ((minFinancial > 0 && sum < minFinancial) || minFinancial == 0)
                    minFinancial = sum;
            }

            #region pass fail and financial

            bool isPassFail = false;
            var passfailCriteria = _db.ProformaEvaluationCriterias.Where(x => x.ProformaPurchaseID == proformaid).FirstOrDefault(x => x.Type == CriteriaType.Pass_Fail);
            if (passfailCriteria != null)
                isPassFail = true;

            var financialCriteria = _db.ProformaEvaluationCriterias.Where(x => x.ProformaPurchaseID == proformaid).FirstOrDefault(x => x.Type == CriteriaType.Financial);


            #endregion

            foreach (var result in proformaEvaluationResults)
            {
                var itemResult = _db.ProformaEvaluationItemResults
                    .Where(x => x.ProformaEvaluationResultID == result.ProformaEvaluationResultID).ToList();

                result.Technical = itemResult.Average(x => x.Technical);

                decimal totalprice = _db.ParticipatingSupplierItems.Where(x => x.ParticipatingSupplier.ProformaPurchaseID == proformaid)
                    .Where(x => x.ParticipatingSupplier.SupplierName == result.SupplierName).Sum(x => x.TPrice);

                if (isPassFail)
                {
                    result.Financial = (float)(minFinancial * 100 / totalprice);
                }
                else
                {
                    if (financialCriteria != null)
                        result.Financial = (float)(minFinancial * (decimal)financialCriteria.Weight / totalprice);
                }

                var passfail = itemResult.FirstOrDefault(x => x.Pass == PassFailStatus.Fail);

                if (passfail != null)
                    result.Pass = passfail.Pass;

                var passfailNa = itemResult.FirstOrDefault(x => x.Pass == PassFailStatus.NA);
                if (passfailNa != null)
                    result.Pass = passfailNa.Pass;
                if (passfail == null && passfailNa == null)
                    result.Pass = PassFailStatus.Pass;

                result.Total = result.Financial + result.Technical;

                _db.ProformaEvaluationResults.Attach(result);
                _db.Entry(result).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }
        //selectors
        public JsonResult GetProformaItemsById(string id)
        {
            int proformaid = int.Parse(id);
            var proformaitems = _db.ProformaItems.Where(x => x.ProformaPurchaseID == proformaid);

            return Json(proformaitems.Select(i => new
            {
                Name = /*i.CatCode + "_" + i.CategoryName + "_" +*/ i.ItmCode + "_" + i.ItemName + "_" + i.Name,
                Code = i.ProformaItemID
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProformaParticipantItemsById(string id)
        {
            int proformaid = int.Parse(id);
            var participatingSuppliers = _db.ParticipatingSupplierItems
                .Where(x => x.IsOutOfStock == false)
                .Where(x => x.ParticipatingSupplier.ProformaPurchaseID == proformaid);

            return Json(participatingSuppliers.Select(i => new
            {
                Name = /*i.CatCode + "_" + i.CategoryName + "_" +*/ i.ItmCode + "_" + i.ItemName + "_" + i.Name,
                Code = i.ProformaItemID
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProformaResultItemsById(string proforma, string award)
        {
            int lotid = int.Parse(proforma);
            int awardid = int.Parse(award);
            var awardObj = _db.WinningSuppliers.Find(awardid);
            var resultItems = _db.ProformaEvaluationResults.Where(x => x.ProformaPurchaseID == lotid)
                .Where(x => x.SupplierName == awardObj.SupplierName);

            return Json(resultItems.Select(i => new
            {
                Name = /*i.CatCode + "_" + i.CategoryName + "_" +*/ i.ItmCode + "_" + i.ItemName + "_" + i.Name,
                Code = i.ProformaItemID
            }), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}

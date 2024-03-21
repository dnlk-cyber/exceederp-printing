using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Proforma
{
    
    public class ProformaValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly DashboardHelper _dashboardHelper = new DashboardHelper();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult PurchaseValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PurchaseValidation> purchasevalidations = _db.PurchaseValidations.Where(x => x.ProformaPurchaseID == id);
            DataSourceResult result = purchasevalidations.ToDataSourceResult(request, purchaseValidation => new
            {
                PurchaseValidationID = purchaseValidation.PurchaseValidationID,
                ProformaPurchaseID = purchaseValidation.ProformaPurchaseID,
                Status = purchaseValidation.Status,
                Remark = purchaseValidation.Remark,
                OrgBranchName = purchaseValidation.OrgBranchName,
                DateCreated = purchaseValidation.DateCreated,
                LastModified = purchaseValidation.LastModified,
                CreatedBy = purchaseValidation.CreatedBy,
                ModifiedBy = purchaseValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseValidations_Create([DataSourceRequest]DataSourceRequest request, PurchaseValidation purchaseValidation, int id)
        {
            if (purchaseValidation.Status == ApprovalStatuses.Approve)
            {
                #region validation

                var rule = _db.GlobalRuleValues.FirstOrDefault(x => x.Key == KeyType.ProformaMinimumNumberOfSupplier);
                var totalSuppliers = _db.ParticipatingSuppliers.Where(x => x.ProformaPurchaseID == id).AsEnumerable()
                    .DistinctByField(x => x.SupplierName).Count();//supplier
                if (rule != null)
                {
                    try
                    {
                        var amount = decimal.Parse(rule.Value);
                        if (totalSuppliers < amount)
                        {
                            ModelState.AddModelError("count",
                                "Number of suppliers should be at least "+amount+". there are only " + totalSuppliers +
                                " suppliers");
                        }
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("rule","Minimum number of proforma supplier setting is not configured.");
                    }
                }

                var profrmaAmountRule = _db.GlobalRuleValues.FirstOrDefault(x => x.Key == KeyType.ProformaPurchaseMaxAmount);
                if (profrmaAmountRule != null)
                {
                    try
                    {
                        var amount = decimal.Parse(profrmaAmountRule.Value);
                        var amountOfBirr = _db.WinningSuppliers.Where(x => x.ProformaPurchaseID == id).SelectMany(x=>x.WinningItems).Sum(x => x.TPrice);
                        if (amountOfBirr >= amount)
                        {
                            ModelState.AddModelError("count", $"The proforma total amount is {amountOfBirr} which is above allowed purchase by proforma " + amount + ".");
                        }
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("rule", "Maximum amount of proforma purchase is not configured.");
                    }
                }

                #endregion
            }

            ProformaPurchase record = _db.ProformaPurchases.Find(id);

            if (ModelState.IsValid)
            {
                
                if (record != null)
                {

                    if (purchaseValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.IsVoid = false;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                        TransferItemsToPo(id);

                        #region purchase status
                        #region get follow up and update status

                        var followup = _db.PurchaseFollowUps.FirstOrDefault(x => x.PurchaseRequisitionID == record.PurchaseRequisitionID);
                        #endregion

                        if (followup != null)
                        {
                            #region system status

                            var followupsystem = new PurchaseFollowUpSystemStatus()
                            {
                                PurchaseFollowUp = followup,
                                Date = DateTime.Now,
                                Description = "Proforma  no : " + record.PurchaseRequisitionID + " for Purchase requisition no : " + record.PurchaseRequisitionID + " approved",
                                User = User.Identity.Name
                            };

                            _db.PurchaseFollowUpSystemStatus.Add(followupsystem);
                            _db.SaveChanges();

                            #endregion
                        }

                        _dashboardHelper.AddNotofication(Request.UserHostAddress, "", "", DateTime.Now, "", "Proforma  no : " + record.PurchaseRequisitionID + " for Purchase requisition no : " + record.PurchaseRequisitionID + " approved by user : " + User.Identity.Name + " at " + DateTime.Now);
                        #endregion
                    }
                    if (purchaseValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
                var entity = new PurchaseValidation
                {
                    ProformaPurchaseID = id,
                    Status = purchaseValidation.Status,
                    Remark = purchaseValidation.Remark,
                    OrgBranchName = purchaseValidation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = purchaseValidation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = purchaseValidation.ModifiedBy
                };

                _db.PurchaseValidations.Add(entity);
                _db.SaveChanges();
                purchaseValidation.PurchaseValidationID = entity.PurchaseValidationID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma approval", "Success", User.Identity.Name, $"approved plan {entity.ProformaPurchaseID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseValidations_Update([DataSourceRequest]DataSourceRequest request, PurchaseValidation purchaseValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseValidation
                {
                    PurchaseValidationID = purchaseValidation.PurchaseValidationID,
                    Status = purchaseValidation.Status,
                    Remark = purchaseValidation.Remark,
                    OrgBranchName = purchaseValidation.OrgBranchName,
                    DateCreated = purchaseValidation.DateCreated,
                    LastModified = purchaseValidation.LastModified,
                    CreatedBy = purchaseValidation.CreatedBy,
                    ModifiedBy = purchaseValidation.ModifiedBy
                };

                _db.PurchaseValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { purchaseValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseValidations_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseValidation purchaseValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseValidation
                {
                    PurchaseValidationID = purchaseValidation.PurchaseValidationID,
                    Status = purchaseValidation.Status,
                    Remark = purchaseValidation.Remark,
                    OrgBranchName = purchaseValidation.OrgBranchName,
                    DateCreated = purchaseValidation.DateCreated,
                    LastModified = purchaseValidation.LastModified,
                    CreatedBy = purchaseValidation.CreatedBy,
                    ModifiedBy = purchaseValidation.ModifiedBy
                };

                _db.PurchaseValidations.Attach(entity);
                _db.PurchaseValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { purchaseValidation }.ToDataSourceResult(request, ModelState));
        }

        private void TransferItemsToPo(int proformaid)
        {
           
            var proforma = _db.ProformaPurchases.Find(proformaid);
            var winners = _db.WinningSuppliers.Where(x => x.ProformaPurchaseID == proformaid).ToList();
            var pr = _db.ProformaPurchaseRequisitions.Where(x => x.ProformaPurchaseID == proformaid).ToList();
            string sr = "";
            foreach (var p in pr)
            {
                sr = sr + "," + p.StoreRequisitionID.ToString();
            }
            
            foreach (var uw in winners)
            {
               
                if (proforma!=null)
                {
                    #region send
                    #region create po
                    var purchaseOrder = new PurchaseOrder
                    {
                        
                        PurchaseRequisitionID = proforma.PurchaseRequisitionID,
                        ProcurementOfficer=proforma.ProcurementOfficer,
                        BranchId =proforma.BranchId, //EmployeeHelper.GetEmployeePlacementBranch(proforma.ProcurementOfficer),
                        BusinessUnitId=proforma.BusinessUnitId,
                        SroreRequisition = sr,
                        SupplierName = uw.SupplierName,
                        IsNewSupplier = uw.IsNewSupplier,
                        SupplierId = uw.SupplierId,
                        POType = POType.CashBase,
                        ProcurementMethod = ProcurementMethod.Proforma,
                        ProformaPurchaseID = proformaid,
                        OrderDate = DateTime.Today,
                        DeliveryPlace = "",
                        DeliveryDate = DateTime.Today,
                        OrgStructureId = 0,

                        ForwardedFrom = UserInformationHelper.GetEmployeeUserId(proforma.ProcurementOfficer),
                        ForwardedTo = UserInformationHelper.GetEmployeeUserId(proforma.ProcurementOfficer),


                        Status = "Editing",
                        Remark = proforma.Remark,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };
                    _db.PurchaseOrders.Add(purchaseOrder);
                    _db.SaveChanges();


                    #endregion

                    #region items

                    var winnerItems = _db.WinningSupplierItems.Where(x => x.WinningSupplierID == uw.WinningSupplierID).ToList();
                    foreach (var item in winnerItems)
                    {
                        var tax = _db.GLTaxRates.Find(item.GLTaxRateID);
                        if (item.Quanity > 0)
                        {
                            #region item

                            if (tax != null)
                            {
                                var proformaItem = new PurchaseOrderItem
                                {
                                    PurchaseOrder = purchaseOrder,
                                    CatCode = item.CatCode,
                                    CategoryName = item.CategoryName,
                                    ItmCode = item.ItmCode,
                                    ItemName =!string.IsNullOrEmpty(item.ItemName) ?  item.ItemName : "NA",
                                    Name = item.Name,
                                    ModelNo = item.ModelNo,
                                    PartNo = item.PartNo,
                                    Quantity = item.Quanity,
                                    UnitPrice = item.UPrice,
                                    GLTaxRateID = item.GLTaxRateID,
                                    TaxAmount = (tax.CalculatedRate * item.Quanity * item.UPrice),
                                    SubTotal = item.UPrice* item.Quanity,
                                    Total = item.TPrice,
                                    IsApproved = item.IsApproved,
                                    ItemSpecification = item.ItemSpecification,
                                    UoM = item.UoM,
                                    ItemType = item.ItemType,

                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Today
                                };
                                _db.PurchaseOrderItems.Add(proformaItem);
                            }

                            _db.SaveChanges();
                            #endregion
                        }

                    }

                    #endregion

                    #region prs

                    var purchaseRequisitions = _db.ProformaPurchaseRequisitions.Where(x => x.ProformaPurchaseID == uw.ProformaPurchaseID).ToList();
                    foreach (var purchaseRequisition in purchaseRequisitions)
                    {

                        var poPurchaseRequisition = new PoPurchaseRequisition
                        {
                            PurchaseOrder = purchaseOrder,
                            PurchaseRequisitionAssignmentId = purchaseRequisition.PurchaseRequisitionAssignmentId,
                            PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                            StoreRequisitionID=purchaseRequisition.StoreRequisitionID,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now
                        };
                        _db.PoPurchaseRequisitions.Add(poPurchaseRequisition);
                        _db.SaveChanges();

                    }

                    #endregion

                    #endregion
                }

            }
           

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}

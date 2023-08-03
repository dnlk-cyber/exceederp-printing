using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Contract;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{
    
    public class TenderValidationController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly DashboardHelper _dashboardHelper = new DashboardHelper();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult TenderValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<TenderValidation> tendervalidations = _db.TenderValidations.Where(x => x.TenderID == id);
            DataSourceResult result = tendervalidations.ToDataSourceResult(request, tenderValidation => new
            {
                TenderValidationID = tenderValidation.TenderValidationID,
                Status = tenderValidation.Status,
                Remark = tenderValidation.Remark,
                OrgBranchName = tenderValidation.OrgBranchName,
                DateCreated = tenderValidation.DateCreated,
                LastModified = tenderValidation.LastModified,
                CreatedBy = tenderValidation.CreatedBy,
                ModifiedBy = tenderValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderValidations_Create([DataSourceRequest]DataSourceRequest request, TenderValidation tenderValidation, int id)
        {
            var record = _db.Tenders.Find(id);
            if (tenderValidation.Status == ApprovalStatuses.Approve)
            {

                if (record != null && record.IsOnlineApproved)
                {
                    ModelState.AddModelError("approved", "this record is already approved");
                }

                if (record != null)
                {
                    foreach (var lot in record.Lots)
                    {
                        var rule = _db.GlobalRuleValues.FirstOrDefault(x => x.Key == KeyType.TenderMinimumNumberOfSupplier);
                        var totalSuppliers = _db.BidPurchasingSupplierses.Where(x => x.LotID == lot.LotID).AsEnumerable()
                            .DistinctByField(x => x.SupplierName).Count();//supplier

                        if (rule != null)
                        {
                            try
                            {
                                var amount = decimal.Parse(rule.Value);
                                if (totalSuppliers < amount)
                                {
                                    ModelState.AddModelError("count",
                                        $"Number of suppliers should be at least { amount }  there are only { totalSuppliers} suppliers in lot {lot.LotNumber}");
                                }
                            }
                            catch (Exception e)
                            {
                                ModelState.AddModelError("rule", "Minimum number of tender supplier setting is not configured.");
                            }
                        }
                       
                       
                    }
                }
            }

            if (ModelState.IsValid)
            {

                if (tenderValidation.Status ==ApprovalStatuses.Approve)
                {
                    if (record != null)
                    {
                        record.Status = "Approved";
                        record.IsVoid = false;
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                        
                        #region purchase status
                        foreach (var lot in record.Lots)
                        {
                            #region get follow up and update status
                            var followup = _db.PurchaseFollowUps.FirstOrDefault(x => x.PurchaseRequisitionID == lot.LotNumber);
                            #endregion

                            if (followup != null)
                            {
                                #region system status

                                var followupsystem = new PurchaseFollowUpSystemStatus()
                                {
                                    PurchaseFollowUp = followup,
                                    Date = DateTime.Now,
                                    Description = "Tender reference no : " + lot.Tender.TenderReference + " for lot no : " + lot.LotNumber + " approved",
                                    User = User.Identity.Name
                                };

                                _db.PurchaseFollowUpSystemStatus.Add(followupsystem);
                                _db.SaveChanges();

                                #endregion
                            }

                           _dashboardHelper.AddNotofication(Request.UserHostAddress, "", "", DateTime.Now, "",
                                "Tender reference no : " + lot.Tender.TenderReference + " for Purchase requisition no : " +
                                lot.LotNumber + " approved by user : " + User.Identity.Name + " at " +
                                DateTime.Now);
                        }
                        #endregion
                    }
                }
                else if (tenderValidation.Status == ApprovalStatuses.Reject)
                {
                    if (record != null)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }


                }

                var entity = new TenderValidation
                {

                    Status = tenderValidation.Status,
                    Remark = tenderValidation.Remark,
                    TenderID = id,
                    OrgBranchName = tenderValidation.OrgBranchName,

                    LastModified = tenderValidation.LastModified,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now.Date,
                    ModifiedBy = tenderValidation.ModifiedBy
                };
                tenderValidation.TenderValidationID = entity.TenderValidationID;

                _db.TenderValidations.Add(entity);
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender approval", "Success", User.Identity.Name, $"approved tender {entity.TenderID} ", Modules.Procurement);
                #endregion

                //#region Supplier Import

                //Tender tender = db.Tenders.Find(id);
                //foreach (Lot tenderLot in tender.Lots)
                //{
                //    List<BidAwardedSupplier> awardedSuppliers =
                //        tenderLot.BidAwardedSupplier.GroupBy(x => x.SupplierName).Select(x => x.FirstOrDefault()).ToList();
                //    foreach (BidAwardedSupplier bidAwardedSupplier in awardedSuppliers)
                //    {
                //        var bidPurchasing =db.BidPurchasingSupplierses.FirstOrDefault(x => x.SupplierName==bidAwardedSupplier.SupplierName);

                //        ProcurementSupplier supplier = new ProcurementSupplier
                //        {
                //            Name = bidPurchasing.SupplierName,
                //            SupplierReference = bidPurchasing.BidPurchasingSuppliersID.ToString(),
                //            CreatedBy = User.Identity.Name,
                //            DateCreated = DateTime.Today,
                //            ModifiedBy = User.Identity.Name,
                //            LastModified = DateTime.Today
                //        };
                //        db.ProcurementSuppliers.Add(supplier);
                //        db.SaveChanges();


                //    }
                //}

                //#endregion
            }


            return Json(new[] { tenderValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderValidations_Update([DataSourceRequest]DataSourceRequest request, TenderValidation tenderValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderValidation
                {
                    TenderValidationID = tenderValidation.TenderValidationID,
                    Status = tenderValidation.Status,
                    Remark = tenderValidation.Remark,
                    OrgBranchName = tenderValidation.OrgBranchName,
                    DateCreated = tenderValidation.DateCreated,
                    LastModified = tenderValidation.LastModified,
                    CreatedBy = tenderValidation.CreatedBy,
                    ModifiedBy = tenderValidation.ModifiedBy
                };

                _db.TenderValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { tenderValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderValidations_Destroy([DataSourceRequest]DataSourceRequest request, TenderValidation tenderValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderValidation
                {
                    TenderValidationID = tenderValidation.TenderValidationID,
                    Status = tenderValidation.Status,
                    Remark = tenderValidation.Remark,
                    OrgBranchName = tenderValidation.OrgBranchName,
                    DateCreated = tenderValidation.DateCreated,
                    LastModified = tenderValidation.LastModified,
                    CreatedBy = tenderValidation.CreatedBy,
                    ModifiedBy = tenderValidation.ModifiedBy
                };

                _db.TenderValidations.Attach(entity);
                _db.TenderValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { tenderValidation }.ToDataSourceResult(request, ModelState));
        }

       

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}

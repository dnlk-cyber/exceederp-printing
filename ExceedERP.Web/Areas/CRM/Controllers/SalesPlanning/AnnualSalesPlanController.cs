using System;
//using System.Data
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.CRM;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;

namespace ExceedERP.Web.Areas.CRM.Controllers.SalesPlanning
{
    [AuthorizeRoles(CRMRoles.SalesPlanApprover, CRMRoles.SalesPlanUser)]
    public class AnnualSalesPlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Years"] = db.GLFiscalYears.Select(y => new { y.GlFiscalYearId, y.Name }).ToList();
            ViewData["Periods"] = db.CRMPeriods.Select(y => new { y.CRMPeriodId, y.Name }).ToList();
            ViewData["Categories"] = db.ProductCategories.Select(c => new { c.Description, c.ProductCategoryId }).ToList();
            ViewData["Branches"] = db.Branch.Select(c => new { c.Name, c.BranchId }).ToList();
            ViewData["CRMStores"] = db.SalesStores.Select(c => new { c.Description, c.SalesStoreId }).ToList();

            return View();
        }
        public ActionResult Plan(int pi, string cat, string subcat)
        {
            ViewData["Years"] = db.GLFiscalYears.Select(y => new { y.GlFiscalYearId, y.Name }).ToList();
            ViewData["Periods"] = db.CRMPeriods.Select(y => new { y.CRMPeriodId, y.Name }).ToList();
            //ViewData["Categories"] = db.ItemCategorys.Select(c => new { c.Name, c.ItemCategoryCode }).ToList();
            ViewData["Branches"] = db.Branch.Select(c => new { c.Name, c.BranchId }).ToList();
            //ViewData["InventoryStores"] = db.InventoryStores.Select(c => new { c.Name, c.StoreCode }).ToList();
            ViewData["CRMStores"] = db.SalesStores.Select(c => new { c.Description, c.SalesStoreId }).ToList();



            ViewBag.Category = "";
            ViewBag.SubCategory = "";
            ViewBag.FiscalYear = "";

            ViewBag.IsReadonly = false;

            var selectedPlan = db.AnnualSalesPlans.Find(pi);
            if(selectedPlan != null)
            {
                //ViewBag.CategoryId = selectedPlan.CategoryId;
                //ViewBag.SubCategoryId = selectedPlan.SubCategoryId == null ? "" : selectedPlan.SubCategoryId;
                var category = db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId.ToString() == cat);
                var subcategory = db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId.ToString() == subcat);
                if(category != null)
                {
                    ViewBag.Category = category.Description;

                }
                if(subcategory != null)
                {
                    ViewBag.SubCategory = subcategory.Description;
                }
                var planYear = db.GLFiscalYears.Find(selectedPlan.GlFiscalYearId);
                if(planYear != null)
                {
                    ViewBag.FiscalYear = planYear.Name;
                }
                ViewBag.IsReadonly = selectedPlan.IsOnlineApproved;
            }
            return View();
        }
        public ActionResult AnnualSalesPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AnnualSalesPlan> annualsalesplans = db.AnnualSalesPlans;
            DataSourceResult result = annualsalesplans.ToDataSourceResult(request, annualSalesPlan => new {
                AnnualSalesPlanId = annualSalesPlan.AnnualSalesPlanId,
                GlFiscalYearId = annualSalesPlan.GlFiscalYearId,
                CategoryId = annualSalesPlan.CategoryId,
                SubCategoryId = annualSalesPlan.SubCategoryId,
                AveragePrice = annualSalesPlan.AveragePrice,
                Profit = annualSalesPlan.Profit,
                TotalAnnualPlanQuantity = annualSalesPlan.TotalAnnualPlanQuantity,
                TotalAnnualPlanValue = annualSalesPlan.TotalAnnualPlanValue,
                TotalMonthlyPlanQuantity = annualSalesPlan.TotalMonthlyPlanQuantity,
                TotalMonthlyPlanValue = annualSalesPlan.TotalMonthlyPlanValue,
                TotalWeeklyPlanQuantity = annualSalesPlan.TotalWeeklyPlanQuantity,
                TotalWeeklyPlanValue = annualSalesPlan.TotalWeeklyPlanValue,
                TotalDailyPlanQuantity = annualSalesPlan.TotalDailyPlanQuantity,
                TotalDailyPlanValue = annualSalesPlan.TotalDailyPlanValue,

                Status = annualSalesPlan.Status,
                Remark = annualSalesPlan.Remark,
                IsVoid = annualSalesPlan.IsVoid,
                VoidBy = annualSalesPlan.VoidBy,
                VoidTime = annualSalesPlan.VoidTime,
                IsOnlineApproved = annualSalesPlan.IsOnlineApproved,
                OnlineApprovedBy = annualSalesPlan.OnlineApprovedBy,
                OnlineApprovedTime = annualSalesPlan.OnlineApprovedTime,
                IsOnlineTransferred = annualSalesPlan.IsOnlineTransferred,
                IsOnlineTransferredBy = annualSalesPlan.IsOnlineTransferredBy,
                OnlineTransferredTime = annualSalesPlan.OnlineTransferredTime,
                IsSendForApproval = annualSalesPlan.IsSendForApproval,
                OrgBranchName = annualSalesPlan.OrgBranchName,
                SendForApprovalBy = annualSalesPlan.SendForApprovalBy,
                SendForApprovalTime = annualSalesPlan.SendForApprovalTime,

                DateCreated = annualSalesPlan.DateCreated,
                LastModified = annualSalesPlan.LastModified,
                CreatedBy = annualSalesPlan.CreatedBy,
                ModifiedBy = annualSalesPlan.ModifiedBy
            });

            return Json(result);
        }
        private bool DuplicateReordExists(AnnualSalesPlan annualSalesPlan)
        {
            return db.AnnualSalesPlans
                .Any(p => p.GlFiscalYearId == annualSalesPlan.GlFiscalYearId &&
                            p.CategoryId == annualSalesPlan.CategoryId &&
                            p.SubCategoryId == annualSalesPlan.SubCategoryId &&
                            p.AnnualSalesPlanId != annualSalesPlan.AnnualSalesPlanId);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualSalesPlans_Create([DataSourceRequest]DataSourceRequest request, AnnualSalesPlan annualSalesPlan)
        {
            if (DuplicateReordExists(annualSalesPlan))
            {
                ModelState.AddModelError("", "Duplicate record not allowed");
            }
            if (ModelState.IsValid)
            {
                var entity = new AnnualSalesPlan
                {
                    GlFiscalYearId = annualSalesPlan.GlFiscalYearId,
                    CategoryId = annualSalesPlan.CategoryId,
                    SubCategoryId = annualSalesPlan.SubCategoryId,
                    AveragePrice = annualSalesPlan.AveragePrice,
                    Profit = annualSalesPlan.Profit,
                    TotalAnnualPlanQuantity = annualSalesPlan.TotalAnnualPlanQuantity,
                    TotalAnnualPlanValue = annualSalesPlan.TotalAnnualPlanValue,
                    TotalMonthlyPlanQuantity = annualSalesPlan.TotalMonthlyPlanQuantity,
                    TotalMonthlyPlanValue = annualSalesPlan.TotalMonthlyPlanValue,
                    TotalWeeklyPlanQuantity = annualSalesPlan.TotalWeeklyPlanQuantity,
                    TotalWeeklyPlanValue = annualSalesPlan.TotalWeeklyPlanValue,
                    TotalDailyPlanQuantity = annualSalesPlan.TotalDailyPlanQuantity,
                    TotalDailyPlanValue = annualSalesPlan.TotalDailyPlanValue,

                    Status = annualSalesPlan.Status,
                    Remark = annualSalesPlan.Remark,
                    IsVoid = annualSalesPlan.IsVoid,
                    VoidBy = annualSalesPlan.VoidBy,
                    VoidTime = annualSalesPlan.VoidTime,
                    IsOnlineApproved = annualSalesPlan.IsOnlineApproved,
                    OnlineApprovedBy = annualSalesPlan.OnlineApprovedBy,
                    OnlineApprovedTime = annualSalesPlan.OnlineApprovedTime,
                    IsOnlineTransferred = annualSalesPlan.IsOnlineTransferred,
                    IsOnlineTransferredBy = annualSalesPlan.IsOnlineTransferredBy,
                    OnlineTransferredTime = annualSalesPlan.OnlineTransferredTime,
                    IsSendForApproval = annualSalesPlan.IsSendForApproval,
                    OrgBranchName = annualSalesPlan.OrgBranchName,
                    SendForApprovalBy = annualSalesPlan.SendForApprovalBy,
                    SendForApprovalTime = annualSalesPlan.SendForApprovalTime,

                    DateCreated = DateTime.Now,
                    LastModified = annualSalesPlan.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = annualSalesPlan.ModifiedBy
                };

                var totalWithoutProfit = entity.TotalAnnualPlanQuantity * entity.AveragePrice;
                var profitAmount = totalWithoutProfit * entity.Profit / 100;
                entity.TotalAnnualPlanValue = totalWithoutProfit + profitAmount;

                // monthly
                entity.TotalMonthlyPlanQuantity = entity.TotalAnnualPlanQuantity / 12;
                entity.TotalMonthlyPlanValue = entity.TotalAnnualPlanValue / 12;

                // daily
                entity.TotalDailyPlanQuantity = entity.TotalAnnualPlanQuantity / 365;
                entity.TotalDailyPlanValue = entity.TotalAnnualPlanValue / 365;

                // weekly
                entity.TotalWeeklyPlanQuantity = entity.TotalDailyPlanQuantity * 7;
                entity.TotalWeeklyPlanValue = entity.TotalDailyPlanValue * 7;


                db.AnnualSalesPlans.Add(entity);
                db.SaveChanges();
                annualSalesPlan.AnnualSalesPlanId = entity.AnnualSalesPlanId;

                AddPlanDetail(entity);
            }

            return Json(new[] { annualSalesPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualSalesPlans_Update([DataSourceRequest]DataSourceRequest request, AnnualSalesPlan annualSalesPlan)
        {
            if (DuplicateReordExists(annualSalesPlan))
            {
                ModelState.AddModelError("", "Duplicate record not allowed");
            }
            if (ModelState.IsValid)
            {
                var entity = new AnnualSalesPlan
                {
                    AnnualSalesPlanId = annualSalesPlan.AnnualSalesPlanId,
                    GlFiscalYearId = annualSalesPlan.GlFiscalYearId,
                    CategoryId = annualSalesPlan.CategoryId,
                    SubCategoryId = annualSalesPlan.SubCategoryId,
                    AveragePrice = annualSalesPlan.AveragePrice,
                    Profit = annualSalesPlan.Profit,
                    TotalAnnualPlanQuantity = annualSalesPlan.TotalAnnualPlanQuantity,
                    TotalAnnualPlanValue = annualSalesPlan.TotalAnnualPlanValue,
                    TotalMonthlyPlanQuantity = annualSalesPlan.TotalMonthlyPlanQuantity,
                    TotalMonthlyPlanValue = annualSalesPlan.TotalMonthlyPlanValue,
                    TotalWeeklyPlanQuantity = annualSalesPlan.TotalWeeklyPlanQuantity,
                    TotalWeeklyPlanValue = annualSalesPlan.TotalWeeklyPlanValue,
                    TotalDailyPlanQuantity = annualSalesPlan.TotalDailyPlanQuantity,
                    TotalDailyPlanValue = annualSalesPlan.TotalDailyPlanValue,

                    Status = annualSalesPlan.Status,
                    Remark = annualSalesPlan.Remark,
                    IsVoid = annualSalesPlan.IsVoid,
                    VoidBy = annualSalesPlan.VoidBy,
                    VoidTime = annualSalesPlan.VoidTime,
                    IsOnlineApproved = annualSalesPlan.IsOnlineApproved,
                    OnlineApprovedBy = annualSalesPlan.OnlineApprovedBy,
                    OnlineApprovedTime = annualSalesPlan.OnlineApprovedTime,
                    IsOnlineTransferred = annualSalesPlan.IsOnlineTransferred,
                    IsOnlineTransferredBy = annualSalesPlan.IsOnlineTransferredBy,
                    OnlineTransferredTime = annualSalesPlan.OnlineTransferredTime,
                    IsSendForApproval = annualSalesPlan.IsSendForApproval,
                    OrgBranchName = annualSalesPlan.OrgBranchName,
                    SendForApprovalBy = annualSalesPlan.SendForApprovalBy,
                    SendForApprovalTime = annualSalesPlan.SendForApprovalTime,

                    DateCreated = annualSalesPlan.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = annualSalesPlan.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                var totalWithoutProfit = entity.TotalAnnualPlanQuantity * entity.AveragePrice;
                var profitAmount = totalWithoutProfit * entity.Profit / 100;
                entity.TotalAnnualPlanValue = totalWithoutProfit + profitAmount;

                // monthly
                entity.TotalMonthlyPlanQuantity = entity.TotalAnnualPlanQuantity / 12;
                entity.TotalMonthlyPlanValue = entity.TotalAnnualPlanValue / 12;

                // daily
                entity.TotalDailyPlanQuantity = entity.TotalAnnualPlanQuantity / 365;
                entity.TotalDailyPlanValue = entity.TotalAnnualPlanValue / 365;

                // weekly
                entity.TotalWeeklyPlanQuantity = entity.TotalDailyPlanQuantity * 7;
                entity.TotalWeeklyPlanValue = entity.TotalDailyPlanValue * 7;



                db.AnnualSalesPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                // if no branch plan exists, create them
                AddPlanDetail(entity);

               new CRMSalesPlanHelper().UpdateBranchPlanOnCompanyPlanUpdate(entity);
            }

            return Json(new[] { annualSalesPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualSalesPlans_Destroy([DataSourceRequest]DataSourceRequest request, AnnualSalesPlan annualSalesPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnnualSalesPlan
                {
                    AnnualSalesPlanId = annualSalesPlan.AnnualSalesPlanId,
                    GlFiscalYearId = annualSalesPlan.GlFiscalYearId,
                    CategoryId = annualSalesPlan.CategoryId,
                    SubCategoryId = annualSalesPlan.SubCategoryId,
                    AveragePrice = annualSalesPlan.AveragePrice,
                    Profit = annualSalesPlan.Profit,
                    TotalAnnualPlanQuantity = annualSalesPlan.TotalAnnualPlanQuantity,
                    TotalAnnualPlanValue = annualSalesPlan.TotalAnnualPlanValue,
                    TotalMonthlyPlanQuantity = annualSalesPlan.TotalMonthlyPlanQuantity,
                    TotalMonthlyPlanValue = annualSalesPlan.TotalMonthlyPlanValue,
                    TotalWeeklyPlanQuantity = annualSalesPlan.TotalWeeklyPlanQuantity,
                    TotalWeeklyPlanValue = annualSalesPlan.TotalWeeklyPlanValue,
                    TotalDailyPlanQuantity = annualSalesPlan.TotalDailyPlanQuantity,
                    TotalDailyPlanValue = annualSalesPlan.TotalDailyPlanValue,

                    Status = annualSalesPlan.Status,
                    Remark = annualSalesPlan.Remark,
                    IsVoid = annualSalesPlan.IsVoid,
                    VoidBy = annualSalesPlan.VoidBy,
                    VoidTime = annualSalesPlan.VoidTime,
                    IsOnlineApproved = annualSalesPlan.IsOnlineApproved,
                    OnlineApprovedBy = annualSalesPlan.OnlineApprovedBy,
                    OnlineApprovedTime = annualSalesPlan.OnlineApprovedTime,
                    IsOnlineTransferred = annualSalesPlan.IsOnlineTransferred,
                    IsOnlineTransferredBy = annualSalesPlan.IsOnlineTransferredBy,
                    OnlineTransferredTime = annualSalesPlan.OnlineTransferredTime,
                    IsSendForApproval = annualSalesPlan.IsSendForApproval,
                    OrgBranchName = annualSalesPlan.OrgBranchName,
                    SendForApprovalBy = annualSalesPlan.SendForApprovalBy,
                    SendForApprovalTime = annualSalesPlan.SendForApprovalTime,

                    DateCreated = annualSalesPlan.DateCreated,
                    LastModified = annualSalesPlan.LastModified,
                    CreatedBy = annualSalesPlan.CreatedBy,
                    ModifiedBy = annualSalesPlan.ModifiedBy
                };

                db.AnnualSalesPlans.Attach(entity);
                db.AnnualSalesPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { annualSalesPlan }.ToDataSourceResult(request, ModelState));
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

        public JsonResult SendForApproval(int id)
        {
            var record = db.AnnualSalesPlans.Find(id);
            if ((record != null && !record.IsSendForApproval && !record.IsOnlineApproved))
            {
                #region validate line items
                var lineItems = db.BranchAnnualSalesPlans.Where(li => li.AnnualSalesPlanId == record.AnnualSalesPlanId).ToList();
                if (!lineItems.Any())
                {
                    var d = new
                    {
                        Message = "No Branch Plans found. Please add Branch Plan first",
                        Status = "NOT_OK"
                    };
                    return Json(d, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //foreach (De lineItem in lineItems)
                    //{
                    //    if (lineItem.TotalQuantity - (lineItem.ShipedQuantity + lineItem.ShipedQuantityTilPreShipment) < 0)
                    //    {
                    //        return Json(new { Message = "Some line items have invalid shipment quantity.", Status = "NOT_OK" });
                    //    }
                    //}
                }
                #endregion

                record.Status = OperationStatuses.WaitingApproval.ToString();
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.AnnualSalesPlans.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Successfully Sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Deposit record not found!",
                    Status = "OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
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
        public JsonResult VoidSalesPlan(int id)
        {
            var record = db.AnnualSalesPlans.Find(id);
            if (record != null && record.IsOnlineApproved)
            {
               

                record.Status = OperationStatuses.Void.ToString();
                record.IsSendForApproval = false;
                record.SendForApprovalBy = null;
                record.SendForApprovalTime = null;

                record.IsVoid = true;
                record.VoidTime = DateTime.Now;
                record.VoidBy = User.Identity.Name;

                record.IsOnlineApproved = false;
                record.OnlineApprovedBy = null;
                record.OnlineApprovedTime = null;

                db.AnnualSalesPlans.Attach(record);
                db.Entry(record).State = EntityState.Modified;

                var validations = db.AnnualSalesPlanValidation.Where(v => v.AnnualSalesPlanId == record.AnnualSalesPlanId).ToList();
                db.AnnualSalesPlanValidation.RemoveRange(validations);

                db.SaveChanges();

                return Json(new { Message = "Void was Successful ", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Sales Plan record not found!",
                    Status = "OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
            }
            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is in Editing Mode. No need to void.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            
            return null;
        }
        private void AddPlanDetail(AnnualSalesPlan salesPlan)
        {
           var existingRec = db.BranchAnnualSalesPlans.Where(p => p.AnnualSalesPlanId == salesPlan.AnnualSalesPlanId).ToList();
            if (!existingRec.Any())
            {
                var branches = db.Branch.ToList();
                if (branches.Any())
                {
                    decimal defaultRatio = 100 / (decimal)branches.Count;
                    var qtyOfBranch = defaultRatio/100 * salesPlan.TotalAnnualPlanQuantity;
                    var valueOfBranchWithoutProfit = qtyOfBranch * salesPlan.AveragePrice;
                    var profitOfBranch = valueOfBranchWithoutProfit * salesPlan.Profit / 100;


                    foreach (var branch in branches)
                    {
                        var branchPlan = new BranchAnnualSalesPlan
                        {
                            BranchId = branch.BranchId,
                            AnnualSalesPlanId = salesPlan.AnnualSalesPlanId,
                            Ratio = defaultRatio,
                            Quantity = qtyOfBranch,
                            Value = valueOfBranchWithoutProfit + profitOfBranch,

                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                        };
                        db.BranchAnnualSalesPlans.Add(branchPlan);
                    }
                    db.SaveChanges();
                }
            }
           
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

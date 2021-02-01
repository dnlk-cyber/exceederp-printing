using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.ProductionStores
{
    public class ProductionIssueController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult Index()
        {
            #region View Data
            ViewData["InventoryStores"] = db.InventoryStores.Select(s => new { s.BranchId, s.StoreCode, s.Name }).ToList();
            ViewData["ProductionPeriods"] = db.ProductionPeriods.Select(p => new { p.ProductionPeriodId, p.Name }).ToList();

            ViewData["ItemCategories"] = db.ItemCategorys.Select(c => new { c.ItemCategoryCode, c.Name }).ToList();
            ViewData["InventoryItems"] = db.InventoryItems.Select(i => new { i.ItemCode, i.Name }).ToList();

            //var taxRates = db.GLTaxRates;
            //var taxWithConcatnatedValue = new List<GLTaxRate>();
            //foreach (GLTaxRate taxRate in taxRates)
            //{
            //    var taxName = new GLTaxRate
            //    {
            //        GLTaxRateID = taxRate.GLTaxRateID,
            //        TaxName = taxRate.TaxName + "( " + Math.Round(taxRate.Rate, 0) + "% )"
            //    };
            //    taxWithConcatnatedValue.Add(taxName);
            //}
            //ViewData["TaxTypes"] = taxWithConcatnatedValue;

            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name }).ToList();
            ViewData["ProcessingPlants"] = db.ProcessingPlants.Select(b => new { b.BranchId, b.ProcessingPlantId, b.Name }).ToList(); 
            #endregion

            return View();
        }

        public ActionResult Issues_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductionIssue> issues = db.ProductionIssues;
            DataSourceResult result = issues.ToDataSourceResult(request, issue => new
            {
                ProductionIssueId = issue.ProductionIssueId,
                BranchId = issue.BranchId,
                ProductionPeriodId = issue.ProductionPeriodId,
                StoreCode = issue.StoreCode,
                IssueDate = issue.IssueDate,
                SupplierAccountCode = issue.SupplierAccountCode,
                ProcessingPlantId = issue.ProcessingPlantId,
                RecipientId = issue.RecipientId,
                Reference = issue.Reference,

                OrgBranchName = issue.OrgBranchName,
                DateCreated = issue.DateCreated,
                LastModified =  issue.LastModified,
                CreatedBy = issue.CreatedBy,
                ModifiedBy = issue.ModifiedBy ?? " ",

                Status = issue.Status,
                IsSendForApproval = issue.IsSendForApproval,
                SendForApprovalBy = issue.SendForApprovalBy,
                SendForApprovalTime = issue.SendForApprovalTime,
                IsOnlineApproved = issue.IsOnlineApproved,
                OnlineApprovedBy = issue.OnlineApprovedBy,
                OnlineApprovedTime = issue.OnlineApprovedTime,
                IsOnlineTransferred = issue.IsOnlineTransferred,
                IsOnlineTransferredBy = issue.IsOnlineTransferredBy,
                OnlineTransferredTime = issue.OnlineTransferredTime,
                IsVoid = issue.IsVoid,
                VoidBy = issue.VoidBy,
                VoidTime = issue.VoidTime,
            });

            return Json(result);
        }

        //private bool CheckExistance(int projectId)
        //{
        //    bool paymentExists = false;
        //    var issue = db.ProjectAdvancePayments.Where(ap => ap.ProjectInformationId == projectId).FirstOrDefault();
        //    if (issue != null)
        //    {
        //        paymentExists = true;
        //    }
        //    return paymentExists;
        //}
        private bool DateIsNotInRange(int id, DateTime date)
        {
            var valueIsNotInRage = false;
            var item = db.ProductionPeriods.Find(id);
            if (item != null)
            {
                if (date < item.DateFrom || date > item.DateTo)
                {
                    valueIsNotInRage = true;
                }
            }
            return valueIsNotInRage;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Issue_Create([DataSourceRequest]DataSourceRequest request, ProductionIssue issue)
        {
            if (ModelState.IsValid)
            {
                if (DateIsNotInRange(issue.ProductionPeriodId, issue.IssueDate))
                {
                    ModelState.AddModelError("", "Date must be within Period Range!!");
                }
                else
                {
                    var entity = new ProductionIssue
                    {
                        ProductionIssueId = issue.ProductionIssueId,
                        BranchId = issue.BranchId,
                        ProductionPeriodId = issue.ProductionPeriodId,
                        StoreCode = issue.StoreCode,
                        IssueDate = issue.IssueDate,
                        SupplierAccountCode = issue.SupplierAccountCode,
                        ProcessingPlantId = issue.ProcessingPlantId,
                        RecipientId = issue.RecipientId,
                        Reference = issue.Reference,

                        OrgBranchName = issue.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = issue.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = issue.ModifiedBy,

                        Status = OperationStatuses.New.ToString(),
                        IsSendForApproval = issue.IsSendForApproval,
                        SendForApprovalBy = issue.SendForApprovalBy,
                        SendForApprovalTime = issue.SendForApprovalTime,
                        IsOnlineApproved = issue.IsOnlineApproved,
                        OnlineApprovedBy = issue.OnlineApprovedBy,
                        OnlineApprovedTime = issue.OnlineApprovedTime,
                        IsOnlineTransferred = issue.IsOnlineTransferred,
                        IsOnlineTransferredBy = issue.IsOnlineTransferredBy,
                        OnlineTransferredTime = issue.OnlineTransferredTime,
                        IsVoid = issue.IsVoid,
                        VoidBy = issue.VoidBy,
                        VoidTime = issue.VoidTime,
                    };

                    db.ProductionIssues.Add(entity);
                    db.SaveChanges();
                    issue.ProductionIssueId = entity.ProductionIssueId;

                   // this.UpdateIssueReference(entity);
                }
            }

            return Json(new[] { issue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Issue_Update([DataSourceRequest]DataSourceRequest request, ProductionIssue issue)
        {
            if (ModelState.IsValid)
            {
                if (DateIsNotInRange(issue.ProductionPeriodId, issue.IssueDate))
                {
                    ModelState.AddModelError("", "Date must be within Period Range!!");
                }
                else
                {
                    var entity = new ProductionIssue
                    {
                        ProductionIssueId = issue.ProductionIssueId,
                        BranchId = issue.BranchId,
                        ProductionPeriodId = issue.ProductionPeriodId,
                        StoreCode = issue.StoreCode,
                        IssueDate = issue.IssueDate,
                        SupplierAccountCode = issue.SupplierAccountCode,
                        ProcessingPlantId = issue.ProcessingPlantId,
                        RecipientId = issue.RecipientId,
                        Reference = issue.Reference,

                        OrgBranchName = issue.OrgBranchName,
                        DateCreated = issue.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = issue.CreatedBy,
                        ModifiedBy = User.Identity.Name,

                        Status = issue.Status,
                        IsSendForApproval = issue.IsSendForApproval,
                        SendForApprovalBy = issue.SendForApprovalBy,
                        SendForApprovalTime = issue.SendForApprovalTime,
                        IsOnlineApproved = issue.IsOnlineApproved,
                        OnlineApprovedBy = issue.OnlineApprovedBy,
                        OnlineApprovedTime = issue.OnlineApprovedTime,
                        IsOnlineTransferred = issue.IsOnlineTransferred,
                        IsOnlineTransferredBy = issue.IsOnlineTransferredBy,
                        OnlineTransferredTime = issue.OnlineTransferredTime,
                        IsVoid = issue.IsVoid,
                        VoidBy = issue.VoidBy,
                        VoidTime = issue.VoidTime,
                    };

                    db.ProductionIssues.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
            }

            return Json(new[] { issue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Issue_Destroy([DataSourceRequest]DataSourceRequest request, ProductionIssue issue)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionIssue
                {
                    ProductionIssueId = issue.ProductionIssueId,
                    BranchId = issue.BranchId,
                    ProductionPeriodId = issue.ProductionPeriodId,
                    StoreCode = issue.StoreCode,
                    IssueDate = issue.IssueDate,
                    SupplierAccountCode = issue.SupplierAccountCode,
                    ProcessingPlantId = issue.ProcessingPlantId,
                    RecipientId = issue.RecipientId,
                    Reference = issue.Reference,

                    OrgBranchName = issue.OrgBranchName,
                    DateCreated = issue.DateCreated,
                    LastModified = issue.LastModified,
                    CreatedBy = issue.CreatedBy,
                    ModifiedBy = issue.ModifiedBy,

                    Status = issue.Status,
                    IsSendForApproval = issue.IsSendForApproval,
                    SendForApprovalBy = issue.SendForApprovalBy,
                    SendForApprovalTime = issue.SendForApprovalTime,
                    IsOnlineApproved = issue.IsOnlineApproved,
                    OnlineApprovedBy = issue.OnlineApprovedBy,
                    OnlineApprovedTime = issue.OnlineApprovedTime,
                    IsOnlineTransferred = issue.IsOnlineTransferred,
                    IsOnlineTransferredBy = issue.IsOnlineTransferredBy,
                    OnlineTransferredTime = issue.OnlineTransferredTime,
                    IsVoid = issue.IsVoid,
                    VoidBy = issue.VoidBy,
                    VoidTime = issue.VoidTime,
                };

                db.ProductionIssues.Attach(entity);
                db.ProductionIssues.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { issue }.ToDataSourceResult(request, ModelState));
        }

      private void UpdateIssueReference(ProductionIssue entity)
        {
            entity.Reference = entity.ProductionIssueId.ToString("D12");

            db.ProductionIssues.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
        public JsonResult SendForApproval(int id)
        {
            var record = db.ProductionIssues.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.IssueItems.Count > 0)
            {
                #region validate price

                foreach (var item in record.IssueItems)
                {
                    if (item.UnitPrice <= 0 )
                    {
                        var formattedData3 = new
                        {
                            Message = "Please fill unit price  information for all items.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData3, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion

                //record.OperationStatus = OperationStatuses.WaitingApproval;
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.ProductionIssues.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
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

            if (record.IssueItems.Count == 0)
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
        //public JsonResult TransferDistribution(string id, string description)
        //{
        //    AccountHelper accountHelper = new AccountHelper();
        //    int myid = int.Parse(id);
        //    WorkInProgress record = db.WorkInProgress.Find(myid);

        //    if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
        //    {
        //        #region check segment length validation

        //        foreach (var dist in record.WorkInProgressItems)
        //        {
        //            string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
        //            if (!String.IsNullOrEmpty(message))
        //            {
        //                var formattedData = new
        //                {
        //                    Message = message,
        //                    Status = "NOT_OK"
        //                };
        //                return Json(formattedData, JsonRequestBehavior.AllowGet);
        //            }

        //        }


        //        #endregion
        //        #region check if debit and credit are equal
        //        decimal debit = 0;
        //        decimal credit = 0;
        //        var wipDistributions = db.WorkInProgressDistribution.Where(d => d.WorkInProgressId == record.WorkInProgressId).ToList();
        //        //var distribution = record.IssueDistribution.ToList();
        //        if (wipDistributions.Any())
        //        {
        //            debit = Math.Round(wipDistributions.Sum(x => x.Debit), 2);
        //            credit = Math.Round(wipDistributions.Sum(x => x.Credit), 2);
        //        }
        //        if (debit != credit)
        //        {
        //            var formattedData = new
        //            {
        //                Message = "Total debit and total credit is not balanced.",
        //                Status = "NOT_OK"
        //            };
        //            return Json(formattedData, JsonRequestBehavior.AllowGet);
        //        }
        //        if (debit == 0)
        //        {
        //            var formattedData = new
        //            {
        //                Message = "Total debit and total credit is zero.",
        //                Status = "NOT_OK"
        //            };
        //            return Json(formattedData, JsonRequestBehavior.AllowGet);
        //        }
        //        #endregion
        //        #region send to GL
        //        string glperiod = "";
        //        string glyear = "";
        //        #region get period

        //        var period = db.ConstructionPeriod.FirstOrDefault(p => p.DateFrom < record.Date && p.DateTo > record.Date);
        //        //var period = db.InventoryPeriods.Find(record.GLPeriodId);


        //        if (period != null)
        //        {
        //            var glperiodObj = db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

        //            if (glperiodObj != null)
        //            {
        //                glperiod = glperiodObj.Name;
        //                glyear = glperiodObj.GlFiscalYear.Name;
        //                if (glperiodObj.Status == FiscalYearStatus.Close)
        //                {
        //                    var formattedData = new
        //                    {
        //                        Message = $"The period { glperiodObj.Name } is closed,the record cannot be transfered to closed period.",
        //                        Status = "NOT_OK"
        //                    };
        //                    return Json(formattedData, JsonRequestBehavior.AllowGet);
        //                }
        //            }

        //            else
        //            {
        //                var formattedData = new
        //                {
        //                    Message = $"The corresponding GL period {period.Name} is not found.",
        //                    Status = "NOT_OK"
        //                };
        //                return Json(formattedData, JsonRequestBehavior.AllowGet);

        //            }
        //        }
        //        else
        //        {
        //            var formattedData = new
        //            {
        //                Message = $"The period is not found in construction period.",
        //                Status = "NOT_OK"
        //            };
        //            return Json(formattedData, JsonRequestBehavior.AllowGet);
        //        }

        //        #endregion
        //        if (wipDistributions.Any())
        //        {
        //            GlRecordJournal journal = new GlRecordJournal
        //            {
        //                DateCreated = DateTime.Now,
        //                CreatedBy = User.Identity.Name,
        //                JournalName = "WIP",
        //                Description = !String.IsNullOrEmpty(description) ? description : "WIP",
        //                FiscalYear = glyear,
        //                PeriodName = glperiod,
        //                CategoryName = "WIP",
        //                SourcesName = "WIP",
        //                JournalNumber = "WIP." + record.WorkInProgressId.ToString(),
        //                EffectiveDate = record.Date,
        //                Status = "Approved",
        //                IsOnlineApproved = true,
        //                OnlineApprovedBy = User.Identity.Name,
        //                OnlineApprovedTime = DateTime.Now,
        //                OrgBranchName = record.OrgBranchName
        //            };

        //            db.GLRecordJournals.Add(journal);
        //            db.SaveChanges();
        //            foreach (WorkInProgressDistribution i in wipDistributions)
        //            {
        //                #region debit and credit

        //                var journalDetail = new GLRecordJournalDetail
        //                {
        //                    DateCreated = DateTime.Now,
        //                    CreatedBy = User.Identity.Name,
        //                    GLRecordJournal = journal,
        //                    AccountCode = i.AccountCode,
        //                    AccountDesc = i.AccountDesc,
        //                    AdditionalDescription = i.AdditionalDescription,
        //                    Debit = i.Debit,
        //                    Credit = i.Credit
        //                };
        //                db.GLRecordJournalDetails.Add(journalDetail);
        //                db.SaveChanges();
        //                #endregion
        //            }
        //            record.IsOnlineTransferred = true;
        //            record.IsOnlineTransferredBy = User.Identity.Name;
        //            record.OnlineTransferredTime = DateTime.Now;
        //            db.Entry(record).State = EntityState.Modified;
        //            db.SaveChanges();
        //            var formattedData = new
        //            {
        //                Message = "Successfully transferred.",
        //                Status = "OK"
        //            };
        //            return Json(formattedData, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            var formattedData = new
        //            {
        //                Message = "There is no distribution to be transferred.",
        //                Status = "NOT_OK"
        //            };
        //            return Json(formattedData, JsonRequestBehavior.AllowGet);
        //        }
        //        #endregion

        //    }
        //    if (record != null && !record.IsOnlineApproved)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "The record cannot be transfered before validation.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }

        //    if (record != null && record.IsOnlineTransferred)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "You cannot transfer this transaction, it is already transfered.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }

        //    return null;
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
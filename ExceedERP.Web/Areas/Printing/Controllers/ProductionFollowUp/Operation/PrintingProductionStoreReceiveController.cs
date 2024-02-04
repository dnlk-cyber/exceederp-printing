using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Operation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpFinishedGoodUser)]
    public class PrintingProductionStoreReceiveController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult Index()
        {
            ViewData["InventoryStores"] = db.InventoryStores.Select(s => new { s.Name, s.StoreCode}).ToList();
            ViewData["ProductionPeriods"] = db.GLPeriods.Select(p  => new { p.Name, p.GLPeriodId}).ToList();
            ViewData["JobOrder"] = db.Jobs.ToList();

            //ViewData["SeedProductionItems"] = db.SeedProductionOrderItems.Select(i => new { i.ProductionCode, i.SeedProductionOrderItemId}).ToList();
            //ViewData["GumProductionItems"] = db.GumProductionOrderItems.Select(i => new { i.ProductionCode, i.GumProductionOrderItemId}).ToList();
            ViewData["ItemCategories"] = db.ItemCategorys.Select(c => new { c.Name, c.ItemCategoryCode}).ToList();
            ViewData["InventoryItems"] = db.InventoryItems.Select(i => new { i.Name, i.ItemCode}).ToList();

            var taxRates = db.GLTaxRates;
            var taxWithConcatnatedValue = new List<GLTaxRate>();
            foreach (GLTaxRate taxRate in taxRates)
            {
                var taxName = new GLTaxRate
                {
                    GLTaxRateID = taxRate.GLTaxRateID,
                    TaxName = taxRate.TaxName + "( " + Math.Round(taxRate.Rate, 0) + "% )"
                };
                taxWithConcatnatedValue.Add(taxName);
            }
            ViewData["TaxTypes"] = taxWithConcatnatedValue;

            ViewData["Branchs"] = db.Branch.Select(b => new { b.BranchId, b.Name}).ToList();

            return View();
        }

        public ActionResult StoreReceives_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingProductionFinishedGoodsStoreReceive> storeReceives = db.PrintingProductionFinishedGoodsStoreReceives;
            DataSourceResult result = storeReceives.ToDataSourceResult(request, receive => new
            {
                FinishedGoodsStoreReceiveId = receive.FinishedGoodsStoreReceiveId,
                ProductionPeriodId = receive.ProductionPeriodId,
                StoreCode = receive.StoreCode,
                ReceiveDate = receive.ReceiveDate,
                Supervisor = receive.Supervisor,
                DeliveredBy = receive.DeliveredBy,
                ReceivedBy = receive.ReceivedBy,
                CustomerId = receive.CustomerId,
                JobId = receive.JobId,
                OrgBranchName = receive.OrgBranchName,
                DateCreated = receive.DateCreated,
                LastModified =  receive.LastModified,
                CreatedBy = receive.CreatedBy,
                ModifiedBy = receive.ModifiedBy ?? " ",

                Status = receive.Status,
                IsSendForApproval = receive.IsSendForApproval,
                SendForApprovalBy = receive.SendForApprovalBy,
                SendForApprovalTime = receive.SendForApprovalTime,
                IsOnlineApproved = receive.IsOnlineApproved,
                OnlineApprovedBy = receive.OnlineApprovedBy,
                OnlineApprovedTime = receive.OnlineApprovedTime,
                IsOnlineTransferred = receive.IsOnlineTransferred,
                IsOnlineTransferredBy = receive.IsOnlineTransferredBy,
                OnlineTransferredTime = receive.OnlineTransferredTime,
                IsVoid = receive.IsVoid,
                VoidBy = receive.VoidBy,
                VoidTime = receive.VoidTime,
            });

            return Json(result);
        }

        //private bool CheckExistance(int projectId)
        //{
        //    bool paymentExists = false;
        //    var receive = db.ProjectAdvancePayments.Where(ap => ap.ProjectInformationId == projectId).FirstOrDefault();
        //    if (receive != null)
        //    {
        //        paymentExists = true;
        //    }
        //    return paymentExists;
        //}
        private bool DateIsNotInRange(int id, DateTime date)
        {
            var valueIsNotInRage = false;
            var item = db.GLPeriods.Find(id);
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
        public ActionResult StoreReceive_Create([DataSourceRequest]DataSourceRequest request, PrintingProductionFinishedGoodsStoreReceive receive)
        {
            if (ModelState.IsValid)
            {
                if (DateIsNotInRange(receive.ProductionPeriodId, receive.ReceiveDate))
                {
                    ModelState.AddModelError("", "Date must be within Period Range!!");
                }
                else
                {
                    var entity = new PrintingProductionFinishedGoodsStoreReceive
                    {
                        FinishedGoodsStoreReceiveId = receive.FinishedGoodsStoreReceiveId,
                        ProductionPeriodId = receive.ProductionPeriodId,
                        StoreCode = receive.StoreCode,
                        ReceiveDate = receive.ReceiveDate,
                        Supervisor = receive.Supervisor,
                        DeliveredBy = receive.DeliveredBy,
                        ReceivedBy = receive.ReceivedBy,
                        JobId = receive.JobId,
                        OrgBranchName = receive.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = receive.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = receive.ModifiedBy,

                        Status = OperationStatuses.New.ToString(),
                        IsSendForApproval = receive.IsSendForApproval,
                        SendForApprovalBy = receive.SendForApprovalBy,
                        SendForApprovalTime = receive.SendForApprovalTime,
                        IsOnlineApproved = receive.IsOnlineApproved,
                        OnlineApprovedBy = receive.OnlineApprovedBy,
                        OnlineApprovedTime = receive.OnlineApprovedTime,
                        IsOnlineTransferred = receive.IsOnlineTransferred,
                        IsOnlineTransferredBy = receive.IsOnlineTransferredBy,
                        OnlineTransferredTime = receive.OnlineTransferredTime,
                        IsVoid = receive.IsVoid,
                        VoidBy = receive.VoidBy,
                        VoidTime = receive.VoidTime,
                    };

                    db.PrintingProductionFinishedGoodsStoreReceives.Add(entity);
                    db.SaveChanges();
                    receive.FinishedGoodsStoreReceiveId = entity.FinishedGoodsStoreReceiveId;

                }
            }

            return Json(new[] { receive }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReceive_Update([DataSourceRequest]DataSourceRequest request, PrintingProductionFinishedGoodsStoreReceive receive)
        {
            if (ModelState.IsValid)
            {
                if (DateIsNotInRange(receive.ProductionPeriodId, receive.ReceiveDate))
                {
                    ModelState.AddModelError("", "Date must be within Period Range!!");
                }
                else
                {
                    var entity = new PrintingProductionFinishedGoodsStoreReceive
                    {
                        FinishedGoodsStoreReceiveId = receive.FinishedGoodsStoreReceiveId,
                        ProductionPeriodId = receive.ProductionPeriodId,
                        StoreCode = receive.StoreCode,
                        ReceiveDate = receive.ReceiveDate,
                        Supervisor = receive.Supervisor,
                        DeliveredBy = receive.DeliveredBy,
                        ReceivedBy = receive.ReceivedBy,
                        JobId = receive.JobId,
                        OrgBranchName = receive.OrgBranchName,
                        DateCreated = receive.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = receive.CreatedBy,
                        ModifiedBy = User.Identity.Name,

                        Status = receive.Status,
                        IsSendForApproval = receive.IsSendForApproval,
                        SendForApprovalBy = receive.SendForApprovalBy,
                        SendForApprovalTime = receive.SendForApprovalTime,
                        IsOnlineApproved = receive.IsOnlineApproved,
                        OnlineApprovedBy = receive.OnlineApprovedBy,
                        OnlineApprovedTime = receive.OnlineApprovedTime,
                        IsOnlineTransferred = receive.IsOnlineTransferred,
                        IsOnlineTransferredBy = receive.IsOnlineTransferredBy,
                        OnlineTransferredTime = receive.OnlineTransferredTime,
                        IsVoid = receive.IsVoid,
                        VoidBy = receive.VoidBy,
                        VoidTime = receive.VoidTime,
                    };

                    db.PrintingProductionFinishedGoodsStoreReceives.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
            }

            return Json(new[] { receive }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreReceive_Destroy([DataSourceRequest]DataSourceRequest request, PrintingProductionFinishedGoodsStoreReceive receive)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProductionFinishedGoodsStoreReceive
                {
                    FinishedGoodsStoreReceiveId = receive.FinishedGoodsStoreReceiveId,
                    ProductionPeriodId = receive.ProductionPeriodId,
                    StoreCode = receive.StoreCode,
                    ReceiveDate = receive.ReceiveDate,
                    Supervisor = receive.Supervisor,
                    DeliveredBy = receive.DeliveredBy,
                    ReceivedBy = receive.ReceivedBy,
                    JobId = receive.JobId,
                    OrgBranchName = receive.OrgBranchName,
                    DateCreated = receive.DateCreated,
                    LastModified = receive.LastModified,
                    CreatedBy = receive.CreatedBy,
                    ModifiedBy = receive.ModifiedBy,

                    Status = receive.Status,
                    IsSendForApproval = receive.IsSendForApproval,
                    SendForApprovalBy = receive.SendForApprovalBy,
                    SendForApprovalTime = receive.SendForApprovalTime,
                    IsOnlineApproved = receive.IsOnlineApproved,
                    OnlineApprovedBy = receive.OnlineApprovedBy,
                    OnlineApprovedTime = receive.OnlineApprovedTime,
                    IsOnlineTransferred = receive.IsOnlineTransferred,
                    IsOnlineTransferredBy = receive.IsOnlineTransferredBy,
                    OnlineTransferredTime = receive.OnlineTransferredTime,
                    IsVoid = receive.IsVoid,
                    VoidBy = receive.VoidBy,
                    VoidTime = receive.VoidTime,
                };

                db.PrintingProductionFinishedGoodsStoreReceives.Attach(entity);
                db.PrintingProductionFinishedGoodsStoreReceives.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { receive }.ToDataSourceResult(request, ModelState));
        }

        //private void UpdateWIPNumber(WorkInProgress workInProgress)
        //{
        //    workInProgress.WorkInProgressNumber = workInProgress.WorkInProgressId.ToString("D10");
        //    db.WorkInProgress.Attach(workInProgress);
        //    db.Entry(workInProgress).State = EntityState.Modified;
        //    db.SaveChanges();
        //}
        public JsonResult SendForApproval(int id)
        {
            var record = db.PrintingProductionFinishedGoodsStoreReceives.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.PrintingProductionFinishedGoodsStoreReceiveItems.Count > 0)
            {
                #region validate price and tax

                foreach (var item in record.PrintingProductionFinishedGoodsStoreReceiveItems)
                {
                    if (item.UnitPrice <= 0 || item.GLTaxRateID == 0)
                    {
                        var formattedData3 = new
                        {
                            Message = "Please fill unit price and tax information for all items.",
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
                db.PrintingProductionFinishedGoodsStoreReceives.Attach(record);
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

            if (record.PrintingProductionFinishedGoodsStoreReceiveItems.Count== 0)
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
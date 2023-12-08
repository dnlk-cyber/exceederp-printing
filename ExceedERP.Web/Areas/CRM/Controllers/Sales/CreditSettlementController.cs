using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Data.Entity;
using ExceedERP.Web.Models;
using ExceedERP.Helpers.CRM;
using ExceedERP.Helpers.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Construction;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CreditSettlementUser, CRMRoles.CreditSettlementApprover)]
    public class CreditSettlementController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /CRM/CreditSettlement/
        public ActionResult Index()
        {

            ViewData["PaymentMethod"] = db.PaymentMethods.ToList();
            ViewData["OrganizationCustomer"] = db.OrganizationCustomers.ToList();
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name }).ToList();
            ViewData["Tax"] = db.GLTaxRates.ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();

            return View();
        }


        public ActionResult CreditSettlements_Read([DataSourceRequest]DataSourceRequest request)
        {

            List<CreditSettlement> voulist = new List<CreditSettlement>();

            try
            {

                //decimal GrandTotal = decimal.Zero;
                //decimal RemainingAmount = decimal.Zero;
                //decimal totalSettlementAmount = decimal.Zero;
                //List<CreditSettlement> CreditSettlements = new List<CreditSettlement>();


                var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
                var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));

                var creditSettlements = db.CreditSettlement.Where(s => bIds.Contains(s.BranchId)).ToList();

                try
                {
                    foreach (var vou in creditSettlements)
                    {
                        var CreditSales = db.CashSalesInvoices.Where(x => x.OrganizationCustomerId == vou.Customer && x.PaymentMethod == CRMConstants.CreditPaymentMethod).ToList();

                        decimal GrandTotal = decimal.Zero;
                        decimal RemainingAmount = decimal.Zero;
                        decimal totalSettlementAmount = decimal.Zero;

                        var SettlementLine = db.LineCreditSettlements.Where(x => x.CreditSettlementId == vou.CreditSettlementId).ToList();
                        totalSettlementAmount = SettlementLine.Any() ? SettlementLine.Sum(l => l.SettelmentAmount) : 0;

                        //foreach (var CreditSale in CreditSales)
                        //{

                        //    var lines =
                        //        db.LineItems.Where(
                        //            x => x.Voucher == CreditSale.CashSalesInvoiceID.ToString() && x.VoucherDefinitionID == "2").ToList();
                        //    //RemainingAmount = RemainingAmount + (lines.Sum(x => x.TotalAmount) - SettlementLine.Sum(x => x.SettelmentAmount));
                        //    GrandTotal = GrandTotal + lines.Sum(x => x.TotalAmount);
                        //}

                        if (CreditSales.Any())
                        {
                            GrandTotal = CreditSales.Sum(s => s.GrandTotal);
                            RemainingAmount = CreditSales.Sum(s => s.AmountDue);
                        }

                        //RemainingAmount = GrandTotal - totalSettlementAmount;
                        string SalesStatus = string.Empty;


                        voulist.Add(new CreditSettlement
                        {
                            CreditSettlementId = vou.CreditSettlementId,
                            Customer = vou.Customer,
                            Date = vou.Date,
                            GrandTotal = GrandTotal,
                            SettlementAmount = totalSettlementAmount,
                            Status = vou.Status,
                            Remaining = RemainingAmount,
                            Remark = vou.Remark,
                            CRMPeriodId = vou.CRMPeriodId,
                            ReceiptNumber = vou.ReceiptNumber,

                            BranchId = vou.BranchId,
                            RecieveMethod = vou.RecieveMethod,
                            ModifiedBy = vou.ModifiedBy,
                            LastModified = vou.LastModified,
                            DateCreated = vou.DateCreated,
                            CreatedBy = vou.CreatedBy,
                            IsOnlineApproved = vou.IsOnlineApproved,
                            IsOnlineTransferred = vou.IsOnlineTransferred,
                            IsOnlineTransferredBy = vou.IsOnlineTransferredBy,
                            OnlineApprovedBy = vou.OnlineApprovedBy,
                            OnlineApprovedTime = vou.OnlineApprovedTime,
                            OnlineTransferredTime = vou.OnlineTransferredTime,
                            OrgBranchName = vou.OrgBranchName,
                            SendForApprovalBy = vou.SendForApprovalBy,
                            IsSendForApproval = vou.IsSendForApproval,
                            IsVoid = vou.IsVoid,
                            SendForApprovalTime = vou.SendForApprovalTime
                        });
                    }
                }
                catch (Exception e)
                {

                }

            }
            catch (Exception e)
            {


            }
            return Json(voulist.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);


        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreditSettlements_Create([DataSourceRequest]DataSourceRequest request, CreditSettlement CreditSettlement)
        {
            //List<CreditSettlement> lit = db.CreditSettlement.Where(li => li.Customer == CreditSettlement.Customer).ToList();
            var unApprovedSettlementOfCustomer = 
                db.CreditSettlement.Where(cs => cs.Customer == CreditSettlement.Customer && !cs.IsOnlineApproved && cs.CreditSettlementId != CreditSettlement.CreditSettlementId).ToList();
            if (unApprovedSettlementOfCustomer.Any())
            {
                ModelState.AddModelError("", "This customer has Waiting Credit Settlement. It needs to be approved first");
            }

            //if (lit.Any())
            //{
            //    ModelState.AddModelError("CreditSettlementExist", " CreditSettlement Already Exist ! ");

            //}
            else
            {
                if (ModelState.IsValid)
                {

                    var entity = new CreditSettlement
                    {

                        Customer = CreditSettlement.Customer,
                        Date = CreditSettlement.Date,
                        IsVoid = CreditSettlement.IsVoid,
                        GrandTotal = CreditSettlement.GrandTotal,
                        BranchId = CreditSettlement.BranchId,
                        CRMPeriodId = CreditSettlement.CRMPeriodId,
                        ReceiptNumber = CreditSettlement.ReceiptNumber,

                        RecieveMethod = CreditSettlement.RecieveMethod,
                        Remark = CreditSettlement.Remark,
                        Status = OperationStatuses.New.ToString(),

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    db.CreditSettlement.Add(entity);
                    db.SaveChanges();
                    CreditSettlement.CreditSettlementId = entity.CreditSettlementId;
                }
            }
            return Json(new[] { CreditSettlement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreditSettlements_Destroy([DataSourceRequest]DataSourceRequest request, CreditSettlement CreditSettlement)
        {
            //List<LineCreditSettlement> line = db.LineCreditSettlements.Where(li => li.CreditSettlementId == CreditSettlement.CreditSettlementId).ToList();


            //foreach (var item in line)
            //{
            //    CashSalesInvoice invoice = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID == item.InvoiceId).FirstOrDefault();
            //    invoice.PaymentMethod = "CASH";

            //    db.CashSalesInvoices.Attach(invoice);
            //    db.Entry(invoice).State = EntityState.Modified;
            //    db.SaveChanges();

            //}
            //CreditSettlement.IsOnlineApproved = true;
            //CreditSettlement.OnlineApprovedBy = User.Identity.Name;
            //CreditSettlement.OnlineApprovedTime = DateTime.Now;

            db.CreditSettlement.Attach(CreditSettlement);
            db.CreditSettlement.Remove(CreditSettlement);
            db.SaveChanges();


            return Json(new[] { CreditSettlement }.ToDataSourceResult(request));
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

        #region Public Methods
        public JsonResult SendForApproval(int id)
        {
            var record = db.CreditSettlement.Find(id);
            if ((record != null && !record.IsSendForApproval && !record.IsOnlineApproved))
            {
                #region validate line items
                var lineItems = db.LineCreditSettlements.Where(li => li.CreditSettlementId == record.CreditSettlementId).ToList();
                if (!lineItems.Any())
                {
                    var d = new
                    {
                        Message = "No Line Items found. Please add Line Items first",
                        Status = "NOT_OK"
                    };
                    return Json(d, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (LineCreditSettlement lineItem in lineItems)
                    {
                        if ((lineItem.RemainingAmount) < 0)
                        {
                            return Json(new { Message = "Some line items have invalid remaining quantity.", Status = "NOT_OK" });
                        }
                    }
                }
                #endregion

                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.CreditSettlement.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Successfully Sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Credit Settlement Record not found!",
                    Status = "NOT_OK"
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
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = db.CreditSettlement.Find(myid);
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var formattedData = new
                {
                    Message = "The record is already transffered, it can't be retransferred.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

                #endregion

            }
            if (record != null && !record.IsOnlineTransferred)
            {

                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                var distributions = db.CreditSettlementDistribution.Where(d => d.CreditSettlementId == record.CreditSettlementId).ToList();
                foreach (var dist in distributions)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        message = $"{dist.AccountCode} Account has both debit and credit value.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        message = $"{dist.AccountCode} Negative values are not allowed.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                //var distribution = record.ArSalesOrderDistribution.ToList();
                if (distributions.Any())
                {
                    debit = distributions.Sum(x => x.Debit);
                    credit = distributions.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is not balanced.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                if (debit <= 0)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit should be positive number.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region send to GL

                string glyear = "";
                string glperiod = "";
                #region get period
                var period = db.GLPeriods.Where(x => x.DateFrom <= record.Date.Date)
                    .Where(x => x.DateTo >= record.Date.Date).ToList();
                if (period.Any())
                {
                    if (period.Count() == 1)
                    {
                        var myPeriod = period.FirstOrDefault();
                        if (myPeriod != null)
                        {
                            glperiod = myPeriod.Name;
                            glyear = myPeriod.GlFiscalYear.Name.ToString();
                            if (myPeriod.Status == FiscalYearStatus.Close)
                            {
                                var formattedData = new
                                {
                                    Message = "The period " + glperiod +
                                              " is closed,the record cannot be transfered to closed period.",
                                    Status = "NOT_OK"
                                };
                                return Json(formattedData, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var formattedData = new
                            {
                                Message = "Period not found.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }

                    else
                    {
                        var formattedData = new
                        {
                            Message = "There are multiple periods defined that contain the Credit Settlement date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    var formattedData = new
                    {
                        Message = "There are no periods defined that contain the credit settlement date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion

                if (distributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "CRM Credit Settlement",
                        Description = !String.IsNullOrEmpty(description) ? description : "CRM Credit Settlement",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "CRM Credit Settlement",
                        SourcesName = "CRM",
                        JournalNumber = "CRMCrSettl." + record.CreditSettlementId.ToString(),
                        SourceId = record.CreditSettlementId.ToString(),
                        Source = JournalSources.Sales.ToString(),
                        EffectiveDate = record.Date.Date,
                        OrgBranchName = record.OrgBranchName,
                        BranchId = record.BranchId,
                        Status = "Approved",
                        Remark = record.Remark,
                        PaymentReference = "",
                        ReferenceDoc = "",
                        InvoiceReferenceDoc = "",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    db.GLRecordJournals.Add(journal);
                    db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in distributions)
                    {
                        #region debit and credit

                        journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit,
                            AccountDesc = i.AccountDesc,
                            AdditionalDescription = i.AdditionalDescription,
                        };
                        db.GLRecordJournalDetails.Add(journalDetail);
                        db.SaveChanges();
                        #endregion
                    }
                    #region update record
                    record.IsOnlineTransferred = true;
                    record.OnlineTransferredTime = DateTime.Now;
                    record.IsOnlineTransferredBy = User.Identity.Name;

                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion

                    #region split journal
                    var journaldetails = db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
                    #endregion
                    #region bank reconciliation
                    //CRMSalesHelper.SendCreditSettlementToBankReconciliation(record, User.Identity.Name);
                    #endregion
                    var formattedData = new
                    {
                        Message = "Successfully transfered.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);

                }
                #endregion

            }
            else
            {
                var formattedData = new
                {
                    Message = "You cannot transfer this transaction, it is already transfered.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            return null;
        }
        public JsonResult RegenerateDistribution(int id)
        {
            var record = db.CreditSettlement.Find(id);
            if ((record != null && !record.IsOnlineTransferred))
            {
                //new CRMPaymentDistibutionHelper().UpdateShipmentDistribution(record);
                new CRMPaymentDistibutionHelper().UpdateCreditSettlementDistribution(record);
                return Json(new { Message = $"Distribution Generated for Credit Settlement number {record.CreditSettlementId}.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Credit Settlement not found!",
                    Status = "NOT_OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "The record is already transfered you cannot regenerate distribution.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
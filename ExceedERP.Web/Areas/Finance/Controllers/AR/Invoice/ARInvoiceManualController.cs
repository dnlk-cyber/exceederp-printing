using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Invoice
{
    public class ArInvoiceManualController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();



        [AuthorizeRoles(FinanceConstants.AccountReceivableView, FinanceConstants.AccountReceivableApprover, FinanceConstants.AccountReceivableInvoice)]
        public ActionResult Index()
        {
            PopulateViewData();

            return View();
        }

        private void PopulateViewData()
        {
            var terms = _db.PaymentTerms.ToList();

            ViewData["terms"] = terms.Select(p => new PaymentTerms
            {
                PaymentTermsDays = p.PaymentTermsDays
            });

            var customers = _db.ARCustomerProfiles.Where(x => x.IsActive).ToList();

            ViewData["customers"] = customers.Select(c => new ArCustomerProfile
            {
                CustomerId = c.CustomerId,
                CompanyName = c.CompanyName
            });


            var tax = _db.GLTaxRates.Where(x => x.ApplyToAr).ToList();

            ViewData["taxes"] = tax.Select(p => new
            {
                Name = p.TaxName,
                GLTaxRateID = p.GLTaxRateID
            });


            var productorService = _db.ARSetupProductServiceAccounts.ToList();

            ViewData["productorServices"] = productorService
                .Select(e => new ARSetupProductServiceAccount
                {
                    Product = e.Product
                })
                .OrderBy(e => e.Product);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView, FinanceConstants.AccountReceivableApprover, FinanceConstants.AccountReceivableInvoice)]
        public ActionResult ARInvoices_Read([DataSourceRequest]DataSourceRequest request, bool isNotTransffered)
        {
            IQueryable<ArInvoice> arinvoices = _db.ARInvoices.Where(x=>x.IsManual);
            if (isNotTransffered)
            {
                List<int> transfferableList = FinanceTransferHelper.GetArInvoiceManualTranferableList();
                arinvoices =
                    arinvoices.Where(x => transfferableList.Contains(x.ArInvoiceId));
            }
            DataSourceResult result = arinvoices.ToDataSourceResult(request, aRInvoice => new
            {
                ArInvoiceId = aRInvoice.ArInvoiceId,
                BranchId = aRInvoice.BranchId,
                BusinessUnitId = aRInvoice.BusinessUnitId,
                CustomerName = aRInvoice.CustomerName,
                CustomerId = aRInvoice.CustomerId,
                TransactionClass = aRInvoice.TransactionClass,
                Date = aRInvoice.Date,
                PaymentTerm = aRInvoice.PaymentTerm,
                DueDate = aRInvoice.DueDate,
                LineAmount = Math.Round(aRInvoice.LineAmount,2),
                TaxAmount =  Math.Round(aRInvoice.TaxAmount, 2),
                TotalAmount = Math.Round(aRInvoice.TotalAmount, 2),
                AmountDue =  Math.Round(aRInvoice.AmountDue, 2),
                InvoiceStatus = aRInvoice.InvoiceStatus,
                IsManual = aRInvoice.IsManual,
                Reference = aRInvoice.Reference,
               
                ArSalesOrderId = aRInvoice.ArSalesOrderId,
                SourceInvoiceId = aRInvoice.SourceInvoiceId,
                IsSendForApproval = aRInvoice.IsSendForApproval,
                IsOnlineTransferred = aRInvoice.IsOnlineTransferred,
                IsOnlineTransferredBy = aRInvoice.IsOnlineTransferredBy,
                OnlineTransferredTime = aRInvoice.OnlineTransferredTime,
                Status = aRInvoice.Status,
                IsOnlineApproved = aRInvoice.IsOnlineApproved,
                OnlineApprovedBy = aRInvoice.OnlineApprovedBy,
                OnlineApprovedTime = aRInvoice.OnlineApprovedTime,
                IsVoid = aRInvoice.IsVoid,
                VoidBy = aRInvoice.VoidBy,
                VoidTime = aRInvoice.VoidTime,
                OrgBranchName = aRInvoice.OrgBranchName,
                DateCreated = aRInvoice.DateCreated,
                LastModified = aRInvoice.LastModified,
                CreatedBy = aRInvoice.CreatedBy,
                ModifiedBy = aRInvoice.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoices_Create([DataSourceRequest]DataSourceRequest request, ArInvoice aRInvoice)
        {
            #region validation
            string customername = "";

            if (String.IsNullOrWhiteSpace(aRInvoice.CustomerId))
                ModelState.AddModelError("exisit", "Customer ID can't be empty.");

            if (!String.IsNullOrWhiteSpace(aRInvoice.CustomerId))
                customername = _db.ARCustomerProfiles.Where(c => c.CustomerId == aRInvoice.CustomerId)
                    .Select(x => x.CompanyName).FirstOrDefault();
            if (String.IsNullOrWhiteSpace(aRInvoice.PaymentTerm))
                ModelState.AddModelError("exisit", "Payment Term can't be empty.");

            if (!String.IsNullOrWhiteSpace(aRInvoice.PaymentTerm))
                aRInvoice.DueDate = aRInvoice.Date.AddDays(int.Parse(aRInvoice.PaymentTerm));

            if (!String.IsNullOrEmpty(aRInvoice.Reference))
            {
                var exists = _db.ARInvoices
                    .Where(x => x.Reference == aRInvoice.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference", $"{aRInvoice.Reference}  already exists in the system.");
            }

            if(aRInvoice.TransactionClass!=TransactionClass.Invoice&& aRInvoice.SourceInvoiceId==0)
                ModelState.AddModelError("sourceinvoice", $"Pease choose invoice for {aRInvoice.TransactionClass}");

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ArInvoice
                {
                    BranchId = aRInvoice.BranchId,
                    BusinessUnitId = aRInvoice.BusinessUnitId,
                    CustomerName = customername,
                    CustomerId = aRInvoice.CustomerId,
                    TransactionClass = aRInvoice.TransactionClass,
                    Date = aRInvoice.Date.Date,
                    PaymentTerm = aRInvoice.PaymentTerm,
                    DueDate = aRInvoice.DueDate.Date,
                    LineAmount = aRInvoice.LineAmount,
                    TaxAmount = aRInvoice.TaxAmount,
                    TotalAmount = aRInvoice.TotalAmount,
                    AmountDue = aRInvoice.AmountDue,
                    InvoiceStatus = aRInvoice.InvoiceStatus,
                    IsManual = true,
                    Reference = aRInvoice.Reference,
                  
                    ArSalesOrderId = aRInvoice.ArSalesOrderId,
                    SourceInvoiceId = aRInvoice.SourceInvoiceId,
                    IsSendForApproval = aRInvoice.IsSendForApproval,
                    IsOnlineTransferred = aRInvoice.IsOnlineTransferred,
                    IsOnlineTransferredBy = aRInvoice.IsOnlineTransferredBy,
                    OnlineTransferredTime = aRInvoice.OnlineTransferredTime,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    IsOnlineApproved = aRInvoice.IsOnlineApproved,
                    OnlineApprovedBy = aRInvoice.OnlineApprovedBy,
                    OnlineApprovedTime = aRInvoice.OnlineApprovedTime,
                    IsVoid = aRInvoice.IsVoid,
                    VoidBy = aRInvoice.VoidBy,
                    VoidTime = aRInvoice.VoidTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = aRInvoice.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRInvoice.ModifiedBy
                };

                _db.ARInvoices.Add(entity);
                _db.SaveChanges();
                aRInvoice.ArInvoiceId = entity.ArInvoiceId;

                if(entity.TransactionClass!=TransactionClass.Invoice)
                 CopyFromsourceInvoice(entity);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR invoice manual", "Success", User.Identity.Name, $"created invoice {entity.ArInvoiceId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRInvoice }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoices_Update([DataSourceRequest]DataSourceRequest request, ArInvoice aRInvoice)
        {

            #region validated record

            string customername = "";

            if (String.IsNullOrWhiteSpace(aRInvoice.CustomerId))
                ModelState.AddModelError("exisit", "Customer ID can't be empty.");

            if (!String.IsNullOrWhiteSpace(aRInvoice.CustomerId))
                customername = _db.ARCustomerProfiles.Where(c => c.CustomerId == aRInvoice.CustomerId)
                    .Select(x => x.CompanyName).FirstOrDefault();
            if (String.IsNullOrWhiteSpace(aRInvoice.PaymentTerm))
                ModelState.AddModelError("exisit", "Payment Term can't be empty.");

            if (!String.IsNullOrWhiteSpace(aRInvoice.PaymentTerm))
                aRInvoice.DueDate = aRInvoice.Date.AddDays(int.Parse(aRInvoice.PaymentTerm));

            if (!String.IsNullOrEmpty(aRInvoice.Reference))
            {
                var exists = _db.ARInvoices
                    .Where(x => x.ArInvoiceId != aRInvoice.ArInvoiceId)
                    .Where(x => x.Reference == aRInvoice.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference", $"{aRInvoice.Reference}  already exists in the system.");
            }

            var parent = _db.ARInvoices.Find(aRInvoice.ArInvoiceId);
            if (parent != null)
            {
                if (parent.IsOnlineApproved)
                {
                    ModelState.AddModelError("approved", "You cannot manipulate this record");
                }

                #region detach local version
                var local = _db.Set<ArInvoice>()
                    .Local
                    .FirstOrDefault(f => f.ArInvoiceId == parent.ArInvoiceId);
                if (local != null)
                {
                    _db.Entry(local).State = EntityState.Detached;
                }
                #endregion

            }


            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new ArInvoice
                    {
                        ArInvoiceId = aRInvoice.ArInvoiceId,
                        BranchId = aRInvoice.BranchId,
                        BusinessUnitId = aRInvoice.BusinessUnitId,
                        CustomerName = customername,
                        CustomerId = aRInvoice.CustomerId,
                        TransactionClass = aRInvoice.TransactionClass,
                        Date = aRInvoice.Date,
                        PaymentTerm = aRInvoice.PaymentTerm,
                        DueDate = aRInvoice.DueDate,
                        LineAmount = aRInvoice.LineAmount,
                        TaxAmount = aRInvoice.TaxAmount,
                        TotalAmount = aRInvoice.TotalAmount,
                        AmountDue = aRInvoice.AmountDue,
                        InvoiceStatus = aRInvoice.InvoiceStatus,
                        IsManual = aRInvoice.IsManual,
                        Reference = aRInvoice.Reference,
                        
                        ArSalesOrderId = aRInvoice.ArSalesOrderId,
                        SourceInvoiceId = aRInvoice.SourceInvoiceId,
                        IsSendForApproval = aRInvoice.IsSendForApproval,
                        IsOnlineTransferred = aRInvoice.IsOnlineTransferred,
                        IsOnlineTransferredBy = aRInvoice.IsOnlineTransferredBy,
                        OnlineTransferredTime = aRInvoice.OnlineTransferredTime,
                        Status = aRInvoice.Status,
                        IsOnlineApproved = aRInvoice.IsOnlineApproved,
                        OnlineApprovedBy = aRInvoice.OnlineApprovedBy,
                        OnlineApprovedTime = aRInvoice.OnlineApprovedTime,
                        IsVoid = aRInvoice.IsVoid,
                        VoidBy = aRInvoice.VoidBy,
                        VoidTime = aRInvoice.VoidTime,
                        OrgBranchName = aRInvoice.OrgBranchName,
                        DateCreated = aRInvoice.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aRInvoice.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ARInvoices.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                if (entity.TransactionClass != TransactionClass.Invoice)
                    CopyFromsourceInvoice(entity);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR invoice manual", "Success", User.Identity.Name, $"updated invoice {entity.ArInvoiceId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { aRInvoice }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoices_Destroy([DataSourceRequest]DataSourceRequest request, ArInvoice aRInvoice)
        {
            if (ModelState.IsValid)
            {
                #region validated record

                var parent = _db.ARInvoices.Find(aRInvoice.ArInvoiceId);
                if (parent != null)
                {
                    if (parent.IsOnlineApproved)
                    {
                        ModelState.AddModelError("approved", "You cannot manipulate this record");
                    }

                    #region detach local version
                    var local = _db.Set<ArInvoice>()
                        .Local
                        .FirstOrDefault(f => f.ArInvoiceId == parent.ArInvoiceId);
                    if (local != null)
                    {
                        _db.Entry(local).State = EntityState.Detached;
                    }
                    #endregion

                }


                #endregion

                var entity = new ArInvoice
                    {
                        ArInvoiceId = aRInvoice.ArInvoiceId,
                        BranchId = aRInvoice.BranchId,
                        BusinessUnitId = aRInvoice.BusinessUnitId,
                        CustomerName = aRInvoice.CustomerName,
                        CustomerId = aRInvoice.CustomerId,
                        TransactionClass = aRInvoice.TransactionClass,
                        Date = aRInvoice.Date,
                        PaymentTerm = aRInvoice.PaymentTerm,
                        DueDate = aRInvoice.DueDate,
                        LineAmount = aRInvoice.LineAmount,
                        TaxAmount = aRInvoice.TaxAmount,
                        TotalAmount = aRInvoice.TotalAmount,
                        AmountDue = aRInvoice.AmountDue,
                        InvoiceStatus = aRInvoice.InvoiceStatus,
                    
                        IsSendForApproval = aRInvoice.IsSendForApproval,
                        IsOnlineTransferred = aRInvoice.IsOnlineTransferred,
                        IsOnlineTransferredBy = aRInvoice.IsOnlineTransferredBy,
                        OnlineTransferredTime = aRInvoice.OnlineTransferredTime,
                        Status = aRInvoice.Status,
                        IsManual = aRInvoice.IsManual,
                        ArSalesOrderId = aRInvoice.ArSalesOrderId,
                        IsOnlineApproved = aRInvoice.IsOnlineApproved,
                        OnlineApprovedBy = aRInvoice.OnlineApprovedBy,
                        OnlineApprovedTime = aRInvoice.OnlineApprovedTime,
                        IsVoid = aRInvoice.IsVoid,
                        VoidBy = aRInvoice.VoidBy,
                        VoidTime = aRInvoice.VoidTime,
                        OrgBranchName = aRInvoice.OrgBranchName,
                        DateCreated = aRInvoice.DateCreated,
                        LastModified = aRInvoice.LastModified,
                        CreatedBy = aRInvoice.CreatedBy,
                        ModifiedBy = aRInvoice.ModifiedBy
                    };

                    _db.ARInvoices.Attach(entity);
                    _db.ARInvoices.Remove(entity);
                    _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Manual AR Invoice" + aRInvoice.ArInvoiceId + " " + aRInvoice.Reference, Modules.Finance);
                #endregion

            }

            return Json(new[] { aRInvoice }.ToDataSourceResult(request, ModelState));
        }

       
        [AuthorizeRoles(FinanceConstants.AccountReceivableInvoice)]
        public JsonResult SendforApproval(string id)
        {
            int myid = int.Parse(id);
            var record = _db.ARInvoices.Find(myid);
            #region check segment length validation

            var distributions = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == myid).ToList();
            AccountHelper accountHelper = new AccountHelper();
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
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "You cannot manipulate this record.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "This record is already sent for approval.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            decimal sumTotalinvoiceLines = 0;
            decimal sumTotalDisDebit = 0;
            decimal sumTotalDisCredit = 0;
            try
            {
                sumTotalinvoiceLines = record.ArInvoiceLines.Sum(x => x.TotalAmount);

            }
            catch (Exception e)
            {
                var formattedData = new
                {
                    Message = "Please add invoice lines and try again",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            try
            {
                sumTotalDisDebit = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == myid).Sum(x => x.Debit);
                sumTotalDisCredit = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == myid).Sum(x => x.Credit);
            }
            catch (Exception e)
            {
                var formattedData = new
                {
                    Message = "Please add distribution and try again",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (sumTotalDisDebit != sumTotalDisCredit)
            {
                var formattedData = new
                {
                    Message = "Total Debit and Total Credit is not equal.Please correct it and try again",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            //if (sumTotalDisDebit != sumTotalinvoiceLines)
            //{
            //    var formattedData = new
            //    {
            //        Message = "Total of distribution and total of invoice lines is not equal.Please correct it and try again",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}
            if (!record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.ARInvoices.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableInvoice)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.ARInvoices.Find(myid);
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                    .Where(x => x.Source == JournalSources.AR_Invoice.ToString())
                    .Where(x => x.SourceId == record.ArInvoiceId.ToString())
                    .FirstOrDefault();
                if (exist != null)
                {
                    var formattedData = new
                    {
                        Message = "The record is already transffered, it can't be retransferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.ArInvoiceDistribution)
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
                var distribution = record.ArInvoiceDistribution.ToList();
                if (distribution.Any())
                {
                    debit = distribution.Sum(x => x.Debit);
                    credit = distribution.Sum(x => x.Credit);
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
                if (debit == 0)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is zero.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region send to GL

                string glyear = "";
                string glperiod = "";
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.Date.Date)
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
                            Message = "There are multiple periods defined that contain the Invoice date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    var formattedData = new
                    {
                        Message = "There are no periods defined that contain the Invoice date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
               
                if (record.ArInvoiceDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "AR Invoice",
                        Description = !String.IsNullOrEmpty(description) ? description : "AR Invoice",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "AR Invoice",
                        SourcesName = "AR",
                        JournalNumber = "ARINV." + record.ArInvoiceId.ToString(),
                        SourceId = record.ArInvoiceId.ToString(),
                        Source = JournalSources.AR_Invoice.ToString(),
                        EffectiveDate = record.Date.Date,
                        OrgBranchName = record.OrgBranchName,
                        BranchId=record.BranchId,
                        Status = "Approved",
                        Remark = record.Reference,
                        PaymentReference = "",
                        ReferenceDoc = record.Reference,
                        InvoiceReferenceDoc = "",
                        DebitSum = debit,
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in record.ArInvoiceDistribution)
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
                            AdditionalDescription = i.AdditionalDescription,
                        };
                        _db.GLRecordJournalDetails.Add(journalDetail);
                        _db.SaveChanges();
                        #endregion
                    }
                    #region update record
                    record.IsOnlineTransferred = true;
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                    #endregion

                    #region split journal
                    var journaldetails = _db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
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

        [AuthorizeRoles(FinanceConstants.AccountReceivableVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.ARInvoices.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.ArInvoiceLines.Count > 0)
            {
                #region validate invoice

                var payments = _db.ARStandardCollectionLines.Where(x => x.InvoiceNumber == record.ArInvoiceId.ToString())
                    .ToList();
                if (payments.Any())
                {
                    var formattedData1 = new
                    {
                        Message = $"The invoice number {record.ArInvoiceId} is already applied in payment \n hence cannot be void.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ARInvoices.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "manual receivable invoice", "Success", User.Identity.Name, $"void receivable invoice {id} ", Modules.Finance);
                #endregion

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

            if (record.ArInvoiceLines.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add invoice lines and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is already void.You can not void again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "The record is transferred to GL.You can not void .",
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

        private void CopyFromsourceInvoice(ArInvoice invoice)
        {
            if (invoice.SourceInvoiceId != null)
            {
                var items = _db.ARInvoiceLines.Where(x => x.ArInvoiceId == invoice.ArInvoiceId).ToList();
                _db.ARInvoiceLines.RemoveRange(items);
                _db.SaveChanges();

                var distributions = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == invoice.ArInvoiceId).ToList();
                _db.ARInvoiceDistributions.RemoveRange(distributions);
                _db.SaveChanges();

                int sourceInvoiceId = (int)invoice.SourceInvoiceId;
                var sourceItems = _db.ARInvoiceLines.Where(x => x.ArInvoiceId == sourceInvoiceId).ToList();
                var sourceDistributions = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == sourceInvoiceId).ToList();

                #region add items

                foreach (var item in sourceItems)
                {
                    var entity = new ArInvoiceLines
                    {
                        Items = item.Items,
                        ArInvoiceId = invoice.ArInvoiceId,
                        Description = item.Description,
                        AccountCode = item.AccountCode,
                        AccountDesc = item.AccountDesc,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        SubTotal = item.Quantity * item.UnitPrice,
                        GLTaxRateID = item.GLTaxRateID,
                        TaxAmount = item.TaxAmount,
                        Discount = item.Discount,
                        TotalAmount = item.TotalAmount,
                        OrgBranchName = item.OrgBranchName,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    _db.ARInvoiceLines.Add(entity);
                    _db.SaveChanges();
                }

                #endregion
                #region add distribution

                foreach (var arInvoiceDistribution in sourceDistributions)
                {
                    var entity = new ArInvoiceDistribution
                    {
                        ArInvoiceId = invoice.ArInvoiceId,
                        AccountCode = arInvoiceDistribution.AccountCode,
                        AccountDesc = arInvoiceDistribution.AccountDesc,
                        Debit = arInvoiceDistribution.Debit,
                        Credit = arInvoiceDistribution.Credit,
                        AdditionalDescription = arInvoiceDistribution.AdditionalDescription,
                        OrgBranchName = arInvoiceDistribution.OrgBranchName,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    _db.ARInvoiceDistributions.Add(entity);
                    _db.SaveChanges();
                }

                #endregion
            }

        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
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

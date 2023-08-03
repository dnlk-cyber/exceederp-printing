using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class ApInvoicesManualController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();



        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayableInvoice)]
        public ActionResult Index()
        {
            PopulateViewData();


            return View();
        }

        private void PopulateViewData()
        {

            var terms = _db.PaymentTerms;

            ViewData["terms"] = terms.Select(e => new
            {
                PaymentTermsDays = e.PaymentTermsDays
            }).AsQueryable();

            var suppliers = _db.ProcurementSuppliers.ToList();

            ViewData["suppliers"] = suppliers.Select(c => new
            {
                SupplierId = c.SupplierId,
                Name = c.Name
            }).AsQueryable();

            var tax = _db.GLTaxRates.Where(x => x.ApplyToAp).ToList();

            ViewData["taxes"] = tax
                .Select(e => new
                {
                    TaxName = e.TaxName
                })
                .AsQueryable();

            var productorService = _db.ApSetupItems.ToList();

            ViewData["productorServices"] = productorService
                .Select(e => new
                {
                    Name = e.Name
                })
                .AsQueryable();


            var witholdings = _db.GLTaxRates.Where(x => x.ApplyToWithholding).ToList();

            ViewData["witholdingType"] = witholdings
                .Select(e => new
                {
                    TaxName = e.TaxName
                })
                .AsQueryable();

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

        }


        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayableInvoice)]
        public ActionResult APPaybleInvoices_Read([DataSourceRequest]DataSourceRequest request, bool isNotTransffered)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<ApPaybleInvoices> appaybleinvoices = _db.APPaybleInvoices
                .Where(x=>x.IsManual)
                .Where(x => mybraches.Contains(x.OrgBranchName));

            if (isNotTransffered)
            {
                List<int> transfferableList = FinanceTransferHelper.GetApManualInvoiceTranferableList();
                appaybleinvoices =
                    appaybleinvoices.Where(x => transfferableList.Contains(x.ApPaybleInvoicesId));
            }
            DataSourceResult result = appaybleinvoices.ToDataSourceResult(request, invoice => new
            {
                BranchId = invoice.BranchId,
                BusinessUnitId = invoice.BusinessUnitId,
                ApPaybleInvoicesId = invoice.ApPaybleInvoicesId,
                IsManual = invoice.IsManual,
                SupplierName = invoice.SupplierName,
                SupplierId = invoice.SupplierId,
                EmployeeId = invoice.EmployeeId,
                Type = invoice.Type,
                PrNo = invoice.PrNo,
                PoNum = invoice.PoNum,
                PurposeOfPayment = invoice.PurposeOfPayment,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceNumber = invoice.InvoiceNumber,
                PaymentTerms = invoice.PaymentTerms,
                InvoiceAmount = invoice.InvoiceAmount,
                AdvanceAmount = invoice.AdvanceAmount,
                GLTaxRateID = invoice.GLTaxRateID,
                WithholdTax = invoice.WithholdTax,
                WithholdingType = invoice.WithholdingType,
                TaxAmount = invoice.TaxAmount,
                WithholdingAmount = invoice.WithholdingAmount,
                Subtotal = invoice.Subtotal,
                WithholdTaxAmount = invoice.WithholdTaxAmount,
                NetPayable = invoice.NetPayable,
                AmountDue = invoice.AmountDue,
                InvoiceStatus = invoice.InvoiceStatus,
                ForBranchId = invoice.ForBranchId,

                Status = invoice.Status,
                Source = invoice.Source,
                Grn = invoice.Grn,

                IsSendForApproval = invoice.IsSendForApproval,
                SendForApprovalBy = invoice.SendForApprovalBy,
                SendForApprovalTime = invoice.SendForApprovalTime,

                IsOnlineApproved = invoice.IsOnlineApproved,
                OnlineApprovedBy = invoice.OnlineApprovedBy,
                OnlineApprovedTime = invoice.OnlineApprovedTime,


                IsOnlineTransferred = invoice.IsOnlineTransferred,
                IsOnlineTransferredBy = invoice.IsOnlineTransferredBy,
                OnlineTransferredTime = invoice.OnlineTransferredTime,

                IsVoid = invoice.IsVoid,
                VoidBy = invoice.VoidBy,
                VoidTime = invoice.VoidTime,
                OrgBranchName = invoice.OrgBranchName,
                DateCreated = invoice.DateCreated,
                LastModified = invoice.LastModified,
                CreatedBy = invoice.CreatedBy,
                ModifiedBy = invoice.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaybleInvoices_Create([DataSourceRequest]DataSourceRequest request, ApPaybleInvoices invoice)
        {
            #region validation

            if(String.IsNullOrWhiteSpace(invoice.SupplierId))
                ModelState.AddModelError("supplier","Please select supplier.");

            if (invoice.EmployeeId==0)
                ModelState.AddModelError("purchaser", "Please select purchaser.");

            if (invoice.PaymentTerms == 0)
                ModelState.AddModelError("purchaser", "Please select payment term.");

            #endregion
            #region data

            if (!String.IsNullOrWhiteSpace(invoice.SupplierId))
            {
                var supplier =
                    _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == invoice.SupplierId);
                if (supplier != null) invoice.SupplierName = supplier.Name;
            }

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ApPaybleInvoices
                {
                    BranchId = invoice.BranchId,
                    BusinessUnitId = invoice.BusinessUnitId,
                    IsManual = true,
                    SupplierName = invoice.SupplierName,
                    SupplierId = invoice.SupplierId,
                    EmployeeId = invoice.EmployeeId,
                    Type = invoice.Type,
                    PrNo = invoice.PrNo,
                    PoNum = invoice.PoNum,
                    PurposeOfPayment = invoice.PurposeOfPayment,
                    InvoiceDate = invoice.InvoiceDate.Date,
                    InvoiceNumber = invoice.InvoiceNumber,
                    PaymentTerms = invoice.PaymentTerms,
                    InvoiceAmount = invoice.InvoiceAmount,
                    AdvanceAmount = invoice.AdvanceAmount,
                    GLTaxRateID = invoice.GLTaxRateID,
                    WithholdTax = invoice.WithholdTax,
                    WithholdingType = invoice.WithholdingType,
                    TaxAmount = invoice.TaxAmount,
                    WithholdingAmount = invoice.WithholdingAmount,
                    Subtotal = invoice.Subtotal,
                    WithholdTaxAmount = invoice.WithholdTaxAmount,
                    NetPayable = invoice.NetPayable,
                    AmountDue = invoice.AmountDue,
                    InvoiceStatus = invoice.InvoiceStatus,
                    ForBranchId = invoice.ForBranchId,

                    IsSendForApproval = invoice.IsSendForApproval,
                    IsOnlineTransferred = invoice.IsOnlineTransferred,
                    IsOnlineTransferredBy = invoice.IsOnlineTransferredBy,
                    OnlineTransferredTime = invoice.OnlineTransferredTime,
                    Status = invoice.Status,
                    Source = GrnSource.ManualInvoice,
                    Grn = invoice.Grn,
                    IsOnlineApproved = invoice.IsOnlineApproved,
                    OnlineApprovedBy = invoice.OnlineApprovedBy,
                    OnlineApprovedTime = invoice.OnlineApprovedTime,
                    IsVoid = invoice.IsVoid,
                    VoidBy = invoice.VoidBy,
                    VoidTime = invoice.VoidTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = invoice.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = invoice.ModifiedBy
                };

                _db.APPaybleInvoices.Add(entity);
                _db.SaveChanges();
                invoice.ApPaybleInvoicesId = entity.ApPaybleInvoicesId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP Invoice manual", "Success", User.Identity.Name, $"created invoice {entity.ApPaybleInvoicesId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { invoice }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaybleInvoices_Update([DataSourceRequest]DataSourceRequest request, ApPaybleInvoices invoice)
        {
            #region validation

            if (String.IsNullOrWhiteSpace(invoice.SupplierId))
                ModelState.AddModelError("supplier", "Please select supplier.");

            if (invoice.EmployeeId == 0)
                ModelState.AddModelError("purchaser", "Please select purchaser.");

            if (invoice.PaymentTerms == 0)
                ModelState.AddModelError("purchaser", "Please select payment term.");

            #endregion
            #region data

            if (!String.IsNullOrWhiteSpace(invoice.SupplierId))
            {
                var supplier =
                    _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == invoice.SupplierId);
                if (supplier != null) invoice.SupplierName = supplier.Name;
            }

            #endregion
            #region validated record

            var parent = _db.APPaybleInvoices.Find(invoice.ApPaybleInvoicesId);
            if (parent != null)
            {
                if (parent.IsOnlineApproved)
                    ModelState.AddModelError("approved", "You cannot manipulate this record.");

                #region detach local version
                var local = _db.Set<ApPaybleInvoices>()
                    .Local
                    .FirstOrDefault(f => f.ApPaybleInvoicesId == parent.ApPaybleInvoicesId);
                if (local != null)
                {
                    _db.Entry(local).State = EntityState.Detached;
                }
                #endregion

            }


            #endregion
            if (ModelState.IsValid)
            {
                  var entity = new ApPaybleInvoices
                    {
                      BranchId = invoice.BranchId,
                      BusinessUnitId = invoice.BusinessUnitId,
                      ApPaybleInvoicesId = invoice.ApPaybleInvoicesId,
                        IsManual = invoice.IsManual,
                        SupplierName = invoice.SupplierName,
                        SupplierId = invoice.SupplierId,
                        EmployeeId = invoice.EmployeeId,
                        Type = invoice.Type,
                        PrNo = invoice.PrNo,
                        PoNum = invoice.PoNum,
                        PurposeOfPayment = invoice.PurposeOfPayment,
                        InvoiceDate = invoice.InvoiceDate,
                        InvoiceNumber = invoice.InvoiceNumber,
                        PaymentTerms = invoice.PaymentTerms,
                        InvoiceAmount = invoice.InvoiceAmount,
                        AdvanceAmount = invoice.AdvanceAmount,
                        GLTaxRateID = invoice.GLTaxRateID,
                        WithholdTax = invoice.WithholdTax,
                        WithholdingType = invoice.WithholdingType,
                        TaxAmount = invoice.TaxAmount,
                        WithholdingAmount = invoice.WithholdingAmount,
                        Subtotal = invoice.Subtotal,
                        WithholdTaxAmount = invoice.WithholdTaxAmount,
                        NetPayable = invoice.NetPayable,
                        AmountDue = invoice.AmountDue,
                        InvoiceStatus = invoice.InvoiceStatus,
                      ForBranchId = invoice.ForBranchId,

                      IsSendForApproval = invoice.IsSendForApproval,
                        IsOnlineTransferred = invoice.IsOnlineTransferred,
                        IsOnlineTransferredBy = invoice.IsOnlineTransferredBy,
                        OnlineTransferredTime = invoice.OnlineTransferredTime,
                        Status = invoice.Status,
                        Source = invoice.Source,
                        IsOnlineApproved = invoice.IsOnlineApproved,
                        OnlineApprovedBy = invoice.OnlineApprovedBy,
                        OnlineApprovedTime = invoice.OnlineApprovedTime,
                        IsVoid = invoice.IsVoid,
                        VoidBy = invoice.VoidBy,
                        VoidTime = invoice.VoidTime,
                        OrgBranchName = invoice.OrgBranchName,
                        DateCreated = invoice.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = invoice.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                   
                    _db.APPaybleInvoices.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP Invoice manual", "Success", User.Identity.Name, $"updated invoice {entity.ApPaybleInvoicesId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { invoice }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaybleInvoices_Destroy([DataSourceRequest]DataSourceRequest request, ApPaybleInvoices invoice)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                var parent = _db.APPaybleInvoices.Find(invoice.ApPaybleInvoicesId);
                if (parent != null)
                {
                    if (parent.IsOnlineApproved)
                    {
                        isvalid = false;
                        //ViewBag.Note = "You cannot manipulate this record";
                        //return View("Error");
                    }

                    #region detach local version
                    var local = _db.Set<ApPaybleInvoices>()
                        .Local
                        .FirstOrDefault(f => f.ApPaybleInvoicesId == parent.ApPaybleInvoicesId);
                    if (local != null)
                    {
                        _db.Entry(local).State = EntityState.Detached;
                    }
                    #endregion

                }


                #endregion

                if (isvalid)
                {
                    var entity = new ApPaybleInvoices
                    {
                        ApPaybleInvoicesId = invoice.ApPaybleInvoicesId,
                        BranchId = invoice.BranchId,
                        BusinessUnitId = invoice.BusinessUnitId,
                        IsManual = invoice.IsManual,
                        SupplierName = invoice.SupplierName,
                        SupplierId = invoice.SupplierId,
                        EmployeeId = invoice.EmployeeId,
                        Type = invoice.Type,
                        PrNo = invoice.PrNo,
                        PoNum = invoice.PoNum,
                        PurposeOfPayment = invoice.PurposeOfPayment,
                        InvoiceDate = invoice.InvoiceDate,
                        InvoiceNumber = invoice.InvoiceNumber,
                        PaymentTerms = invoice.PaymentTerms,
                        InvoiceAmount = invoice.InvoiceAmount,
                        AdvanceAmount = invoice.AdvanceAmount,
                        GLTaxRateID = invoice.GLTaxRateID,
                        WithholdTax = invoice.WithholdTax,
                        WithholdingType = invoice.WithholdingType,
                        TaxAmount = invoice.TaxAmount,
                        WithholdingAmount = invoice.WithholdingAmount,
                        Subtotal = invoice.Subtotal,
                        WithholdTaxAmount = invoice.WithholdTaxAmount,
                        NetPayable = invoice.NetPayable,
                        AmountDue = invoice.AmountDue,
                        InvoiceStatus = invoice.InvoiceStatus,
                        ForBranchId = invoice.ForBranchId,

                        IsSendForApproval = invoice.IsSendForApproval,
                        IsOnlineTransferred = invoice.IsOnlineTransferred,
                        IsOnlineTransferredBy = invoice.IsOnlineTransferredBy,
                        OnlineTransferredTime = invoice.OnlineTransferredTime,
                        Status = invoice.Status,
                        Source = invoice.Source,
                        Grn = invoice.Grn,
                        IsOnlineApproved = invoice.IsOnlineApproved,
                        OnlineApprovedBy = invoice.OnlineApprovedBy,
                        OnlineApprovedTime = invoice.OnlineApprovedTime,
                        IsVoid = invoice.IsVoid,
                        VoidBy = invoice.VoidBy,
                        VoidTime = invoice.VoidTime,
                        OrgBranchName = invoice.OrgBranchName,
                        DateCreated = invoice.DateCreated,
                        LastModified = invoice.LastModified,
                        CreatedBy = invoice.CreatedBy,
                        ModifiedBy = invoice.ModifiedBy
                    };

                    _db.APPaybleInvoices.Attach(entity);
                    _db.APPaybleInvoices.Remove(entity);
                    _db.SaveChanges();

                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted manual payable invoice " + invoice.ApPaybleInvoicesId + " " + invoice.Remark, Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { invoice }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayableInvoice)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }


        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayableInvoice)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.APPaybleInvoices.Find(id);
            #region check segment length validation

            var distributions = _db.APPaybleInvoiceDistributions.Where(x => x.ApPaybleInvoicesId == id).ToList();
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
            decimal sumTotalinvoiceLines = 0;
            decimal sumTotalDisDebit = 0;
            decimal sumTotalDisCredit = 0;
            try
            {
                sumTotalinvoiceLines = _db.APPaybleInvoicesLines.Where(x => x.ApPaybleInvoicesId == record.ApPaybleInvoicesId).Sum(x => x.InvoiceAmount);

            }
            catch (Exception e)
            {
                var formattedData = new
                {
                    Message = "Please add invoice lines and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            try
            {
                sumTotalDisDebit = _db.APPaybleInvoiceDistributions.Where(x => x.ApPaybleInvoicesId == record.ApPaybleInvoicesId).Sum(x => x.Debit);
                sumTotalDisCredit = _db.APPaybleInvoiceDistributions.Where(x => x.ApPaybleInvoicesId == record.ApPaybleInvoicesId).Sum(x => x.Credit);
            }
            catch (Exception e)
            {
                var formattedData = new
                {
                    Message = "Please add distribution and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            //if (record != null && record.InvoiceAmount != sumTotalinvoiceLines)
            //{
            //    var formattedData = new
            //    {
            //        Message = "Invoice amount and total of invoice lines is not equal.Please correct it and try again.",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}
            if (sumTotalDisDebit != sumTotalDisCredit)
            {
                var formattedData = new
                {
                    Message = "Total Debit and Total Credit is not equal.Please correct it and try again.",
                    Status = "NOT_OK"
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

            if (record.APPaybleInvoicesLines.Count == 0)
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

            if (!record.IsSendForApproval && !record.IsOnlineApproved && record.APPaybleInvoicesLines.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.APPaybleInvoices.Attach(record);
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


        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        public JsonResult TransferDistribution(string id,string description)
        {
            int myid = int.Parse(id);
            var record = _db.APPaybleInvoices.Find(myid);
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
                    .Where(x => x.Source == JournalSources.AP_Invoice.ToString())
                    .Where(x => x.SourceId == record.ApPaybleInvoicesId.ToString())
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
                foreach (var dist in record.APPaybleInvoiceDistribution)
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
                var distribution = record.APPaybleInvoiceDistribution.ToList();
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
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.InvoiceDate.Date)
                    .Where(x => x.DateTo >= record.InvoiceDate.Date).ToList();
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
               
                if (record.APPaybleInvoiceDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "AP Invoice",
                        Description = !String.IsNullOrEmpty(description) ? description : "AP Invoice",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "AP Invoice",
                        SourcesName = "AP",
                        JournalNumber = "APINV." + record.ApPaybleInvoicesId.ToString(),
                        SourceId = record.ApPaybleInvoicesId.ToString(),
                        Source = JournalSources.AP_Invoice.ToString(),
                        EffectiveDate = record.InvoiceDate.Date,
                        OrgBranchName = record.OrgBranchName,
                        Status = "Approved",
                        Remark = record.Remark,
                        PaymentReference = "",
                        ReferenceDoc = "",
                        BranchId = 0,//to change
                        InvoiceReferenceDoc = record.InvoiceNumber,
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in record.APPaybleInvoiceDistribution)
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

        [AuthorizeRoles(FinanceConstants.AccountPayableVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.APPaybleInvoices.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.APPaybleInvoicesLines.Count > 0)
            {
                #region validate invoice

                var payments = _db.APPayments.Where(x => x.InvoiceNumber == record.ApPaybleInvoicesId.ToString())
                    .ToList();
                if (payments.Any())
                {
                    var formattedData1 = new
                    {
                        Message = $"The invoice number {record.ApPaybleInvoicesId} is already applied in payment \n hence cannot be void.",
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
                _db.APPaybleInvoices.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "manual payable invoice", "Success", User.Identity.Name, $"void payable invoice {id} ", Modules.Finance);
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

            if (record.APPaybleInvoicesLines.Count == 0)
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

      
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}

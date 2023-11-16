using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.StandardCollection
{
    public class ArStandardCollectionController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

      
       

        [AuthorizeRoles(FinanceConstants.AccountReceivableView, FinanceConstants.AccountReceivableApprover + "," +FinanceConstants.AccountReceivableCollection)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var witholdings = _db.GLTaxRates.Where(x => x.ApplyToWithholding).ToList();

            ViewData["witholdingType"] = witholdings
                .Select(e => new
                {
                    TaxName = e.TaxName
                })
                .AsQueryable();


            var methods = _db.PaymentMethods;

            ViewData["methods"] = methods.Select(e => new
            {
                Method = e.Method
            }).AsQueryable();


            var customers = _db.ARCustomerProfiles;

            ViewData["customers"] = customers.Select(c => new 
            {
                CompanyName = c.CompanyName,
                CustomerId = c.CustomerId
            }).AsQueryable();

            var invoices =
                _db.ARInvoices.Where(x => x.IsOnlineApproved).Where(x => x.InvoiceStatus == InvoiceStatus.Open).ToList();

            ViewData["invoices"] = invoices
                .Select(e => new
                {
                    ArInvoiceId = e.ArInvoiceId
                })
                .AsQueryable();

            var tax = _db.GLTaxRates.Where(x => x.ApplyToAr).ToList();
            ViewData["taxes"] = tax
                .Select(e => new
                {
                    TaxName = e.TaxName
                })
                .AsQueryable();
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView, FinanceConstants.AccountReceivableApprover + "," +FinanceConstants.AccountReceivableCollection)]
        public ActionResult ARStandardCollections_Read([DataSourceRequest]DataSourceRequest request, bool isNotTransffered)
        {
            IQueryable<ArStandardCollection> arstandardcollections = _db.ARStandardCollections;
            if (isNotTransffered)
            {
                List<int> transfferableList = FinanceTransferHelper.GetArStandardCollectionTranferableList();
                arstandardcollections =
                    arstandardcollections.Where(x => transfferableList.Contains(x.ArStandardCollectionId));
            }
            DataSourceResult result = arstandardcollections.ToDataSourceResult(request, aRStandardCollection => new
            {
                ArStandardCollectionId = aRStandardCollection.ArStandardCollectionId,
                ReceiptNumber = aRStandardCollection.ReceiptNumber,
                LineAmount = Math.Round(aRStandardCollection.LineAmount, 2),
                TaxAmount = Math.Round(aRStandardCollection.TaxAmount, 2),
                TotalAmount = Math.Round(aRStandardCollection.TotalAmount, 2),
                ReceiptDate = aRStandardCollection.ReceiptDate,
                ReceiptMethod = aRStandardCollection.ReceiptMethod,
                CustomerName = aRStandardCollection.CustomerName,
                IsNewCustomer = aRStandardCollection.IsNewCustomer,
                CustomerId = aRStandardCollection.CustomerId,
                UnidentifiedBalance = aRStandardCollection.UnidentifiedBalance,
                AppliedBalance = aRStandardCollection.AppliedBalance,
                OnAccountBalance = aRStandardCollection.OnAccountBalance,
                UnappliedBalance = aRStandardCollection.UnappliedBalance,
                PrepaymentBalance = aRStandardCollection.PrepaymentBalance,
                Reference = aRStandardCollection.Reference,
              

                IsSendForApproval = aRStandardCollection.IsSendForApproval,
                IsOnlineTransferred = aRStandardCollection.IsOnlineTransferred,
                IsOnlineTransferredBy = aRStandardCollection.IsOnlineTransferredBy,
                OnlineTransferredTime = aRStandardCollection.OnlineTransferredTime,
                Status = aRStandardCollection.Status,
                IsOnlineApproved = aRStandardCollection.IsOnlineApproved,
                OnlineApprovedBy = aRStandardCollection.OnlineApprovedBy,
                OnlineApprovedTime = aRStandardCollection.OnlineApprovedTime,

                IsVoid = aRStandardCollection.IsVoid,
                VoidBy = aRStandardCollection.VoidBy,
                VoidTime = aRStandardCollection.VoidTime,

                OrgBranchName = aRStandardCollection.OrgBranchName,
                DateCreated = aRStandardCollection.DateCreated,
                LastModified = aRStandardCollection.LastModified,
                CreatedBy = aRStandardCollection.CreatedBy,
                ModifiedBy = aRStandardCollection.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARStandardCollections_Create([DataSourceRequest]DataSourceRequest request, ArStandardCollection aRStandardCollection)
        {
            #region validation

            if (aRStandardCollection.IsNewCustomer && String.IsNullOrWhiteSpace(aRStandardCollection.CustomerName))
                ModelState.AddModelError("customer", "Please enter customer name.");
            if (!aRStandardCollection.IsNewCustomer && String.IsNullOrWhiteSpace(aRStandardCollection.CustomerId))
                ModelState.AddModelError("customer", "Please choose customer.");

            //if (!String.IsNullOrEmpty(aRStandardCollection.ReceiptNumber))
            //{
            //    var exists = _db.ARStandardCollections
            //        .Where(x => x.ReceiptNumber == aRStandardCollection.ReceiptNumber && x.IsVoid == false).ToList();

            //    if (exists.Any())
            //        ModelState.AddModelError("Receipt number", $"{aRStandardCollection.ReceiptNumber}  already exists in the system.");
            //}

            if (!String.IsNullOrEmpty(aRStandardCollection.Reference))
            {
                var exists = _db.ARStandardCollections
                    .Where(x => x.Reference == aRStandardCollection.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference", $"{aRStandardCollection.Reference}  already exists in the system.");
            }


            #endregion

            if (!aRStandardCollection.IsNewCustomer && !String.IsNullOrWhiteSpace(aRStandardCollection.CustomerId))
            {
                var customer = _db.ARCustomerProfiles.FirstOrDefault(x => x.CustomerId == aRStandardCollection.CustomerId);
                if (customer != null) aRStandardCollection.CustomerName = customer.CompanyName;
            }

            if (aRStandardCollection.IsNewCustomer)
                aRStandardCollection.CustomerId = "";

            if (ModelState.IsValid)
            {
                var entity = new ArStandardCollection
                {
                    ReceiptNumber = aRStandardCollection.ReceiptNumber,
                    LineAmount = aRStandardCollection.LineAmount,
                    TaxAmount = aRStandardCollection.TaxAmount,
                    TotalAmount = aRStandardCollection.TotalAmount,
                    ReceiptDate = aRStandardCollection.ReceiptDate.Date,
                    ReceiptMethod = aRStandardCollection.ReceiptMethod,
                    CustomerName = aRStandardCollection.CustomerName,
                    IsNewCustomer = aRStandardCollection.IsNewCustomer,
                    CustomerId = aRStandardCollection.CustomerId,
                    UnidentifiedBalance = aRStandardCollection.UnidentifiedBalance,
                    AppliedBalance = aRStandardCollection.AppliedBalance,
                    OnAccountBalance = aRStandardCollection.OnAccountBalance,
                    UnappliedBalance = aRStandardCollection.UnappliedBalance,
                    PrepaymentBalance = aRStandardCollection.PrepaymentBalance,
                    Reference = aRStandardCollection.Reference,
                    

                    IsSendForApproval = aRStandardCollection.IsSendForApproval,
                    IsOnlineTransferred = aRStandardCollection.IsOnlineTransferred,
                    IsOnlineTransferredBy = aRStandardCollection.IsOnlineTransferredBy,
                    OnlineTransferredTime = aRStandardCollection.OnlineTransferredTime,
                    Status = aRStandardCollection.Status,
                    IsOnlineApproved = aRStandardCollection.IsOnlineApproved,
                    OnlineApprovedBy = aRStandardCollection.OnlineApprovedBy,
                    OnlineApprovedTime = aRStandardCollection.OnlineApprovedTime,
                    IsVoid = aRStandardCollection.IsVoid,
                    VoidBy = aRStandardCollection.VoidBy,
                    VoidTime = aRStandardCollection.VoidTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = aRStandardCollection.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRStandardCollection.ModifiedBy
                };

                _db.ARStandardCollections.Add(entity);
                _db.SaveChanges();
                aRStandardCollection.ArStandardCollectionId = entity.ArStandardCollectionId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR standard collection", "Success", User.Identity.Name, $"created standard collection {entity.ArStandardCollectionId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRStandardCollection }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARStandardCollections_Update([DataSourceRequest]DataSourceRequest request, ArStandardCollection aRStandardCollection)
        {
            #region validation

            if (aRStandardCollection.IsNewCustomer && String.IsNullOrWhiteSpace(aRStandardCollection.CustomerName))
                ModelState.AddModelError("customer", "Please enter customer name.");
            if (!aRStandardCollection.IsNewCustomer && String.IsNullOrWhiteSpace(aRStandardCollection.CustomerId))
                ModelState.AddModelError("customer", "Please choose customer.");

            //if (!String.IsNullOrEmpty(aRStandardCollection.ReceiptNumber))
            //{
            //    var exists = _db.ARStandardCollections
            //        .Where(x=>x.ArStandardCollectionId!=aRStandardCollection.ArStandardCollectionId)
            //        .Where(x => x.ReceiptNumber == aRStandardCollection.ReceiptNumber && x.IsVoid == false).ToList();

            //    if (exists.Any())
            //        ModelState.AddModelError("Receipt number", $"{aRStandardCollection.ReceiptNumber}  already exists in the system.");
            //}

            if (!String.IsNullOrEmpty(aRStandardCollection.Reference))
            {
                var exists = _db.ARStandardCollections
                    .Where(x => x.ArStandardCollectionId != aRStandardCollection.ArStandardCollectionId)
                    .Where(x => x.Reference == aRStandardCollection.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference", $"{aRStandardCollection.Reference}  already exists in the system.");
            }

            #endregion

            if (!aRStandardCollection.IsNewCustomer && !String.IsNullOrWhiteSpace(aRStandardCollection.CustomerId))
            {
                var customer = _db.ARCustomerProfiles.FirstOrDefault(x => x.CustomerId == aRStandardCollection.CustomerId);
                if (customer != null) aRStandardCollection.CustomerName = customer.CompanyName;
            }

            if (aRStandardCollection.IsNewCustomer)
                aRStandardCollection.CustomerId = "";


            if (ModelState.IsValid)
            {
                    var entity = new ArStandardCollection
                    {
                        ArStandardCollectionId = aRStandardCollection.ArStandardCollectionId,
                        ReceiptNumber = aRStandardCollection.ReceiptNumber,
                        LineAmount = aRStandardCollection.LineAmount,
                        TaxAmount = aRStandardCollection.TaxAmount,
                        TotalAmount = aRStandardCollection.TotalAmount,
                        ReceiptDate = aRStandardCollection.ReceiptDate,
                        ReceiptMethod = aRStandardCollection.ReceiptMethod,
                        CustomerName = aRStandardCollection.CustomerName,
                        IsNewCustomer = aRStandardCollection.IsNewCustomer,
                        CustomerId = aRStandardCollection.CustomerId,
                        UnidentifiedBalance = aRStandardCollection.UnidentifiedBalance,
                        AppliedBalance = aRStandardCollection.AppliedBalance,
                        OnAccountBalance = aRStandardCollection.OnAccountBalance,
                        UnappliedBalance = aRStandardCollection.UnappliedBalance,
                        PrepaymentBalance = aRStandardCollection.PrepaymentBalance,
                        Reference = aRStandardCollection.Reference,
                      

                        IsSendForApproval = aRStandardCollection.IsSendForApproval,
                        IsOnlineTransferred = aRStandardCollection.IsOnlineTransferred,
                        IsOnlineTransferredBy = aRStandardCollection.IsOnlineTransferredBy,
                        OnlineTransferredTime = aRStandardCollection.OnlineTransferredTime,
                        Status = aRStandardCollection.Status,
                        IsOnlineApproved = aRStandardCollection.IsOnlineApproved,
                        OnlineApprovedBy = aRStandardCollection.OnlineApprovedBy,
                        OnlineApprovedTime = aRStandardCollection.OnlineApprovedTime,
                        IsVoid = aRStandardCollection.IsVoid,
                        VoidBy = aRStandardCollection.VoidBy,
                        VoidTime = aRStandardCollection.VoidTime,
                        OrgBranchName = aRStandardCollection.OrgBranchName,
                        DateCreated = aRStandardCollection.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aRStandardCollection.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ARStandardCollections.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR standard collection", "Success", User.Identity.Name, $"updated standard collection {entity.ArStandardCollectionId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { aRStandardCollection }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARStandardCollections_Destroy([DataSourceRequest]DataSourceRequest request, ArStandardCollection aRStandardCollection)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                //#region validated record

                //var parent = _db.ARStandardCollections.Find(aRStandardCollection.ArStandardCollectionId);
                //if (parent != null)
                //{
                //    if (parent.IsOnlineApproved)
                //    {
                //        isvalid = false;
                //        //ViewBag.Note = "You cannot manipulate this record";
                //        //return View("Error");
                //    }

                //    #region detach local version
                //    var local = _db.Set<ARMiscelaneousCollection>()
                //        .Local
                //        .FirstOrDefault(f => f.ARMiscelaneousCollectionID == parent.ArStandardCollectionId);
                //    if (local != null)
                //    {
                //        _db.Entry(local).State = EntityState.Detached;
                //    }
                //    #endregion

                //}


                //#endregion

                if (isvalid)
                {
                    var entity = new ArStandardCollection
                    {
                        ArStandardCollectionId = aRStandardCollection.ArStandardCollectionId,
                        ReceiptNumber = aRStandardCollection.ReceiptNumber,
                        LineAmount = aRStandardCollection.LineAmount,
                        TaxAmount = aRStandardCollection.TaxAmount,
                        TotalAmount = aRStandardCollection.TotalAmount,
                        ReceiptDate = aRStandardCollection.ReceiptDate,
                        ReceiptMethod = aRStandardCollection.ReceiptMethod,
                        CustomerName = aRStandardCollection.CustomerName,
                        CustomerId = aRStandardCollection.CustomerId,
                        UnidentifiedBalance = aRStandardCollection.UnidentifiedBalance,
                        AppliedBalance = aRStandardCollection.AppliedBalance,
                        OnAccountBalance = aRStandardCollection.OnAccountBalance,
                        UnappliedBalance = aRStandardCollection.UnappliedBalance,
                        PrepaymentBalance = aRStandardCollection.PrepaymentBalance,
                       

                        IsSendForApproval = aRStandardCollection.IsSendForApproval,
                        IsOnlineTransferred = aRStandardCollection.IsOnlineTransferred,
                        IsOnlineTransferredBy = aRStandardCollection.IsOnlineTransferredBy,
                        OnlineTransferredTime = aRStandardCollection.OnlineTransferredTime,
                        Status = aRStandardCollection.Status,
                        IsOnlineApproved = aRStandardCollection.IsOnlineApproved,
                        OnlineApprovedBy = aRStandardCollection.OnlineApprovedBy,
                        OnlineApprovedTime = aRStandardCollection.OnlineApprovedTime,
                        IsVoid = aRStandardCollection.IsVoid,
                        VoidBy = aRStandardCollection.VoidBy,
                        VoidTime = aRStandardCollection.VoidTime,
                        OrgBranchName = aRStandardCollection.OrgBranchName,
                        DateCreated = aRStandardCollection.DateCreated,
                        LastModified = aRStandardCollection.LastModified,
                        CreatedBy = aRStandardCollection.CreatedBy,
                        ModifiedBy = aRStandardCollection.ModifiedBy
                    };

                    _db.ARStandardCollections.Attach(entity);
                    _db.ARStandardCollections.Remove(entity);
                    _db.SaveChanges();

                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Standard Collection" + aRStandardCollection.ArStandardCollectionId + " " + aRStandardCollection.ReceiptNumber, Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { aRStandardCollection }.ToDataSourceResult(request, ModelState));
        }

        
        [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]

        public JsonResult SendforApproval(string id)
        {
            int myid = int.Parse(id);
            var record = _db.ARStandardCollections.Find(myid);
            #region check segment length validation

            var distributions = _db.ARStandardCollectionDistributions.Where(x => x.ArStandardCollectionId == myid).ToList();
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
            if (record==null)
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
            decimal sumOfCollectionAmt = 0;
            decimal sumTotalDisDebit = 0;
            decimal sumTotalDisCredit = 0;
            try
            {
                sumOfCollectionAmt = record.ARStandardCollectionLines.Sum(x => x.ApplyAmount);

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
                sumTotalDisDebit = _db.ARStandardCollectionDistributions.Where(x => x.ArStandardCollectionId == myid).Sum(x => x.Debit);
                sumTotalDisCredit = _db.ARStandardCollectionDistributions.Where(x => x.ArStandardCollectionId == myid).Sum(x => x.Credit);
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
            //if (sumTotalDisDebit != sumOfCollectionAmt)
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
                _db.ARStandardCollections.Attach(record);
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
        
        [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.ARStandardCollections.Find(myid);
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
                   .Where(x => x.Source == JournalSources.AR_Collection.ToString())
                   .Where(x => x.SourceId == record.ArStandardCollectionId.ToString())
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
                foreach (var dist in record.ARStandardCollectionDistribution)
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
                var distribution = record.ARStandardCollectionDistribution.ToList();
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
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.ReceiptDate.Date)
                    .Where(x => x.DateTo >= record.ReceiptDate.Date).ToList();
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
                        Message = "There are no periods defined that contain the Collection date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
               
                if (record.ARStandardCollectionDistribution.Any())
                {
                    var invoice = _db.ARStandardCollectionLines.Where(x => x.ArStandardCollectionId.ToString() == id)
                        .Select(x => x.InvoiceNumber).ToList();
                    string invoices = "";
                    foreach (var i in invoice)
                    {
                        invoices=invoices + "," + i;
                    }
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "AR Standard Collection",
                        Description = !String.IsNullOrEmpty(description) ? description : "AR Standard Collection",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "AR Standard Collection",
                        SourcesName = "AR",
                        JournalNumber = "ARSTC." + record.ArStandardCollectionId.ToString(),
                        SourceId = record.ArStandardCollectionId.ToString(),
                        Source = JournalSources.AR_Collection.ToString(),
                        EffectiveDate = record.ReceiptDate.Date,
                        OrgBranchName = record.OrgBranchName,
                        BranchId=int.Parse(record.OrgBranchName),
                        Status = "Approved",
                        Remark = record.Reference,
                        PaymentReference = record.ReceiptNumber,
                        ReferenceDoc = record.Reference,
                        InvoiceReferenceDoc = invoices,
                        DebitSum = debit,
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (var i in record.ARStandardCollectionDistribution)
                    {
                        #region debit and credit

                        var journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            AccountDesc = i.AccountDesc,
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
            var record = _db.ARStandardCollections.Find(id);
            var lines = _db.ARStandardCollectionLines.Where(x => x.ArStandardCollectionId == id).ToList();
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.ARStandardCollectionLines.Count > 0)
            {
                #region validate invoice
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ARStandardCollections.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                #region reverse invoice

                foreach (var line in lines)
                {
                    string invoiceno = line.InvoiceNumber.Split('_')[1];
                    var invoice = _db.ARInvoices.FirstOrDefault(x => x.ArInvoiceId.ToString() == invoiceno);
                    if (invoice != null)
                    {
                        if ((invoice.AmountDue + line.ApplyAmount) > invoice.TotalAmount)
                            invoice.AmountDue = invoice.TotalAmount;
                        else
                        {
                            invoice.AmountDue = invoice.AmountDue + line.ApplyAmount;
                        }

                        _db.Entry(invoice).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                   
                }

                #endregion

                //copy
                //CopyRecord(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "standard collection", "Success", User.Identity.Name, $"void standard collection {id} ", Modules.Finance);
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

            if (record.ARStandardCollectionLines.Count == 0)
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

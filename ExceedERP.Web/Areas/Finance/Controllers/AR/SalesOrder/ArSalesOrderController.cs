using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
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

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.SalesOrder
{
    public class ArSalesOrderController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FinanceConstants.ArSalesOrderView, FinanceConstants.ArSalesOrderApprover, FinanceConstants.ArSalesOrder)]
        public ActionResult Index()
        {
            PopulateViewData();

            return View();
        }

        private void PopulateViewData()
        {
            var uoMs = _db.UoMs;

            ViewData["Units"] = uoMs;

            var terms = _db.PaymentTerms.ToList();

            ViewData["terms"] = terms.Select(p => new PaymentTerms
            {
                PaymentTermsDays = p.PaymentTermsDays
            });

            var partners = _db.Partners.Where(x => x.IsActive).ToList();

            ViewData["Partners"] = partners.Select(c => new 
            {
                Code = c.PartnerId,
                Name = c.Name
            });

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;


            var tax = _db.GLTaxRates.Where(x => x.ApplyToAr).ToList();

            ViewData["taxes"] = tax.Select(p => new
            {
                Name = p.TaxName,
                GLTaxRateID = p.GLTaxRateID
            });


            var productorService = _db.ARSetupProductServiceAccounts.ToList();
            ViewData["productorServices"] = productorService;

            var productItems = _db.ArProductService.ToList();
            ViewData["productorServicesItems"] = productItems.Select(p => new
            {
                ItemName = p.Code+"-"+p.ItemName,
                ArProductServiceId = p.ArProductServiceId
            });
        }
        [AuthorizeRoles(FinanceConstants.ArSalesOrderView, FinanceConstants.ArSalesOrderApprover, FinanceConstants.ArSalesOrder)]
        public ActionResult ArSalesOrders_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArSalesOrder> arsalesorders = _db.ArSalesOrders;
            DataSourceResult result = arsalesorders.ToDataSourceResult(request, arSalesOrder => new {
                ArSalesOrderId = arSalesOrder.ArSalesOrderId,
                OrderType = arSalesOrder.OrderType,
                CustomerId = arSalesOrder.CustomerId,
                IsNewCustomer = arSalesOrder.IsNewCustomer,
                CustomerName = arSalesOrder.CustomerName,
                OrderDate = arSalesOrder.OrderDate,
                Station = arSalesOrder.Station,
                SalesPersonId = arSalesOrder.SalesPersonId,
                PartnerId = arSalesOrder.PartnerId,
                PaymentTerm = arSalesOrder.PaymentTerm,
                OrderReference = arSalesOrder.OrderReference,
                OrderStatus = arSalesOrder.OrderStatus,
                AmountDue = arSalesOrder.AmountDue,
                InvoiceStatus = arSalesOrder.InvoiceStatus,
                IsReceiptPrinted = arSalesOrder.IsReceiptPrinted,
                BranchId = arSalesOrder.BranchId,
                Product = arSalesOrder.Product,
                Repeatition = arSalesOrder.Repeatition,
                Length = arSalesOrder.Length,

                IsOnlineTransferred = arSalesOrder.IsOnlineTransferred,
                OnlineTransferredBy = arSalesOrder.OnlineTransferredBy,
                OnlineTransferredTime = arSalesOrder.OnlineTransferredTime,
                Status = arSalesOrder.Status,
                IsOnlineApproved = arSalesOrder.IsOnlineApproved,
                OnlineApprovedBy = arSalesOrder.OnlineApprovedBy,
                OnlineApprovedTime = arSalesOrder.OnlineApprovedTime,
                isVoid = arSalesOrder.isVoid,
                VoidBy = arSalesOrder.VoidBy,
                VoidTime = arSalesOrder.VoidTime,
                IsSendForApproval = arSalesOrder.IsSendForApproval,
                SendForApprovalBy = arSalesOrder.SendForApprovalBy,
                SendForApprovalTime = arSalesOrder.SendForApprovalTime,
                OrgBranchName = arSalesOrder.OrgBranchName,
                DateCreated = arSalesOrder.DateCreated,
                LastModified = arSalesOrder.LastModified,
                CreatedBy = arSalesOrder.CreatedBy,
                ModifiedBy = arSalesOrder.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrder)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrders_Create([DataSourceRequest]DataSourceRequest request, ArSalesOrder arSalesOrder)
        {
            #region validation

            if (arSalesOrder.OrderType == OrderType.Credit)
            {
                if (String.IsNullOrWhiteSpace(arSalesOrder.CustomerId))
                    ModelState.AddModelError("customer", "Please choose customer.");
            }

            if (arSalesOrder.OrderType == OrderType.Cash)
            {
                if (arSalesOrder.IsNewCustomer && String.IsNullOrWhiteSpace(arSalesOrder.CustomerName))
                    ModelState.AddModelError("customer", "Please enter customer name.");
                if (!arSalesOrder.IsNewCustomer && String.IsNullOrWhiteSpace(arSalesOrder.CustomerId))
                    ModelState.AddModelError("customer", "Please choose customer.");
            }

            if (!String.IsNullOrEmpty(arSalesOrder.OrderReference))
            {
                var exists = _db.ArSalesOrders
                    .Where(x => x.OrderReference == arSalesOrder.OrderReference && x.isVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Order Reference", $"{arSalesOrder.OrderReference}  already exists in the system.");
            }

            #endregion

            if (!arSalesOrder.IsNewCustomer && !String.IsNullOrWhiteSpace(arSalesOrder.CustomerId))
            {
                var customer = _db.ARCustomerProfiles.FirstOrDefault(x => x.CustomerId == arSalesOrder.CustomerId);
                if (customer != null) arSalesOrder.CustomerName = customer.CompanyName;
            }

            if (arSalesOrder.IsNewCustomer)
                arSalesOrder.CustomerId = "";

            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrder
                {
                    OrderType = arSalesOrder.OrderType,
                    CustomerId = arSalesOrder.CustomerId,
                    IsNewCustomer = arSalesOrder.IsNewCustomer,
                    CustomerName = arSalesOrder.CustomerName,
                    OrderDate = arSalesOrder.OrderDate.Date,
                    Station = arSalesOrder.Station,
                    SalesPersonId = arSalesOrder.SalesPersonId,
                    PartnerId = arSalesOrder.PartnerId,
                    PaymentTerm = arSalesOrder.PaymentTerm,
                    OrderReference = arSalesOrder.OrderReference,
                    OrderStatus = arSalesOrder.OrderStatus,
                    AmountDue = arSalesOrder.AmountDue,
                    InvoiceStatus = InvoiceStatus.New,
                    IsReceiptPrinted = arSalesOrder.IsReceiptPrinted,
                    BranchId = arSalesOrder.BranchId,
                    Product = arSalesOrder.Product,
                    Repeatition = arSalesOrder.Repeatition,
                    Length = arSalesOrder.Length,

                    IsOnlineTransferred = arSalesOrder.IsOnlineTransferred,
                    OnlineTransferredBy = arSalesOrder.OnlineTransferredBy,
                    OnlineTransferredTime = arSalesOrder.OnlineTransferredTime,
                    Status = arSalesOrder.Status,
                    IsOnlineApproved = arSalesOrder.IsOnlineApproved,
                    OnlineApprovedBy = arSalesOrder.OnlineApprovedBy,
                    OnlineApprovedTime = arSalesOrder.OnlineApprovedTime,
                    isVoid = arSalesOrder.isVoid,
                    VoidBy = arSalesOrder.VoidBy,
                    VoidTime = arSalesOrder.VoidTime,
                    IsSendForApproval = arSalesOrder.IsSendForApproval,
                    SendForApprovalBy = arSalesOrder.SendForApprovalBy,
                    SendForApprovalTime = arSalesOrder.SendForApprovalTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = arSalesOrder.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = arSalesOrder.ModifiedBy
                };

                _db.ArSalesOrders.Add(entity);
                _db.SaveChanges();
                arSalesOrder.ArSalesOrderId = entity.ArSalesOrderId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR sales order", "Success", User.Identity.Name, $"created sales order {entity.ArSalesOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrder }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrder)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrders_Update([DataSourceRequest]DataSourceRequest request, ArSalesOrder arSalesOrder)
        {
            #region validation

            if (arSalesOrder.OrderType == OrderType.Credit)
            {
                if (String.IsNullOrWhiteSpace(arSalesOrder.CustomerId))
                    ModelState.AddModelError("customer", "Please choose customer.");
            }

            if (arSalesOrder.OrderType == OrderType.Cash)
            {
                if (arSalesOrder.IsNewCustomer && String.IsNullOrWhiteSpace(arSalesOrder.CustomerName))
                    ModelState.AddModelError("customer", "Please enter customer name.");
                if (!arSalesOrder.IsNewCustomer && String.IsNullOrWhiteSpace(arSalesOrder.CustomerId))
                    ModelState.AddModelError("customer", "Please choose customer.");
            }

            if (!String.IsNullOrEmpty(arSalesOrder.OrderReference))
            {
                var exists = _db.ArSalesOrders
                    .Where(x => x.ArSalesOrderId != arSalesOrder.ArSalesOrderId)
                    .Where(x => x.OrderReference == arSalesOrder.OrderReference && x.isVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Order Reference", $"{arSalesOrder.OrderReference}  already exists in the system.");
            }

            #endregion

            if (!arSalesOrder.IsNewCustomer && !String.IsNullOrWhiteSpace(arSalesOrder.CustomerId))
            {
                var customer = _db.ARCustomerProfiles.FirstOrDefault(x => x.CustomerId == arSalesOrder.CustomerId);
                if (customer != null) arSalesOrder.CustomerName = customer.CompanyName;
            }
            if (arSalesOrder.IsNewCustomer)
                arSalesOrder.CustomerId = "";

            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrder
                {
                    ArSalesOrderId = arSalesOrder.ArSalesOrderId,
                    OrderType = arSalesOrder.OrderType,
                    CustomerId = arSalesOrder.CustomerId,
                    IsNewCustomer = arSalesOrder.IsNewCustomer,
                    CustomerName = arSalesOrder.CustomerName,
                    OrderDate = arSalesOrder.OrderDate,
                    Station = arSalesOrder.Station,
                    SalesPersonId = arSalesOrder.SalesPersonId,
                    PartnerId = arSalesOrder.PartnerId,
                    PaymentTerm = arSalesOrder.PaymentTerm,
                    OrderReference = arSalesOrder.OrderReference,
                    OrderStatus = arSalesOrder.OrderStatus,
                    AmountDue = arSalesOrder.AmountDue,
                    InvoiceStatus = arSalesOrder.InvoiceStatus,
                    IsReceiptPrinted = arSalesOrder.IsReceiptPrinted,
                    BranchId = arSalesOrder.BranchId,
                    Product = arSalesOrder.Product,
                    Repeatition = arSalesOrder.Repeatition,
                    Length = arSalesOrder.Length,

                    IsOnlineTransferred = arSalesOrder.IsOnlineTransferred,
                    OnlineTransferredBy = arSalesOrder.OnlineTransferredBy,
                    OnlineTransferredTime = arSalesOrder.OnlineTransferredTime,
                    Status = arSalesOrder.Status,
                    IsOnlineApproved = arSalesOrder.IsOnlineApproved,
                    OnlineApprovedBy = arSalesOrder.OnlineApprovedBy,
                    OnlineApprovedTime = arSalesOrder.OnlineApprovedTime,
                    isVoid = arSalesOrder.isVoid,
                    VoidBy = arSalesOrder.VoidBy,
                    VoidTime = arSalesOrder.VoidTime,
                    IsSendForApproval = arSalesOrder.IsSendForApproval,
                    SendForApprovalBy = arSalesOrder.SendForApprovalBy,
                    SendForApprovalTime = arSalesOrder.SendForApprovalTime,
                    OrgBranchName = arSalesOrder.OrgBranchName,
                    DateCreated = arSalesOrder.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = arSalesOrder.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ArSalesOrders.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR sales order", "Success", User.Identity.Name, $"updated sales order {entity.ArSalesOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrder }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrder)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrders_Destroy([DataSourceRequest]DataSourceRequest request, ArSalesOrder arSalesOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrder
                {
                    ArSalesOrderId = arSalesOrder.ArSalesOrderId,
                    OrderType = arSalesOrder.OrderType,
                    CustomerId = arSalesOrder.CustomerId,
                    OrderDate = arSalesOrder.OrderDate,
                    Station = arSalesOrder.Station,
                    SalesPersonId = arSalesOrder.SalesPersonId,
                    PartnerId = arSalesOrder.PartnerId,
                    PaymentTerm = arSalesOrder.PaymentTerm,
                    OrderReference = arSalesOrder.OrderReference,
                    OrderStatus = arSalesOrder.OrderStatus,
                    AmountDue = arSalesOrder.AmountDue,
                    InvoiceStatus = arSalesOrder.InvoiceStatus,
                    BranchId = arSalesOrder.BranchId,

                    IsOnlineTransferred = arSalesOrder.IsOnlineTransferred,
                    OnlineTransferredBy = arSalesOrder.OnlineTransferredBy,
                    OnlineTransferredTime = arSalesOrder.OnlineTransferredTime,
                    Status = arSalesOrder.Status,
                    IsOnlineApproved = arSalesOrder.IsOnlineApproved,
                    OnlineApprovedBy = arSalesOrder.OnlineApprovedBy,
                    OnlineApprovedTime = arSalesOrder.OnlineApprovedTime,
                    isVoid = arSalesOrder.isVoid,
                    VoidBy = arSalesOrder.VoidBy,
                    VoidTime = arSalesOrder.VoidTime,
                    IsSendForApproval = arSalesOrder.IsSendForApproval,
                    SendForApprovalBy = arSalesOrder.SendForApprovalBy,
                    SendForApprovalTime = arSalesOrder.SendForApprovalTime,
                    OrgBranchName = arSalesOrder.OrgBranchName,
                    DateCreated = arSalesOrder.DateCreated,
                    LastModified = arSalesOrder.LastModified,
                    CreatedBy = arSalesOrder.CreatedBy,
                    ModifiedBy = arSalesOrder.ModifiedBy
                };

                _db.ArSalesOrders.Attach(entity);
                _db.ArSalesOrders.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted sales order " + arSalesOrder.ArSalesOrderId + " " + arSalesOrder.OrderReference, Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrder }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrder)]
        public JsonResult SendforApproval(string id)
        {
            int myid = int.Parse(id);
            var record = _db.ArSalesOrders.Find(myid);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.ArSalesOrderLines.Count > 0)
            {
                #region check segment length validation

                var distributions = _db.ArSalesOrderDistributions.Where(x => x.ArSalesOrderId == myid).ToList();
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in distributions)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        var formattedData1 = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                    }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        message = $"{dist.AccountCode} Account has both debit and credit value.";
                        var formattedData1 = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        var formattedData1 = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        var formattedData1 = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        message = $"{dist.AccountCode} Negative values are not allowed.";
                        var formattedData1 = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                }


                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.ArSalesOrders.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
              
            }
            if (record != null && record.ArSalesOrderLines.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "You cannot manipulate this record.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsSendForApproval)
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
                sumTotalinvoiceLines = record.ArSalesOrderLines.Sum(x => x.TotalAmount);

            }
            catch (Exception e)
            {
               //Logger.Error("Messsage: "+e.Message+ "\n StackTrace: " + e.StackTrace + "\n TargetSite: " + e.TargetSite);
                var formattedData = new
                {
                    Message = "Please add sales lines and try again",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            //try
            //{
            //    sumTotalDisDebit = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == myid).Sum(x => x.Debit);
            //    sumTotalDisCredit = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == myid).Sum(x => x.Credit);
            //}
            //catch (Exception e)
            //{
            //    var formattedData = new
            //    {
            //        Message = "Please add distribution and try again",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}

            //if (sumTotalDisDebit != sumTotalDisCredit)
            //{
            //    var formattedData = new
            //    {
            //        Message = "Total Debit and Total Credit is not equal.Please correct it and try again",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}
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
                
            }
            return null;
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrder)]
        public JsonResult TransferDistribution(string id,string description)
        {
            int myid = int.Parse(id);
            var record = _db.ArSalesOrders.Find(myid);
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
                   .Where(x => x.Source == JournalSources.Sales.ToString())
                   .Where(x => x.SourceId == record.ArSalesOrderId.ToString())
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
                foreach (var dist in record.ArSalesOrderDistribution)
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
                var distribution = record.ArSalesOrderDistribution.ToList();
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
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.OrderDate.Date)
                    .Where(x => x.DateTo >= record.OrderDate.Date).ToList();
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
                        Message = "There are no periods defined that contain the Sales Order date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
               
                if (record.ArSalesOrderDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "AR Sales Order",
                        Description = !String.IsNullOrEmpty(description) ? description : "AR Sales Order",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "AR Sales Order",
                        SourcesName = "AR",
                        JournalNumber = "ARSALORD." + record.ArSalesOrderId.ToString(),
                        SourceId = record.ArSalesOrderId.ToString(),
                        Source = JournalSources.Sales.ToString(),
                        EffectiveDate = record.OrderDate.Date,
                        OrgBranchName = record.OrgBranchName,
                        BranchId=record.BranchId,
                        Status = "Approved",
                        Remark = record.OrderReference,
                        PaymentReference = record.OrderReference,
                        ReferenceDoc = "",
                        InvoiceReferenceDoc = "",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in record.ArSalesOrderDistribution)
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

        [AuthorizeRoles(FinanceConstants.ArSalesOrderReceipt)]
        public JsonResult GenerateCashXml(string id)
        {
            int myid = int.Parse(id);
            var arSales = _db.ArSalesOrders.Find(myid);
            if (arSales != null && !arSales.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (arSales != null && arSales.OrderType==OrderType.Credit)
            {
                var formattedData = new
                {
                    Message = "The order type is credit,hence you can't use this feature.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (arSales != null && arSales.IsReceiptPrinted)
            {
                var formattedData = new
                {
                    Message = "The receiet is already generated.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (arSales != null && arSales.OrderType==OrderType.Credit)
            {
                var formattedData = new
                {
                    Message = "The order is not chash sales.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            if (arSales != null)
            {
                try
                {
                    #region MyRegion

                    #region generate xml

                    var arSalesLine = _db.ArSalesOrderLines.Where(x => x.ArSalesOrderId == arSales.ArSalesOrderId).ToList();

                    XmlDocument xmlDoc = new XmlDocument();
                    XmlNode rootNode = xmlDoc.CreateElement("Invoice");
                    xmlDoc.AppendChild(rootNode);

                    XmlNode userNode10 = xmlDoc.CreateElement("Plant_Code");
                    userNode10.InnerText = "";
                    rootNode.AppendChild(userNode10);

                    XmlNode userNode11 = xmlDoc.CreateElement("Plant_Name");
                    userNode11.InnerText = "";
                    rootNode.AppendChild(userNode11);

                    XmlNode userNode120 = xmlDoc.CreateElement("Invoice_Type");
                    userNode120.InnerText = "Invoice";
                    rootNode.AppendChild(userNode120);

                    XmlNode userNode12 = xmlDoc.CreateElement("Reference_Number");
                    userNode12.InnerText = arSales.ArSalesOrderId.ToString();
                    rootNode.AppendChild(userNode12);
               
                    XmlNode userNode13 = xmlDoc.CreateElement("Reference_Number_1");
                    userNode13.InnerText = arSales.OrderReference;
                    rootNode.AppendChild(userNode13);

                    XmlNode userNode14 = xmlDoc.CreateElement("Reference_Number_2");
                    userNode14.InnerText = "";
                    rootNode.AppendChild(userNode14);

                    XmlNode userNode15 = xmlDoc.CreateElement("Reference_Number_3");
                    userNode15.InnerText = "";
                    rootNode.AppendChild(userNode15);


                    XmlNode userNode2 = xmlDoc.CreateElement("Invoice_Date");
                    userNode2.InnerText = arSales.OrderDate.ToString("yyyy-MM-dd HH:mm:ss");
                    rootNode.AppendChild(userNode2);

                    XmlNode userNode3 = xmlDoc.CreateElement("Customer_Code");
                    userNode3.InnerText = arSales.CustomerId.ToString();
                    rootNode.AppendChild(userNode3);

                    XmlNode userNode4 = xmlDoc.CreateElement("Customer_Name");
                    userNode4.InnerText = arSales.CustomerName;
                    rootNode.AppendChild(userNode4);

                    XmlNode userNode41 = xmlDoc.CreateElement("Customer_TIN");
                    userNode41.InnerText = "";
                    rootNode.AppendChild(userNode41);

                    XmlNode userNode5 = xmlDoc.CreateElement("Payment_Type");
                    userNode5.InnerText = arSales.OrderType.ToString();
                    rootNode.AppendChild(userNode5);

                    XmlNode userNode6 = xmlDoc.CreateElement("Invoice_DiscOrAdd_Amount");
                    userNode6.InnerText = "0.00";
                    rootNode.AppendChild(userNode6);

                    foreach (var arsale in arSalesLine)
                    {
                        int did = int.Parse(arsale.SubItem);
                        var item =  _db.ArProductService.Find(did);

                        XmlNode userNode7 = xmlDoc.CreateElement("Line_Items");
                        rootNode.AppendChild(userNode7);

                        XmlNode userNode70 = xmlDoc.CreateElement("Item_ID");
                        if (item != null) userNode70.InnerText = item.Code;
                        userNode7.AppendChild(userNode70);

                        XmlNode userNode701 = xmlDoc.CreateElement("Item_Description");
                        if (item != null) userNode701.InnerText = item.ItemName;
                        userNode7.AppendChild(userNode701);

                        XmlNode userNode71 = xmlDoc.CreateElement("Item_Quantity");
                        userNode71.InnerText = arsale.Quantity.ToString(); ;
                        userNode7.AppendChild(userNode71);

                        XmlNode userNode72 = xmlDoc.CreateElement("Item_UOM");
                        userNode72.InnerText = arsale.UoM.ToString(); ;
                        userNode7.AppendChild(userNode72);

                        XmlNode userNode73 = xmlDoc.CreateElement("Item_Unit_Price");
                        userNode73.InnerText = arsale.UnitPrice.ToString(); ;
                        userNode7.AppendChild(userNode73);

                        var percent = _db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == arsale.GLTaxRateID);

                        if (percent != null)
                        {
                            XmlNode userNode74 = xmlDoc.CreateElement("Item_Tax_Percent");
                            userNode74.InnerText = percent.Rate.ToString(); ;
                            userNode7.AppendChild(userNode74);
                        }

                        XmlNode userNode741 = xmlDoc.CreateElement("Item_DiscOrAdd_Amount");
                        userNode741.InnerText = (-arsale.Discount).ToString();
                        userNode7.AppendChild(userNode741);

                    }

                    // Save the document to a file and auto-indent the output.
                    string subPath = "~/App_Data/SalesOrder/" + "Export";
                    string physicalPath = Server.MapPath(subPath + "/" + id.ToString() + ".xml");
                    using (XmlTextWriter writer = new XmlTextWriter(physicalPath, null))
                    {
                        writer.Formatting = Formatting.Indented;
                        xmlDoc.Save(writer);
                    }

                    #endregion

                    arSales.IsReceiptPrinted = true;
                    _db.Entry(arSales).State = EntityState.Modified;
                    _db.SaveChanges();

                    var formattedData = new
                    {
                        Message = "Successfully Generated.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);

                    #endregion
                }
                catch (Exception e)
                {
                   //Logger.Error("Messsage: "+e.Message+ "\n StackTrace: " + e.StackTrace + "\n TargetSite: " + e.TargetSite);
                    var formattedData = new
                    {
                        Message = "The operation failed,try again.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

            }

            return null;
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrderReceipt)]
        public JsonResult GenerateCreditXml(string id)
        {
            int myid = int.Parse(id);
            var partialPrint = _db.ArSalesOrderPartialPrints.Find(myid);
            if (partialPrint != null && !partialPrint.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (partialPrint != null && partialPrint.ArSalesOrder.OrderType == OrderType.Cash)
            {
                var formattedData = new
                {
                    Message = "The order type is cash,hence you can't use this feature.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (partialPrint != null && partialPrint.IsReceiptPrinted)
            {
                var formattedData = new
                {
                    Message = "The receiet is already generated.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (partialPrint != null && partialPrint.ArSalesOrder.OrderType == OrderType.Cash)
            {
                var formattedData = new
                {
                    Message = "The order is not credit sales.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            if (partialPrint != null)
            {
                try
                {
                    #region MyRegion

                    #region generate xml

                    var arSalesLine = _db.ArSalesOrderPartials.Where(x => x.ArSalesOrderPartialPrintId == partialPrint.ArSalesOrderPartialPrintId).ToList();

                    XmlDocument xmlDoc = new XmlDocument();
                    XmlNode rootNode = xmlDoc.CreateElement("Invoice");
                    xmlDoc.AppendChild(rootNode);

                    XmlNode userNode10 = xmlDoc.CreateElement("Plant_Code");
                    userNode10.InnerText = "";
                    rootNode.AppendChild(userNode10);

                    XmlNode userNode11 = xmlDoc.CreateElement("Plant_Name");
                    userNode11.InnerText = "";
                    rootNode.AppendChild(userNode11);

                    XmlNode userNode120 = xmlDoc.CreateElement("Invoice_Type");
                    userNode120.InnerText = "Invoice";
                    rootNode.AppendChild(userNode120);

                    XmlNode userNode12 = xmlDoc.CreateElement("Reference_Number");
                    userNode12.InnerText = partialPrint.ArSalesOrderId.ToString();
                    rootNode.AppendChild(userNode12);

                    XmlNode userNode13 = xmlDoc.CreateElement("Reference_Number_1");
                    userNode13.InnerText = partialPrint.ArSalesOrder.OrderReference;
                    rootNode.AppendChild(userNode13);

                    XmlNode userNode14 = xmlDoc.CreateElement("Reference_Number_2");
                    userNode14.InnerText = "";
                    rootNode.AppendChild(userNode14);

                    XmlNode userNode15 = xmlDoc.CreateElement("Reference_Number_3");
                    userNode15.InnerText = "";
                    rootNode.AppendChild(userNode15);


                    XmlNode userNode2 = xmlDoc.CreateElement("Invoice_Date");
                    userNode2.InnerText = partialPrint.Date.ToString("yyyy-MM-dd HH:mm:ss");
                    rootNode.AppendChild(userNode2);

                    XmlNode userNode3 = xmlDoc.CreateElement("Customer_Code");
                    userNode3.InnerText = partialPrint.ArSalesOrder.CustomerId.ToString();
                    rootNode.AppendChild(userNode3);

                    XmlNode userNode4 = xmlDoc.CreateElement("Customer_Name");
                    userNode4.InnerText = partialPrint.ArSalesOrder.CustomerName;
                    rootNode.AppendChild(userNode4);

                    XmlNode userNode41 = xmlDoc.CreateElement("Customer_TIN");
                    userNode41.InnerText = "";
                    rootNode.AppendChild(userNode41);

                    XmlNode userNode5 = xmlDoc.CreateElement("Payment_Type");
                    userNode5.InnerText = partialPrint.ArSalesOrder.OrderType.ToString();
                    rootNode.AppendChild(userNode5);

                    XmlNode userNode6 = xmlDoc.CreateElement("Invoice_DiscOrAdd_Amount");
                    userNode6.InnerText = "0.00";
                    rootNode.AppendChild(userNode6);

                    foreach (var arsale in arSalesLine)
                    {
                        int did = int.Parse(arsale.SubItem);
                        var item = _db.ArProductService.Find(did);

                        XmlNode userNode7 = xmlDoc.CreateElement("Line_Items");
                        rootNode.AppendChild(userNode7);

                        XmlNode userNode70 = xmlDoc.CreateElement("Item_ID");
                        if (item != null) userNode70.InnerText = item.Code;
                        userNode7.AppendChild(userNode70);

                        XmlNode userNode701 = xmlDoc.CreateElement("Item_Description");
                        if (item != null) userNode701.InnerText = item.ItemName;
                        userNode7.AppendChild(userNode701);

                        XmlNode userNode71 = xmlDoc.CreateElement("Item_Quantity");
                        userNode71.InnerText = arsale.Quantity.ToString(); ;
                        userNode7.AppendChild(userNode71);

                        XmlNode userNode72 = xmlDoc.CreateElement("Item_UOM");
                        userNode72.InnerText = arsale.UoM.ToString(); ;
                        userNode7.AppendChild(userNode72);

                        XmlNode userNode73 = xmlDoc.CreateElement("Item_Unit_Price");
                        userNode73.InnerText = arsale.UnitPrice.ToString(); ;
                        userNode7.AppendChild(userNode73);

                        var percent = _db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == arsale.GLTaxRateID);

                        if (percent != null)
                        {
                            XmlNode userNode74 = xmlDoc.CreateElement("Item_Tax_Percent");
                            userNode74.InnerText = percent.Rate.ToString(); ;
                            userNode7.AppendChild(userNode74);
                        }

                        XmlNode userNode741 = xmlDoc.CreateElement("Item_DiscOrAdd_Amount");
                        userNode741.InnerText = (-arsale.Discount).ToString();
                        userNode7.AppendChild(userNode741);

                    }

                    // Save the document to a file and auto-indent the output.
                    string subPath = "~/App_Data/SalesOrder/" + "Export";
                    string physicalPath = Server.MapPath(subPath + "/" + id.ToString() + ".xml");
                    using (XmlTextWriter writer = new XmlTextWriter(physicalPath, null))
                    {
                        writer.Formatting = Formatting.Indented;
                        xmlDoc.Save(writer);
                    }

                    #endregion

                    partialPrint.IsReceiptPrinted = true;
                    _db.Entry(partialPrint).State = EntityState.Modified;
                    _db.SaveChanges();

                    var formattedData = new
                    {
                        Message = "Successfully Generated.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);

                    #endregion
                }
                catch (Exception e)
                {
                   //Logger.Error("Messsage: "+e.Message+ "\n StackTrace: " + e.StackTrace + "\n TargetSite: " + e.TargetSite);
                    var formattedData = new
                    {
                        Message = "The operation failed,try again.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

            }

           

            return null;
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrder)]
        public JsonResult Void(int id)
        {
            var record = _db.ArSalesOrders.Find(id);
            if (record != null && record.IsOnlineApproved && !record.isVoid && !record.IsReceiptPrinted && !record.IsOnlineTransferred && record.ArSalesOrderLines.Count > 0)
            {

                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.isVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ArSalesOrders.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "sales order", "Success", User.Identity.Name, $"void sales order {id} ", Modules.Finance);
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

            if (record.ArSalesOrderAdvanceLines.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add lines and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.isVoid)
            {
                var formattedData = new
                {
                    Message = "The record is already void.You can not void again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsReceiptPrinted)
            {
                var formattedData = new
                {
                    Message = "Receipt is already printed fromthis sales order,hence can't void .",
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

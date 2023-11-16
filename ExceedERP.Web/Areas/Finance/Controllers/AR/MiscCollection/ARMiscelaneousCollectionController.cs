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

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.MiscCollection
{
    public class ArMiscelaneousCollectionController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        

       [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover,FinanceConstants.AccountReceivableCollection)]
        public ActionResult Index()
       {
           PopulateViewData();

           return View();
       }

        private void PopulateViewData()
        {
            var branches = _db.Branch;
            var tax = _db.GLTaxRates.Where(x => x.ApplyToAr).ToList();
            ViewData["taxes"] = tax
                .Select(e => new GLTaxRate
                {
                    TaxName = e.TaxName
                })
                .OrderBy(e => e.TaxName);

            var methods = _db.PaymentMethods.ToList();
            ViewData["methods"] = methods
                .Select(e => new PaymentMethod
                {
                    Method = e.Method
                })
                .OrderBy(e => e.Method);

            var witholdings = _db.GLTaxRates.Where(x => x.ApplyToWithholding).ToList();

            ViewData["witholdingType"] = witholdings
                .Select(e => new GLTaxRate
                {
                    TaxName = e.TaxName
                })
                .OrderBy(e => e.TaxName);

            ViewData["Branches"] = branches;
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover,FinanceConstants.AccountReceivableCollection)]
        public ActionResult ARMiscelaneousCollections_Read([DataSourceRequest]DataSourceRequest request, bool isNotTransffered)
        {
            IQueryable<ARMiscelaneousCollection> armiscelaneouscollections = _db.ARMiscelaneousCollections;
            if (isNotTransffered)
            {
                List<int> transfferableList = FinanceTransferHelper.GetArMiscCollectionTranferableList();
                armiscelaneouscollections =
                    armiscelaneouscollections.Where(x => transfferableList.Contains(x.ARMiscelaneousCollectionID));
            }
            DataSourceResult result = armiscelaneouscollections.ToDataSourceResult(request, aRMiscelaneousCollection => new
            {
                ARMiscelaneousCollectionID = aRMiscelaneousCollection.ARMiscelaneousCollectionID,
                CustomerId = aRMiscelaneousCollection.CustomerId,
                IsNewCustomer = aRMiscelaneousCollection.IsNewCustomer,
                CustomerName = aRMiscelaneousCollection.CustomerName,
                ReceiptNumber = aRMiscelaneousCollection.ReceiptNumber,
                LineAmount = Math.Round(aRMiscelaneousCollection.LineAmount, 2),
                TaxAmount = Math.Round(aRMiscelaneousCollection.TaxAmount, 2),
                TotalAmount = Math.Round(aRMiscelaneousCollection.TotalAmount, 2),
                ReceiptDate = aRMiscelaneousCollection.ReceiptDate,
                ReceiptMethod = aRMiscelaneousCollection.ReceiptMethod,
                IsDirectCollection = aRMiscelaneousCollection.IsDirectCollection,
                BusinessUnitId = aRMiscelaneousCollection.BusinessUnitId,
                BranchId = aRMiscelaneousCollection.BranchId,
                Remark = aRMiscelaneousCollection.Remark,

                IsSendForApproval = aRMiscelaneousCollection.IsSendForApproval,
                IsOnlineTransferred = aRMiscelaneousCollection.IsOnlineTransferred,
                IsOnlineTransferredBy = aRMiscelaneousCollection.IsOnlineTransferredBy,
                OnlineTransferredTime = aRMiscelaneousCollection.OnlineTransferredTime,
                Status = aRMiscelaneousCollection.Status,
                IsOnlineApproved = aRMiscelaneousCollection.IsOnlineApproved,
                OnlineApprovedBy = aRMiscelaneousCollection.OnlineApprovedBy,
                OnlineApprovedTime = aRMiscelaneousCollection.OnlineApprovedTime,
                IsVoid = aRMiscelaneousCollection.IsVoid,
                VoidBy = aRMiscelaneousCollection.VoidBy,
                VoidTime = aRMiscelaneousCollection.VoidTime,
                OrgBranchName = aRMiscelaneousCollection.OrgBranchName,
                DateCreated = aRMiscelaneousCollection.DateCreated,
                LastModified = aRMiscelaneousCollection.LastModified,
                CreatedBy = aRMiscelaneousCollection.CreatedBy,
                ModifiedBy = aRMiscelaneousCollection.ModifiedBy
            });

            return Json(result);
        }

       [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARMiscelaneousCollections_Create([DataSourceRequest]DataSourceRequest request, ARMiscelaneousCollection aRMiscelaneousCollection)
        {
            #region validation


            if (aRMiscelaneousCollection.IsNewCustomer && String.IsNullOrWhiteSpace(aRMiscelaneousCollection.CustomerName))
                ModelState.AddModelError("customer", "Please enter customer name.");
            if (!aRMiscelaneousCollection.IsNewCustomer && String.IsNullOrWhiteSpace(aRMiscelaneousCollection.CustomerId))
                ModelState.AddModelError("customer", "Please choose customer.");

            if (!String.IsNullOrEmpty(aRMiscelaneousCollection.Remark))
            {
                var exists = _db.ARMiscelaneousCollections
                    .Where(x => x.Remark == aRMiscelaneousCollection.Remark && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Remark", $"Remark {aRMiscelaneousCollection.Remark}  already exists in the system.");
            }

            #endregion

            if (!aRMiscelaneousCollection.IsNewCustomer && !String.IsNullOrWhiteSpace(aRMiscelaneousCollection.CustomerId))
            {
                var customer = _db.ARCustomerProfiles.FirstOrDefault(x => x.CustomerId == aRMiscelaneousCollection.CustomerId);
                if (customer != null) aRMiscelaneousCollection.CustomerName = customer.CompanyName;
            }

            if (aRMiscelaneousCollection.IsNewCustomer)
                aRMiscelaneousCollection.CustomerId = "";

            if (ModelState.IsValid)
            {

                var entity = new ARMiscelaneousCollection
                {
                    CustomerId = aRMiscelaneousCollection.CustomerId,
                    IsNewCustomer = aRMiscelaneousCollection.IsNewCustomer,
                    CustomerName = aRMiscelaneousCollection.CustomerName,
                    IsDirectCollection = aRMiscelaneousCollection.IsDirectCollection,
                    ReceiptNumber = aRMiscelaneousCollection.ReceiptNumber,
                    LineAmount = aRMiscelaneousCollection.LineAmount,
                    TaxAmount = aRMiscelaneousCollection.TaxAmount,
                    TotalAmount = aRMiscelaneousCollection.TotalAmount,
                    
                    ReceiptDate = aRMiscelaneousCollection.ReceiptDate.Date,
                    ReceiptMethod = aRMiscelaneousCollection.ReceiptMethod,
                    BusinessUnitId = aRMiscelaneousCollection.BusinessUnitId,
                    BranchId = aRMiscelaneousCollection.BranchId,
                    Remark = aRMiscelaneousCollection.Remark,

                    IsSendForApproval = aRMiscelaneousCollection.IsSendForApproval,
                    IsOnlineTransferred = aRMiscelaneousCollection.IsOnlineTransferred,
                    IsOnlineTransferredBy = aRMiscelaneousCollection.IsOnlineTransferredBy,
                    OnlineTransferredTime = aRMiscelaneousCollection.OnlineTransferredTime,
                    Status = aRMiscelaneousCollection.Status,
                    IsOnlineApproved = aRMiscelaneousCollection.IsOnlineApproved,
                    OnlineApprovedBy = aRMiscelaneousCollection.OnlineApprovedBy,
                    OnlineApprovedTime = aRMiscelaneousCollection.OnlineApprovedTime,
                    IsVoid = aRMiscelaneousCollection.IsVoid,
                    VoidBy = aRMiscelaneousCollection.VoidBy,
                    VoidTime = aRMiscelaneousCollection.VoidTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = aRMiscelaneousCollection.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRMiscelaneousCollection.ModifiedBy
                };

                _db.ARMiscelaneousCollections.Add(entity);
                _db.SaveChanges();
                aRMiscelaneousCollection.ARMiscelaneousCollectionID = entity.ARMiscelaneousCollectionID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR misc collection", "Success", User.Identity.Name, $"created misc collection {entity.ARMiscelaneousCollectionID} ", Modules.Finance);
                #endregion


            }
            return Json(new[] { aRMiscelaneousCollection }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARMiscelaneousCollections_Update([DataSourceRequest]DataSourceRequest request, ARMiscelaneousCollection aRMiscelaneousCollection)
        {
            #region validation


            if (aRMiscelaneousCollection.IsNewCustomer && String.IsNullOrWhiteSpace(aRMiscelaneousCollection.CustomerName))
                ModelState.AddModelError("customer", "Please enter customer name.");
            if (!aRMiscelaneousCollection.IsNewCustomer && String.IsNullOrWhiteSpace(aRMiscelaneousCollection.CustomerId))
                ModelState.AddModelError("customer", "Please choose customer.");

            

            if (!String.IsNullOrEmpty(aRMiscelaneousCollection.Remark))
            {
                var exists = _db.ARMiscelaneousCollections
                    .Where(x => x.ARMiscelaneousCollectionID != aRMiscelaneousCollection.ARMiscelaneousCollectionID)
                    .Where(x => x.Remark == aRMiscelaneousCollection.Remark && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Remark", $"Remark {aRMiscelaneousCollection.Remark}  already exists in the system.");
            }


            #endregion

            if (!aRMiscelaneousCollection.IsNewCustomer && !String.IsNullOrWhiteSpace(aRMiscelaneousCollection.CustomerId))
            {
                var customer = _db.ARCustomerProfiles.FirstOrDefault(x => x.CustomerId == aRMiscelaneousCollection.CustomerId);
                if (customer != null) aRMiscelaneousCollection.CustomerName = customer.CompanyName;
            }

            if (aRMiscelaneousCollection.IsNewCustomer)
                aRMiscelaneousCollection.CustomerId = "";

            #region validated record

            var parent = _db.ARMiscelaneousCollections.Find(aRMiscelaneousCollection.ARMiscelaneousCollectionID);
            if (parent != null)
            {
                if (parent.IsOnlineApproved)
                    ModelState.AddModelError("approved", "You cannot manipulate this record");

                #region detach local version
                var local = _db.Set<ARMiscelaneousCollection>()
                    .Local
                    .FirstOrDefault(f => f.ARMiscelaneousCollectionID == parent.ARMiscelaneousCollectionID);
                if (local != null)
                {
                    _db.Entry(local).State = EntityState.Detached;
                }
                #endregion

            }


            #endregion

            if (ModelState.IsValid)
            {
                    var entity = new ARMiscelaneousCollection
                    {
                        ARMiscelaneousCollectionID = aRMiscelaneousCollection.ARMiscelaneousCollectionID,
                        CustomerId = aRMiscelaneousCollection.CustomerId,
                        IsNewCustomer = aRMiscelaneousCollection.IsNewCustomer,
                        CustomerName = aRMiscelaneousCollection.CustomerName,
                        IsDirectCollection = aRMiscelaneousCollection.IsDirectCollection,
                        ReceiptNumber = aRMiscelaneousCollection.ReceiptNumber,
                        LineAmount = aRMiscelaneousCollection.LineAmount,
                        TaxAmount = aRMiscelaneousCollection.TaxAmount,
                        TotalAmount = aRMiscelaneousCollection.TotalAmount,
                        ReceiptDate = aRMiscelaneousCollection.ReceiptDate,
                        ReceiptMethod = aRMiscelaneousCollection.ReceiptMethod,
                        BusinessUnitId = aRMiscelaneousCollection.BusinessUnitId,
                        BranchId = aRMiscelaneousCollection.BranchId,
                        Remark = aRMiscelaneousCollection.Remark,

                        IsSendForApproval = aRMiscelaneousCollection.IsSendForApproval,
                        IsOnlineTransferred = aRMiscelaneousCollection.IsOnlineTransferred,
                        IsOnlineTransferredBy = aRMiscelaneousCollection.IsOnlineTransferredBy,
                        OnlineTransferredTime = aRMiscelaneousCollection.OnlineTransferredTime,
                        Status = aRMiscelaneousCollection.Status,
                        IsOnlineApproved = aRMiscelaneousCollection.IsOnlineApproved,
                        OnlineApprovedBy = aRMiscelaneousCollection.OnlineApprovedBy,
                        OnlineApprovedTime = aRMiscelaneousCollection.OnlineApprovedTime,
                        IsVoid = aRMiscelaneousCollection.IsVoid,
                        VoidBy = aRMiscelaneousCollection.VoidBy,
                        VoidTime = aRMiscelaneousCollection.VoidTime,
                        OrgBranchName = aRMiscelaneousCollection.OrgBranchName,
                        DateCreated = aRMiscelaneousCollection.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aRMiscelaneousCollection.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ARMiscelaneousCollections.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR misc collection", "Success", User.Identity.Name, $"updated misc collection {entity.ARMiscelaneousCollectionID} ", Modules.Finance);
                #endregion
            }
            return Json(new[] { aRMiscelaneousCollection }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARMiscelaneousCollections_Destroy([DataSourceRequest]DataSourceRequest request, ARMiscelaneousCollection aRMiscelaneousCollection)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                var parent = _db.ARMiscelaneousCollections.Find(aRMiscelaneousCollection.ARMiscelaneousCollectionID);
                if (parent != null)
                {
                    if (parent.IsOnlineApproved)
                    {
                        isvalid = false;
                        //ViewBag.Note = "You cannot manipulate this record";
                        //return View("Error");
                    }

                    #region detach local version
                    var local = _db.Set<ARMiscelaneousCollection>()
                        .Local
                        .FirstOrDefault(f => f.ARMiscelaneousCollectionID == parent.ARMiscelaneousCollectionID);
                    if (local != null)
                    {
                        _db.Entry(local).State = EntityState.Detached;
                    }
                    #endregion

                }


                #endregion

                if (isvalid)
                {
                    var entity = new ARMiscelaneousCollection
                    {
                        ARMiscelaneousCollectionID = aRMiscelaneousCollection.ARMiscelaneousCollectionID,
                        CustomerId = aRMiscelaneousCollection.CustomerId,
                        IsNewCustomer = aRMiscelaneousCollection.IsNewCustomer,
                        CustomerName = aRMiscelaneousCollection.CustomerName,
                        IsDirectCollection = aRMiscelaneousCollection.IsDirectCollection,
                        ReceiptNumber = aRMiscelaneousCollection.ReceiptNumber,
                        ReceiptDate = aRMiscelaneousCollection.ReceiptDate,
                        ReceiptMethod = aRMiscelaneousCollection.ReceiptMethod,
                        LineAmount = aRMiscelaneousCollection.LineAmount,
                        TaxAmount = aRMiscelaneousCollection.TaxAmount,
                        TotalAmount = aRMiscelaneousCollection.TotalAmount,
                        BusinessUnitId = aRMiscelaneousCollection.BusinessUnitId,
                        BranchId = aRMiscelaneousCollection.BranchId,

                        IsSendForApproval = aRMiscelaneousCollection.IsSendForApproval,
                        IsOnlineTransferred = aRMiscelaneousCollection.IsOnlineTransferred,
                        IsOnlineTransferredBy = aRMiscelaneousCollection.IsOnlineTransferredBy,
                        OnlineTransferredTime = aRMiscelaneousCollection.OnlineTransferredTime,
                        Status = aRMiscelaneousCollection.Status,
                        IsOnlineApproved = aRMiscelaneousCollection.IsOnlineApproved,
                        OnlineApprovedBy = aRMiscelaneousCollection.OnlineApprovedBy,
                        OnlineApprovedTime = aRMiscelaneousCollection.OnlineApprovedTime,
                        IsVoid = aRMiscelaneousCollection.IsVoid,
                        VoidBy = aRMiscelaneousCollection.VoidBy,
                        VoidTime = aRMiscelaneousCollection.VoidTime,
                        OrgBranchName = aRMiscelaneousCollection.OrgBranchName,
                        DateCreated = aRMiscelaneousCollection.DateCreated,
                        LastModified = aRMiscelaneousCollection.LastModified,
                        CreatedBy = aRMiscelaneousCollection.CreatedBy,
                        ModifiedBy = aRMiscelaneousCollection.ModifiedBy
                    };

                    _db.ARMiscelaneousCollections.Attach(entity);
                    _db.ARMiscelaneousCollections.Remove(entity);
                    _db.SaveChanges();

                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Misc. Collection" + aRMiscelaneousCollection.ARMiscelaneousCollectionID + " " + aRMiscelaneousCollection.ReceiptNumber, Modules.Finance);
                    #endregion


                }
            }
            return Json(new[] { aRMiscelaneousCollection }.ToDataSourceResult(request, ModelState));
        }

     
       [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        public JsonResult SendforApproval(string id)
        {
            int myid = int.Parse(id);
            var record = _db.ARMiscelaneousCollections.Find(myid);
            #region check segment length validation

            var distributions = _db.ARMiscelaneousCollectionDistributions.Where(x => x.ARMiscelaneousCollectionID == myid).ToList();
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
            decimal sumOfCollectionAmt = 0;
            decimal sumTotalDisDebit = 0;
            decimal sumTotalDisCredit = 0;
            try
            {
                sumOfCollectionAmt = record.ArMiscelaneousCollectionLines.Sum(x=>x.TotalAmount);

            }
            catch (Exception e)
            {
                var formattedData = new
                {
                    Message = "Please add collection lines and try again",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            try
            {
                sumTotalDisDebit = _db.ARMiscelaneousCollectionDistributions.Where(x => x.ARMiscelaneousCollectionID == myid).Sum(x => x.Debit);
                sumTotalDisCredit = _db.ARMiscelaneousCollectionDistributions.Where(x => x.ARMiscelaneousCollectionID == myid).Sum(x => x.Credit);
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
                _db.ARMiscelaneousCollections.Attach(record);
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
        public JsonResult TransferDistribution(string id,string description)
        {
            int myid = int.Parse(id);
            var record = _db.ARMiscelaneousCollections.Find(myid);
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
                    .Where(x => x.Source == JournalSources.AR_MiscCollection.ToString())
                    .Where(x => x.SourceId == record.ARMiscelaneousCollectionID.ToString())
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
                foreach (var dist in record.ARMiscelaneousCollectionDistribution)
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
                var distribution = record.ARMiscelaneousCollectionDistribution.ToList();
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
               
                if (record.ARMiscelaneousCollectionDistribution.Any())
                {
                    var journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "AR Miscellaneous Collection",
                        Description = !String.IsNullOrEmpty(description) ? description : "AR Miscellaneous Collection",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "AR Miscellaneous Collection",
                        SourcesName = "AR",
                        JournalNumber = "ARSMC." + record.ARMiscelaneousCollectionID.ToString(),
                        SourceId = record.ARMiscelaneousCollectionID.ToString(),
                        Source = JournalSources.AR_MiscCollection.ToString(),
                        EffectiveDate = record.ReceiptDate.Date,
                        OrgBranchName = record.OrgBranchName,
                        BranchId=record.BranchId,
                        Status = "Approved",
                        Remark = record.Remark,
                        PaymentReference = record.ReceiptNumber,
                        ReferenceDoc = record.Remark,
                        InvoiceReferenceDoc = "",
                        DebitSum = debit,
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in record.ARMiscelaneousCollectionDistribution)
                    {
                        #region debit and credit

                        journalDetail = new GLRecordJournalDetail
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
            var record = _db.ARMiscelaneousCollections.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.ArMiscelaneousCollectionLines.Count > 0)
            {
               
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ARMiscelaneousCollections.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "misc. collection", "Success", User.Identity.Name, $"void misc. collection {id} ", Modules.Finance);
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

            if (record.ArMiscelaneousCollectionLines.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add lines and try again.",
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

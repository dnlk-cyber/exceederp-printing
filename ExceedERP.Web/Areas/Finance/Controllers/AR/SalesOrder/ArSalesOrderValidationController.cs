using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.SalesOrder
{
    public class ArSalesOrderValidationController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [AuthorizeRoles(FinanceConstants.ArSalesOrderView, FinanceConstants.ArSalesOrderApprover, FinanceConstants.ArSalesOrder)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrderView, FinanceConstants.ArSalesOrderApprover, FinanceConstants.ArSalesOrder)]
        public ActionResult ArSalesOrderValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArSalesOrderValidation> arsalesordervalidations = _db.ArSalesOrderValidations.Where(x=>x.ArSalesOrderId==id);
            DataSourceResult result = arsalesordervalidations.ToDataSourceResult(request, arSalesOrderValidation => new {
                ArSalesOrderValidationId = arSalesOrderValidation.ArSalesOrderValidationId,
                Status = arSalesOrderValidation.Status,
                Remark = arSalesOrderValidation.Remark,
                OrgBranchName = arSalesOrderValidation.OrgBranchName,
                DateCreated = arSalesOrderValidation.DateCreated,
                LastModified = arSalesOrderValidation.LastModified,
                CreatedBy = arSalesOrderValidation.CreatedBy,
                ModifiedBy = arSalesOrderValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ArSalesOrderValidation arSalesOrderValidation)
        {
            var record = _db.ArSalesOrders.Find(id);
          
            #region Checking the grn

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                if (record.ArSalesOrderLines.Count == 0 && arSalesOrderValidation.Status == ApprovalStatuses.Approve)
                {
                    string message = "The Record Does not Contain Any Item";
                    ModelState.AddModelError(String.Empty, message);
                }
               
                
            }
            else
            {
                ModelState.AddModelError("No Found", "Record not found");
            }


            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (arSalesOrderValidation.Status == ApprovalStatuses.Approve)
                    {
                        
                        
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                        if (record.OrderType == OrderType.Cash)
                        {
                           // TransferToStandardCollection(record);
                        }
                        else if (record.OrderType == OrderType.Credit)
                        {
                           // TransferToInvoice(record);
                        }

                    }
                    else if (arSalesOrderValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new ArSalesOrderValidation
                    {
                        ArSalesOrderId = id,
                        Status = arSalesOrderValidation.Status,
                        Remark = arSalesOrderValidation.Remark,
                        OrgBranchName = arSalesOrderValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = arSalesOrderValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = arSalesOrderValidation.ModifiedBy
                    };

                    _db.ArSalesOrderValidations.Add(entity);
                    _db.SaveChanges();
                    arSalesOrderValidation.ArSalesOrderValidationId = entity.ArSalesOrderValidationId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AR sales order approval", "Success", User.Identity.Name, $"approved sales order {entity.ArSalesOrderId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { arSalesOrderValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderValidations_Update([DataSourceRequest]DataSourceRequest request, ArSalesOrderValidation arSalesOrderValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderValidation
                {
                    ArSalesOrderId = arSalesOrderValidation.ArSalesOrderId,
                    ArSalesOrderValidationId = arSalesOrderValidation.ArSalesOrderValidationId,
                    Status = arSalesOrderValidation.Status,
                    Remark = arSalesOrderValidation.Remark,
                    OrgBranchName = arSalesOrderValidation.OrgBranchName,
                    DateCreated = arSalesOrderValidation.DateCreated,
                    LastModified = arSalesOrderValidation.LastModified,
                    CreatedBy = arSalesOrderValidation.CreatedBy,
                    ModifiedBy = arSalesOrderValidation.ModifiedBy
                };

                _db.ArSalesOrderValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.ArSalesOrderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderValidations_Destroy([DataSourceRequest]DataSourceRequest request, ArSalesOrderValidation arSalesOrderValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderValidation
                {
                    ArSalesOrderId = arSalesOrderValidation.ArSalesOrderId,
                    ArSalesOrderValidationId = arSalesOrderValidation.ArSalesOrderValidationId,
                    Status = arSalesOrderValidation.Status,
                    Remark = arSalesOrderValidation.Remark,
                    OrgBranchName = arSalesOrderValidation.OrgBranchName,
                    DateCreated = arSalesOrderValidation.DateCreated,
                    LastModified = arSalesOrderValidation.LastModified,
                    CreatedBy = arSalesOrderValidation.CreatedBy,
                    ModifiedBy = arSalesOrderValidation.ModifiedBy
                };

                _db.ArSalesOrderValidations.Attach(entity);
                _db.ArSalesOrderValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderValidation }.ToDataSourceResult(request, ModelState));
        }

        
        void TransferToStandardCollection(ArSalesOrder arSalesOrder)
        {
            throw new NotImplementedException();
        }
        void TransferToInvoice(ArSalesOrder arSalesOrder)
        {
            throw new NotImplementedException();
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}

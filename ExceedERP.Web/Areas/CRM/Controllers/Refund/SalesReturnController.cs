using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Construction;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.SalesReturnUser, CRMRoles.SalesReturnApprover)]
    public class SalesReturnController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();
            ViewData["Products"] = db.Products.Select(p => new { p.ProductID, Name = p.ItemCode + "-" + p.Name});
            ViewData["Customers"] = db.OrganizationCustomers;

			UpdateEmptyAmounts();
			return View();
        }

        public ActionResult SalesReturn_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<SalesReturn> returns = db.SalesReturn;
            DataSourceResult result = returns.ToDataSourceResult(request, salesReturn => new 
            {
                SalesReturnId = salesReturn.SalesReturnId,
                CustomerId = salesReturn.CustomerId,
                CustomerTin = salesReturn.CustomerTin,
                BranchId = salesReturn.BranchId,
                CRMPeriodId = salesReturn.CRMPeriodId,
                Reason = salesReturn.Reason,
                Date = salesReturn.Date,
				TotalAmount = salesReturn.TotalAmount,
				AmountDue = salesReturn.AmountDue,
				Paid = salesReturn.Paid,
				BusinessUnitId = salesReturn.BusinessUnitId,

                Status = salesReturn.Status,
                Remark = salesReturn.Remark,
                IsVoid = salesReturn.IsVoid,
                VoidBy = salesReturn.VoidBy,
                VoidTime = salesReturn.VoidTime,
                IsOnlineApproved = salesReturn.IsOnlineApproved,
                OnlineApprovedBy = salesReturn.OnlineApprovedBy,
                OnlineApprovedTime = salesReturn.OnlineApprovedTime,
                IsOnlineTransferred = salesReturn.IsOnlineTransferred,
                IsOnlineTransferredBy = salesReturn.IsOnlineTransferredBy,
                OnlineTransferredTime = salesReturn.OnlineTransferredTime,
                IsSendForApproval = salesReturn.IsSendForApproval,
                OrgBranchName = salesReturn.OrgBranchName,
                SendForApprovalBy = salesReturn.SendForApprovalBy,
                SendForApprovalTime = salesReturn.SendForApprovalTime,
                DateCreated = salesReturn.DateCreated,
                LastModified = salesReturn.LastModified,
                CreatedBy = salesReturn.CreatedBy,
                ModifiedBy = salesReturn.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesReturn_Create([DataSourceRequest]DataSourceRequest request, SalesReturn salesReturn)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesReturn
                {
                    SalesReturnId = salesReturn.SalesReturnId,
                    CustomerId = salesReturn.CustomerId,
                    CustomerTin = salesReturn.CustomerTin,
                    BranchId = salesReturn.BranchId,
                    CRMPeriodId = salesReturn.CRMPeriodId,
                    Reason = salesReturn.Reason,
                    Date = salesReturn.Date,
					TotalAmount = salesReturn.TotalAmount,
					AmountDue = salesReturn.AmountDue,
					Paid = salesReturn.Paid,
					BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name),

					Status = OperationStatuses.New.ToString(),
                    Remark = salesReturn.Remark,
                    IsVoid = salesReturn.IsVoid,
                    VoidBy = salesReturn.VoidBy,
                    VoidTime = salesReturn.VoidTime,
                    IsOnlineApproved = salesReturn.IsOnlineApproved,
                    OnlineApprovedBy = salesReturn.OnlineApprovedBy,
                    OnlineApprovedTime = salesReturn.OnlineApprovedTime,
                    IsOnlineTransferred = salesReturn.IsOnlineTransferred,
                    IsOnlineTransferredBy = salesReturn.IsOnlineTransferredBy,
                    OnlineTransferredTime = salesReturn.OnlineTransferredTime,
                    IsSendForApproval = salesReturn.IsSendForApproval,
                    OrgBranchName = salesReturn.OrgBranchName,
                    SendForApprovalBy = salesReturn.SendForApprovalBy,
                    SendForApprovalTime = salesReturn.SendForApprovalTime,

                    DateCreated = DateTime.Now,
                    LastModified = salesReturn.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = salesReturn.ModifiedBy
                };

                db.SalesReturn.Add(entity);
                db.SaveChanges();
                salesReturn.SalesReturnId = entity.SalesReturnId;
            }

            return Json(new[] { salesReturn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesReturn_Update([DataSourceRequest]DataSourceRequest request, SalesReturn salesReturn)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesReturn
                {
                    SalesReturnId = salesReturn.SalesReturnId,
                    CustomerId = salesReturn.CustomerId,
                    CustomerTin = salesReturn.CustomerTin,
                    BranchId = salesReturn.BranchId,
                    CRMPeriodId = salesReturn.CRMPeriodId,
                    Reason = salesReturn.Reason,
                    Date = salesReturn.Date,
					TotalAmount = salesReturn.TotalAmount,
					AmountDue = salesReturn.AmountDue,
					Paid = salesReturn.Paid,
					BusinessUnitId = salesReturn.BusinessUnitId,

					Status = salesReturn.Status,
                    Remark = salesReturn.Remark,
                    IsVoid = salesReturn.IsVoid,
                    VoidBy = salesReturn.VoidBy,
                    VoidTime = salesReturn.VoidTime,
                    IsOnlineApproved = salesReturn.IsOnlineApproved,
                    OnlineApprovedBy = salesReturn.OnlineApprovedBy,
                    OnlineApprovedTime = salesReturn.OnlineApprovedTime,
                    IsOnlineTransferred = salesReturn.IsOnlineTransferred,
                    IsOnlineTransferredBy = salesReturn.IsOnlineTransferredBy,
                    OnlineTransferredTime = salesReturn.OnlineTransferredTime,
                    IsSendForApproval = salesReturn.IsSendForApproval,
                    OrgBranchName = salesReturn.OrgBranchName,
                    SendForApprovalBy = salesReturn.SendForApprovalBy,
                    SendForApprovalTime = salesReturn.SendForApprovalTime,

                    DateCreated = salesReturn.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = salesReturn.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.SalesReturn.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { salesReturn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesReturn_Destroy([DataSourceRequest]DataSourceRequest request, SalesReturn salesReturn)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesReturn
                {
                    SalesReturnId = salesReturn.SalesReturnId,
                    CustomerId = salesReturn.CustomerId,
                    CustomerTin = salesReturn.CustomerTin,
                    BranchId = salesReturn.BranchId,
                    CRMPeriodId = salesReturn.CRMPeriodId,
                    Reason = salesReturn.Reason,
                    Date = salesReturn.Date,
					TotalAmount = salesReturn.TotalAmount,
					AmountDue = salesReturn.AmountDue,
					Paid = salesReturn.Paid,
					BusinessUnitId = salesReturn.BusinessUnitId,

					Status = salesReturn.Status,
                    Remark = salesReturn.Remark,
                    IsVoid = salesReturn.IsVoid,
                    VoidBy = salesReturn.VoidBy,
                    VoidTime = salesReturn.VoidTime,
                    IsOnlineApproved = salesReturn.IsOnlineApproved,
                    OnlineApprovedBy = salesReturn.OnlineApprovedBy,
                    OnlineApprovedTime = salesReturn.OnlineApprovedTime,
                    IsOnlineTransferred = salesReturn.IsOnlineTransferred,
                    IsOnlineTransferredBy = salesReturn.IsOnlineTransferredBy,
                    OnlineTransferredTime = salesReturn.OnlineTransferredTime,
                    IsSendForApproval = salesReturn.IsSendForApproval,
                    OrgBranchName = salesReturn.OrgBranchName,
                    SendForApprovalBy = salesReturn.SendForApprovalBy,
                    SendForApprovalTime = salesReturn.SendForApprovalTime,
                    DateCreated = salesReturn.DateCreated,
                    LastModified = salesReturn.LastModified,
                    CreatedBy = salesReturn.CreatedBy,
                    ModifiedBy = salesReturn.ModifiedBy
                };

                db.SalesReturn.Attach(entity);
                db.SalesReturn.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { salesReturn }.ToDataSourceResult(request, ModelState));
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
            var record = db.SalesReturn.Find(id);
            if ((record != null && !record.IsSendForApproval && !record.IsOnlineApproved))
            {
                #region validate line items
                var lineItems = db.SalesReturnLines.Where(li => li.SalesReturnId == record.SalesReturnId).ToList();
                if (!lineItems.Any())
                {
                    var d = new
                    {
                        Message = "No Line Items found. Please add Line Items first",
                        Status = "NOT_OK"
                    };
                    return Json(d, JsonRequestBehavior.AllowGet);
                }
                
                #endregion

                record.Status = OperationStatuses.WaitingApproval.ToString();
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.SalesReturn.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Successfully Sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Record not found!",
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

        [AuthorizeRoles(CRMRoles.CRMVoid)]
        public JsonResult Void(int id)
        {
            var record = db.SalesReturn.Find(id);

            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred)
            {
                record.Status = OperationStatuses.Void.ToString();
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;

                db.SalesReturn.Attach(record);
                db.Entry(record).State = EntityState.Modified;

                #region remove validation lines
                var validationLine = db.SalesReturnValidation.Where(l => l.SalesReturnId == record.SalesReturnId).ToList();
                db.SalesReturnValidation.RemoveRange(validationLine);
                #endregion

                db.SaveChanges();

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
                    Message = "The record was not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is already void. You can not void again.",
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
		private void UpdateEmptyAmounts()
		{
			var recordWithEmptyAmount =
				db.SalesReturn
				.Where(s => s.IsOnlineApproved && s.AmountDue == 0)
				.ToList();
			foreach (var salesReturn in recordWithEmptyAmount)
			{
				var lines = db.SalesReturnLines
					.Where(l => l.SalesReturnId == salesReturn.SalesReturnId)
					.ToList();
				decimal amount = lines.Any()
					? lines.Sum(l => l.TotalAmount)
					: 0;
				salesReturn.TotalAmount = amount;

				db.SalesReturn.Attach(salesReturn);
				db.Entry(salesReturn).State = EntityState.Modified;
				db.SaveChanges();
			}
		}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

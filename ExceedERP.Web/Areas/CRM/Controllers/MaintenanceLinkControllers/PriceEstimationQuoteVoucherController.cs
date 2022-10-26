using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Web.Areas.CRM.Models;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.PriceEstimationUser, CRMRoles.PriceEstimationFirstApprover, CRMRoles.PriceEstimationSecondApprover,CRMRoles.PriceEstimationApprover, CRMRoles.CRMVoid)]
    public class PriceEstimationQuoteVoucherController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult Vouchers_Read([DataSourceRequest]DataSourceRequest request)
        {
            var quotes = db.PriceEstimationQuoteVouchers.ToList();
            foreach (var vou in quotes)
            {
                decimal grandTotal = 0;
                var lines = db.PriceEstimationQuoteVoucherLines.Where(t => t.PriceEstimationQuoteVoucherId == vou.PriceEstimationQuoteVoucherId).ToList();
                var services = db.PriceEstimationQuoteVoucherServices.Where(t => t.PriceEstimationQuoteVoucherId == vou.PriceEstimationQuoteVoucherId).ToList();
                if (lines.Any())
                {
                    grandTotal = lines.Sum(l => l.TotalAmount);
                }
                if (services.Any())
                {
                    grandTotal += services.Sum(s => s.TotalAmount);
                }

                vou.GrandTotal = grandTotal;

            }
            return Json(quotes.OrderByDescending(x => x.PriceEstimationQuoteVoucherId).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);


        }
        
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Vouchers_Create([DataSourceRequest]DataSourceRequest request, PriceEstimationQuoteVoucher priceEstimationQuoteVoucher)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriceEstimationQuoteVoucher
                {
                    PriceEstimationQuoteVoucherId = priceEstimationQuoteVoucher.PriceEstimationQuoteVoucherId,
                    OrganizationCustomerId = priceEstimationQuoteVoucher.OrganizationCustomerId,
                    Date = priceEstimationQuoteVoucher.Date,
                    PlateNo = priceEstimationQuoteVoucher.PlateNo,
                    BankAccountId = priceEstimationQuoteVoucher.BankAccountId,
                    GrandTotal = priceEstimationQuoteVoucher.GrandTotal,
                    Status = CRMVoucherStatusConstatns.Editing,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.PriceEstimationQuoteVouchers.Add(entity);
                db.SaveChanges();
                priceEstimationQuoteVoucher.PriceEstimationQuoteVoucherId = entity.PriceEstimationQuoteVoucherId;

               
            }

            return Json(new[] { priceEstimationQuoteVoucher }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Vouchers_Update([DataSourceRequest]DataSourceRequest request, PriceEstimationQuoteVoucher priceEstimationQuoteVoucher)
        {
           
            if (ModelState.IsValid)
            {

                var entity = new PriceEstimationQuoteVoucher
                {
                    PriceEstimationQuoteVoucherId = priceEstimationQuoteVoucher.PriceEstimationQuoteVoucherId,
                    OrganizationCustomerId = priceEstimationQuoteVoucher.OrganizationCustomerId,
                    Date = priceEstimationQuoteVoucher.Date,
                    PlateNo = priceEstimationQuoteVoucher.PlateNo,
                    BankAccountId = priceEstimationQuoteVoucher.BankAccountId,
                    GrandTotal = priceEstimationQuoteVoucher.GrandTotal,

                    DateCreated = priceEstimationQuoteVoucher.DateCreated,
                    CreatedBy = priceEstimationQuoteVoucher.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.PriceEstimationQuoteVouchers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { priceEstimationQuoteVoucher }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Vouchers_Delete([DataSourceRequest]DataSourceRequest request, PriceEstimationQuoteVoucher priceEstimationQuoteVoucher)
        {

            if (ModelState.IsValid)
            {

                var entity = new PriceEstimationQuoteVoucher
                {
                    PriceEstimationQuoteVoucherId = priceEstimationQuoteVoucher.PriceEstimationQuoteVoucherId,
                    OrganizationCustomerId = priceEstimationQuoteVoucher.OrganizationCustomerId,
                    Date = priceEstimationQuoteVoucher.Date,
                    PlateNo = priceEstimationQuoteVoucher.PlateNo,
                    BankAccountId = priceEstimationQuoteVoucher.BankAccountId,
                    GrandTotal = priceEstimationQuoteVoucher.GrandTotal,

                    DateCreated = priceEstimationQuoteVoucher.DateCreated,
                    CreatedBy = priceEstimationQuoteVoucher.CreatedBy,
                    LastModified = priceEstimationQuoteVoucher.LastModified,
                    ModifiedBy = priceEstimationQuoteVoucher.ModifiedBy
                };

                db.PriceEstimationQuoteVouchers.Attach(entity);
                db.PriceEstimationQuoteVouchers.Remove(entity);
                db.SaveChanges();
               
            }


            return Json(new[] { priceEstimationQuoteVoucher }.ToDataSourceResult(request, ModelState));
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

        #region public methods
        public JsonResult SendForApproval(int id)
        {
            var record = db.PriceEstimationQuoteVouchers.Find(id);
            if ((record != null && !record.IsSendForApproval && !record.IsOnlineApproved))
            {
                #region validate line items
                var lineItems = db.PriceEstimationQuoteVoucherLines.Where(li => li.PriceEstimationQuoteVoucherId == record.PriceEstimationQuoteVoucherId).ToList();
                var services = db.PriceEstimationQuoteVoucherServices.Where(li => li.PriceEstimationQuoteVoucherId == record.PriceEstimationQuoteVoucherId).ToList();
                if (!lineItems.Any() && !services.Any())
                {
                    var d = new
                    {
                        Message = "No Line Items or Services found. Please add some services and line items",
                        Status = "NOT_OK"
                    };
                    return Json(d, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (PriceEstimationQuoteVoucherLine lineItem in lineItems)
                    {
                        if (lineItem.Quantity < 0)
                        {
                            return Json(new { Message = "Some line items have invalid  quantity.", Status = "NOT_OK" });
                        }
                        if (lineItem.TotalAmount < 0)
                        {
                            return Json(new { Message = "Some line items have invalid  amount.", Status = "NOT_OK" });
                        }
                    }
                    foreach (PriceEstimationQuoteVoucherService lineItem in services)
                    {
                        //if (lineItem.Quantity < 0)
                        //{
                        //    return Json(new { Message = "Some line items have invalid  quantity.", Status = "NOT_OK" });
                        //}
                        if (lineItem.TotalAmount < 0)
                        {
                            return Json(new { Message = "Some services have invalid  amount.", Status = "NOT_OK" });
                        }
                    }
                }
                #endregion

                record.Status = OperationStatuses.WaitingApproval.ToString();
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.PriceEstimationQuoteVouchers.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Successfully Sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Sales Quote not found!",
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
        public JsonResult SendForSecondApproval(int id)
        {
            var record = db.PriceEstimationQuoteVouchers.Find(id);
            if ((record != null && !record.IsSecondSendForApproval && !record.IsSecondOnlineApproved))
            {
                #region validate line items
                var lineItems = db.PriceEstimationQuoteVoucherLines.Where(li => li.PriceEstimationQuoteVoucherId == record.PriceEstimationQuoteVoucherId).ToList();
                var services = db.PriceEstimationQuoteVoucherServices.Where(li => li.PriceEstimationQuoteVoucherId == record.PriceEstimationQuoteVoucherId).ToList();
                if (!lineItems.Any() && !services.Any())
                {
                    var d = new
                    {
                        Message = "No Line Items or Services found. Please add some services and line items",
                        Status = "NOT_OK"
                    };
                    return Json(d, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (PriceEstimationQuoteVoucherLine lineItem in lineItems)
                    {
                        if (lineItem.Quantity < 0)
                        {
                            return Json(new { Message = "Some line items have invalid  quantity.", Status = "NOT_OK" });
                        }
                        if (lineItem.TotalAmount < 0)
                        {
                            return Json(new { Message = "Some line items have invalid  amount.", Status = "NOT_OK" });
                        }
                    }
                    foreach (PriceEstimationQuoteVoucherService lineItem in services)
                    {
                        //if (lineItem.Quantity < 0)
                        //{
                        //    return Json(new { Message = "Some line items have invalid  quantity.", Status = "NOT_OK" });
                        //}
                        if (lineItem.TotalAmount < 0)
                        {
                            return Json(new { Message = "Some services have invalid  amount.", Status = "NOT_OK" });
                        }
                    }
                }
                #endregion

                record.Status = CRMVoucherStatusConstatns.WaitingSecondApproval;
                record.IsSecondSendForApproval = true;
                record.SecondSendForApprovalBy = User.Identity.Name;
                record.SecondSendForApprovalTime = DateTime.Now;
                db.PriceEstimationQuoteVouchers.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Successfully Sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Sales Quote not found!",
                    Status = "OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
            }
            if (record.IsSecondSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsSecondOnlineApproved)
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
        public JsonResult SendForFirstApproval(int id)
        {
            var record = db.PriceEstimationQuoteVouchers.Find(id);
            if ((record != null && !record.IsMiddleSendForApproval && !record.IsMiddleOnlineApproved))
            {
                #region validate line items
                var lineItems = db.PriceEstimationQuoteVoucherLines.Where(li => li.PriceEstimationQuoteVoucherId == record.PriceEstimationQuoteVoucherId).ToList();
                var services = db.PriceEstimationQuoteVoucherServices.Where(li => li.PriceEstimationQuoteVoucherId == record.PriceEstimationQuoteVoucherId).ToList();
                if (!lineItems.Any() && !services.Any())
                {
                    var d = new
                    {
                        Message = "No Line Items or Services found. Please add some services and line items",
                        Status = "NOT_OK"
                    };
                    return Json(d, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (PriceEstimationQuoteVoucherLine lineItem in lineItems)
                    {
                        if (lineItem.Quantity < 0)
                        {
                            return Json(new { Message = "Some line items have invalid  quantity.", Status = "NOT_OK" });
                        }
                        if (lineItem.TotalAmount < 0)
                        {
                            return Json(new { Message = "Some line items have invalid  amount.", Status = "NOT_OK" });
                        }
                    }
                    foreach (PriceEstimationQuoteVoucherService lineItem in services)
                    {
                        //if (lineItem.Quantity < 0)
                        //{
                        //    return Json(new { Message = "Some line items have invalid  quantity.", Status = "NOT_OK" });
                        //}
                        if (lineItem.TotalAmount < 0)
                        {
                            return Json(new { Message = "Some services have invalid  amount.", Status = "NOT_OK" });
                        }
                    }
                }
                #endregion

                record.Status = CRMVoucherStatusConstatns.WaitingFirstApproval;
                record.IsMiddleSendForApproval = true;
                record.MiddleSendForApprovalBy = User.Identity.Name;
                record.MiddleSendForApprovalTime = DateTime.Now;
                db.PriceEstimationQuoteVouchers.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Successfully Sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Sales Quote not found!",
                    Status = "OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
            }
            if (record.IsMiddleSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsMiddleOnlineApproved)
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
            var record = db.PriceEstimationQuoteVouchers.Find(id);

            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.IsMiddleSendForApproval = false;
                record.IsMiddleOnlineApproved = false;
                record.IsSecondSendForApproval = false;
                record.IsSecondOnlineApproved = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                db.PriceEstimationQuoteVouchers.Attach(record);
                db.Entry(record).State = EntityState.Modified;
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
                    Message = "The record is not found.",
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

        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

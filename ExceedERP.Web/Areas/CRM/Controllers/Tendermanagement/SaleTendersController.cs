using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.CRM.TenderManagement;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
	// [AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpAdmin)]
	public class SaleTendersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index(){
                ViewData["productCategories"] = db.ProductCategories.ToList();       
                ViewData["products"] = db.Products.ToList();
                ViewData["productCategoriess"] = db.SalesLotItems.ToList();
                ViewData["productss"] = db.SalesLotItems.ToList();
                ViewData["saleslotPurchase"] = db.SalesLotPurchases.ToList();
            return View();
            

            
        }
       
        public ActionResult SaleTenders_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SaleTender> saleTenders = db.SaleTenders;
            DataSourceResult result = saleTenders.ToDataSourceResult(request, saleTender => new {
                SaleTenderId =saleTender.SaleTenderId,
                FloatDate = saleTender.FloatDate,
                EvaluationDate = saleTender.EvaluationDate,
                ClosingDate = saleTender.ClosingDate,
                TenderReference=saleTender.TenderReference,
                Status = saleTender.Status,
                TenderName=saleTender.TenderName,
                IsOnlineApproved = saleTender.IsOnlineApproved,
                OnlineApprovedTime = saleTender.OnlineApprovedTime,
                OnlineTransferredTime = saleTender.OnlineTransferredTime,
                OnlineApprovedBy = saleTender.OnlineApprovedBy,
                IsOnlineTransferredBy = saleTender.IsOnlineTransferredBy,
                IsVoid = saleTender.IsVoid,
                IsOnlineTransferred = saleTender.IsOnlineTransferred,
                OrgBranchName = saleTender.OrgBranchName,
                IsSendForApproval = saleTender.IsSendForApproval,
                SendForApprovalBy = saleTender.SendForApprovalBy,
                SendForApprovalTime = saleTender.SendForApprovalTime,
                VoidBy = saleTender.VoidBy,
                VoidTime = saleTender.VoidTime,
                DateCreated = saleTender.DateCreated,
                CreatedBy = saleTender.CreatedBy,
                ModifiedBy = saleTender.ModifiedBy,
                LastModified = saleTender.LastModified,
                Remark = saleTender.Remark

            });

            return Json(result);
        }

        public ActionResult SaleTenders_Create([DataSourceRequest]DataSourceRequest request, SaleTender saleTender)
        {
            if (saleTender.FloatDate > saleTender.EvaluationDate||saleTender.FloatDate >= saleTender.ClosingDate || saleTender.ClosingDate < saleTender.EvaluationDate)
            {
                ModelState.AddModelError("Check Date Entry", "Closing date must be greater than Float Date and Evaluation Date must be greater than or equal to Closing Date.");
               
            }
            if(saleTender.EvaluationDate.Date==saleTender.FloatDate.Date||saleTender.FloatDate.Date==saleTender.ClosingDate.Date)
            {
                ModelState.AddModelError("date error", "Float date should not be equal to evaluation date and closing date");
            }
                if (!ModelState.IsValid)
                {
                    // Pass the error messages to the view
                    ViewBag.ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return View(saleTender);
                }
                if (ModelState.IsValid)
                {
                    var entity = GetMappedObject(saleTender);
                    db.SaleTenders.Add(entity);
                    db.SaveChanges();
                    saleTender.SaleTenderId = entity.SaleTenderId;
                }
            
            return Json(new[] { saleTender }.ToDataSourceResult(request, ModelState));
        }
        

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaleTenders_Update([DataSourceRequest]DataSourceRequest request, SaleTender saleTender)
        {
          
            if (saleTender.FloatDate > saleTender.ClosingDate || saleTender.ClosingDate > saleTender.EvaluationDate)
            {
                ModelState.AddModelError("Check Date Entry", "Closing date must be greater than Float Date and Evaluation Date must be greater than or equal to Closing Date.");
                
            }
           
                if (ModelState.IsValid)
                {
                    var entity = GetMappedObject(saleTender);
                    db.SaleTenders.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            
            return Json(new[] { saleTender }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaleTenders_Destroy([DataSourceRequest]DataSourceRequest request, SaleTender saleTender)
        {

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(saleTender);
                db.SaleTenders.Attach(entity);
                db.SaleTenders.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { saleTender }.ToDataSourceResult(request, ModelState));
        }

        private SaleTender GetMappedObject(SaleTender saleTender)
        {
            var entity = new SaleTender
            {
                SaleTenderId = saleTender.SaleTenderId,
                FloatDate = saleTender.FloatDate,
                EvaluationDate = saleTender.EvaluationDate,
                ClosingDate = saleTender.ClosingDate,
                TenderReference = saleTender.TenderReference,
                Status = saleTender.Status,
                IsOnlineApproved = saleTender.IsOnlineApproved,
                OnlineApprovedTime = saleTender.OnlineApprovedTime,
                OnlineTransferredTime = saleTender.OnlineTransferredTime,
                OnlineApprovedBy = saleTender.OnlineApprovedBy,
                IsOnlineTransferredBy = saleTender.IsOnlineTransferredBy,
                IsVoid = saleTender.IsVoid,
                TenderName = saleTender.TenderName,
                IsOnlineTransferred = saleTender.IsOnlineTransferred,
                OrgBranchName = saleTender.OrgBranchName,
                IsSendForApproval = saleTender.IsSendForApproval,
                SendForApprovalBy = saleTender.SendForApprovalBy,
                SendForApprovalTime = saleTender.SendForApprovalTime,
                VoidBy = saleTender.VoidBy,
                VoidTime = saleTender.VoidTime,
                DateCreated = saleTender.DateCreated,
                CreatedBy = saleTender.CreatedBy,
                ModifiedBy = saleTender.ModifiedBy,
                LastModified = saleTender.LastModified,
                Remark = saleTender.Remark

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

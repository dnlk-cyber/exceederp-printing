using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.HRM.Payroll.BackPays;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    public class BackPaymentDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();      
      
      
        public ActionResult BackPaymentDetails_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
          
            IQueryable<BackPaymentDetail> BackPaymentDetails = db.BackPaymentDetails.Where(x => x.BackPaymentLineId==id);
            DataSourceResult result = BackPaymentDetails.ToDataSourceResult(request, BackPaymentDetail => new {
                BackPaymentDetailId = BackPaymentDetail.BackPaymentDetailId,
                BackPaymentLineId = BackPaymentDetail.BackPaymentLineId,
                GLPeriodId = BackPaymentDetail.GLPeriodId,
                Description = BackPaymentDetail.Description,
                Type = BackPaymentDetail.Type,
                Amount = BackPaymentDetail.Amount,
               
                DateCreated = BackPaymentDetail.DateCreated,
                LastModified = BackPaymentDetail.LastModified,
                CreatedBy = BackPaymentDetail.CreatedBy,
                ModifiedBy = BackPaymentDetail.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BackPaymentDetails_Create([DataSourceRequest]DataSourceRequest request, BackPaymentDetail BackPaymentDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new BackPaymentDetail
                {
                    GLPeriodId = BackPaymentDetail.GLPeriodId,
                    Type = BackPaymentDetail.Type,
                    BackPaymentLineId = BackPaymentDetail.BackPaymentLineId,
                    Description = BackPaymentDetail.Description,
                    Amount = BackPaymentDetail.Amount,                  
                    DateCreated = DateTime.Now,
                    LastModified = BackPaymentDetail.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = BackPaymentDetail.ModifiedBy
                };

                db.BackPaymentDetails.Add(entity);
                db.SaveChanges();
                BackPaymentDetail.BackPaymentDetailId = entity.BackPaymentDetailId;
            }

            return Json(new[] { BackPaymentDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BackPaymentDetails_Update([DataSourceRequest]DataSourceRequest request, BackPaymentDetail BackPaymentDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new BackPaymentDetail
                {
                    BackPaymentDetailId = BackPaymentDetail.BackPaymentDetailId,
                    BackPaymentLineId = BackPaymentDetail.BackPaymentLineId,
                    GLPeriodId = BackPaymentDetail.GLPeriodId,
                    Description = BackPaymentDetail.Description,
                    Type = BackPaymentDetail.Type,
                    Amount = BackPaymentDetail.Amount,
                   
                    DateCreated = BackPaymentDetail.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = BackPaymentDetail.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.BackPaymentDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { BackPaymentDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BackPaymentDetails_Destroy([DataSourceRequest]DataSourceRequest request, BackPaymentDetail BackPaymentDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new BackPaymentDetail
                {
                    BackPaymentDetailId = BackPaymentDetail.BackPaymentDetailId,
                    BackPaymentLineId = BackPaymentDetail.BackPaymentLineId,
                    GLPeriodId = BackPaymentDetail.GLPeriodId,
                    Description = BackPaymentDetail.Description,
                    Type = BackPaymentDetail.Type,
                    Amount = BackPaymentDetail.Amount,
                   
                    DateCreated = BackPaymentDetail.DateCreated,
                    LastModified = BackPaymentDetail.LastModified,
                    CreatedBy = BackPaymentDetail.CreatedBy,
                    ModifiedBy = BackPaymentDetail.ModifiedBy
                };
                #region delete log
               // UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee loan information of employee " + loan.Description + " and period " + loan.BackPaymentLineId);
                #endregion
                db.BackPaymentDetails.Attach(entity);
                db.BackPaymentDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { BackPaymentDetail }.ToDataSourceResult(request, ModelState));
        }
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

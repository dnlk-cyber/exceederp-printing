using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class SupportGroupDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SupportGroupDetails_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            IQueryable<SupportGroupDetail> Groups = db.SupportGroupDetails.Where(x=>x.SupportGroupId == Id);
            DataSourceResult result = Groups.ToDataSourceResult(request);

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupportGroupDetails_Create([DataSourceRequest]DataSourceRequest request, SupportGroupDetail model, int Id)
        {
            var Group = db.SupportGroupDetails.Where(x => x.Name == model.Name && x.SupportGroupId == Id).ToList();
            if (Group.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("SameName", " Name Already Registered ! ");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var entity = new SupportGroupDetail
                {
                    Name = model.Name,
                        SupportGroupId = Id,
                    Remark = model.Remark,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now

                };

                db.SupportGroupDetails.Add(entity);
                db.SaveChanges();
                model.SupportGroupDetailId = entity.SupportGroupDetailId;
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupportGroupDetails_Update([DataSourceRequest]DataSourceRequest request, SupportGroupDetail model)
        {
            var Groups = db.SupportGroupDetails.Where(x => x.SupportGroupDetailId == model.SupportGroupDetailId).FirstOrDefault();


           
            Groups.Name = model.Name;
            Groups.Remark = model.Remark;
            Groups.LastModified = DateTime.Now;
            Groups.ModifiedBy = User.Identity.Name;


            db.SupportGroupDetails.Attach(Groups);
            db.Entry(Groups).State = EntityState.Modified;
            db.SaveChanges();


            return Json(new[] { Groups }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupportGroupDetails_Destroy([DataSourceRequest]DataSourceRequest request, SupportGroupDetail model)
        {
            var Memeber = db.SupportGroupDetails.Where(x=>x.SupportGroupDetailId == model.SupportGroupDetailId  ).FirstOrDefault();
                   db.SupportGroupDetails.Attach(Memeber);
                   db.SupportGroupDetails.Remove(Memeber);
                   db.SaveChanges();
                   UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Support Groups Member number " + model.SupportGroupDetailId);

             

            return Json(new[] { model }.ToDataSourceResult(request));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

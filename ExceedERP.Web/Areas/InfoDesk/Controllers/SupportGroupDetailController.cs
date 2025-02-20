
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.InfoDesk.Support;
namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    [Authorize]
    public class SupportGroupDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SupportGroupDetails_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            IQueryable<SupportGroupDetail_HelpDesk> Groups = db.SupportGroupDetail_HelpDesks.Where(x => x.SupportGroupId == Id);
            DataSourceResult result = Groups.ToDataSourceResult(request);

            return Json(result);
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupportGroupDetails_Create([DataSourceRequest]DataSourceRequest request, SupportGroupDetail_HelpDesk model, int Id)
        {
            var Group = db.SupportGroupDetail_HelpDesks.Where(x => x.EmployeeId == model.EmployeeId && x.SupportGroupId == Id).ToList();
            if (Group.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("SameName", " Name Already Registered ! ");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var entity = new SupportGroupDetail_HelpDesk
                {
                    EmployeeId = model.EmployeeId,
                    SupportGroupId = Id,
                    Remark = model.Remark,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now

                };

                db.SupportGroupDetail_HelpDesks.Add(entity);
                db.SaveChanges();
                model.SupportGroupDetail_HelpDeskId = entity.SupportGroupDetail_HelpDeskId;
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupportGroupDetails_Update([DataSourceRequest]DataSourceRequest request, SupportGroupDetail_HelpDesk model)
        {
            var Groups = db.SupportGroupDetail_HelpDesks.Where(x => x.SupportGroupDetail_HelpDeskId == model.SupportGroupDetail_HelpDeskId).FirstOrDefault();



            Groups.EmployeeId = model.EmployeeId;
            Groups.Remark = model.Remark;
            Groups.LastModified = DateTime.Now;
            Groups.ModifiedBy = User.Identity.Name;


            db.SupportGroupDetail_HelpDesks.Attach(Groups);
            db.Entry(Groups).State = EntityState.Modified;
            db.SaveChanges();


            return Json(new[] { Groups }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SupportGroupDetails_Destroy([DataSourceRequest]DataSourceRequest request, SupportGroupDetail_HelpDesk model)
        {
            var Memeber = db.SupportGroupDetail_HelpDesks.Where(x => x.SupportGroupDetail_HelpDeskId == model.SupportGroupDetail_HelpDeskId).FirstOrDefault();
            db.SupportGroupDetail_HelpDesks.Attach(Memeber);
            db.SupportGroupDetail_HelpDesks.Remove(Memeber);
            db.SaveChanges();
            UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Support Groups Member number " + model.SupportGroupDetail_HelpDeskId);



            return Json(new[] { model }.ToDataSourceResult(request));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

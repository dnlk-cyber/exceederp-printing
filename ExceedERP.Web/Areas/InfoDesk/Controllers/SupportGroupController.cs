
using ExceedERP.Helpers.Common;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.InfoDesk.Support;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{

    [Authorize]
    public class SupportGroupController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext udb = new ApplicationDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["Empoloyees"] = db.Person_Employee.ToList();
            ViewData["Group"] = db.SupportGroup_HelpDesks.ToList();
            var sysusers = udb.Users.Select(o => new
            {
                CustomerID = o.Id,
                CustomerName = o.FirstName
            });
            var users = sysusers.ToList();
            ViewData["SysUsers"] = users;
            var branches = db.Branch.Select(x => new BranchVM { BranchId= x.BranchId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["branches"] = branches;
            var businessUnits = db.BusinessUnits.Select(x => new BussinesUnitVM { BusinessUnitId = x.BusinessUnitId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["businessUnits"] = businessUnits;
          
        }

        public ActionResult Groups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SupportGroup_HelpDesk> Groups = db.SupportGroup_HelpDesks;
            DataSourceResult result = Groups.ToDataSourceResult(request);

            return Json(result);
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Groups_Create([DataSourceRequest]DataSourceRequest request, SupportGroup_HelpDesk model)
        {
            var Group = db.SupportGroup_HelpDesks.Where(x => x.Name == model.Name).ToList();
            if (Group.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("SameName", " Name Already Registered ! ");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var entity = new SupportGroup_HelpDesk
                {
                    Description = model.Description,
                    BranchId = model.BranchId,
                    BusinessUnitId = model.BusinessUnitId,
                    Name = model.Name,
                    Remark = model.Remark,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now

                };

                db.SupportGroup_HelpDesks.Add(entity);
                db.SaveChanges();
                model.SupportGroup_HelpDeskId = entity.SupportGroup_HelpDeskId;
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Groups_Update([DataSourceRequest]DataSourceRequest request, SupportGroup_HelpDesk model)
        {
            var Groups = db.SupportGroup_HelpDesks.Where(x => x.SupportGroup_HelpDeskId == model.SupportGroup_HelpDeskId).FirstOrDefault();


            Groups.Description = model.Description;
            Groups.BranchId = model.BranchId;
            Groups.BusinessUnitId = model.BusinessUnitId;
            Groups.Name = model.Name;
            Groups.Remark = model.Remark;
            Groups.LastModified = DateTime.Now;
            Groups.ModifiedBy = User.Identity.Name;


            db.SupportGroup_HelpDesks.Attach(Groups);
            db.Entry(Groups).State = EntityState.Modified;
            db.SaveChanges();


            return Json(new[] { Groups }.ToDataSourceResult(request, ModelState));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Groups_Destroy([DataSourceRequest]DataSourceRequest request, SupportGroup_HelpDesk model)
        {
            var Memeber = db.SupportGroup_HelpDesks.Where(x => x.SupportGroup_HelpDeskId == model.SupportGroup_HelpDeskId).FirstOrDefault();
            db.SupportGroup_HelpDesks.Attach(Memeber);
            db.SupportGroup_HelpDesks.Remove(Memeber);
            db.SaveChanges();
            UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Support Groups number " + model.SupportGroup_HelpDeskId);



            return Json(new[] { model }.ToDataSourceResult(request));


        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

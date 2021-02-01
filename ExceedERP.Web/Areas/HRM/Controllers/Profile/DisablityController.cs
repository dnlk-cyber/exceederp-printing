
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
   
    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin, HrmConstants.HRProfileUser)]
    public class DisablityController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
    
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]

        public ActionResult Disabilities_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<Disability> disabilities = db.Disabilities.Where(x => x.EmployeeId == id);
            DataSourceResult result = disabilities.ToDataSourceResult(request, disability => new
            {
                DisabilityId = disability.DisabilityId,
                EmployeeId = id,
                Type = disability.Type,
                Remark = disability.Remark,
                DateCreated = disability.DateCreated,
                LastModified = disability.LastModified,
                CreatedBy = disability.CreatedBy,
                ModifiedBy = disability.ModifiedBy

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
     
        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        public ActionResult Disabilities_Create([DataSourceRequest]DataSourceRequest request, Disability disability, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Disability
                {
                    EmployeeId = id,
                    Type = disability.Type,
                    Remark = disability.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = disability.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = disability.ModifiedBy
                };

                db.Disabilities.Add(entity);
                db.SaveChanges();
                disability.DisabilityId = entity.DisabilityId;
            }

            return Json(new[] { disability }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
      
        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        public ActionResult Disabilities_Update([DataSourceRequest]DataSourceRequest request, Disability disability)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Disability
                {
                    DisabilityId = disability.DisabilityId,
                    EmployeeId = disability.EmployeeId,
                    Type = disability.Type,
                    Remark = disability.Remark,
                    DateCreated = disability.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = disability.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.Disabilities.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { disability }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Disabilities_Destroy([DataSourceRequest]DataSourceRequest request, Disability disability)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Disability
                {
                    DisabilityId = disability.DisabilityId,
                    EmployeeId = disability.EmployeeId,
                    Type = disability.Type,
                    Remark = disability.Remark,
                    DateCreated = disability.DateCreated,
                    LastModified = disability.LastModified,
                    CreatedBy = disability.CreatedBy,
                    ModifiedBy = disability.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted disability information of employee Id" + disability.EmployeeId);
                #endregion
                db.Disabilities.Attach(entity);
                db.Disabilities.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { disability }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

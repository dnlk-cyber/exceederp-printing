
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
   
    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser)]
    public class EmployeeTypesController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
    
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]

        public ActionResult EmployeeTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<EmployeeType> types = db.EmployeeTypes;
            DataSourceResult result = types.ToDataSourceResult(request, employeeType => new
            {
                EmployeeTypeId = employeeType.EmployeeTypeId,
                Name = employeeType.Name,
                Remark = employeeType.Remark,
                DateCreated = employeeType.DateCreated,
                LastModified = employeeType.LastModified,
                CreatedBy = employeeType.CreatedBy,
                ModifiedBy = employeeType.ModifiedBy

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
     
        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        public ActionResult EmployeeType_Create([DataSourceRequest]DataSourceRequest request, EmployeeType employeeType)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                //var entity = new EmployeeType
                //{
                //    EmployeeTypeId = employeeType.EmployeeTypeId,
                //    Name = employeeType.Name,
                //    Remark = employeeType.Remark,
                //    DateCreated = DateTime.Now,
                //    LastModified = employeeType.LastModified,
                //    CreatedBy = User.Identity.Name,
                //    ModifiedBy = employeeType.ModifiedBy
                //};

                //db.EmployeeTypes.Add(entity);
                //db.SaveChanges();
                //employeeType.EmployeeTypeId = entity.EmployeeTypeId;
                EmployeeType entity;
                int generatedId;
                do
                {
                    entity = new EmployeeType
                    {
                        Name = employeeType.Name,
                        Remark = employeeType.Remark,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    db.EmployeeTypes.Add(entity);
                    db.SaveChanges(); 

                    generatedId = entity.EmployeeTypeId;

                    // If ID == 2 => delete and try again
                    if (generatedId == 2)
                    {
                        db.EmployeeTypes.Remove(entity);
                        db.SaveChanges();
                    }

                } while (generatedId == 2);//if generated Id is 2 it regenerate it again untill id !=2

                employeeType.EmployeeTypeId = entity.EmployeeTypeId;
            }

            return Json(new[] { employeeType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
      
        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        public ActionResult EmployeeType_Update([DataSourceRequest]DataSourceRequest request, EmployeeType employeeType)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmployeeType
                {
                    EmployeeTypeId = employeeType.EmployeeTypeId,
                    Name = employeeType.Name,
                    Remark = employeeType.Remark,
                    DateCreated = employeeType.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = employeeType.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.EmployeeTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { employeeType }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeType_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeType employeeType)
        {
            ModelState.Clear();
            var isExist = db.Person_Employee.Any(x => x.EmployeeTypeId == employeeType.EmployeeTypeId);
            if (isExist)
            {
                ModelState.AddModelError("", "You can't delete it.It is applied in Employee Profile");
            }
            if (ModelState.IsValid)
            {
                var entity = new EmployeeType
                {
                    EmployeeTypeId = employeeType.EmployeeTypeId,
                    Name = employeeType.Name,
                    Remark = employeeType.Remark,
                    DateCreated = employeeType.DateCreated,
                    LastModified = employeeType.LastModified,
                    CreatedBy = employeeType.CreatedBy,
                    ModifiedBy = employeeType.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee type setting. Id" + employeeType.EmployeeTypeId);
                #endregion
                db.EmployeeTypes.Attach(entity);
                db.EmployeeTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{
           [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class HolidayDefinitionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HolidayDefinitions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<HolidayDefinition> holidaydefinitions = db.HolidayDefinitions;
            DataSourceResult result = holidaydefinitions.ToDataSourceResult(request, holidayDefinition => new {
                Description = holidayDefinition.Description,
                HolidayDefinitionID = holidayDefinition.HolidayDefinitionID,
                Type = holidayDefinition.Type,
                IsFixed = holidayDefinition.IsFixed,
                WillClose = holidayDefinition.WillClose,
                CreatedBy = holidayDefinition.CreatedBy,
                DateCreated = holidayDefinition.DateCreated,
                LastModified = holidayDefinition.LastModified,
                ModifiedBy = holidayDefinition.ModifiedBy,
                OrgBranchName = holidayDefinition.OrgBranchName,
                Remark = holidayDefinition.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HolidayDefinitions_Create([DataSourceRequest]DataSourceRequest request, HolidayDefinition holidayDefinition)
        {
            List<HolidayDefinition> holidayDefinitions = db.HolidayDefinitions.Where(s => s.Description == holidayDefinition.Description).ToList();
            if (holidayDefinitions.Any())
            {
                ModelState.AddModelError("DuplicateEntire", "Holiday Definition with Description Already Exist");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new HolidayDefinition
                    {
                        Description = holidayDefinition.Description,
                        Type = holidayDefinition.Type,
                        IsFixed = holidayDefinition.IsFixed,
                        WillClose = holidayDefinition.WillClose,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = holidayDefinition.Remark
                    };

                    db.HolidayDefinitions.Add(entity);
                    db.SaveChanges();
                    holidayDefinition.HolidayDefinitionID = entity.HolidayDefinitionID;
                }
            }
            return Json(new[] { holidayDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HolidayDefinitions_Update([DataSourceRequest]DataSourceRequest request, HolidayDefinition holidayDefinition)
        {
             List<HolidayDefinition> holidayDefinitions = db.HolidayDefinitions.Where(s => s.Description == holidayDefinition.Description).ToList();
             if (holidayDefinitions.Any())
             {
                 ModelState.AddModelError("DuplicateEntire", "Holiday Definition with Description Already Exist");
             }
             else
             {
                 if (ModelState.IsValid)
                 {
                     var entity = new HolidayDefinition
                     {
                         HolidayDefinitionID = holidayDefinition.HolidayDefinitionID,
                         Description = holidayDefinition.Description,
                         Type = holidayDefinition.Type,
                         IsFixed = holidayDefinition.IsFixed,
                         WillClose = holidayDefinition.WillClose,
                         Remark = holidayDefinition.Remark
                     };

                     db.HolidayDefinitions.Attach(entity);
                     db.Entry(entity).State = EntityState.Modified;
                     db.SaveChanges();
                 }
             }
            return Json(new[] { holidayDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HolidayDefinitions_Destroy([DataSourceRequest]DataSourceRequest request, HolidayDefinition holidayDefinition)
        {
            if (ModelState.IsValid)
            {
                var entity = new HolidayDefinition
                {
                    HolidayDefinitionID = holidayDefinition.HolidayDefinitionID,
                    Type = holidayDefinition.Type,
                    IsFixed = holidayDefinition.IsFixed,
                    WillClose = holidayDefinition.WillClose,
                    Remark = holidayDefinition.Remark
                };

                db.HolidayDefinitions.Attach(entity);
                db.HolidayDefinitions.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Holiday Definition  number " + holidayDefinition.HolidayDefinitionID);
		
            }

            return Json(new[] { holidayDefinition }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}


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
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
   
    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser)]
    public class EmployeeAssociationController : BaseController
    {
        // GET: HRM/EmployeeExperience
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {

            return View();
        }
    
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin, HrmConstants.HRProfileUser)]

        public ActionResult EmployeeAssociation_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<EmployeeAssociation> emergencies = db.EmployeeAssociation.Where(c => c.EmployeeId == id);
            DataSourceResult result = emergencies.ToDataSourceResult(request, emergency => new
            {
                AssociationId = emergency.AssociationId,
                Name = emergency.Name,
                LookupId = emergency.LookupId,
                EmployeeId = emergency.EmployeeId,
                DateOfBirth = emergency.DateOfBirth,
                DateOfBirthAm = emergency.DateOfBirthAm,
                Remark = emergency.Remark,
                DateCreated = emergency.DateCreated,
                LastModified = emergency.LastModified,
                CreatedBy = emergency.CreatedBy,
                ModifiedBy = emergency.ModifiedBy
            });

            return Json(result);
        }
      
        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeAssociation_Create([DataSourceRequest]DataSourceRequest request, EmployeeAssociation emergency, int id)
        {
            ModelState.Clear();
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                // Ethiopian Date Parsing (Amharic)
                if (!string.IsNullOrWhiteSpace(emergency.DateOfBirthAm))
                {
                    emergency.DateOfBirth = HrHelper.ParseEthiopicDate(emergency.DateOfBirthAm);
                }
               

               
            }
            else if(emergency.DateOfBirth.HasValue)
            {
                // Convert Gregorian to Amharic format
                emergency.DateOfBirthAm = XAPI.EthiopicDateTime.GetEthiopicDate(
                    emergency.DateOfBirth.Value.Day,
                    emergency.DateOfBirth.Value.Month,
                    emergency.DateOfBirth.Value.Year
                );

               
            }
            if (ModelState.IsValid)
            {
                var entity = new EmployeeAssociation
                {
                    AssociationId = emergency.AssociationId,
                    Name = emergency.Name,
                    LookupId = emergency.LookupId,
                    DateOfBirth = emergency.DateOfBirth,
                    DateOfBirthAm = emergency.DateOfBirthAm,
                    EmployeeId = id,

                    Remark = emergency.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = emergency.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = emergency.ModifiedBy
                };

                db.EmployeeAssociation.Add(entity);
                db.SaveChanges();
                emergency.AssociationId = entity.AssociationId;
            }

            return Json(new[] { emergency }.ToDataSourceResult(request, ModelState));
        }
    
        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeAssociation_Update([DataSourceRequest]DataSourceRequest request, EmployeeAssociation emergency)
        {
            ModelState.Clear();
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                // Ethiopian Date Parsing (Amharic)
                if (!string.IsNullOrWhiteSpace(emergency.DateOfBirthAm))
                {
                    emergency.DateOfBirth = HrHelper.ParseEthiopicDate(emergency.DateOfBirthAm);
                }



            }
            else if (emergency.DateOfBirth.HasValue)
            {
                // Convert Gregorian to Amharic format
                emergency.DateOfBirthAm = XAPI.EthiopicDateTime.GetEthiopicDate(
                    emergency.DateOfBirth.Value.Day,
                    emergency.DateOfBirth.Value.Month,
                    emergency.DateOfBirth.Value.Year
                );


            }
            if (ModelState.IsValid)
            {
                var entity = new EmployeeAssociation
                {
                    AssociationId = emergency.AssociationId,
                    Name = emergency.Name,
                    LookupId = emergency.LookupId,
                    EmployeeId = emergency.EmployeeId,
                    Remark = emergency.Remark,
                    DateOfBirth = emergency.DateOfBirth,
                    DateOfBirthAm = emergency.DateOfBirthAm,
                    DateCreated = emergency.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = emergency.ModifiedBy
                };

                db.EmployeeAssociation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { emergency }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeAssociation_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeAssociation emergency)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmployeeAssociation
                {
                    AssociationId = emergency.AssociationId,
                    Name = emergency.Name,
                    LookupId = emergency.LookupId,
                    EmployeeId = emergency.EmployeeId,
                    Remark = emergency.Remark,
                    DateOfBirth = emergency.DateOfBirth,
                    DateOfBirthAm = emergency.DateOfBirthAm,
                    DateCreated = emergency.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = emergency.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted assocation information of employee Id" + emergency.EmployeeId);
                #endregion
                db.EmployeeAssociation.Attach(entity);
                db.EmployeeAssociation.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { emergency }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
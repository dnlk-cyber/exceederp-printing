
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    [Authorize(Roles = HR_Profile_Admin + "," + HR_Profile_User + "," + HR_Admin)]
    public class EmployeeCourtAssociationController : BaseController
    {
        // GET: HRM/EmployeeExperience
          private ExceedDbContext db = new ExceedDbContext();
          private const string HR_Profile_Admin = "HR_Profile_Admin";
          private const string HR_Profile_User = "HR_Profile_User";
          private const string HR_Admin = "HR_Admin";
          public static string DobAmharic { get; set; }
          public static DateTime Dob { get; set; }
        public ActionResult Index()
        {
          
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateOfBirthAmharic")
            {
                DobAmharic = value;
                var splitDate = DobAmharic.Split('/');
                Dob = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
           

        }
         [Authorize(Roles = HR_Profile_Admin + "," + HR_Profile_User + "," + HR_Admin)]
        public ActionResult EmployeeAssociation_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<EmployeeCourtAssociation> emergencies = db.EmployeeCourtAssociation.Where(c => c.EmployeeId == id);
            DataSourceResult result = emergencies.ToDataSourceResult(request, emergency => new
            {
                AssociationId = emergency.AssociationId,
                Name = emergency.Name ,
                AssociationType = emergency.AssociationType,
                EmployeeId = emergency.EmployeeId,
                DateOfBirth = emergency.DateOfBirth,
                Gender = emergency.Gender,
                Remark = emergency.Remark,
                DateCreated = emergency.DateCreated,
                LastModified = emergency.LastModified,
                CreatedBy = emergency.CreatedBy,
                ModifiedBy = emergency.ModifiedBy,
                DateOfBirthAmharic = emergency.DateOfBirthAmharic,
            });

            return Json(result);
        }
          [Authorize(Roles = HR_Profile_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeAssociation_Create([DataSourceRequest]DataSourceRequest request, EmployeeCourtAssociation emergency, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (Dob.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Date of birth date should be less or equal to the current date!!");
                }
                else if (DobAmharic== null)
                {

                    ModelState.AddModelError("", "Date of birth date value is mandatory!!");
                }
                else
                {
                    var entity = new EmployeeCourtAssociation
                    {
                        AssociationId = emergency.AssociationId,
                        Name = emergency.Name,
                        AssociationType = emergency.AssociationType,
                        EmployeeId = id,
                        DateOfBirth = Dob,
                        Gender = emergency.Gender,
                        Remark = emergency.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = emergency.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = emergency.ModifiedBy,
                        DateOfBirthAmharic = DobAmharic,
                    };

                    db.EmployeeCourtAssociation.Add(entity);
                    db.SaveChanges();
                    emergency.AssociationId = entity.AssociationId;
                }
            }

            return Json(new[] { emergency }.ToDataSourceResult(request, ModelState));
        }
          [Authorize(Roles = HR_Profile_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeAssociation_Update([DataSourceRequest]DataSourceRequest request, EmployeeCourtAssociation emergency)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DobAmharic == null)
                {
                    DobAmharic = emergency.DateOfBirthAmharic;
                    var splitDate = DobAmharic.Split('/');
                    Dob = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (Dob.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Date of birth date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new EmployeeCourtAssociation
                    {
                        AssociationId = emergency.AssociationId,
                        Name = emergency.Name,
                        AssociationType = emergency.AssociationType,
                        EmployeeId = emergency.EmployeeId,
                        Remark = emergency.Remark,
                        DateCreated = emergency.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = emergency.ModifiedBy,
                        DateOfBirth = Dob,
                        Gender = emergency.Gender,
                        DateOfBirthAmharic = DobAmharic,
                    };

                    db.EmployeeCourtAssociation.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { emergency }.ToDataSourceResult(request, ModelState));
        }
         [Authorize(Roles = HR_Profile_Admin + "," + HR_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeAssociation_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeCourtAssociation emergency)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmployeeCourtAssociation
                {
                    AssociationId = emergency.AssociationId,
                    Name = emergency.Name,
                    AssociationType = emergency.AssociationType,
                    EmployeeId = emergency.EmployeeId,                 
                    Remark = emergency.Remark,
                    DateCreated = emergency.DateCreated,
                    LastModified = DateTime.Now ,
                    CreatedBy = emergency.CreatedBy,
                    ModifiedBy = User .Identity .Name ,
                    DateOfBirth = emergency.DateOfBirth,
                    Gender = emergency.Gender,
                    DateOfBirthAmharic = emergency.DateOfBirthAmharic,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted assocation information of employee Id" + emergency.EmployeeId);
                #endregion
                db.EmployeeCourtAssociation.Attach(entity);
                db.EmployeeCourtAssociation.Remove(entity);
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
using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class CompanyEmailController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string DateAmahic { get; set; }
        public static DateTime Date { get; set; }
        public ActionResult Index()
        {


            return View();
        }


        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult CompanyEmail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<CompanyEmail> CompanyEmails = db.CompanyEmails.Where(x => x.MainCompanyId == id);
            DataSourceResult result = CompanyEmails.ToDataSourceResult(request, CompanyEmail => new
            {
                CompanyEmailId = CompanyEmail.CompanyEmailId,
                MainCompanyId = CompanyEmail.MainCompanyId,
                EmailAddress = CompanyEmail.EmailAddress,
                Password = CompanyEmail.Password,
                ServerType = CompanyEmail.ServerType,

            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyEmail_Create([DataSourceRequest]DataSourceRequest request, CompanyEmail CompanyEmail, int idTwo)
        {

            if (ModelState.IsValid)
            {
                var entity = new CompanyEmail
                {
                    MainCompanyId = idTwo,
                    CompanyEmailId = CompanyEmail.CompanyEmailId,
                    EmailAddress = CompanyEmail.EmailAddress,
                   // Password = Encrypt.EncryptString(CompanyEmail.EmailAddress, CompanyEmail.Password),
                    ServerType = CompanyEmail.ServerType,
                };
                if (entity.EmailAddress.Contains("gmail"))
                {
                    entity.ServerType = "smtp.gmail.com";
                }
                if (entity.EmailAddress.Contains("yahoo"))
                {
                    entity.ServerType = "smtp.mail.yahoo.com";
                }
                db.CompanyEmails.Add(entity);
                db.SaveChanges();
                CompanyEmail.CompanyEmailId = entity.CompanyEmailId;

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                
                    UserHelper.OperationLog(Request.UserHostAddress, "Company Email ", "Success", User.Identity.Name, User.Identity.Name + " Add new Company email " + entity.EmailAddress,Modules.HRM);
                

                #endregion

            }

            return Json(new[] { CompanyEmail }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyEmail_Update([DataSourceRequest]DataSourceRequest request, CompanyEmail CompanyEmail)
        {

            if (ModelState.IsValid)
            {


                var entity = new CompanyEmail
                {
                    CompanyEmailId = CompanyEmail.CompanyEmailId,
                    EmailAddress = CompanyEmail.EmailAddress,
                    Password = CompanyEmail.Password,
                    MainCompanyId = CompanyEmail.MainCompanyId,
                    ServerType = CompanyEmail.ServerType,
                };

                db.CompanyEmails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Company Email ", "Success", User.Identity.Name, User.Identity.Name + " Updated Company email " + entity.EmailAddress,Modules.HRM);
                

                #endregion
            }

            return Json(new[] { CompanyEmail }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyEmail_Destroy([DataSourceRequest]DataSourceRequest request, CompanyEmail CompanyEmail)
        {
            if (ModelState.IsValid)
            {
                var entity = new CompanyEmail
                {
                    CompanyEmailId = CompanyEmail.CompanyEmailId,
                    EmailAddress = CompanyEmail.EmailAddress,
                    Password = CompanyEmail.Password,
                    MainCompanyId = CompanyEmail.MainCompanyId,
                    ServerType = CompanyEmail.ServerType,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Company Email Address"  + CompanyEmail.EmailAddress);
                #endregion
                db.CompanyEmails.Attach(entity);
                db.CompanyEmails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { CompanyEmail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

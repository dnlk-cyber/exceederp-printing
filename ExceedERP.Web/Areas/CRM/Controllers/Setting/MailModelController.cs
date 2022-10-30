using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using System.Net.Mail;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class MailModelController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

       public ActionResult MailModels_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MailModel> mailmodels = db.MailModels;
            DataSourceResult result = mailmodels.ToDataSourceResult(request, mailModel => new
            {
                MailModelID = mailModel.MailModelID,
                From = mailModel.From,
                To = mailModel.To,
                Subject = mailModel.Subject,
                CreatedBy = mailModel.CreatedBy,
                DateCreated = mailModel.DateCreated,
                LastModified = mailModel.LastModified,
                ModifiedBy = mailModel.ModifiedBy,
                OrgBranchName = mailModel.OrgBranchName,

                Body = mailModel.Body
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MailModels_Create([DataSourceRequest]DataSourceRequest request, MailModel mailModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new MailModel
                {
                    From = mailModel.From,
                    To = mailModel.To,
                    Subject = mailModel.Subject,
                    Body = mailModel.Body
                };
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.To.Add(mailModel.To);
                mail.From = new MailAddress(mailModel.From);
                mail.Subject = mailModel.Subject;
                string Body = mailModel.Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("minaleh", "adobinadobinhhh");
                try
                {
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {

                }
            }

            return Json(new[] { mailModel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MailModels_Update([DataSourceRequest]DataSourceRequest request, MailModel mailModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new MailModel
                {
                    MailModelID = mailModel.MailModelID,
                    From = mailModel.From,
                    To = mailModel.To,
                    Subject = mailModel.Subject,
                    Body = mailModel.Body
                };

                db.MailModels.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { mailModel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MailModels_Destroy([DataSourceRequest]DataSourceRequest request, MailModel mailModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new MailModel
                {
                    MailModelID = mailModel.MailModelID,
                    From = mailModel.From,
                    To = mailModel.To,
                    Subject = mailModel.Subject,
                    Body = mailModel.Body
                };

                db.MailModels.Attach(entity);
                db.MailModels.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { mailModel }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

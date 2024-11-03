using System;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser,HrmConstants.HRAdmin)]
    public class AnnouncementController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string DateAmharic { get; set; }
        public static DateTime Date { get; set; }
        public ActionResult Index()
        {
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateAnnAmharic")
            {
                DateAmharic = value;
                var splitDate = DateAmharic.Split('/');
                Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
          
        }
        public ActionResult Announcmentes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Announcment> announcmentes = db.Announcmentes.OrderBy (x=> x.Date );
            DataSourceResult result = announcmentes.ToDataSourceResult(request, announcment => new {
                AnnouncmentId = announcment.AnnouncmentId,
                Title = announcment.Title,
                Date = announcment.Date,
                DateAnnAmharic = announcment.DateAnnAmharic,
                Remark = announcment.Remark,
                OrgBranchName = announcment.OrgBranchName,
                DateCreated = announcment.DateCreated,
                LastModified = announcment.LastModified,
                CreatedBy = announcment.CreatedBy,
                ModifiedBy = announcment.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Announcmentes_Create([DataSourceRequest]DataSourceRequest request, Announcment announcment)
        {
            ModelState.Clear();
           
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    ModelState.AddModelError("", "Date value is mandatory!!");
                }
                else
                {
                    var entity = new Announcment
                    {
                        Title = announcment.Title,
                        Date = Date,
                        DateAnnAmharic = DateAmharic,
                        Remark = announcment.Remark,
                        OrgBranchName = announcment.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = announcment.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = announcment.ModifiedBy
                    };

                    db.Announcmentes.Add(entity);
                    db.SaveChanges();
                    announcment.AnnouncmentId = entity.AnnouncmentId;
                }
            }

            return Json(new[] { announcment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Announcmentes_Update([DataSourceRequest]DataSourceRequest request, Announcment announcment)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    DateAmharic = announcment.DateAnnAmharic;
                    var splitDate = DateAmharic.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                var entity = new Announcment
                {
                    AnnouncmentId = announcment.AnnouncmentId,
                    Title = announcment.Title,
                    Date = Date,
                    DateAnnAmharic = DateAmharic,
                    Remark = announcment.Remark,
                    OrgBranchName = announcment.OrgBranchName,
                    DateCreated = announcment.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = announcment.CreatedBy,
                    ModifiedBy = User.Identity .Name 
                };

                db.Announcmentes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { announcment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Announcmentes_Destroy([DataSourceRequest]DataSourceRequest request, Announcment announcment)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Announcment
                {
                    AnnouncmentId = announcment.AnnouncmentId,
                    Title = announcment.Title,
                    Date = announcment.Date,
                    DateAnnAmharic = announcment.DateAnnAmharic,
                    Remark = announcment.Remark,
                    OrgBranchName = announcment.OrgBranchName,
                    DateCreated = announcment.DateCreated,
                    LastModified = announcment.LastModified,
                    CreatedBy = announcment.CreatedBy,
                    ModifiedBy = announcment.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Announcment " + announcment.Title);
                #endregion
                db.Announcmentes.Attach(entity);
                db.Announcmentes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { announcment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

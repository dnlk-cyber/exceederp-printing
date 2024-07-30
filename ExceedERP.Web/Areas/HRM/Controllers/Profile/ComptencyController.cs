
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
    public class ComptencyController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string FromAmharic { get; set; }
        public static string ToAmharic { get; set; }
        public static DateTime From { get; set; }
        public static DateTime To { get; set; }
        public ActionResult Index()
        {
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "FromComAm")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToCompAm")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }

        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
        public ActionResult Competencies_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<Competency> competencies = db.Competencies.Where(x => x.EmployeeId == id);
            DataSourceResult result = competencies.ToDataSourceResult(request, competency => new
            {
                EmployeeId = id,
                CompetencyId = competency.CompetencyId,
                Level = competency.Level,
                Status = competency.Status,
                From = competency.From,
                To = competency.To,
                LookupId = competency.LookupId,
                Remark = competency.Remark,
                Organization = competency.Organization,
                DateCreated = competency.DateCreated,
                LastModified = competency.LastModified,
                CreatedBy = competency.CreatedBy,
                ModifiedBy = competency.ModifiedBy,
                FromComAm = competency.FromComAm,
                ToCompAm = competency.ToCompAm,
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Competencies_Create([DataSourceRequest]DataSourceRequest request, Competency competency, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistance = db.Competencies.Where(x => x.LookupId == competency.LookupId && x.EmployeeId == id).FirstOrDefault();
                //if (checkexistance != null)
                //{
                //    ModelState.AddModelError("", "This skill type is already registered!!");
                //}
                if (FromAmharic == null || ToAmharic == null)
                {
                    ModelState.AddModelError("", "Date values are mandatory!!");
                }
                else if (From.Date > To.Date)
                {

                    ModelState.AddModelError("", "Start date should be less or equal to end date!!");
                }
                else
                {
                    var entity = new Competency
                    {
                        EmployeeId = id,
                        Level = competency.Level,
                        LookupId = competency.LookupId,
                        Status = competency.Status,
                        From = From,
                        To = To,
                        Organization = competency.Organization,
                        Remark = competency.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = competency.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = competency.ModifiedBy,
                        FromComAm = FromAmharic,
                        ToCompAm = ToAmharic,
                    };

                    db.Competencies.Add(entity);
                    db.SaveChanges();
                    competency.CompetencyId = entity.CompetencyId;
                }
            }

            return Json(new[] { competency }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Competencies_Update([DataSourceRequest]DataSourceRequest request, Competency competency)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (FromAmharic == null)
                {
                    FromAmharic = competency.FromComAm;
                    var splitDate = FromAmharic.Split('/');
                    From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (ToAmharic == null)
                {
                    ToAmharic = competency.ToCompAm;
                    var splitDate = FromAmharic.Split('/');
                    To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (From.Date > To.Date)
                {

                    ModelState.AddModelError("", "Start date should be less or equal to end date!!");
                }
                else
                {
                    var entity = new Competency
                    {
                        CompetencyId = competency.CompetencyId,
                        Level = competency.Level,
                        LookupId = competency.LookupId,
                        Status = competency.Status,
                        From = From,
                        To = To,
                        Organization = competency.Organization,
                        Remark = competency.Remark,
                        EmployeeId = competency.EmployeeId,
                        DateCreated = competency.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = competency.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        FromComAm = FromAmharic,
                        ToCompAm = ToAmharic,
                    };

                    db.Competencies.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { competency }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Competencies_Destroy([DataSourceRequest]DataSourceRequest request, Competency competency)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Competency
                {
                    CompetencyId = competency.CompetencyId,
                    Level = competency.Level,
                    LookupId = competency.LookupId,
                    Status = competency.Status,
                    From = competency.From,
                    To = competency.To,
                    Organization = competency.Organization,
                    Remark = competency.Remark,
                    EmployeeId = competency.EmployeeId,
                    DateCreated = competency.DateCreated,
                    LastModified = competency.LastModified,
                    CreatedBy = competency.CreatedBy,
                    ModifiedBy = competency.ModifiedBy,
                    FromComAm = competency.FromComAm,
                    ToCompAm = competency.ToCompAm,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Competency  of employee Id" + competency.EmployeeId);
                #endregion
                db.Competencies.Attach(entity);
                db.Competencies.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { competency }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

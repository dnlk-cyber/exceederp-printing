
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
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
    public class ComptencyController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
     
        public ActionResult Index()
        {
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "FromComAm")
            {
                var FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                var From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToCompAm")
            {
                var ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                var To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
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
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrWhiteSpace(competency?.FromComAm) ||
                    string.IsNullOrWhiteSpace(competency?.ToCompAm))
                {
                    ModelState.AddModelError("", "from, To and   Clearance Date fields are mandatory");
                    return Json(new[] { competency }.ToDataSourceResult(request, ModelState));
                }

                competency.From = HrHelper.ParseEthiopicDate(competency.FromComAm);
                competency.To = HrHelper.ParseEthiopicDate(competency.ToCompAm);
            }
            else
            {
                competency.FromComAm = XAPI.EthiopicDateTime.GetEthiopicDate(competency.From.Day,
                                                                                        competency.From.Month,
                                                                                        competency.From.Year);
                competency.ToCompAm = XAPI.EthiopicDateTime.GetEthiopicDate(competency.To.Day,
                                                                                    competency.To.Month,
                                                                                    competency.To.Year);
            }
            if (competency.From.Date > competency.To.Date)
            {

                ModelState.AddModelError("", "Start date should be less or equal to end date!!");
                return Json(new[] { competency }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                
                  var entity = new Competency
                  {
                        EmployeeId = id,
                        Level = competency.Level,
                        LookupId = competency.LookupId,
                        Status = competency.Status,
                        From = competency.From,
                        To = competency.To,
                        Organization = competency.Organization,
                        Remark = competency.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = competency.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = competency.ModifiedBy,
                        FromComAm = competency.FromComAm,
                        ToCompAm = competency.ToCompAm,

                  };
                db.Competencies.Add(entity);
                db.SaveChanges();
                competency.CompetencyId = entity.CompetencyId;
                
            }

            return Json(new[] { competency }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Competencies_Update([DataSourceRequest]DataSourceRequest request, Competency competency)
        {
            ModelState.Clear();
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrWhiteSpace(competency?.FromComAm) ||
                    string.IsNullOrWhiteSpace(competency?.ToCompAm))
                {
                    ModelState.AddModelError("", "from, To and   Clearance Date fields are mandatory");
                    return Json(new[] { competency }.ToDataSourceResult(request, ModelState));
                }

                competency.From = HrHelper.ParseEthiopicDate(competency.FromComAm);
                competency.To = HrHelper.ParseEthiopicDate(competency.ToCompAm);
            }
            else
            {
                competency.FromComAm = XAPI.EthiopicDateTime.GetEthiopicDate(competency.From.Day,
                                                                                        competency.From.Month,
                                                                                        competency.From.Year);
                competency.ToCompAm = XAPI.EthiopicDateTime.GetEthiopicDate(competency.To.Day,
                                                                                    competency.To.Month,
                                                                                    competency.To.Year);
            }
            if (competency.From.Date > competency.To.Date)
            {

                ModelState.AddModelError("", "Start date should be less or equal to end date!!");
                return Json(new[] { competency }.ToDataSourceResult(request, ModelState));
            }

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
                    LastModified = DateTime.Now,
                    CreatedBy = competency.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    FromComAm = competency.FromComAm,
                    ToCompAm = competency.ToCompAm ,
                };

                db.Competencies.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                
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

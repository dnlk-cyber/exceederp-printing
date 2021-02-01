
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    public class EmpTrainingEducationController : BaseController
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
            if (name == "FromTrAm")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToAmharic")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
           
        }
        public ActionResult EmpTrainingEducations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<Training> emptrainingeducations = db.EmpTrainingEducations;
            DataSourceResult result = emptrainingeducations.ToDataSourceResult(request, empTrainingEducation => new {
                EmpEduTrainId = empTrainingEducation.EmpEduTrainId,
                EmployeeId = id,
                OrganizationName = empTrainingEducation.OrganizationName,
                OrgAddress = empTrainingEducation.OrgAddress,
                IsTraining = empTrainingEducation.IsTraining,
                From = empTrainingEducation.From,
                To = empTrainingEducation.To,
                Score = empTrainingEducation.Score,
                TotalCost = empTrainingEducation.TotalCost,
                PercentCoverdByOrg = empTrainingEducation.PercentCoverdByOrg,
                Remark = empTrainingEducation.Remark,
                DateCreated = empTrainingEducation.DateCreated,
                LastModified = empTrainingEducation.LastModified,
                CreatedBy = empTrainingEducation.CreatedBy,
                ModifiedBy = empTrainingEducation.ModifiedBy,
                FromTrAm = empTrainingEducation.FromTrAm,
                ToTraAm = empTrainingEducation.ToTraAm

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpTrainingEducations_Create([DataSourceRequest]DataSourceRequest request, Training empTrainingEducation, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (FromAmharic == null)
                {
                    ModelState.AddModelError("", "Date from value is mandatory!!");
                }
                else if (ToAmharic == null)
                {
                    ModelState.AddModelError("", "Date to value is mandatory!!");
                }
                else if (From.Date > To.Date)
                {
                    ModelState.AddModelError("", "Please check the date range, The year of experience should be non negative!!");
                }
                else if (From.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Start date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new Training
                    {
                        OrganizationName = empTrainingEducation.OrganizationName,
                        EmployeeId = id,
                        OrgAddress = empTrainingEducation.OrgAddress,
                        IsTraining = empTrainingEducation.IsTraining,
                        From = From,
                        To = To,
                        Score = empTrainingEducation.Score,
                        TotalCost = empTrainingEducation.TotalCost,
                        PercentCoverdByOrg = empTrainingEducation.PercentCoverdByOrg,
                        Remark = empTrainingEducation.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = empTrainingEducation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = empTrainingEducation.ModifiedBy,
                        FromTrAm = FromAmharic,
                        ToTraAm = ToAmharic
                    };

                    db.EmpTrainingEducations.Add(entity);
                    db.SaveChanges();
                    empTrainingEducation.EmpEduTrainId = entity.EmpEduTrainId;
                }
            }

            return Json(new[] { empTrainingEducation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpTrainingEducations_Update([DataSourceRequest]DataSourceRequest request, Training empTrainingEducation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (FromAmharic == null)
                {
                    FromAmharic = empTrainingEducation.FromTrAm;
                    var splitDate = FromAmharic.Split('/');
                    From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (ToAmharic == null)
                {
                    ToAmharic = empTrainingEducation.ToTraAm;
                    var splitDate = ToAmharic.Split('/');
                    To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (From.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Start date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new Training
                    {
                        EmpEduTrainId = empTrainingEducation.EmpEduTrainId,
                        OrganizationName = empTrainingEducation.OrganizationName,
                        OrgAddress = empTrainingEducation.OrgAddress,
                        IsTraining = empTrainingEducation.IsTraining,
                        From = From,
                        To = To,
                        Score = empTrainingEducation.Score,
                        TotalCost = empTrainingEducation.TotalCost,
                        PercentCoverdByOrg = empTrainingEducation.PercentCoverdByOrg,
                        Remark = empTrainingEducation.Remark,
                        DateCreated = empTrainingEducation.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = empTrainingEducation.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        FromTrAm = FromAmharic,
                        ToTraAm = ToAmharic
                    };

                    db.EmpTrainingEducations.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new[] { empTrainingEducation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpTrainingEducations_Destroy([DataSourceRequest]DataSourceRequest request, Training empTrainingEducation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Training
                {
                    EmpEduTrainId = empTrainingEducation.EmpEduTrainId,
                    OrganizationName = empTrainingEducation.OrganizationName,
                    OrgAddress = empTrainingEducation.OrgAddress,
                    IsTraining = empTrainingEducation.IsTraining,
                    From = empTrainingEducation.From,
                    To = empTrainingEducation.To,
                    Score = empTrainingEducation.Score,
                    TotalCost = empTrainingEducation.TotalCost,
                    PercentCoverdByOrg = empTrainingEducation.PercentCoverdByOrg,
                    Remark = empTrainingEducation.Remark,
                    DateCreated = empTrainingEducation.DateCreated,
                    LastModified = empTrainingEducation.LastModified ,
                    CreatedBy = empTrainingEducation.CreatedBy,
                    ModifiedBy = empTrainingEducation.ModifiedBy ,
                    FromTrAm = empTrainingEducation.FromTrAm,
                    ToTraAm = empTrainingEducation.ToTraAm
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee training information of employee Id" + empTrainingEducation.EmployeeId);
                #endregion
                db.EmpTrainingEducations.Attach(entity);
                db.EmpTrainingEducations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { empTrainingEducation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project_Input
{
    public class ProjectEstimationIndexLabourController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static decimal DeseretAllowance { get; set; }
        //public static decimal IndexFactor { get; set; }
        // GET: Construction/ProjectEstimationIndexLabour
        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;
            DeseretAllowance = 0;

            var desertAllowance =
                db.AllowanceDefinations.Where(x => x.AllowanceType == AllowanceType.DESERT).FirstOrDefault();
            if (desertAllowance != null)
            {
                DeseretAllowance = desertAllowance.Amount;
            }
            //var setting =
            //    db.Configurations.Where(x => x.Category == "Construction" && x.Reference == "INDEX_FaCTOR")
            //        .FirstOrDefault();
            //if (setting != null)
            //{
            //    IndexFactor = Convert.ToDecimal(setting.CurrentValue);
            //}

            return View();
        }

        public ActionResult ProjectEstimationIndexLabour_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<ProjectEstimationIndexLabour> projectestimationindexlabour = db.ProjectEstimationIndexLabour;
            DataSourceResult result = projectestimationindexlabour.ToDataSourceResult(request,
                projectEstimationIndexLabour => new
                {
                    ProjectEstimationIndexLabourId = projectEstimationIndexLabour.ProjectEstimationIndexLabourId,

                    SalaryperMonthMin = projectEstimationIndexLabour.SalaryperMonthMin,
                    SalaryperMonthMax = projectEstimationIndexLabour.SalaryperMonthMax,
                    SalaryperdayMin = projectEstimationIndexLabour.SalaryperdayMin,
                    SalaryperdayMax = projectEstimationIndexLabour.SalaryperdayMax,
                    HourlyAverageSalary = projectEstimationIndexLabour.HourlyAverageSalary,
                    IndexedHourlySalary = projectEstimationIndexLabour.IndexedHourlySalary > 0 ? projectEstimationIndexLabour.IndexedHourlySalary :
                                            projectEstimationIndexLabour.HourlyAverageSalary * projectEstimationIndexLabour.IndexFactory,
                    ApprovedIndexedHourlySalary = projectEstimationIndexLabour.ApprovedIndexedHourlySalary,
                    ProjectJobTitleId = projectEstimationIndexLabour.ProjectJobTitleId,
                    Unit = projectEstimationIndexLabour.Unit,
                    IndexFactory = projectEstimationIndexLabour.IndexFactory,
                    BirrMonth = projectEstimationIndexLabour.BirrMonth,
                    IdealBirrDay = projectEstimationIndexLabour.IdealBirrDay,
                    ActualBirrDay = projectEstimationIndexLabour.ActualBirrDay,
                    IdealBirrDays = projectEstimationIndexLabour.IdealBirrDays,
                    ActualBirrDays = projectEstimationIndexLabour.ActualBirrDays,
                    DesertBirrDays = projectEstimationIndexLabour.DesertBirrDays,
                    NetBirrDays = projectEstimationIndexLabour.NetBirrDays,
                    NetBirrHour = projectEstimationIndexLabour.NetBirrHour,
                    FormHr = projectEstimationIndexLabour.FormHr,

                });

            return Json(result);
        }

        private bool CheckExisatnce(int titleId)
        {
            var labours = db.ProjectEstimationIndexLabour.Where(x => x.ProjectJobTitleId == titleId).FirstOrDefault();
            if (labours != null)
            {
                return true;
            }
            return false;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationIndexLabour_Create([DataSourceRequest] DataSourceRequest request,
            ProjectEstimationIndexLabour projectEstimationIndexLabour)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationIndexLabour();
                if (CheckExisatnce(projectEstimationIndexLabour.ProjectJobTitleId))
                {

                    ModelState.AddModelError("", "This Job title is already registered!!");
                }
                else
                {
                    entity.ProjectEstimationIndexLabourId = projectEstimationIndexLabour.ProjectEstimationIndexLabourId;

                    entity.ProjectJobTitleId = projectEstimationIndexLabour.ProjectJobTitleId;
                    entity.Unit = GetBasicSalary(projectEstimationIndexLabour.ProjectJobTitleId,
                        projectEstimationIndexLabour.Unit);

                    entity.SalaryperMonthMin = this.GetMinMonthlySalary(projectEstimationIndexLabour.ProjectJobTitleId);
                    entity.SalaryperMonthMax = this.GetMaxMonthlySalary(projectEstimationIndexLabour.ProjectJobTitleId);
                    entity.SalaryperdayMin = entity.SalaryperMonthMin / 26;
                    entity.SalaryperdayMax = entity.SalaryperMonthMax / 26;
                    entity.HourlyAverageSalary = (this.GetAverageMonthlySalary(projectEstimationIndexLabour.ProjectJobTitleId)) / (26 * 8);
                    entity.IndexedHourlySalary = entity.HourlyAverageSalary * ConsSettingHelper.GetLabourRateIndexFactor();
                    entity.ApprovedIndexedHourlySalary = entity.ApprovedIndexedHourlySalary > 0 ? entity.ApprovedIndexedHourlySalary : entity.IndexedHourlySalary;

                    entity.IndexFactory = ConsSettingHelper.GetLabourRateIndexFactor();
                    entity.BirrMonth = entity.Unit * entity.IndexFactory;
                    entity.IdealBirrDay = entity.BirrMonth / 24;
                    entity.ActualBirrDay = (entity.IdealBirrDay * 30) / 24;
                    entity.IdealBirrDays = GetPerdimeAmount(entity.Unit);
                    entity.ActualBirrDays = (entity.IdealBirrDays * 30) / 26;
                    entity.DesertBirrDays = (DeseretAllowance / 100) * entity.ActualBirrDays;
                    entity.NetBirrDays = entity.ActualBirrDay + entity.ActualBirrDays + entity.DesertBirrDays;
                    entity.NetBirrHour = entity.NetBirrDays / 8;
                    entity.FormHr = projectEstimationIndexLabour.FormHr;

                    db.ProjectEstimationIndexLabour.Add(entity);
                    db.SaveChanges();
                    projectEstimationIndexLabour.ProjectEstimationIndexLabourId = entity.ProjectEstimationIndexLabourId;
                }
            }

            return Json(new[] { projectEstimationIndexLabour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationIndexLabour_Update([DataSourceRequest] DataSourceRequest request,
            ProjectEstimationIndexLabour projectEstimationIndexLabour)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationIndexLabour();

                entity.ProjectEstimationIndexLabourId = projectEstimationIndexLabour.ProjectEstimationIndexLabourId;

                entity.SalaryperMonthMin = this.GetMinMonthlySalary(projectEstimationIndexLabour.ProjectJobTitleId);
                entity.SalaryperMonthMax = this.GetMaxMonthlySalary(projectEstimationIndexLabour.ProjectJobTitleId);
                entity.SalaryperdayMin = entity.SalaryperMonthMin / 26;
                entity.SalaryperdayMax = entity.SalaryperMonthMax / 26;
                entity.HourlyAverageSalary = (this.GetAverageMonthlySalary(projectEstimationIndexLabour.ProjectJobTitleId)) / (26 * 8);
                entity.IndexedHourlySalary = entity.HourlyAverageSalary * ConsSettingHelper.GetLabourRateIndexFactor();
                entity.ApprovedIndexedHourlySalary = projectEstimationIndexLabour.ApprovedIndexedHourlySalary;

                entity.ProjectJobTitleId = projectEstimationIndexLabour.ProjectJobTitleId;
                entity.Unit = GetBasicSalary(projectEstimationIndexLabour.ProjectJobTitleId, projectEstimationIndexLabour.Unit);
                entity.BirrMonth = entity.Unit * projectEstimationIndexLabour.IndexFactory;
                entity.IdealBirrDay = entity.BirrMonth / 24;
                entity.ActualBirrDay = (entity.IdealBirrDay * 30) / 24;
                entity.IdealBirrDays = GetPerdimeAmount(entity.Unit);
                entity.ActualBirrDays = (entity.IdealBirrDays * 30) / 26;
                entity.DesertBirrDays = (DeseretAllowance / 100) * entity.ActualBirrDays;
                entity.NetBirrDays = entity.ActualBirrDay + entity.ActualBirrDays + entity.DesertBirrDays;
                entity.NetBirrHour = entity.NetBirrDays / 8;
                entity.IndexFactory = projectEstimationIndexLabour.IndexFactory;
                entity.FormHr = projectEstimationIndexLabour.FormHr;


                db.ProjectEstimationIndexLabour.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationIndexLabour }.ToDataSourceResult(request, ModelState));
        }

        private decimal GetPerdimeAmount(decimal salary)
        {
            decimal perdimeAmount = 0;
            var perdimSeting = db.PerdimeSetting.Where(x => salary >= x.Minmum && salary <= x.Maximum).FirstOrDefault();
            if (perdimSeting != null)
            {
                return perdimSeting.Amount;
            }
            return 0;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationIndexLabour_Destroy([DataSourceRequest] DataSourceRequest request,
            ProjectEstimationIndexLabour projectEstimationIndexLabour)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationIndexLabour
                {
                    ProjectEstimationIndexLabourId = projectEstimationIndexLabour.ProjectEstimationIndexLabourId,

                    ProjectJobTitleId = projectEstimationIndexLabour.ProjectJobTitleId,
                    Unit = projectEstimationIndexLabour.Unit,
                    IndexFactory = projectEstimationIndexLabour.IndexFactory,
                    BirrMonth = projectEstimationIndexLabour.BirrMonth,
                    IdealBirrDay = projectEstimationIndexLabour.IdealBirrDay,
                    ActualBirrDay = projectEstimationIndexLabour.ActualBirrDay,
                    IdealBirrDays = projectEstimationIndexLabour.IdealBirrDays,
                    ActualBirrDays = projectEstimationIndexLabour.ActualBirrDays,
                    DesertBirrDays = projectEstimationIndexLabour.DesertBirrDays,
                    NetBirrDays = projectEstimationIndexLabour.NetBirrDays,
                    NetBirrHour = projectEstimationIndexLabour.NetBirrHour,
                    FormHr = projectEstimationIndexLabour.FormHr,


                };

                db.ProjectEstimationIndexLabour.Attach(entity);
                db.ProjectEstimationIndexLabour.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationIndexLabour }.ToDataSourceResult(request, ModelState));
        }

        public decimal GetBasicSalary(int title, decimal salary)
        {
            db = new ExceedDbContext();
            decimal SalaryAmount;
            if (title == 0) return SalaryAmount = salary;
            var struc = db.ProjectJobTitles.Where(
            x => x.ProjectJobTitleId == title).FirstOrDefault();
            if (struc != null)
            {
                SalaryAmount = struc.Salary;
                return SalaryAmount;
            }
            return 0;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public decimal GetAverageMonthlySalary(int jobTitleId)
        {
            db = new ExceedDbContext();

            var currentJobTitle = db.ProjectJobTitles.Where(pjt => pjt.ProjectJobTitleId == jobTitleId).FirstOrDefault();
            decimal averageMonthlySalary = new decimal();
            if (currentJobTitle != null)
            {
                //var allSimilarTitles = db.ProjectJobTitles.Where(pjt => pjt.LookupId == currentJobTitle.LookupId).ToList();
                var allSimilarTitles = db.SalariesJobTitles.Where(pjt => pjt.LookupId == currentJobTitle.LookupId).ToList();
                if (allSimilarTitles.Count() > 0)
                {
                    var salaries = new HashSet<decimal>(allSimilarTitles.Select(t => t.Salary));
                    var min = salaries.Min();
                    var max = salaries.Max();
                    var av = (min + max) / 2;
                    averageMonthlySalary = av;
                }


            }
            return averageMonthlySalary;
        }
        public decimal GetMinMonthlySalary(int jobTitleId)
        {
            db = new ExceedDbContext();

            var currentJobTitle = db.ProjectJobTitles.Where(pjt => pjt.ProjectJobTitleId == jobTitleId).FirstOrDefault();
            decimal minMonthlySalary = new decimal();
            if (currentJobTitle != null)
            {
                var allSimilarTitles = db.ProjectJobTitles.Where(pjt => pjt.LookupId == currentJobTitle.LookupId).ToList();
                var salaries = new HashSet<decimal>(allSimilarTitles.Select(t => t.Salary));
                var min = salaries.Min();
                minMonthlySalary = min;

            }
            return minMonthlySalary;
        }
        public decimal GetMaxMonthlySalary(int jobTitleId)
        {
            db = new ExceedDbContext();

            var currentJobTitle = db.ProjectJobTitles.Where(pjt => pjt.ProjectJobTitleId == jobTitleId).FirstOrDefault();
            decimal maxMonthlySalary = new decimal();
            if (currentJobTitle != null)
            {
                var allSimilarTitles = db.ProjectJobTitles.Where(pjt => pjt.LookupId == currentJobTitle.LookupId).ToList();
                var salaries = new HashSet<decimal>(allSimilarTitles.Select(t => t.Salary));
                var max = salaries.Max();
                maxMonthlySalary = max;

            }
            return maxMonthlySalary;
        }
    }
}
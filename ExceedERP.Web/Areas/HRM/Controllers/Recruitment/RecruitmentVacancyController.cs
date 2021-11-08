using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    
    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
    public class RecruitmentVacancyController : ExceedERP.Web.Controllers.BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string VaccancyCode { get; set; }
        public static int VacancyId { get; set; }
        public static int current { get; set; }
        public static int tobe { get; set; }
        public string FromAmh { get; set; }
        public string ToAmh { get; set; }
        public static DateTime From { get; set; }
        public static DateTime To { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var vacancies = db.VaccancyPlans.ToList();
            ViewData["Vacancy"] = vacancies;
            GetOrganizations();
            VaccancyCode = "";
            List<MainCompanyStructure> workUnits = new List<MainCompanyStructure>();
            var branchCostCenter = db.BranchCostCenter.ToList();

            foreach (var b in branchCostCenter)
            {
                var oneWorkUnit = db.MainCompanyStructures.Where(x => x.OrgStructureId == b.OrgStructureId).FirstOrDefault();
                if (oneWorkUnit != null)
                {
                    workUnits.Add(oneWorkUnit);
                }

            }
            ViewData["workunits"] = workUnits;
            var branch = db.Branch.ToList();
            ViewData["Branches"] = branch;
            var mediacode = db.RecruitmentVacancy.ToList();
            ViewData["MediaCode"] = mediacode;
        }
        public int CheckPostions(int OrgId, int title)
        {
            tobe = 0;
            current = 0;

            var toBepostions = db.JobTitleMapping.Where(x => x.OrgStructureId == OrgId && x.LookupId == title).FirstOrDefault();
            var currentStaff = db.Placements.Where(x => x.OrgStructureId == OrgId && x.LookupId == title && x.isCurrent).ToList().Count;
            int quntity = 0;
            if (toBepostions != null)
            {
                tobe = toBepostions.Quntity;

            }
            return tobe - currentStaff;


        }
        private string GetTitleName(int id)
        {

            try
            {
                db = new ExceedDbContext();
                var name = db.Lookups.Where(x => x.LookupId == id).FirstOrDefault().Description;
                return name;
            }
            catch (Exception)
            {

                return "";
            }
        }
        public void SetVacancy(string vacancy)
        {
            VaccancyCode = "";
            VaccancyCode = vacancy;
            RedirectToAction("Index");
        }
   
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult RecruitmentPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<Vacancy> vacancies = db.VaccancyPlans.Where(x => mybraches.Contains(x.OrgBranchName));
            List<Vacancy> vacancyPlan = new List<Vacancy>();


            #region vv


            //if (VaccancyCode != "" && VaccancyCode != null)
            //{
            //    var recruitmentVac = db.RecruitmentVacancy.Where(x => x.VaccancyCode == VaccancyCode);
            //    foreach (var r in recruitmentVac)
            //    {
            //        var onerec = db.RecruitmentPlans.Where(x => x.RecruitmentPlanId == r.RecruitmentPlanId).FirstOrDefault();
            //        if (onerec != null)
            //        {
            //            RecruitmentPlanList.Add(onerec);
            //        }

            //    }



            //}
            //else
            //{

            //    foreach (var plan in recruitmentplans)
            //    {
            //        plan.Status = PlanStatus(plan.OrgStructureId, plan.LookupId);
            //        if (plan.Status > 1)
            //        {
            //            RecruitmentPlanList.Add(plan);
            //        }
            //    }
            //}


            #endregion
            DataSourceResult result = vacancies.ToDataSourceResult(request, vacancyLists => new
            {
                RecruitmentPlanId = vacancyLists.RecruitmentPlanId,
                //LookupId = vacancyLists.LookupId,
                VacancyType = vacancyLists.VacancyType,

                //OrgStructureId = vacancyLists.OrgStructureId,
                //JobtitleName = vacancyLists.JobtitleName,
                //NoOfVacantPost = vacancyLists.NoOfVacantPost,
                //EduLevel = vacancyLists.EduLevel,
                //RelevantExp = vacancyLists.RelevantExp,
                //SpecialSkill = vacancyLists.SpecialSkill,
                //PlannedSalary = vacancyLists.PlannedSalary,
                //RecruiTime = recruitmentPlan.RecruiTime,
                //RecruitmentType = vacancyLists.RecruitmentType,
                From = vacancyLists.From,
                To = vacancyLists.To,
                //Experience = vacancyLists.Experience,
                //RankId = vacancyLists.RankId,
                Remark = vacancyLists.Remark,
                //Status = vacancyLists.Status,
                DateCreated = vacancyLists.DateCreated,
                LastModified = vacancyLists.LastModified,
                CreatedBy = vacancyLists.CreatedBy,
                ModifiedBy = vacancyLists.ModifiedBy,

                //RecruiTimeAmharic = recruitmentPlan.RecruiTimeAmharic,
                //FromAmharic = vacancyLists.FromAmharic,
                //ToAmharic = vacancyLists.ToAmharic,
                //Check = vacancyLists.Check,
                OrgBranchName = vacancyLists.OrgBranchName,
                BranchId = vacancyLists.BranchId,
                //WorkUnitId = vacancyLists.WorkUnitId,
            });

            return Json(result);
        }
        public ActionResult Recruitment_Read([DataSourceRequest]DataSourceRequest request, RecruitmentVacancy recruitmentVacancy)
        {

            List<VacancyForMediaViewModel> vacancyList = new List<VacancyForMediaViewModel>();

           
            var vacancyPlan = db.VaccancyPlans.Where(x=>x.IsActive).ToList();
            if (vacancyPlan.Any())
            {
                foreach (var vacancy in vacancyPlan)
                {
                    var dd = db.RecruitmentVacancy.Where(x => x.VacancyId == vacancy.VacancyId);
                    if (!dd.Any())
                    {

                        //var recruitmentsPlan =
                        //    db.RecruitmentPlans.FirstOrDefault(x => x.RecruitmentPlanId == vacancy.RecruitmentPlanId);
                        //if (recruitmentsPlan != null)
                        //{
                            VacancyForMediaViewModel job = new VacancyForMediaViewModel();


                            job.VacancyCode = vacancy.VaccancyCode;
                            job.VacancyId = vacancy.VacancyId;
                            job.RecruitmentSource = vacancy.RecruSource;


                        //job.VacancyForMediaId = vacancy.RecruitmentPlanId;
                        //job.RecruitmentPlanId = recruitmentsPlan.RecruitmentPlanId;
                        var look = db.Lookups.FirstOrDefault(x=>x.LookupId == vacancy.JobTitle);
                        if(look != null)
                        {
                            job.JobtitleName = look.Description;
                        }

                        var department = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == vacancy.Department);
                        if (department != null)
                        {
                            job.Department = department.Name;
                        }

                        job.RecruitmentType = vacancy.VacancyType;
                            job.NoOfVacantPost = vacancy.NoOfVacant;
                            job.FromAmharic = vacancy.FromVacAm;
                            job.ToAmharic = vacancy.ToVacAm;

                            vacancyList.Add(job);

                       // }
                    }
                }
            }

            




            DataSourceResult result = vacancyList.AsQueryable().ToDataSourceResult(request, vacancyLists => new
            {
                VacancyForMediaId = vacancyLists.VacancyForMediaId,
                JobtitleName = vacancyLists.JobtitleName,
                RecruitmentType = vacancyLists.RecruitmentType,
                NoOfVacantPost = vacancyLists.NoOfVacantPost,
               // RecruitmentPlanId = vacancyLists.RecruitmentPlanId,
                VacancyId = vacancyLists.VacancyId,
               // LookupId = vacancyLists.LookupId,
                VacancyCode = vacancyLists.VacancyCode,
                Department = vacancyLists.Department,
                FromAmharic = vacancyLists.FromAmharic,
                ToAmharic = vacancyLists.ToAmharic,
                RecruitmentSource = vacancyLists.RecruitmentSource

            });

            return Json(result);
        }
        private int PlanStatus(int orgid, int id)
        {
            var toBepostions = db.JobTitleMapping.Where(x => x.OrgStructureId == orgid && x.LookupId == id).FirstOrDefault();
            var currentStaff = db.Placements.Where(x => x.OrgStructureId == orgid && x.LookupId == id).ToList().Count;
            int quntity = 0;
            if (toBepostions != null)
            {
                quntity = toBepostions.Quntity;


            }
            return quntity - currentStaff;
        }
        public ActionResult RecruitmentVacancy_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<RecruitmentVacancy> recruitmentvacancy = db.RecruitmentVacancy.Where(x => mybraches.Contains(x.OrgBranchName));
            DataSourceResult result = recruitmentvacancy.ToDataSourceResult(request, recruitmentVacancy => new
            {
                RecruitmentVacancyId = recruitmentVacancy.RecruitmentVacancyId,
                Remark = recruitmentVacancy.Remark,
                VacancyId = recruitmentVacancy.VacancyId,
                OrgBranchName = recruitmentVacancy.OrgBranchName,
                DateCreated = recruitmentVacancy.DateCreated,
                LastModified = recruitmentVacancy.LastModified,
                CreatedBy = recruitmentVacancy.CreatedBy,
                ModifiedBy = recruitmentVacancy.ModifiedBy,
                VaccancyCode = recruitmentVacancy.MediaVacancyCode
            });

            return Json(result);
        }

        public ActionResult VacancyMediaCode_Read([DataSourceRequest] DataSourceRequest request)
        {

            List<VacancyForMediaViewModel> recruitmentvacancy = new List<VacancyForMediaViewModel>();

            var recruitmentVacancyMedia = db.RecruitmentVacancy.ToList();
            var jobtitle = "";

            foreach (var plan in recruitmentVacancyMedia)
            {
                VacancyForMediaViewModel vacancyForMedia = new VacancyForMediaViewModel();


                vacancyForMedia.MediaVacancyCode = plan.MediaVacancyCode;
                vacancyForMedia.VacancyId = plan.VacancyId;
                vacancyForMedia.VacancyCode = plan.Vacancy.VaccancyCode;
                var look = db.Lookups.FirstOrDefault(x => x.LookupId == plan.Vacancy.JobTitle);
                if (look != null)
                {
                    vacancyForMedia.JobtitleName = look.Description;
                }

             
                //var vacancy = db.VaccancyPlans.FirstOrDefault(x=>x.VacancyId == plan.VacancyId);
                //if(vacancy != null)
                //{
                //    var recPlan = db.RecruitmentPlans.FirstOrDefault(x=>x.RecruitmentPlanId == vacancy.RecruitmentPlanId);
                //    if(recPlan != null)
                //    {
                //        vacancyForMedia.JobtitleName = recPlan.JobtitleName;
                //    }
                //}

                vacancyForMedia.DateCreated = plan.DateCreated;
                recruitmentvacancy.Add(vacancyForMedia);


            }



            DataSourceResult result = recruitmentvacancy.ToDataSourceResult(request, recruitmentVacancy => new
            {

                Remark = recruitmentVacancy.Remark,
                VacancyId = recruitmentVacancy.VacancyId,
                MediaVacancyCode = recruitmentVacancy.MediaVacancyCode,
                VacancyCode = recruitmentVacancy.VacancyCode,
                JobtitleName = recruitmentVacancy.JobtitleName,
                DateCreated = recruitmentVacancy.DateCreated
            });
            return Json(result);
        }
        public JsonResult GetOrganizations()
        {

            var structres = db.MainCompanyStructures.AsQueryable();
            ViewData["organization"] = structres;
            return Json(structres.Select(s => new { OrgStructureId = s.OrgStructureId, Name = s.Name }), JsonRequestBehavior.AllowGet);
        }
     
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecruitmentVacancy_Create([DataSourceRequest]DataSourceRequest request, RecruitmentVacancy recruitmentVacancy)
        {
            if (ModelState.IsValid)
            {
                var entity = new RecruitmentVacancy
                {
                    VacancyId = recruitmentVacancy.VacancyId,
                    Remark = recruitmentVacancy.Remark,
                    DateCreated = recruitmentVacancy.DateCreated,
                    LastModified = recruitmentVacancy.LastModified,
                    CreatedBy = recruitmentVacancy.CreatedBy,

                    ModifiedBy = recruitmentVacancy.ModifiedBy,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                };

                db.RecruitmentVacancy.Add(entity);
                db.SaveChanges();
                recruitmentVacancy.RecruitmentVacancyId = entity.RecruitmentVacancyId;
            }

            return Json(new[] { recruitmentVacancy }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<VacancyForMediaViewModel> RecruitmentVacancies)
        {
            ModelState.Clear();
            var results = new List<Vacancy>();

            RecruitmentVacancy RecruitmentVacancy = new RecruitmentVacancy();

            ExceedDbContext dbs = new ExceedDbContext();

            int recruitmentMediaCode = db.VaccancyPlans.Max(x => x.VacancyId);
           // List<RecruitmentVacancy> checking = db.RecruitmentVacancy.ToList();


            var recruitmentVacancies = RecruitmentVacancies as VacancyForMediaViewModel[] ?? RecruitmentVacancies.Where(x=>x.Check).ToArray();
            var recruitment = recruitmentVacancies.FirstOrDefault();
            if (recruitment != null && recruitment.RecruitmentSource == RecruitmentSource.EXTERNAL)
            {

                var aaa = recruitmentVacancies.Where(x => x.RecruitmentSource == RecruitmentSource.INTERNAL);
                if (aaa.Any())
                {
                    ModelState.AddModelError("", "Internal and External Vacancy Cannot have the same Media Code");
                }
            }

            if (recruitment != null && recruitment.RecruitmentSource == RecruitmentSource.INTERNAL)
            {
                var aaa = recruitmentVacancies.Where(x => x.RecruitmentSource == RecruitmentSource.EXTERNAL);
                if (aaa.Any())
                {
                    ModelState.AddModelError("", "Internal and External Vacancy Cannot have the same Media Code");
                }
            }




            if (RecruitmentVacancies != null && ModelState.IsValid)
            {
                int cc = db.RecruitmentVacancy.Count() + 1;
                int increment = recruitmentMediaCode + 1;


                string code = "VAC-" + cc + "-" + DateTime.Now.Year;
                
                foreach (var product in recruitmentVacancies)
                {


                    if (product.Check)
                    {

                        //FromAmh = product.FromAmharic;
                        //var splitDateFrom = FromAmh.Split('/');
                        //From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateFrom[0]), Convert.ToInt32(splitDateFrom[1]), Convert.ToInt32(splitDateFrom[2]));
                        //ToAmh = product.ToAmharic;
                        //var splitDateTo = ToAmh.Split('/');
                        //To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));


                        RecruitmentVacancy = new RecruitmentVacancy();
                        RecruitmentVacancy.VacancyId = product.VacancyId;
                        RecruitmentVacancy.CreatedBy = User.Identity.Name;
                        RecruitmentVacancy.DateCreated = DateTime.Now;
                        RecruitmentVacancy.ModifiedBy = null;
                        UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        RecruitmentVacancy.LastModified = null;
                        RecruitmentVacancy.MediaVacancyCode = code;
                        RecruitmentVacancy.FromAmh = product.FromAmharic;
                        RecruitmentVacancy.ToAmh = product.ToAmharic;
                        RecruitmentVacancy.From = product.From;
                        RecruitmentVacancy.To = product.To;



                        db.RecruitmentVacancy.Add(RecruitmentVacancy);
                        db.SaveChanges();

                        #region Notification


                        var userRole = UserInformationHelper.GetUsersInRole(HrmConstants.HRAdmin)
                            .FirstOrDefault();

                        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                       
                        
                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(empId),
                                Title = "New Vacancy Announcement",
                                Message = "There is New Vacancies"
                            };

                            Notify.NotifyUser(notif);                        

                        #endregion

                    }



                }

            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
        
        [AuthorizeRoles( HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecruitmentVacancy_Update([DataSourceRequest]DataSourceRequest request, RecruitmentVacancy recruitmentVacancy)
        {
            if (ModelState.IsValid)
            {
                var entity = new RecruitmentVacancy
                {
                    RecruitmentVacancyId = recruitmentVacancy.RecruitmentVacancyId,
                    VacancyId = recruitmentVacancy.VacancyId,
                    Remark = recruitmentVacancy.Remark,
                    OrgBranchName = recruitmentVacancy.OrgBranchName,
                    DateCreated = recruitmentVacancy.DateCreated,
                    LastModified = recruitmentVacancy.LastModified,
                    CreatedBy = recruitmentVacancy.CreatedBy,
                    ModifiedBy = recruitmentVacancy.ModifiedBy,

                };

                db.RecruitmentVacancy.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { recruitmentVacancy }.ToDataSourceResult(request, ModelState));
        }
       
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecruitmentVacancy_Destroy([DataSourceRequest]DataSourceRequest request, RecruitmentVacancy recruitmentVacancy)
        {
            if (ModelState.IsValid)
            {
                var entity = new RecruitmentVacancy
                {
                    RecruitmentVacancyId = recruitmentVacancy.RecruitmentVacancyId,
                    VacancyId = recruitmentVacancy.VacancyId,
                    Remark = recruitmentVacancy.Remark,
                    OrgBranchName = recruitmentVacancy.OrgBranchName,
                    DateCreated = recruitmentVacancy.DateCreated,
                    LastModified = recruitmentVacancy.LastModified,
                    CreatedBy = recruitmentVacancy.CreatedBy,
                    ModifiedBy = recruitmentVacancy.ModifiedBy,

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted recruitment vacancy ");
                #endregion
                db.RecruitmentVacancy.Attach(entity);
                db.RecruitmentVacancy.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { recruitmentVacancy }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

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

namespace ExceedERP.Web.Areas.Construction.Controllers.Performance
{
    public class MonthlyProjectCommunicationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult MonthlyProjectCommunication_Read([DataSourceRequest]DataSourceRequest request, int pIdReadCommuncation)
        {
            IQueryable<MonthlyProjectCommunication> communications = db.MonthlyProjectCommunication.Where(x => x.PeriodMappingId == pIdReadCommuncation);
            DataSourceResult result = communications.ToDataSourceResult(request, comn => new
            {
                MonthlyProjectCommunicationId = comn.MonthlyProjectCommunicationId,
                PeriodMappingId = comn.PeriodMappingId,
                ComnWithinProject = comn.ComnWithinProject,
                ComnWithHeadOffice = comn.ComnWithHeadOffice,
                ComnWithConsultant = comn.ComnWithConsultant,
                NumberOfComplaints = comn.NumberOfComplaints,
                GoodGovernanceComplaints = comn.GoodGovernanceComplaints,
                MaxAllowablePoints = comn.MaxAllowablePoints,
                TotalPoints = comn.TotalPoints,

                CloseDiscussionWithinProject = comn.CloseDiscussionWithinProject,
                MobilizationAndDisscusstionWthinWorkers = comn.MobilizationAndDisscusstionWthinWorkers,
                CommunicationAndSupportBetweenProjects = comn.CommunicationAndSupportBetweenProjects,
                WrittenComn = comn.WrittenComn,
                TelephoneComn = comn.TelephoneComn,
                EmailComn = comn.EmailComn,
                ResponseFromHeadOffice = comn.ResponseFromHeadOffice,
                ComnLevel = comn.ComnLevel,

                OrgBranchName = comn.OrgBranchName,
                DateCreated = comn.DateCreated,
                LastModified = comn.LastModified,
                CreatedBy = comn.CreatedBy,
                ModifiedBy = comn.ModifiedBy
            });

            return Json(result);
        }
        private bool MonthlyProjectCommunicationExists(int periodId)
        {
            var valueIsAvailable = false;
            var existingValue = db.MonthlyProjectCommunication.FirstOrDefault(p => p.PeriodMappingId == periodId);
            if (existingValue != null)
            {
                valueIsAvailable = true;
            }
            return valueIsAvailable;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyProjectCommunication_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectCommunication comn, int pIdCreateCommuncation)
        {
            if (ModelState.IsValid)
            {
                var errorsExist = false;
                var componentWait = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.COMMUNICATION, pIdCreateCommuncation);
                if (componentWait == 0)
                {
                    ModelState.AddModelError("", "Component weight is not set for COMMUNICATION");
                    errorsExist = true;
                }
                if (MonthlyProjectCommunicationExists(pIdCreateCommuncation))
                {
                    ModelState.AddModelError("", "Communication for this period exists");
                    errorsExist = true;
                }
                if (!errorsExist)
                {
                    var entity = new MonthlyProjectCommunication
                    {
                        MonthlyProjectCommunicationId = comn.MonthlyProjectCommunicationId,
                        PeriodMappingId = pIdCreateCommuncation,
                        ComnWithinProject = comn.CloseDiscussionWithinProject + comn.MobilizationAndDisscusstionWthinWorkers + comn.CommunicationAndSupportBetweenProjects,
                        ComnWithHeadOffice = comn.WrittenComn + comn.TelephoneComn + comn.EmailComn + comn.ResponseFromHeadOffice,
                        ComnWithConsultant = PerformanceEvnHelper.CalculateScoredPoints(comn),
                        NumberOfComplaints = comn.NumberOfComplaints,
                        //GoodGovernanceComplaints = (-1) * comn.NumberOfComplaints,
                        GoodGovernanceComplaints = comn.GoodGovernanceComplaints,
                        MaxAllowablePoints = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.COMMUNICATION, pIdCreateCommuncation),
                        TotalPoints = comn.TotalPoints,

                        CloseDiscussionWithinProject = comn.CloseDiscussionWithinProject,
                        MobilizationAndDisscusstionWthinWorkers = comn.MobilizationAndDisscusstionWthinWorkers,
                        CommunicationAndSupportBetweenProjects = comn.CommunicationAndSupportBetweenProjects,
                        WrittenComn = comn.WrittenComn,
                        TelephoneComn = comn.TelephoneComn,
                        EmailComn = comn.EmailComn,
                        ResponseFromHeadOffice = comn.ResponseFromHeadOffice,
                        ComnLevel = comn.ComnLevel,

                        OrgBranchName = comn.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = comn.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = comn.ModifiedBy
                    };
                    entity.TotalPoints = entity.ComnWithConsultant + entity.ComnWithinProject + entity.ComnWithHeadOffice + entity.GoodGovernanceComplaints;
                    entity.TotalPoints = (decimal)entity.MaxAllowablePoints < entity.TotalPoints ? (decimal)entity.MaxAllowablePoints : entity.TotalPoints;
                    db.MonthlyProjectCommunication.Add(entity);
                    db.SaveChanges();
                    comn.MonthlyProjectCommunicationId = entity.MonthlyProjectCommunicationId;
                }
               
                //PerformanceEvnHelper.UpdateMonthlyProjectComn(entity.MonthlyProjectCommunicationId);
            }
            return Json(new[] { comn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyProjectCommunication_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectCommunication comn)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectCommunication
                {
                    MonthlyProjectCommunicationId = comn.MonthlyProjectCommunicationId,
                    PeriodMappingId = comn.PeriodMappingId,
                    ComnWithinProject = comn.CloseDiscussionWithinProject + comn.MobilizationAndDisscusstionWthinWorkers + comn.CommunicationAndSupportBetweenProjects,
                    ComnWithHeadOffice = comn.WrittenComn + comn.TelephoneComn + comn.EmailComn + comn.ResponseFromHeadOffice,
                    ComnWithConsultant = PerformanceEvnHelper.CalculateScoredPoints(comn),
                    NumberOfComplaints = comn.NumberOfComplaints,
                    //GoodGovernanceComplaints = (-1) * comn.NumberOfComplaints,
                    GoodGovernanceComplaints = comn.GoodGovernanceComplaints,
                    MaxAllowablePoints = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.COMMUNICATION, comn.PeriodMappingId),
                    TotalPoints = comn.TotalPoints,

                    CloseDiscussionWithinProject = comn.CloseDiscussionWithinProject,
                    MobilizationAndDisscusstionWthinWorkers = comn.MobilizationAndDisscusstionWthinWorkers,
                    CommunicationAndSupportBetweenProjects = comn.CommunicationAndSupportBetweenProjects,
                    WrittenComn = comn.WrittenComn,
                    TelephoneComn = comn.TelephoneComn,
                    EmailComn = comn.EmailComn,
                    ResponseFromHeadOffice = comn.ResponseFromHeadOffice,
                    ComnLevel = comn.ComnLevel,

                    OrgBranchName = comn.OrgBranchName,
                    DateCreated = comn.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = comn.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.TotalPoints = entity.ComnWithConsultant + entity.ComnWithinProject + entity.ComnWithHeadOffice + entity.GoodGovernanceComplaints;
                entity.TotalPoints = (decimal)entity.MaxAllowablePoints < entity.TotalPoints ? (decimal)entity.MaxAllowablePoints : entity.TotalPoints;

                db.MonthlyProjectCommunication.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //PerformanceEvnHelper.UpdateMonthlyProjectComn(entity.MonthlyProjectCommunicationId);

            }

            return Json(new[] { comn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyProjectCommunication_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectCommunication comn)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectCommunication
                {
                    MonthlyProjectCommunicationId = comn.MonthlyProjectCommunicationId,
                    PeriodMappingId = comn.PeriodMappingId,
                    ComnWithinProject = comn.ComnWithinProject,
                    ComnWithHeadOffice = comn.ComnWithHeadOffice,
                    ComnWithConsultant = comn.ComnWithConsultant,
                    NumberOfComplaints = comn.NumberOfComplaints,
                    GoodGovernanceComplaints = comn.GoodGovernanceComplaints,
                    MaxAllowablePoints = comn.MaxAllowablePoints,
                    TotalPoints = comn.TotalPoints,

                    CloseDiscussionWithinProject = comn.CloseDiscussionWithinProject,
                    MobilizationAndDisscusstionWthinWorkers = comn.MobilizationAndDisscusstionWthinWorkers,
                    CommunicationAndSupportBetweenProjects = comn.CommunicationAndSupportBetweenProjects,
                    WrittenComn = comn.WrittenComn,
                    TelephoneComn = comn.TelephoneComn,
                    EmailComn = comn.EmailComn,
                    ResponseFromHeadOffice = comn.ResponseFromHeadOffice,
                    ComnLevel = comn.ComnLevel,

                    OrgBranchName = comn.OrgBranchName,
                    DateCreated = comn.DateCreated,
                    LastModified = comn.LastModified,
                    CreatedBy = comn.CreatedBy,
                    ModifiedBy = comn.ModifiedBy
                };

                db.MonthlyProjectCommunication.Attach(entity);
                db.MonthlyProjectCommunication.Remove(entity);
                db.SaveChanges();

                //PerformanceEvnHelper.UpdateMonthlyProjectComn(entity.MonthlyProjectCommunicationId);

            }

            return Json(new[] { comn }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
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

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class ProjectAdvancePaymentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult AdvancePayment_Read([DataSourceRequest]DataSourceRequest request, int projectIdReadAdvance)
        {
            IQueryable<ProjectAdvancePayment> payments = db.ProjectAdvancePayments.Where(adp => adp.ProjectInformationId == projectIdReadAdvance);
            DataSourceResult result = payments.ToDataSourceResult(request, payment => new {
                ProjectAdvancePaymentId = payment.ProjectAdvancePaymentId,
                ProjectInformationId = payment.ProjectInformationId,
                TotalProjectAmount = payment.TotalProjectAmount,
                AdvancePaymentRate = payment.AdvancePaymentRate,
                ApprovedPaymentRate = payment.ApprovedPaymentRate,
                Amount = payment.Amount,
                ApprovedAmount = payment.ApprovedAmount,

                OrgBranchName = payment.OrgBranchName,
                DateCreated = payment.DateCreated,
                LastModified = payment.LastModified,
                CreatedBy = payment.CreatedBy,
                ModifiedBy = payment.ModifiedBy
            });

            return Json(result);
        }

        private bool CheckExistance(int projectId)
        {
            bool paymentExists = false;
            var payment = db.ProjectAdvancePayments.Where( ap => ap.ProjectInformationId== projectId).FirstOrDefault();
            if (payment != null)
            {
                paymentExists = true;
            }
            return paymentExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvancePayment_Create([DataSourceRequest]DataSourceRequest request, ProjectAdvancePayment payment,int projectIdCreateAdvance)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(projectIdCreateAdvance))
                {
                    ModelState.AddModelError("", "Advance Payment Exists for this project!!");
                }
                else
                {
                    var entity = new ProjectAdvancePayment
                    {
                        //ProjectAdvancePaymentId = payment.ProjectAdvancePaymentId,
                        ProjectInformationId = projectIdCreateAdvance,
                        TotalProjectAmount = this.GetTotalProjectAmount(projectIdCreateAdvance),
                        AdvancePaymentRate = ConsSettingHelper.GetCurrentAdvancedPaymentRate(projectIdCreateAdvance),
                        ApprovedPaymentRate = payment.ApprovedPaymentRate,
                        Amount = payment.Amount,
                        ApprovedAmount = payment.ApprovedAmount,

                        OrgBranchName = payment.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = payment.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = payment.ModifiedBy
                    };
                    entity.Amount = (entity.AdvancePaymentRate / 100) * entity.TotalProjectAmount;
                    entity.ApprovedAmount = entity.ApprovedAmount == 0? (entity.AdvancePaymentRate / 100) * entity.TotalProjectAmount : entity.ApprovedAmount;


                    db.ProjectAdvancePayments.Add(entity);
                    db.SaveChanges();
                    payment.ProjectAdvancePaymentId = entity.ProjectAdvancePaymentId;

                    payment.TotalProjectAmount = entity.TotalProjectAmount;
                    payment.AdvancePaymentRate = entity.AdvancePaymentRate;
                    payment.Amount = entity.Amount;
                    payment.ApprovedAmount = entity.ApprovedAmount;
                }
            }

            return Json(new[] { payment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvancePayment_Update([DataSourceRequest]DataSourceRequest request, ProjectAdvancePayment payment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectAdvancePayment
                {
                    ProjectAdvancePaymentId = payment.ProjectAdvancePaymentId,
                    ProjectInformationId = payment.ProjectInformationId,
                    TotalProjectAmount = this.GetTotalProjectAmount(payment.ProjectInformationId),
                    AdvancePaymentRate = ConsSettingHelper.GetCurrentAdvancedPaymentRate(payment.ProjectInformationId),
                    ApprovedPaymentRate = payment.ApprovedPaymentRate,
                    Amount = payment.Amount,
                    ApprovedAmount = payment.ApprovedAmount,

                    OrgBranchName = payment.OrgBranchName,
                    DateCreated = payment.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = payment.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.Amount = (entity.AdvancePaymentRate / 100) * entity.TotalProjectAmount;
                entity.ApprovedAmount = entity.ApprovedAmount == 0 ? (entity.AdvancePaymentRate / 100) * entity.TotalProjectAmount : entity.ApprovedAmount;

                db.ProjectAdvancePayments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                payment.TotalProjectAmount = entity.TotalProjectAmount;
                payment.AdvancePaymentRate = entity.AdvancePaymentRate;
                payment.Amount = entity.Amount;
                payment.ApprovedAmount = entity.ApprovedAmount;
            }

            return Json(new[] { payment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvancePayment_Destroy([DataSourceRequest]DataSourceRequest request, ProjectAdvancePayment payment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectAdvancePayment
                {
                    ProjectAdvancePaymentId = payment.ProjectAdvancePaymentId,
                    ProjectInformationId = payment.ProjectInformationId,
                    TotalProjectAmount = payment.TotalProjectAmount,
                    AdvancePaymentRate = payment.AdvancePaymentRate,
                    ApprovedPaymentRate = payment.ApprovedPaymentRate,
                    Amount = payment.Amount,
                    ApprovedAmount = payment.ApprovedAmount,

                    OrgBranchName = payment.OrgBranchName,
                    DateCreated = payment.DateCreated,
                    LastModified = payment.LastModified,
                    CreatedBy = payment.CreatedBy,
                    ModifiedBy = payment.ModifiedBy
                };

                db.ProjectAdvancePayments.Attach(entity);
                db.ProjectAdvancePayments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { payment }.ToDataSourceResult(request, ModelState));
        }
        private decimal GetTotalProjectAmount(int projId) {
            decimal amount = new decimal();
            var mainContract = ContractMgmtHelper.GetProjectMainContract(projId);
            if(mainContract.TotalProjectAmount != null)
            {
                amount = (decimal)mainContract.TotalProjectAmount;
            }
            return amount;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
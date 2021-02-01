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
    
    public class ConsPaymentAgingPeriodController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        //[Route("ConsPaymentAgingPeriod")]
        //[Route("ConsPaymentAgingPeriod/Idex")]
        //[Route("PaymentAge")]
        //[Route("PaymentAge/Index")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PaymentAging_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ConsPaymentAgingPeriod> agingPeriods = db.ConsPaymentAgingPeriods;
            DataSourceResult result = agingPeriods.ToDataSourceResult(request, agingPeriod => new {
                ConsPaymentAgingPeriodId = agingPeriod.ConsPaymentAgingPeriodId,
                Min = agingPeriod.Min,
                Max = agingPeriod.Max,
                TimeUnit = agingPeriod.TimeUnit,
                ConcatnatedRateDescription = agingPeriod.ConcatnatedRateDescription,
                ProvisionRate = agingPeriod.ProvisionRate,

                OrgBranchName = agingPeriod.OrgBranchName,
                DateCreated = agingPeriod.DateCreated,
                LastModified = agingPeriod.LastModified,
                CreatedBy = agingPeriod.CreatedBy,
                ModifiedBy = agingPeriod.ModifiedBy
            });

            return Json(result);
        }

        //private bool CheckExistance(C)
        //{
        //    bool paymentExists = false;
        //    var agingPeriod = db.ConsPaymentAgingPeriods.Where(ap => ap.Min == projectId).FirstOrDefault();
        //    if (agingPeriod != null)
        //    {
        //        paymentExists = true;
        //    }
        //    return paymentExists;
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentAging_Create([DataSourceRequest]DataSourceRequest request, ConsPaymentAgingPeriod agingPeriod)
        {
            if (ModelState.IsValid)
            {
                var concatnatedDescription = this.GetConcatnatedDescription(agingPeriod);

                //if (CheckExistance(projectIdCreateAdvance))
                //{
                //    ModelState.AddModelError("", "Advance Payment Exists for this project!!");
                //}
                //else
                //{
                var entity = new ConsPaymentAgingPeriod
                    {
                        ConsPaymentAgingPeriodId = agingPeriod.ConsPaymentAgingPeriodId,
                        Min = agingPeriod.Min,
                        Max = agingPeriod.Max,
                        TimeUnit = agingPeriod.TimeUnit,
                        ConcatnatedRateDescription = concatnatedDescription,
                        ProvisionRate = agingPeriod.ProvisionRate,

                        OrgBranchName = agingPeriod.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = agingPeriod.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = agingPeriod.ModifiedBy
                    };
                   

                    db.ConsPaymentAgingPeriods.Add(entity);
                    db.SaveChanges();
                    agingPeriod.ConsPaymentAgingPeriodId = entity.ConsPaymentAgingPeriodId;

                    
                //}
            }

            return Json(new[] { agingPeriod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentAging_Update([DataSourceRequest]DataSourceRequest request, ConsPaymentAgingPeriod agingPeriod)
        {
            if (ModelState.IsValid)
            {
                var concatnatedDescription = this.GetConcatnatedDescription(agingPeriod);

                var entity = new ConsPaymentAgingPeriod
                {
                    ConsPaymentAgingPeriodId = agingPeriod.ConsPaymentAgingPeriodId,
                    Min = agingPeriod.Min,
                    Max = agingPeriod.Max,
                    TimeUnit = agingPeriod.TimeUnit,
                    ConcatnatedRateDescription = concatnatedDescription,
                    ProvisionRate = agingPeriod.ProvisionRate,

                    OrgBranchName = agingPeriod.OrgBranchName,
                    DateCreated = agingPeriod.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = agingPeriod.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                
                db.ConsPaymentAgingPeriods.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

               
            }

            return Json(new[] { agingPeriod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentAging_Destroy([DataSourceRequest]DataSourceRequest request, ConsPaymentAgingPeriod agingPeriod)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsPaymentAgingPeriod
                {
                    ConsPaymentAgingPeriodId = agingPeriod.ConsPaymentAgingPeriodId,
                    Min = agingPeriod.Min,
                    Max = agingPeriod.Max,
                    TimeUnit = agingPeriod.TimeUnit,
                    ConcatnatedRateDescription = agingPeriod.ConcatnatedRateDescription,
                    ProvisionRate = agingPeriod.ProvisionRate,

                    OrgBranchName = agingPeriod.OrgBranchName,
                    DateCreated = agingPeriod.DateCreated,
                    LastModified = agingPeriod.LastModified,
                    CreatedBy = agingPeriod.CreatedBy,
                    ModifiedBy = agingPeriod.ModifiedBy
                };

                db.ConsPaymentAgingPeriods.Attach(entity);
                db.ConsPaymentAgingPeriods.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { agingPeriod }.ToDataSourceResult(request, ModelState));
        }
        private string GetConcatnatedDescription(ConsPaymentAgingPeriod record)
        {
            var des = string.Empty;
            var unt = record.TimeUnit;
            if (unt == AgingPeriodTimeUnit.Day)
            {
                des = record.Min + "-" + record.Max + " days";
            }
            else if (unt == AgingPeriodTimeUnit.Month)
            {
                des = record.Min + "-" + record.Max + " months";
            }
            else if (unt == AgingPeriodTimeUnit.Year)
            {
                des = record.Min + "-" + record.Max + " years";
            }
            return des;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
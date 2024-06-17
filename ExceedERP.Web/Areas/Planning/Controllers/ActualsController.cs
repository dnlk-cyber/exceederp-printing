﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User)]
    public class ActualsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var objectives = db.Objectives.Select(O => new
            {
                O.ObjectiveId,
                O.Name,

            }).ToList();
            var indicators = db.Indicators.Select(I => new
            {
                I.IndicatorId,
                I.Name,
            }).ToList();
            var fiscalYears = from f in db.GLFiscalYears
                              join
                    p in db.Plans on f.GlFiscalYearId equals p.GlFiscalYearId
                              select new
                              {
                                  f.GlFiscalYearId,
                                  f.Name,

                              };
            ViewData["GlFiscalYears"] = fiscalYears.DistinctByField(x => x.GlFiscalYearId).ToList();
            ViewData["Objectives"] = objectives;
            ViewData["Indicators"] = indicators;
            ViewData["GlobalIndicators"] = db.GlobalIndicators.Select(x => new { x.GlobalIndicatorId, x.Indicator });
            ViewData["periods"] = db.GLPeriods.ToList().Select(x => new { x.GLPeriodId, period = x.Name }).ToList();
            ViewData["departmentLists"] = db.MainCompanyStructures.Select(x=>new { x.OrgStructureId,x.Name}).ToList();

            return View();
        }

        public ActionResult Actuals_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Actual> acuals = db.Actuals/*.Where(x=>x.IsDepartnental)*/;
            DataSourceResult result = acuals.ToDataSourceResult(request, actual => new {
                ActualId = actual.ActualId,
                GlFiscalYearId = actual.GlFiscalYearId,
                PlanId = actual.PlanId,
                IsPersonal = actual.IsPersonal,
                IsDepartnental = actual.IsDepartnental,
                DateCreated = actual.DateCreated,
                CreatedBy = actual.CreatedBy,
                LastModified = actual.LastModified,
                ModifiedBy = actual.ModifiedBy,
                Description = actual.Description
            });

            return Json(result);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Actuals_Create([DataSourceRequest]DataSourceRequest request, Actual actual)
        {
            var exists = db.Actuals.Any(s => s.GlFiscalYearId == actual.GlFiscalYearId/*&&s.IsDepartnental*/);
            if (exists)
            {
                ModelState.AddModelError("exists", "This is already registered ");
            }
            var plan = db.Plans.FirstOrDefault(x => x.GlFiscalYearId == actual.GlFiscalYearId/*&&x.IsDepartnental*/);
            if(plan != null)
            {
                actual.PlanId = plan.PlanId ;
            }
            if (plan == null)
            {
                ModelState.AddModelError("exist", "you have to add a Plan for this fiscal year first");
            }

            if (ModelState.IsValid)
            {
                actual.IsDepartnental = true;
                actual.CreatedBy = User.Identity.Name;
                actual.DateCreated = DateTime.Now.Date;
                var entity = GetMappedObject(actual);
                db.Actuals.Add(entity);
                db.SaveChanges();
                actual.ActualId = entity.ActualId;

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Actuals", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);

                #endregion
            }

            return Json(new[] { actual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Actuals_Update([DataSourceRequest]DataSourceRequest request, Actual actual)
        {
            var exists = db.Actuals.Any(s => s.GlFiscalYearId == actual.GlFiscalYearId&& s.ActualId != actual.ActualId);
            if (exists)
            {
                ModelState.AddModelError("exists", "This is already registered ");
            }
            var plan = db.Plans.FirstOrDefault(x => x.GlFiscalYearId == actual.GlFiscalYearId);
            if(plan != null)
            {
                actual.PlanId = plan.PlanId;
            }
            else if(plan == null)
            {
                ModelState.AddModelError("exists", "you have to plan first for this fiscal year ");

            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(actual);
                db.Actuals.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Actual", "Success", User.Identity.Name,  " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion
            }

            return Json(new[] { actual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Actuals_Destroy([DataSourceRequest]DataSourceRequest request, Actual actual)
        {
            var bscActuals = db.BSCActuals.Where(x => x.ActualId == actual.ActualId).ToList();
            if (bscActuals.Any())
            {
                ModelState.AddModelError("exists", "You Can't delete this ! you have to delete child Data first");
            }
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(actual);
                #region delete log
                var yearDelete = db.GLFiscalYears.Find(actual.GlFiscalYearId);
                var delete = yearDelete.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "     fiscal year " + delete);
                #endregion
                db.Actuals.Attach(entity);
                db.Actuals.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { actual }.ToDataSourceResult(request, ModelState));
        }
        private Actual GetMappedObject(Actual actual)
        {
            var entity = new Actual
            {
                ActualId = actual.ActualId,
                GlFiscalYearId = actual.GlFiscalYearId,
                PlanId = actual.PlanId,
                IsDepartnental = actual.IsDepartnental,
                DateCreated = actual.DateCreated,
                CreatedBy = actual.CreatedBy,
                LastModified = actual.LastModified,
                ModifiedBy = actual.ModifiedBy,
                Description = actual.Description
            };
            return entity;
        }
        public JsonResult RefreshActualDataFromPlanningChanges(int id)
        {
            var actual = db.Actuals.Find(id);
            if(actual != null)
            {
                var plan = db.Plans.FirstOrDefault(x => x.GlFiscalYearId == actual.GlFiscalYearId);
                if(plan != null)
                {
                   // pupulateActualDetails(actual.ActualId, plan.PlanId, actual.GlFiscalYearId);

                }

            }
            return Json("refreshed");
        }
        //private void pupulateActualDetails(int actualId, int planId, int fiscalyearId)
        //{
        //    var bscPlans = db.BSCPlans.Where(x => x.PlanId == planId).ToList();
        //    foreach (var bscPlan in bscPlans)
        //    {
        //        var bscActual = db.BSCActuals.Where(x => x.ActualId == actualId && x.ObjectiveId == bscPlan.ObjectiveId && x.OrgStructureId == bscPlan.OrgStructureId).FirstOrDefault();
        //        if (bscActual  != null)
        //        {
                    



        //                var entity = new BSCActual
        //                {
        //                    ObjectiveId = bscPlan.ObjectiveId,
        //                    OrgStructureId = bscPlan.OrgStructureId,
        //                    ActualId = actualId
        //                };
        //                db.BSCActuals.Attach(entity);
        //                db.Entry(entity).State = EntityState.Modified;
        //                db.SaveChanges();
        //                var planDetails = db.PlanningDetails.Where(x => x.BSCPlanId == bscPlan.BSCPlanId).ToList();
        //            foreach (var planDetail in planDetails)
        //            {
        //                var actualdetails = db.ActualDetails.Where(x => x.BSCActualId == bscActual.BSCActualId
        //                                                               && x.IndicatorId == planDetail.IndicatorId
        //                                                           /* && x.SubIndicatorId == planDetail.SubIndicatorId*/).ToList();
        //                if (actualdetails.Count() > 0)
        //                {

        //                    foreach (var actualdetail in actualdetails)
        //                    {


        //                        //var entity1 = new ActualDetail
        //                        //{
        //                        //    IndicatorId = planDetail.IndicatorId,
        //                        //    SubIndicatorId = planDetail.SubIndicatorId,
        //                        //    BSCActualId = entity.BSCActualId,
        //                        //};
        //                        //db.ActualDetails.Add(entity1);
        //                        //db.SaveChanges();

        //                        var monthlyPlans = db.MonthlyPlans.Where(x => x.PlanningWithSubIndicatorId == planDetail.PlanningDetailId).ToList();
        //                        foreach (var monthlyplan in monthlyPlans)
        //                        {

        //                            var entity2 = new MonthlyActual
        //                            {
        //                                GLPeriodId = monthlyplan.GLPeriodId,
        //                                PlanValue = monthlyplan.Value,
        //                                ActualValue = 0
        //                            };
        //                            db.MonthlyActuals.Add(entity2);
        //                            db.SaveChanges();

        //                        }
        //                    }
        //                }
        //                else
        //                {

        //                    var entity1 = new ActualDetail
        //                    {
        //                        IndicatorId = planDetail.IndicatorId,
        //                        BSCActualId = entity.BSCActualId,
        //                    };
        //                    db.ActualDetails.Add(entity1);
        //                    db.SaveChanges();

        //                    var monthlyPlans = db.MonthlyPlans.Where(x => x.PlanningDetailId == planDetail.PlanningDetailId ).ToList();
        //                    foreach (var monthlyplan in monthlyPlans)
        //                    {
        //                        var entity2 = new MonthlyActual
        //                        {
        //                            ActualDetailId = entity1.ActualDetailId,
        //                            GLPeriodId = monthlyplan.GLPeriodId,
        //                            PlanValue = monthlyplan.Value,
        //                            ActualValue = 0
        //                        };
        //                        db.MonthlyActuals.Add(entity2);
        //                        db.SaveChanges();

        //                    }
                            
                            
        //                }
        //            }
                    
        //        }
        //        else
        //        {
        //            var entity = new BSCActual
        //            {
        //                ObjectiveId = bscPlan.ObjectiveId,
        //                OrgStructureId = bscPlan.OrgStructureId,
        //                ActualId = actualId
        //            };
        //            db.BSCActuals.Add(entity);
        //            db.SaveChanges();
        //            var planDetails = db.PlanningDetails.Where(x => x.BSCPlanId == bscPlan.BSCPlanId).ToList();
        //            foreach (var planDetail in planDetails)
        //            {
        //                var entity1 = new ActualDetail
        //                {
        //                    IndicatorId = planDetail.IndicatorId,
        //                    BSCActualId = entity.BSCActualId,
        //                };
        //                db.ActualDetails.Add(entity1);
        //                db.SaveChanges();
        //                var monthlyPlans = db.MonthlyPlans.Where(x => x.PlanningDetailId == planDetail.PlanningDetailId).ToList();
        //                foreach (var monthlyplan in monthlyPlans)
        //                {
        //                    var entity2 = new MonthlyActual
        //                    {
        //                        ActualDetailId = entity1.ActualDetailId,
        //                        GLPeriodId = monthlyplan.GLPeriodId,
        //                        PlanValue = monthlyplan.Value,
        //                        ActualValue = 0
        //                    };
        //                    db.MonthlyActuals.Add(entity2);
        //                    db.SaveChanges();
        //                    var weeklyPlans = db.WeeklyPlans.Where(x => x.MonthlyPlanId == monthlyplan.MonthlyPlanId).ToList();
        //                    foreach (var weeklyPlan in weeklyPlans)
        //                    {
        //                        var entity3 = new WeeklyActual
        //                        {
        //                            MonthlyActualId = entity2.MonthlyActualId,
        //                            WeekOne = weeklyPlan.WeekOne,
        //                            WeekTwo = weeklyPlan.WeekTwo,
        //                            WeekThree = weeklyPlan.WeekThree,
        //                            WeekFour = weeklyPlan.WeekFour,

        //                        };
        //                        db.WeeklyActuals.Add(entity3);
        //                        db.SaveChanges();
        //                    }
        //                }
        //            }
        //        }   
        //    }


            
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

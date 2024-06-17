﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User)]
    public class BSCPlansController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

       
        public ActionResult BSCPlans_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<BSCPlan> bscplans = db.BSCPlans.Where(b=>b.PlanId==id);
            DataSourceResult result = bscplans.ToDataSourceResult(request, bSCPlan => new {
                BSCPlanId = bSCPlan.BSCPlanId,
                ObjectiveId = bSCPlan.ObjectiveId,
                OrgStructureId = bSCPlan.OrgStructureId,
                IsFirstLevelApproved = bSCPlan.IsFirstLevelApproved,
                IsSecondLevel = bSCPlan.IsSecondLevel,
                IsThirdLevel = bSCPlan.IsThirdLevel,
                IsCopied = bSCPlan.IsCopied,
                //HasChild = GetHasChid(workUnitId, bSCPlan.OrgStructureId),
                PlanId = bSCPlan.PlanId
            });

            return Json(result);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BSCPlans_Create([DataSourceRequest]DataSourceRequest request, BSCPlan bSCPlan,int id)
        {
            var exists = db.BSCPlans.Any(s => s.ObjectiveId == bSCPlan.ObjectiveId&& s.OrgStructureId ==bSCPlan.OrgStructureId && s.PlanId==id);
            if (exists)
            {
                ModelState.AddModelError("exists", "This is already registered");
            }
            if (ModelState.IsValid)
            {
                bSCPlan.PlanId = id;
                var entity = GetMappedObject(bSCPlan);
                db.BSCPlans.Add(entity);
                db.SaveChanges();
                bSCPlan.BSCPlanId = entity.BSCPlanId;
                PopulatethePlanningDetails(bSCPlan.OrgStructureId, bSCPlan.ObjectiveId, entity.BSCPlanId,id);
                // PopulateThePeriod(entity.BSCPlanId, id);
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Bsc Actuals", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);

                #endregion


            }
            return Json(new[] { bSCPlan }.ToDataSourceResult(request, ModelState));
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BSCPlans_Update([DataSourceRequest]DataSourceRequest request, BSCPlan bSCPlan)
        {
            var exists = db.BSCPlans.Any(s => s.ObjectiveId == bSCPlan.ObjectiveId && s.OrgStructureId == bSCPlan.OrgStructureId && s.PlanId == bSCPlan.OrgStructureId);
            if (exists)
            {
                ModelState.AddModelError("exists", "This is already registered");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(bSCPlan);
                db.BSCPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "BSCPlan", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { bSCPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BSCPlans_Destroy([DataSourceRequest]DataSourceRequest request, BSCPlan bSCPlan)
        {
            var planDetails = db.PlanningDetails.Where(x => x.BSCPlanId == bSCPlan.BSCPlanId).ToList();
            if (planDetails.Any())
            {
                ModelState.AddModelError("exists", "You Can't delete this ! you have to delete child Data first");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(bSCPlan);
                #region delete log
                var objective = db.Objectives.Find(bSCPlan.ObjectiveId);
                var delete = objective.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "     Objective deleted  " + delete);
                #endregion

                db.BSCPlans.Attach(entity);
                db.BSCPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { bSCPlan }.ToDataSourceResult(request, ModelState));
        }
        private BSCPlan GetMappedObject(BSCPlan bSCPlan)
        {
            var entity = new BSCPlan
            {
                BSCPlanId = bSCPlan.BSCPlanId,
                ObjectiveId = bSCPlan.ObjectiveId,
                OrgStructureId = bSCPlan.OrgStructureId,
                IsFirstLevelApproved = bSCPlan.IsFirstLevelApproved,
                IsSecondLevel = bSCPlan.IsSecondLevel,
                IsThirdLevel = bSCPlan.IsThirdLevel,
                IsCopied = bSCPlan.IsCopied,
                PlanId = bSCPlan.PlanId
            };
            return entity;
        }
        private int PopulatethePlanningDetails(int orgStructureId,int objectiveId, int bscId,int planId)
        {
            //var placement = db.Placements.Where(x => x.isCurrent && x.IsHead && x.OrgStructureId == orgStructureId).FirstOrDefault();
            //if (placement != null)
            //{
            //    var orgstructureIds = db.MainCompanyStructures.Where(x => x.Parent == orgStructureId).Select(x => x.OrgStructureId);
            //    if (orgstructureIds.Count() > 0)
            //    {
            //       var bscPlans = db.BSCPlans.Where(x => x.PlanId == planId && orgstructureIds.Contains(x.OrgStructureId)).ToList();
            //       if( bscPlans.Count() > 0)
            //       {
            //            var objectiveids = bscPlans.Select(x => x.ObjectiveId).Distinct().ToList();
            //            foreach (var objectiveid in objectiveids)
            //            {
            //                var allIndicators = db.Indicators.Where(x => objectiveids.Contains(x.ObjectiveId)).ToList();
            //                var allDistictIndicators = allIndicators.DistinctByField(x => x.IndicatorId).ToList();
            //                foreach (var indicatorrr in allDistictIndicators)
            //                {
            //                    var details = db.PlanningDetails.Where(x => x.IndicatorId == indicatorrr.IndicatorId);
            //                    if (details.Count() > 0)
            //                    {
            //                        var entity = new PlanningDetail
            //                        {
            //                            IndicatorId = indicatorrr.IndicatorId,
            //                            BSCPlanId = bscId,
            //                            KPIType = indicatorrr.KPIType,
            //                            ParentId = indicatorrr.ParentId,
            //                            HasSubIndicator = indicatorrr.HasSubIndicator,
            //                            Unit = indicatorrr.Unit,
            //                            Baseline = details.Sum(x=>x.Baseline),
            //                            Target = details.Sum(x => x.Target),
            //                            Plan = details.Sum(x => x.Plan),


            //                        };
            //                        db.PlanningDetails.Add(entity);
            //                        db.SaveChanges();
            //                        var mantlyPlans = db.MonthlyPlans.Where(x => x.GetDetail.IndicatorId == indicatorrr.IndicatorId);
            //                        foreach(var )
            //                    }
            //                    else
            //                    {
            //                        var entity = new PlanningDetail
            //                        {
            //                            IndicatorId = indicatorrr.IndicatorId,
            //                            BSCPlanId = bscId,
            //                            KPIType = indicatorrr.KPIType,
            //                            ParentId = indicatorrr.ParentId,
            //                            HasSubIndicator = indicatorrr.HasSubIndicator,
            //                            Unit = indicatorrr.Unit,
            //                            Baseline = 0,
            //                            Target = 0,
            //                            Plan = 0,


            //                        };
            //                        db.PlanningDetails.Add(entity);
            //                        db.SaveChanges();

            //                    }
                               
            //                }
            //            }


            //        }
                  
                   
            //    }
            //}

            var indicators = db.Indicators.Where(x => x.ObjectiveId == objectiveId).ToList();
                  
            foreach(var indcator in indicators)
            {
               
                var entity = new PlanningDetail
                {
                    IndicatorId = indcator.IndicatorId,
                    BSCPlanId = bscId,
                    KPIType = indcator.KPIType,
                    ParentId = indcator.ParentId,
                    GlobalIndicatorId = indcator.GlobalIndicatorId,
                    HasSubIndicator = indcator.HasSubIndicator,
                    Unit = indcator.Unit,
                    Baseline = 0,
                    Target = 0,
                    Plan = 0,


                };
                db.PlanningDetails.Add(entity);
                db.SaveChanges();
               
                var fiscalYear = db.Plans.Find(planId);
                if (fiscalYear != null)
                {

                    var periods = db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId).ToList();
                    foreach (var period in periods)
                    {


                        var entity2 = new MonthlyPlan
                        {
                            GLPeriodId = period.GLPeriodId,
                            PlanningDetailId = entity.PlanningDetailId,
                            Value = 0,
                            ActualValue = 0,

                        };
                        db.MonthlyPlans.Add(entity2);
                        db.SaveChanges();

                    }

                }
               
                    
                
            }
            return 0;
        }
        public JsonResult RefreshToAddNewAddedItems(int id)
        {
            int count = 0;
            var record = db.BSCPlans.Find(id);
            if (record != null)
            {
                #region validate item Addition

                var indicators = db.Indicators.Where(x => x.ObjectiveId == record.ObjectiveId).ToList();
                foreach (var indcator in indicators)
                {
                    var exist = db.PlanningDetails.Where(x => x.IndicatorId == indcator.IndicatorId && x.BSCPlanId == id).FirstOrDefault();
                    if (exist == null)
                    {


                        var entity = new PlanningDetail
                        {
                            IndicatorId = indcator.IndicatorId,
                            BSCPlanId = record.BSCPlanId,
                            KPIType = indcator.KPIType,
                            ParentId = indcator.ParentId,
                            GlobalIndicatorId = indcator.GlobalIndicatorId,
                            HasSubIndicator = indcator.HasSubIndicator,
                            Unit = indcator.Unit,
                            Baseline = 0,
                            Target = 0,
                            Plan = 0,


                        };
                        db.PlanningDetails.Add(entity);
                        db.SaveChanges();

                        //var fiscalYear = db.Plans.Where(x=>x.PlanId==record.GetPlan.PlanId).FirstOrDefault();
                        //if (fiscalYear != null)
                        //{

                        var periods = db.GLPeriods.Where(x => x.GlFiscalYearId == record.GetPlan.GlFiscalYearId).ToList();
                        foreach (var period in periods)
                        {


                            var entity2 = new MonthlyPlan
                            {
                                GLPeriodId = period.GLPeriodId,
                                PlanningDetailId = entity.PlanningDetailId,
                                Value = 0,

                            };
                            db.MonthlyPlans.Add(entity2);
                            db.SaveChanges();

                        }

                       // }
                        count++;
                    }
                    else if (exist != null)
                    {

                        exist.IndicatorId = indcator.IndicatorId;
                        exist.BSCPlanId = record.BSCPlanId;
                        exist.KPIType = indcator.KPIType;
                        exist.ParentId = indcator.ParentId;
                        exist.GlobalIndicatorId = indcator.GlobalIndicatorId;
                        exist.HasSubIndicator = indcator.HasSubIndicator;
                        exist.Unit = indcator.Unit;
                        db.Entry(exist).State = EntityState.Modified;
                        db.SaveChanges();
                        count++;
                    }

                }
                if (count > 0)
                {
                    var formattedData = new
                    {
                        Message = " Item Updated Successfully .",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = " there no Updated  Item",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion
            }
            var formattedData12 = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
            return Json(formattedData12, JsonRequestBehavior.AllowGet);
        }

       
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

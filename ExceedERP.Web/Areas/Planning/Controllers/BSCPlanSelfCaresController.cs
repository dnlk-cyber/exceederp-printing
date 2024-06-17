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
    public class BSCPlanSelfCaresController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

       private bool GetHasChid(int id,int OthWorkId)
       {

            var placement = db.Placements.Where(x => x.isCurrent && x.IsHead && x.OrgStructureId == id).FirstOrDefault();
            if (placement != null)
            {
                var orgstructures = db.MainCompanyStructures
                                   .Where(x => x.Parent == id && x.OrgStructureId != id)
                                   .ToList();
                var Recordorgstructures = (orgstructures.Count() > 0) ? orgstructures.Where(x => x.OrgStructureId == OthWorkId).FirstOrDefault() : null;
                if (Recordorgstructures !=null)
                {
                    return true;

                }
                return false;
            }
            return false;
        }
        public ActionResult BSCPlanSelfCares_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var workUnitId = EmployeeHelper.GetEmployeeWorkUnitId(employeeId);
            var bscplans = db.BSCPlans.Where(x => x.OrgStructureId == workUnitId).Where(b => b.PlanId == id).ToList();
            var placement = db.Placements.Where(x => x.isCurrent && x.IsHead && x.OrgStructureId == workUnitId).FirstOrDefault();
            if (placement != null)
            {
                List<int> orgstructureIds = db.MainCompanyStructures
                                .Where(x => x.Parent == workUnitId)
                                .Select(x => x.OrgStructureId)
                                .ToList();
                orgstructureIds.Add(workUnitId);
                if (orgstructureIds.Count() > 0)
                {
                   bscplans = db.BSCPlans.Where(x => x.PlanId == id && orgstructureIds.Contains(x.OrgStructureId)/*||x.OrgStructureId==workUnitId*/).ToList()/*.DistinctByField(x=>x.OrgStructureId).ToList()*/;

                }

            }
            DataSourceResult result = bscplans.ToDataSourceResult(request, bSCPlan => new {
                BSCPlanId = bSCPlan.BSCPlanId,
                ObjectiveId = bSCPlan.ObjectiveId,
                OrgStructureId = bSCPlan.OrgStructureId,
                IsFirstLevelApproved = bSCPlan.IsFirstLevelApproved,
                IsSecondLevel = bSCPlan.IsSecondLevel,
                IsThirdLevel = bSCPlan.IsThirdLevel,
                IsCopied = bSCPlan.IsCopied,
                HasChild = GetHasChid(workUnitId, bSCPlan.OrgStructureId),
                PlanId = bSCPlan.PlanId
            });

            return Json(result);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BSCPlanSelfCares_Create([DataSourceRequest]DataSourceRequest request, BSCPlan bSCPlan,int id)
        {
            var exists = db.BSCPlans.Any(s => s.ObjectiveId == bSCPlan.ObjectiveId && s.PlanId==id);
            if (exists)
            {
                ModelState.AddModelError("exists", "This is already registered ");
            }
            if (ModelState.IsValid)
            {
                bSCPlan.PlanId = id;
                var entity = GetMappedObject(bSCPlan);
                db.BSCPlans.Add(entity);
                db.SaveChanges();
                bSCPlan.BSCPlanId = entity.BSCPlanId;
                PopulatethePlanningDetails(bSCPlan.ObjectiveId, entity.BSCPlanId,id);
                // PopulateThePeriod(entity.BSCPlanId, id);
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Bsc Actuals", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);

                #endregion

            }
            return Json(new[] { bSCPlan }.ToDataSourceResult(request, ModelState));
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BSCPlanSelfCares_Update([DataSourceRequest]DataSourceRequest request, BSCPlan bSCPlan)
        {
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
        public ActionResult BSCPlanSelfCares_Destroy([DataSourceRequest]DataSourceRequest request, BSCPlan bSCPlan)
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
        private int PopulatethePlanningDetails(int objectiveId, int bscId,int planId)
        {
            var indicators = db.Indicators.Where(x=>x.ObjectiveId==objectiveId).ToList();
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

                        };
                        db.MonthlyPlans.Add(entity2);
                        db.SaveChanges();

                    }

                }
               
                    
                
            }
            return 0;
        }
        public JsonResult ApproveBScPlan(int id)
        {
            var record = db.BSCPlans.Find(id);
            if (record != null)
            {
                record.IsFirstLevelApproved = true;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                var formattedData12 = new
                {
                    Message = "It is approved succesfully.",
                    Status = "okay"
                };
                return Json(formattedData12, JsonRequestBehavior.AllowGet);
            }

            var formattedData = new
            {
                Message = "Record is not found.",
                Status = "NOT_OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CopyBScPlan1(int bscPlanId)
        {
            var recordToBeCopied = db.BSCPlans.Where(x=>!x.IsCopied&&x.IsFirstLevelApproved).Where(x=>x.BSCPlanId==bscPlanId).FirstOrDefault();
            if (recordToBeCopied != null)
            {
                
               
                var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var placement = db.Placements.Where(x => x.isCurrent &&x.EmployeeId==employeeId && x.IsHead ).FirstOrDefault();
                if (placement != null)
                {
                    var orgstructures = db.MainCompanyStructures
                                    .Where(x => x.Parent == placement.OrgStructureId).ToList();
                    var Recordorgstructures =(orgstructures.Any())? orgstructures.Where(x => x.OrgStructureId == recordToBeCopied.OrgStructureId).FirstOrDefault():null;
                    if (Recordorgstructures !=null)
                    {
                        var bscplan = db.BSCPlans.Where(x => x.OrgStructureId == placement.OrgStructureId).Where(x=>x.PlanId== recordToBeCopied.PlanId).FirstOrDefault();
                        if (bscplan != null)
                        {
                            var planningDetails = db.PlanningDetails.Where(x => x.BSCPlanId == recordToBeCopied.BSCPlanId).ToList();
                            foreach (var planningDetail in planningDetails)
                            {

                                var PlanningDetailUpper = db.PlanningDetails.Where(x =>  x.GlobalIndicatorId == planningDetail.GlobalIndicatorId
                                                                                        &&x.HasSubIndicator==planningDetail.HasSubIndicator
                                                                                          && x.KPIType == planningDetail.KPIType
                                                                                          && x.BSCPlanId==bscplan.BSCPlanId).FirstOrDefault();

                                if(PlanningDetailUpper != null)
                                {
                                    //var lowwerdetail = db.PlanningDetails.Where(x => x.IndicatorId == planningDetail.IndicatorId && x.BSCPlanId == bscplan.BSCPlanId);
                                    PlanningDetailUpper.Plan += planningDetail.Plan;         //lowwerdetail.Count()>0? lowwerdetail.Sum(x => x.Plan): planningDetail.Plan;
                                    PlanningDetailUpper.Target += planningDetail.Target;     //lowwerdetail.Count()>0? lowwerdetail.Sum(x => x.Target): planningDetail.Target;
                                    PlanningDetailUpper.Baseline += planningDetail.Target;   //lowwerdetail.Count()>0? lowwerdetail.Sum(x => x.Baseline): planningDetail.Baseline;
                                    PlanningDetailUpper.KPIType = planningDetail.KPIType;
                                    PlanningDetailUpper.ParentId = planningDetail.ParentId;
                                    PlanningDetailUpper.GlobalIndicatorId = planningDetail.GlobalIndicatorId;
                                    PlanningDetailUpper.IndicatorId = planningDetail.IndicatorId;
                                    PlanningDetailUpper.HasSubIndicator = planningDetail.HasSubIndicator;
                                    PlanningDetailUpper.BSCPlanId = bscplan.BSCPlanId;
                                    db.Entry(PlanningDetailUpper).State = EntityState.Modified;
                                    //db.SaveChanges();
                                    var existdMothlyPlanToCopieds = db.MonthlyPlans.Where(x => x.PlanningDetailId == planningDetail.PlanningDetailId
                                                                                                  &&x.GetDetail.GlobalIndicatorId == planningDetail.GlobalIndicatorId
                                                                                                 && x.GetDetail.HasSubIndicator == planningDetail.HasSubIndicator
                                                                                                && x.GetDetail.KPIType == planningDetail.KPIType
                                                                                            ).ToList();
                                    
                                    foreach (var monthlyplanToCopied in existdMothlyPlanToCopieds)
                                    {
                                        var monthlyplan = db.MonthlyPlans.FirstOrDefault(x => x.GLPeriodId == monthlyplanToCopied.GLPeriodId
                                                                                         &&x.PlanningDetailId== PlanningDetailUpper.PlanningDetailId
                                                                                         && x.GetDetail.GlobalIndicatorId == planningDetail.GlobalIndicatorId
                                                                                         && x.GetDetail.HasSubIndicator == planningDetail.HasSubIndicator
                                                                                          && x.GetDetail.KPIType == planningDetail.KPIType);
                                        if(monthlyplan != null)
                                        {

                                           // var existdMothlyPlanUppersCopy = existdMothlyPlanUppers.Where(x => x.GLPeriodId == periodId);
                                            //monthlyplan.Value = existdMothlyPlanUppersCopy.Count() > 0 ? existdMothlyPlanUppersCopy.Sum(x => x.Value) : 0;
                                            monthlyplan.Value += monthlyplanToCopied.Value;
                                            db.Entry(monthlyplan).State = EntityState.Modified;
                                           // db.SaveChanges();
                                            //var monthlyPlanIds = existdMothlyPlanUppersCopy.Select(x => x.MonthlyPlanId);
                                            var weeklyPlan = db.WeeklyPlans.FirstOrDefault(x => x.MonthlyPlanId == monthlyplan.MonthlyPlanId);
                                            if(weeklyPlan != null)
                                            {

                                                var weeklyPlans = db.WeeklyPlans.Where(x =>x.MonthlyPlanId== monthlyplanToCopied.MonthlyPlanId).ToList();
                                                weeklyPlan.WeekOne += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekOne) : weeklyPlan.WeekOne;
                                                weeklyPlan.WeekTwo += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekTwo) : weeklyPlan.WeekTwo;
                                                weeklyPlan.WeekThree += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekThree) : weeklyPlan.WeekThree;
                                                weeklyPlan.WeekFour += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekFour) : weeklyPlan.WeekFour;
                                                db.Entry(weeklyPlan).State = EntityState.Modified;
                                               // db.SaveChanges();

                                            }
                                            db.SaveChanges();

                                        }
                                        else
                                        {
                                            var periods = db.GLPeriods.Where(x => x.GlFiscalYearId == bscplan.GetPlan.GlFiscalYearId).ToList();
                                            foreach (var period in periods)
                                            {

                                                var monthlyPlans = db.MonthlyPlans.Where(x => x.GLPeriodId == period.GLPeriodId)
                                                                                .Where(x => x.PlanningDetailId == planningDetail.PlanningDetailId)
                                                                                .ToList();
                                                var entity2 = new MonthlyPlan
                                                {
                                                    GLPeriodId = period.GLPeriodId,
                                                    ActualValue = 0,
                                                    PlanningDetailId = monthlyplanToCopied.PlanningDetailId,
                                                    Value = monthlyPlans.Count() > 0 ? monthlyPlans.Sum(x => x.Value) : 0,

                                                };
                                                db.MonthlyPlans.Add(entity2);
                                                db.SaveChanges();

                                                var monthlyPlanIds = monthlyPlans.Select(x => x.MonthlyPlanId).ToList();
                                                // var weeklyPlan = db.WeeklyPlans.FirstOrDefault(x => monthlyPlanIds.Contains(x.MonthlyPlanId));
                                                WeeklyPlan weeklyPlan = new WeeklyPlan();
                                                var weeklyPlans = db.WeeklyPlans.Where(x => monthlyPlanIds.Contains(x.MonthlyPlanId)).ToList();
                                                weeklyPlan.WeekOne = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekOne) : weeklyPlan.WeekOne;
                                                weeklyPlan.WeekTwo = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekTwo) : weeklyPlan.WeekTwo;
                                                weeklyPlan.WeekThree = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekThree) : weeklyPlan.WeekThree;
                                                weeklyPlan.WeekFour = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekFour) : weeklyPlan.WeekFour;
                                                weeklyPlan.ActualWeekOne = 0;
                                                weeklyPlan.ActualWeekTwo = 0;
                                                weeklyPlan.ActualWeekThree = 0;
                                                weeklyPlan.ActualWeekFour = 0;
                                                weeklyPlan.MonthlyPlanId = entity2.MonthlyPlanId;
                                                db.WeeklyPlans.Add(weeklyPlan);
                                                // db.Entry(weeklyPlan).State = EntityState.Modified;
                                                db.SaveChanges();


                                            }
                                        }
                                    }
                                }
                                else
                                {

                                    

                                    var entity = new PlanningDetail
                                    {
                                        IndicatorId = planningDetail.IndicatorId,
                                        GlobalIndicatorId = planningDetail.GlobalIndicatorId,
                                        BSCPlanId = bscplan.BSCPlanId,
                                        KPIType = planningDetail.KPIType,
                                        ParentId = planningDetail.ParentId,
                                        HasSubIndicator = planningDetail.HasSubIndicator,
                                        Unit = planningDetail.Unit,
                                        Baseline = planningDetail.Baseline,
                                        Target = planningDetail.Target,
                                        Plan = planningDetail.Plan,


                                    };
                                    db.PlanningDetails.Add(entity);
                                    db.SaveChanges();
                                    var periods = db.GLPeriods.Where(x => x.GlFiscalYearId == bscplan.GetPlan.GlFiscalYearId).ToList();
                                    foreach (var period in periods)
                                    {

                                        var monthlyPlans = db.MonthlyPlans.Where(x => x.GLPeriodId == period.GLPeriodId)
                                                                        .Where(x => x.PlanningDetailId == planningDetail.PlanningDetailId).ToList();
                                        if (monthlyPlans.Any())
                                        {
                                            var entity2 = new MonthlyPlan
                                            {
                                                GLPeriodId = period.GLPeriodId,
                                                PlanningDetailId = entity.PlanningDetailId,
                                                Value = monthlyPlans.Count() > 0 ? monthlyPlans.Sum(x => x.Value) : 0,
                                                ActualValue = 0
                                            };
                                            db.MonthlyPlans.Add(entity2);
                                            db.SaveChanges();
                                            var monthlyPlanIds = monthlyPlans.Select(x => x.MonthlyPlanId);
                                            WeeklyPlan weeklyPlan = new WeeklyPlan();
                                            var weeklyPlans = db.WeeklyPlans.Where(x => monthlyPlanIds.Contains(x.MonthlyPlanId)).ToList();
                                            weeklyPlan.WeekOne = weeklyPlans.Any()  ? weeklyPlans.Sum(x => x.WeekOne) : 0;
                                            weeklyPlan.WeekTwo = weeklyPlans.Any() ? weeklyPlans.Sum(x => x.WeekTwo) : 0;
                                            weeklyPlan.WeekThree = weeklyPlans.Any()? weeklyPlans.Sum(x => x.WeekThree) : 0;
                                            weeklyPlan.WeekFour = weeklyPlans.Any()? weeklyPlans.Sum(x => x.WeekFour) : 0;
                                            weeklyPlan.ActualWeekOne = 0;
                                            weeklyPlan.ActualWeekTwo = 0;
                                            weeklyPlan.ActualWeekThree = 0;
                                            weeklyPlan.ActualWeekFour = 0;
                                            weeklyPlan.MonthlyPlanId = entity2.MonthlyPlanId;
                                            db.WeeklyPlans.Add(weeklyPlan);
                                            db.SaveChanges();
                                        }
                                       
                                        

                                    }





                                    
                                }
                            }
                            recordToBeCopied.IsCopied = true;
                            db.Entry(recordToBeCopied).State = EntityState.Modified;
                            db.SaveChanges();
                            var formattedData14 = new
                            {
                                Message = "copied SuccessFully",
                                Status = "okay"
                            };
                            return Json(formattedData14, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var entity11 = new BSCPlan
                            {
                                OrgStructureId = placement.OrgStructureId,
                                ObjectiveId = recordToBeCopied.ObjectiveId,
                                PlanId = recordToBeCopied.PlanId,
                                IsCopied = true,
                                IsFirstLevelApproved = true,
                                IsSecondLevel = true,
                                IsThirdLevel = false,


                            };
                            db.BSCPlans.Add(entity11);
                            db.SaveChanges();
                            var indicators = db.PlanningDetails.Where(x => x.BSCPlanId == recordToBeCopied.BSCPlanId).ToList();

                            foreach (var indcator in indicators)
                            {

                                var entity = new PlanningDetail
                                {
                                    IndicatorId = indcator.IndicatorId,
                                    BSCPlanId = entity11.BSCPlanId,
                                    KPIType = indcator.KPIType,
                                    ParentId = indcator.ParentId,
                                    GlobalIndicatorId = indcator.GlobalIndicatorId,
                                    HasSubIndicator = indcator.HasSubIndicator,
                                    Unit = indcator.Unit,
                                    Baseline = indcator.Baseline,
                                    Target = indcator.Target,
                                    Plan = indcator.Plan,
                                    
                                };
                                db.PlanningDetails.Add(entity);
                                db.SaveChanges();
                                var periods = db.GLPeriods.Where(x => x.GlFiscalYearId == recordToBeCopied.GetPlan.GlFiscalYearId).ToList();
                                foreach (var period in periods)
                                {

                                    var monthlyPlans = db.MonthlyPlans.Where(x => x.GLPeriodId == period.GLPeriodId)
                                                                    .Where(x => x.PlanningDetailId == indcator.PlanningDetailId)
                                                                    .ToList();
                                    var entity2 = new MonthlyPlan
                                    {
                                        GLPeriodId = period.GLPeriodId,
                                        PlanningDetailId = entity.PlanningDetailId,
                                        Value = monthlyPlans.Count() > 0 ? monthlyPlans.Sum(x => x.Value) : 0,
                                        ActualValue = 0,

                                    };
                                    db.MonthlyPlans.Add(entity2);
                                    db.SaveChanges();

                                    var monthlyPlanIds = monthlyPlans.Select(x => x.MonthlyPlanId);
                                    var weeklyPlan = db.WeeklyPlans.FirstOrDefault(x => monthlyPlanIds.Contains( x.MonthlyPlanId ));
                                    if (weeklyPlan != null)
                                    {
                                        var weeklyPlans = db.WeeklyPlans.Where(x => monthlyPlanIds.Contains(x.MonthlyPlanId)).ToList();
                                        weeklyPlan.WeekOne = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekOne) : weeklyPlan.WeekOne;
                                        weeklyPlan.WeekTwo = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekTwo) : weeklyPlan.WeekTwo;
                                        weeklyPlan.WeekThree = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekThree) : weeklyPlan.WeekThree;
                                        weeklyPlan.WeekFour = weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekFour) : weeklyPlan.WeekThree;
                                        db.Entry(weeklyPlan).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                }





                            }
                            recordToBeCopied.IsSecondLevel = true;
                            recordToBeCopied.IsCopied = true;
                            db.Entry(recordToBeCopied).State = EntityState.Modified;
                            db.SaveChanges();
                            var formattedData14 = new
                            {
                                Message = "copied SuccessFully",
                                Status = "okay"
                            };
                            return Json(formattedData14, JsonRequestBehavior.AllowGet);
                        }
                        
                    }
                    else
                    {
                        var formattedData13 = new
                        {
                            Message = "you can not copy b/c This is your own dept Or no  Work Unit  is  under you",
                            Status = "okay"
                        };
                        return Json(formattedData13, JsonRequestBehavior.AllowGet);

                    }

                }
                else
                {
                    var formattedData12 = new
                    {
                        Message = "your are not Department Head.",
                        Status = "okay"
                    };
                    return Json(formattedData12, JsonRequestBehavior.AllowGet);

                }
               
            }

            var formattedData = new
            {
                Message = "Record is  Copied Once or it is not Approved.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CopyBScPlan(int bscPlanId)
        {
            var recordToBeCopied = db.BSCPlans.FirstOrDefault(x => x.BSCPlanId == bscPlanId && !x.IsCopied && x.IsFirstLevelApproved);

            if (recordToBeCopied == null)
            {
                return Json(new { Message = "Record is already copied or not approved.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }

            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employeeId && x.IsHead);

            if (placement == null)
            {
                return Json(new { Message = "You are not a department head.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }

            var orgstructures = db.MainCompanyStructures.Where(x => x.Parent == placement.OrgStructureId).ToList();
            var Recordorgstructures = orgstructures.FirstOrDefault(x => x.OrgStructureId == recordToBeCopied.OrgStructureId);

            if (Recordorgstructures == null)
            {
                return Json(new { Message = "You cannot copy because this is your own department or no work unit is under you.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }

            var bscplan = db.BSCPlans.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId && x.GetPlan.GlFiscalYearId == recordToBeCopied.GetPlan.GlFiscalYearId);

            if (bscplan == null)
            {
                // Create a new BSCPlan
                var entity11 = new BSCPlan
                {
                    OrgStructureId = placement.OrgStructureId,
                    ObjectiveId = recordToBeCopied.ObjectiveId,
                    PlanId = recordToBeCopied.PlanId,
                    IsCopied = true,
                    IsFirstLevelApproved = true,
                    IsSecondLevel = true,
                    IsThirdLevel = false
                };
                db.BSCPlans.Add(entity11);
                db.SaveChanges();

                // Copy PlanningDetails and MonthlyPlans
                CopyPlanningDetailsAndMonthlyPlans(recordToBeCopied.BSCPlanId, entity11.BSCPlanId);

                // Update the copied record
                recordToBeCopied.IsSecondLevel = true;
                recordToBeCopied.IsCopied = true;
                db.Entry(recordToBeCopied).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Copied successfully.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // BSCPlan already exists for this OrgStructure
                // Copy PlanningDetails and MonthlyPlans
                CopyPlanningDetailsAndMonthlyPlans(recordToBeCopied.BSCPlanId, bscplan.BSCPlanId);

                // Update the copied record
                recordToBeCopied.IsCopied = true;
                db.Entry(recordToBeCopied).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Copied successfully.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
        }

        private void CopyPlanningDetailsAndMonthlyPlans(int sourceBSCPlanId, int targetBSCPlanId)
        {
            var planningDetails = db.PlanningDetails.Where(x => x.BSCPlanId == sourceBSCPlanId).ToList();

            foreach (var planningDetail in planningDetails)
            {
                // Copy PlanningDetail
                var existInDesitination = db.PlanningDetails.Where(x => x.BSCPlanId == targetBSCPlanId
                                                    && x.GlobalIndicatorId == planningDetail.GlobalIndicatorId
                                                    && x.HasSubIndicator == planningDetail.HasSubIndicator
                                                    &&x.KPIType ==planningDetail.KPIType).FirstOrDefault();
                if (existInDesitination == null)
                {
                    var newPlanningDetail = new PlanningDetail
                    {
                        IndicatorId = planningDetail.IndicatorId,
                        GlobalIndicatorId = planningDetail.GlobalIndicatorId,
                        BSCPlanId = targetBSCPlanId,
                        KPIType = planningDetail.KPIType,
                        ParentId = planningDetail.ParentId,
                        HasSubIndicator = planningDetail.HasSubIndicator,
                        Unit = planningDetail.Unit,
                        Baseline = planningDetail.Baseline,
                        Target = planningDetail.Target,
                        Plan = planningDetail.Plan
                    };
                    db.PlanningDetails.Add(newPlanningDetail);
                    db.SaveChanges();

                    // Copy MonthlyPlans
                    var monthlyPlans = db.MonthlyPlans.Where(x => x.PlanningDetailId == planningDetail.PlanningDetailId).ToList();
                    foreach (var monthlyPlan in monthlyPlans)
                    {
                        var newMonthlyPlan = new MonthlyPlan
                        {
                            GLPeriodId = monthlyPlan.GLPeriodId,
                            PlanningDetailId = newPlanningDetail.PlanningDetailId,
                            Value = monthlyPlan.Value,
                            ActualValue = monthlyPlan.ActualValue
                        };
                        db.MonthlyPlans.Add(newMonthlyPlan);
                        db.SaveChanges();

                        // Copy WeeklyPlans
                        var weeklyPlans = db.WeeklyPlans.Where(x => x.MonthlyPlanId == monthlyPlan.MonthlyPlanId).ToList();
                        foreach (var weeklyPlan in weeklyPlans)
                        {
                            var newWeeklyPlan = new WeeklyPlan
                            {
                                MonthlyPlanId = newMonthlyPlan.MonthlyPlanId,
                                WeekOne = weeklyPlan.WeekOne,
                                WeekTwo = weeklyPlan.WeekTwo,
                                WeekThree = weeklyPlan.WeekThree,
                                WeekFour = weeklyPlan.WeekFour,
                                ActualWeekOne = weeklyPlan.ActualWeekOne,
                                ActualWeekTwo = weeklyPlan.ActualWeekTwo,
                                ActualWeekThree = weeklyPlan.ActualWeekThree,
                                ActualWeekFour = weeklyPlan.ActualWeekFour
                            };
                            db.WeeklyPlans.Add(newWeeklyPlan);
                        }
                    }

                }
                else
                {
                    existInDesitination.Plan += planningDetail.Plan;         //lowwerdetail.Count()>0? lowwerdetail.Sum(x => x.Plan): planningDetail.Plan;
                    existInDesitination.Target += planningDetail.Target;     //lowwerdetail.Count()>0? lowwerdetail.Sum(x => x.Target): planningDetail.Target;
                    existInDesitination.Baseline += planningDetail.Target;   //lowwerdetail.Count()>0? lowwerdetail.Sum(x => x.Baseline): planningDetail.Baseline;
                    existInDesitination.KPIType = planningDetail.KPIType;
                    existInDesitination.ParentId = planningDetail.ParentId;
                    existInDesitination.GlobalIndicatorId = planningDetail.GlobalIndicatorId;
                    existInDesitination.IndicatorId = planningDetail.IndicatorId;
                    existInDesitination.HasSubIndicator = planningDetail.HasSubIndicator;
                    existInDesitination.BSCPlanId = targetBSCPlanId;
                    db.Entry(existInDesitination).State = EntityState.Modified;
                    var existdMothlyPlanToCopieds = db.MonthlyPlans.Where(x => x.PlanningDetailId == planningDetail.PlanningDetailId
                                                                                  && x.GetDetail.GlobalIndicatorId == planningDetail.GlobalIndicatorId
                                                                                 && x.GetDetail.HasSubIndicator == planningDetail.HasSubIndicator
                                                                                && x.GetDetail.KPIType == planningDetail.KPIType
                                                                            ).ToList();

                    foreach (var monthlyplanToCopied in existdMothlyPlanToCopieds)
                    {
                        var monthlyplan = db.MonthlyPlans.FirstOrDefault(x => x.GLPeriodId == monthlyplanToCopied.GLPeriodId
                                                                         && x.PlanningDetailId == existInDesitination.PlanningDetailId
                                                                         && x.GetDetail.GlobalIndicatorId == planningDetail.GlobalIndicatorId
                                                                         && x.GetDetail.HasSubIndicator == planningDetail.HasSubIndicator
                                                                          && x.GetDetail.KPIType == planningDetail.KPIType);

                        if (monthlyplan != null)
                        {

                            
                            monthlyplan.Value += monthlyplanToCopied.Value;
                            db.Entry(monthlyplan).State = EntityState.Modified;
                            var weeklyPlan = db.WeeklyPlans.FirstOrDefault(x => x.MonthlyPlanId == monthlyplan.MonthlyPlanId);
                            if (weeklyPlan != null)
                            {

                                var weeklyPlans = db.WeeklyPlans.Where(x => x.MonthlyPlanId == monthlyplanToCopied.MonthlyPlanId).ToList();
                                weeklyPlan.WeekOne += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekOne) : weeklyPlan.WeekOne;
                                weeklyPlan.WeekTwo += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekTwo) : weeklyPlan.WeekTwo;
                                weeklyPlan.WeekThree += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekThree) : weeklyPlan.WeekThree;
                                weeklyPlan.WeekFour += weeklyPlans.Count() > 0 ? weeklyPlans.Sum(x => x.WeekFour) : weeklyPlan.WeekFour;
                                db.Entry(weeklyPlan).State = EntityState.Modified;
                                // db.SaveChanges();

                            }
                            db.SaveChanges();

                        }
                        else
                        {
                                                        
                            var entity2 = new MonthlyPlan
                            {
                                GLPeriodId = monthlyplanToCopied.GLPeriodId,
                                PlanningDetailId = existInDesitination.PlanningDetailId,
                                Value = monthlyplanToCopied.Value,
                                ActualValue = monthlyplanToCopied.ActualValue,

                            };
                            db.MonthlyPlans.Add(entity2);
                            db.SaveChanges();

                            var weeklyPlanToBecopied = db.WeeklyPlans.Where(x =>x.MonthlyPlanId== monthlyplanToCopied.MonthlyPlanId).ToList();
                            WeeklyPlan weeklyPlan = new WeeklyPlan();
                            weeklyPlan.WeekOne = weeklyPlanToBecopied.Count() > 0 ? weeklyPlanToBecopied.Sum(x => x.WeekOne) : weeklyPlan.WeekOne;
                            weeklyPlan.WeekTwo = weeklyPlanToBecopied.Count() > 0 ? weeklyPlanToBecopied.Sum(x => x.WeekTwo) : weeklyPlan.WeekTwo;
                            weeklyPlan.WeekThree = weeklyPlanToBecopied.Count() > 0 ? weeklyPlanToBecopied.Sum(x => x.WeekThree) : weeklyPlan.WeekThree;
                            weeklyPlan.WeekFour = weeklyPlanToBecopied.Count() > 0 ? weeklyPlanToBecopied.Sum(x => x.WeekFour) : weeklyPlan.WeekFour;
                            weeklyPlan.ActualWeekOne = 0;
                            weeklyPlan.ActualWeekTwo = 0;
                            weeklyPlan.ActualWeekThree = 0;
                            weeklyPlan.ActualWeekFour = 0;
                            weeklyPlan.MonthlyPlanId = entity2.MonthlyPlanId;
                            db.WeeklyPlans.Add(weeklyPlan);
                            db.SaveChanges();


                          
                        }
                    }
                }
               
            }
            db.SaveChanges();
        }
    //    private void CopyPlanningDetailsAndMonthlyPlans1(int sourceBSCPlanId, int targetBSCPlanId)
    //    {
    //        var planningDetails = db.PlanningDetails
    //            .Include(x => x.MonthlyPlans)
    //            .Where(x => x.BSCPlanId == sourceBSCPlanId)
    //            .ToList();

    //        foreach (var planningDetail in planningDetails)
    //        {
    //            var existingDestination = db.PlanningDetails.FirstOrDefault(x =>
    //                x.BSCPlanId == targetBSCPlanId &&
    //                x.GlobalIndicatorId == planningDetail.GlobalIndicatorId &&
    //                x.HasSubIndicator == planningDetail.HasSubIndicator &&
    //                x.KPIType == planningDetail.KPIType);

    //            if (existingDestination == null)
    //            {
    //                Copy PlanningDetail
    //                var newPlanningDetail = new PlanningDetail
    //                {
    //                     Populate properties (e.g., IndicatorId, GlobalIndicatorId, etc.)
    //                     Example:
    //                     IndicatorId = planningDetail.IndicatorId,
    //                     GlobalIndicatorId = planningDetail.GlobalIndicatorId,
    //                     BSCPlanId = targetBSCPlanId,
    //                     ...
    //                };
    //            db.PlanningDetails.Add(newPlanningDetail);

    //            Copy MonthlyPlans
    //                foreach (var monthlyPlan in planningDetail.MonthlyPlans)
    //            {
    //                var newMonthlyPlan = new MonthlyPlan
    //                    {
    //                         Populate properties (e.g., GLPeriodId, Value, ActualValue, etc.)
    //                         Example:
    //                         GLPeriodId = monthlyPlan.GLPeriodId,
    //                         Value = monthlyPlan.Value,
    //                         ActualValue = monthlyPlan.ActualValue,
    //                         ...
    //                    };
    //            newPlanningDetail.MonthlyPlans.Add(newMonthlyPlan);

    //            Copy WeeklyPlans
    //                    foreach (var weeklyPlan in monthlyPlan.WeeklyPlans)
    //            {
    //                var newWeeklyPlan = new WeeklyPlan
    //                        {
    //                             Populate properties (e.g., WeekOne, WeekTwo, etc.)
    //                             Example:
    //                             WeekOne = weeklyPlan.WeekOne,
    //                             WeekTwo = weeklyPlan.WeekTwo,
    //                             ...
    //                        };
    //            newMonthlyPlan.WeeklyPlans.Add(newWeeklyPlan);
    //        }
    //    }
    //}
    //            else
    //            {
    //                 Update existing PlanningDetail
    //                existingDestination.Plan += planningDetail.Plan;
    //existingDestination.Target += planningDetail.Target;
    //                existingDestination.Baseline += planningDetail.Target;

    //                 Update MonthlyPlans
    //                foreach (var monthlyPlan in planningDetail.MonthlyPlans)
    //                {
    //                    var existingMonthlyPlan = existingDestination.MonthlyPlans
    //                        .FirstOrDefault(x => x.GLPeriodId == monthlyPlan.GLPeriodId);
    //                    if (existingMonthlyPlan != null)
    //                    {
    //                        existingMonthlyPlan.Value += monthlyPlan.Value;

    //                         Update WeeklyPlans
    //                        foreach (var weeklyPlan in monthlyPlan.WeeklyPlans)
    //                        {
    //                            var existingWeeklyPlan = existingMonthlyPlan.WeeklyPlans
    //                                .FirstOrDefault(x => /* Check condition */);
    //                            if (existingWeeklyPlan != null)
    //                            {
    //                                 Update existing weekly plan
    //                            }
    //                            else
    //                            {
    //                                 Create new weekly plan
    //                            }
    //                        }
    //                    }
    //                    else
    //                    {
    //                         Create new monthly plan and weekly plans
    //                    }
    //                }
    //            }
    //        }

    //         Save changes to the database
    //        db.SaveChanges();
    //    }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

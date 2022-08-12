using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using TradeMarketSystem.Controllers;
using TradeMarketSystem.Core.Localization;
using TradeMarketSystem.Core.Model.Common;
using TradeMarketSystem.Core.Model.CustomerService;
using TradeMarketSystem.Core.Model.Daily_Market;
using TradeMarketSystem.DataAccess.Context;
using TradeMarketSystem.Helper.Common;
using TradeMarketSystem.Helpers;
using TradeMarketSystem.Web.Hubs;

namespace TradeMarketSystem.Areas.DailyMarket.Controllers.Setting
{
    public class WeeklyPlanController : BaseController
    {
        private TradeDbContext db = new TradeDbContext();

        [AuthorizeRoles(DailyMarketRoles.DMPriceEntry, DailyMarketRoles.DMPlanApprover, DailyMarketRoles.DMPlanner)]
        public ActionResult Index()
        {
            ViewData["BudgetYears"] = db.BudgetYears.ToList();
            ViewData["Periods"] = db.Period.ToList();
            var quarters = db.QuarterlyPlans.ToList();

            ViewData["Quarters"] = quarters.Select(s => new {
                QuarterlyPlanId = s.QuarterlyPlanId,

                Name = s.Name,
                NameAm = s.NameAm
            }).ToList();
            var months = db.MonthlyPlans.ToList();
            ViewData["MonthlyPlans"] = months.Select(s => new {
                MonthlyPlanId = s.MonthlyPlanId,

                Name = getName(s.MonthlyPlanId),
                NameAm = getName(s.MonthlyPlanId)
            }).ToList();
            return View();
        }

        private string getName(int monthlyPlanId)
        {
            var month = db.MonthlyPlans.Find(monthlyPlanId);
            if (month != null)
            {
                var quarter = db.QuarterlyPlans.Find(month.QuarterlyPlanId);
                if (quarter != null)
                {
                    var halfYear = db.HalfYearlyPlans.Find(quarter.HalfYearlyPlanId);
                    if (halfYear != null)
                    {

                        var year = db.YearlyPlans.Find(halfYear.YearlyPlanId);
                        if (year != null)
                        {
                            var budgetYear = db.BudgetYears.Find(year.BudgetYearId);
                            if (budgetYear != null)
                            {
                                string cultureName = "";
                                HttpCookie cultureCookie = Request.Cookies["_culture"];
                                if (cultureCookie != null)
                                {
                                    cultureName = cultureCookie.Value;
                                }
                                if (cultureName.IsCaseInsensitiveEqual("am") || cultureName.IsCaseInsensitiveEqual("or"))
                                {
                                    return budgetYear.Year + "_" + halfYear.NameAm + "_" + quarter.NameAm + "_" + month.NameAm;

                                }
                                else
                                {
                                    return budgetYear.Year + "_" + halfYear.Name + "_" + quarter.Name + "_" + month.Name;

                                }
                            }
                        }
                    }
                }

            }
            return "";

        }

        public ActionResult WeeklyPlans_Read([DataSourceRequest] DataSourceRequest request, int id,string user)
        {
            IQueryable<WeeklyPlan> WeeklyPlans = db.WeeklyPlans.Where(q=>q.MonthlyPlanId==id);
            if(user!=null)
            {
                WeeklyPlans = WeeklyPlans.Where(q => q.User == user);
            }

           

            DataSourceResult result = WeeklyPlans.ToDataSourceResult(request, weeklyPlan => new
            {
                MonthlyPlanId = weeklyPlan.MonthlyPlanId,
                WeeklyPlanId = weeklyPlan.WeeklyPlanId,
                StartDate = weeklyPlan.StartDate,
                EndDate = weeklyPlan.EndDate,
                EndDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(weeklyPlan.EndDate.Value.Day, weeklyPlan.EndDate.Value.Month, weeklyPlan.EndDate.Value.Year),
                StartDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(weeklyPlan.StartDate.Value.Day, weeklyPlan.StartDate.Value.Month, weeklyPlan.StartDate.Value.Year),
                Plan = weeklyPlan.Plan,
                Name = weeklyPlan.Name,
                NameAm = weeklyPlan.NameAm,
                Actual = weeklyPlan.Actual,
                IsApproved = weeklyPlan.IsApproved,
                IsTransferred = weeklyPlan.IsTransferred,
                Performance = weeklyPlan.Performance,
                User = weeklyPlan.User
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult WeeklyPlanUsers_Read([DataSourceRequest] DataSourceRequest request, int id,string user)
        {
            user = User.Identity.Name;
            IQueryable<WeeklyPlan> WeeklyPlans = db.WeeklyPlans.Where(q=>q.MonthlyPlanId==id);

            var WeeklyPlanns = WeeklyPlans.Where(q => q.User == User.Identity.Name);
            var weekPlanList = new List<WeeklyPlan>();
            WeeklyPlans = WeeklyPlans.Take(4);
            if (WeeklyPlans != null)
            {

                foreach (var weekPlan in WeeklyPlans)
                {
                    var Item = WeeklyPlanns.Where(q => q.Name == weekPlan.Name).FirstOrDefault();
                    if (Item != null)
                    {
                        weekPlan.WeeklyPlanId = Item.WeeklyPlanId;
                        weekPlan.Plan = Item.Plan;
                        weekPlan.User = Item.User;
                    }
                    else
                    {
                        weekPlan.User = null;
                        weekPlan.Plan = 0;

                    }


                    weekPlanList.Add(weekPlan);
                }
                WeeklyPlans = weekPlanList.AsQueryable();

            }



            DataSourceResult result = WeeklyPlans.ToDataSourceResult(request, weeklyPlan => new
            {
                MonthlyPlanId = weeklyPlan.MonthlyPlanId,
                WeeklyPlanId = weeklyPlan.WeeklyPlanId,
                StartDate = weeklyPlan.StartDate,
                EndDate = weeklyPlan.EndDate,
                EndDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(weeklyPlan.EndDate.Value.Day, weeklyPlan.EndDate.Value.Month, weeklyPlan.EndDate.Value.Year),
                StartDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(weeklyPlan.StartDate.Value.Day, weeklyPlan.StartDate.Value.Month, weeklyPlan.StartDate.Value.Year),
                Plan = weeklyPlan.Plan,
                Name = weeklyPlan.Name,
                NameAm = weeklyPlan.NameAm,
                Actual = weeklyPlan.Actual,
                IsApproved = weeklyPlan.IsApproved,
                IsTransferred = weeklyPlan.IsTransferred,
                Performance = weeklyPlan.Performance,
                User = weeklyPlan.User
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult WeeklyPlanAdmins_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            IQueryable<WeeklyPlan> WeeklyPlans = db.WeeklyPlans.Where(q => q.MonthlyPlanId == id);
           
            
                var weeklyValidationUsers = db.WeeklyPlanValidations.Where(q => q.IsWeeklyApproved && q.MonthlyPlanId == id).ToList().Select(s => s.User);
                WeeklyPlans = WeeklyPlans.Where(q => weeklyValidationUsers.Contains(q.User));

            DataSourceResult result = WeeklyPlans.ToDataSourceResult(request, weeklyPlan => new
            {
                MonthlyPlanId = weeklyPlan.MonthlyPlanId,
                WeeklyPlanId = weeklyPlan.WeeklyPlanId,
                StartDate = weeklyPlan.StartDate,
                EndDate = weeklyPlan.EndDate,
                EndDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(weeklyPlan.EndDate.Value.Day, weeklyPlan.EndDate.Value.Month, weeklyPlan.EndDate.Value.Year),
                StartDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(weeklyPlan.StartDate.Value.Day, weeklyPlan.StartDate.Value.Month, weeklyPlan.StartDate.Value.Year),
                Plan = weeklyPlan.Plan,
                Name = weeklyPlan.Name,
                NameAm = weeklyPlan.NameAm,
                Actual = weeklyPlan.Actual,
                IsApproved = weeklyPlan.IsApproved,
                IsTransferred = weeklyPlan.IsTransferred,
                Performance = weeklyPlan.Performance,
                User = weeklyPlan.User
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeklyPlans_Create([DataSourceRequest] DataSourceRequest request, WeeklyPlan weeklyPlan, int id)
        {
            var monthlPlan = db.MonthlyPlans.Find(id);
            var totMonthPlans = db.MonthlyPlans.Where(q => q.MonthlyPlanId == weeklyPlan.MonthlyPlanId).ToList().Sum(s => s.Plan);
            if ((totMonthPlans + weeklyPlan.Plan) > monthlPlan.Plan)
            {
                ModelState.AddModelError("Error", "Summation of weekly plans must equals monthly plan!");
            }
            var monthlyPlan = db.MonthlyPlans.Find(id);

            if (weeklyPlan.StartDate < monthlyPlan.StartDate || weeklyPlan.StartDate > monthlyPlan.EndDate)
            {
                ModelState.AddModelError("key", "Entered start date between the month period!");

            }
            if (weeklyPlan.EndDate > monthlyPlan.EndDate || weeklyPlan.EndDate < monthlyPlan.StartDate)
            {
                ModelState.AddModelError("key", "Entered start date between the month period!");

            }
            if (ModelState.IsValid)
            {
                var entity = new WeeklyPlan
                {
                    MonthlyPlanId = id,
                    WeeklyPlanId = weeklyPlan.WeeklyPlanId,
                    StartDate = weeklyPlan.StartDate,
                    EndDate = weeklyPlan.EndDate,
                    EndDateEth = weeklyPlan.EndDateEth,
                    StartDateEth = weeklyPlan.StartDateEth,
                    Plan = weeklyPlan.Plan,
                    Name = weeklyPlan.Name,
                    NameAm = weeklyPlan.NameAm,
                    Actual = weeklyPlan.Actual,
                    Performance = weeklyPlan.Performance,
                    User = weeklyPlan.User
                };

                db.WeeklyPlans.Add(entity);
                db.SaveChanges();
                weeklyPlan.WeeklyPlanId = entity.WeeklyPlanId;
                #region Create log
                UserHelper.OperationLog(Request.UserHostAddress, "Create", "Success", User.Identity.Name, "Created Price Entry Record with Id: " + entity.WeeklyPlanId, Core.Model.Global.GlobalEnum.TradeGroup.DailyMarket);
                #endregion
            }

            return Json(new[] { weeklyPlan }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult WeeklyPlans_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<WeeklyPlan> weeklyPlans, int id)
        {

            if (weeklyPlans != null && ModelState.IsValid)
            {
                var monthlyPlan = db.MonthlyPlans.Find(id);
                var weekIds = weeklyPlans.ToList().Select(s => s.WeeklyPlanId);
                var weeklyPlansNotUpdated = db.WeeklyPlans.Where(q => !weekIds.Contains(q.WeeklyPlanId) && q.MonthlyPlanId == id).ToList().Sum(s => s.Plan);
                if ((weeklyPlansNotUpdated + weeklyPlans.Sum(s=>s.Plan) > monthlyPlan.Plan))
                {
                    ModelState.AddModelError("Error", Resource.DailyMarket_WeeklyErrorPlanValidation);
                }
                foreach (var weeklyPlan in weeklyPlans)
                { 
                

                //if (weeklyPlan.StartDate < monthlyPlan.StartDate || weeklyPlan.StartDate > monthlyPlan.EndDate)
                //{
                //    ModelState.AddModelError("key", "Entered start date between the month period!");

                //}
                //if (weeklyPlan.EndDate > monthlyPlan.EndDate || weeklyPlan.EndDate < monthlyPlan.StartDate)
                //{
                //    ModelState.AddModelError("key", "Entered start date between the month period!");

                //}

                if (ModelState.IsValid)
                {
                    var entity = db.WeeklyPlans.Find(weeklyPlan.WeeklyPlanId);
                        var entityPerUser = db.WeeklyPlans.Where(q => q.User == User.Identity.Name&&q.Name==weeklyPlan.Name && q.MonthlyPlanId == weeklyPlan.MonthlyPlanId).FirstOrDefault();
                    if (entity != null)
                    {
                        //if (User.IsInRole("DM_Plan_Approver"))
                        //{
                        //    entity.IsApproved = true;
                        //    entity.ApprovedBy = User.Identity.Name;
                        //}
                        entity.NameAm = weeklyPlan.NameAm;
                        entity.Name = weeklyPlan.Name;
                        entity.Plan = weeklyPlan.Plan;
                        entity.StartDate = weeklyPlan.StartDate;
                        entity.StartDateEth = weeklyPlan.StartDateEth;
                        entity.EndDate = weeklyPlan.EndDate;
                        entity.EndDateEth = weeklyPlan.EndDateEth;
                            var weekValidation = db.WeeklyPlanValidations.Where(q => q.MonthlyPlanId == id && q.User == User.Identity.Name).FirstOrDefault();

                            if (User.IsInRole(DailyMarketRoles.DMPriceEntry)&& entityPerUser!=null)
                        {
                                db.WeeklyPlans.Attach(entity);
                                db.Entry(entity).State = EntityState.Modified;
                                if (weekValidation == null)
                                {
                                    weekValidation = new WeeklyPlanValidation();
                                    weekValidation.User = User.Identity.Name;
                                    weekValidation.MonthlyPlanId = id;
                                    db.WeeklyPlanValidations.Add(weekValidation);
                                    db.SaveChanges();
                                }

                            }
                        else if (User.IsInRole(DailyMarketRoles.DMPriceEntry))
                        {
                               entity.User = User.Identity.Name;
                                db.WeeklyPlans.Add(entity);
                                if(weekValidation==null)
                                {
                                     weekValidation = new WeeklyPlanValidation();
                                   
                                        weekValidation.User = User.Identity.Name;
                                    weekValidation.MonthlyPlanId = id;
                                    db.WeeklyPlanValidations.Add(weekValidation);

                                }

                            }
                            

                        }
                }
            }
                    db.SaveChanges();
                    #region Update log
                    UserHelper.OperationLog(Request.UserHostAddress, "Update", "Success", User.Identity.Name, "Updated LED Display Weekly plans Id: " + weeklyPlans.Select(s=>s.WeeklyPlanId), Core.Model.Global.GlobalEnum.TradeGroup.DailyMarket);
                    #endregion
                


            }

            return Json(new[] { weeklyPlans }.ToDataSourceResult(request, ModelState));
            
            
             
            

        }
        
        public ActionResult WeeklyPlan_SentForApproval([DataSourceRequest] DataSourceRequest request, int id)
        {
            var weeklyPlans = db.WeeklyPlans.Where(q => q.MonthlyPlanId == id&&q.User==User.Identity.Name).ToList();

            var monthlyPlan = db.MonthlyPlans.Find(id);
            var weekPlans = db.WeeklyPlanValidations.Where(q => q.MonthlyPlanId == id).ToList();
            var weekValidationPlan = db.WeeklyPlanValidations.Find(id);
            var weekValidation = db.WeeklyPlanValidations.Where(q => q.MonthlyPlanId == id && q.User == User.Identity.Name).FirstOrDefault();

            object response = null;
            if (monthlyPlan != null&& weekValidation!=null)
            {
                var weekIds = weeklyPlans.ToList().Select(s => s.WeeklyPlanId);
                var weeklyPlansU = db.WeeklyPlans.Where(q => !weekIds.Contains(q.WeeklyPlanId) && q.MonthlyPlanId == id).ToList().Sum(s => s.Plan);
                if ((weeklyPlansU  > monthlyPlan.Plan))
                {
                    ModelState.AddModelError("Error", Resource.DailyMarket_WeeklyErrorPlanValidation);
                }

                weekValidation.IsWeeklyTransferred = true;
                db.WeeklyPlanValidations.Attach(weekValidation);
                db.Entry(weekValidation).State = EntityState.Modified;
                var usersInRole = UserExtension.GetUsersInRole(DailyMarketRoles.DMPlanApprover).Select(x => x.UserName).ToList();

               // var users = UserExtension.GetUsersInSelectedWereda(profile.WeredaId).ToList(); 
               foreach(var user in usersInRole)
                {
                    NotifModels notif = new NotifModels
                    {
                        UserId = user,
                        SourceId = weekValidation.WeeklyPlanValidationId,
                        Title = "Approval Request Notification",
                        TitleAm = "የዕቅድ መጽደቅ መጠየቅያ",
                        Message = "You Have new plan  approval request",
                        MessageAm = "አዲስ የዕቅድ ፈቃድ ጥያቄ አለዎት",
                        NotificationType = NotifType.DailyMarketPlan,
                        URL = "/DailyMarket/WeeklyPlanValidation"
                    };

                    Notify.NotifyUser(notif);
                }
                
                db.SaveChanges();
                response = new
                {
                    Success = true,
                    Message = Resource.DailyMarket_PlanSentForApprooval
                };
                return Json(response);

            }
            response = new
            {
                Success = false,
                Message = Resource.DailyMarket_YearlyPlanNotFound

            };
            return Json(response);

        }

        public ActionResult WeeklyPlan_Approval([DataSourceRequest] DataSourceRequest request, int id, string user)
        {
            var weeklyPlans = db.WeeklyPlans.Where(q => q.MonthlyPlanId == id).ToList();

            var monthlyPlan = db.MonthlyPlans.Find(id);
            var weekPlans = db.WeeklyPlanValidations.Where(q => q.MonthlyPlanId == id).ToList();
            var weekValidationPlan = db.WeeklyPlanValidations.Find(id);
            var weekValidation = db.WeeklyPlanValidations.Where(q => q.MonthlyPlanId == id && q.User == user).FirstOrDefault();

            object response = null;
            if (monthlyPlan != null && weekValidation != null)
            {
                var weekIds = weeklyPlans.ToList().Select(s => s.WeeklyPlanId);
                var weeklyPlansU = db.WeeklyPlans.Where(q => !weekIds.Contains(q.WeeklyPlanId) && q.MonthlyPlanId == id).ToList().Sum(s => s.Plan);
                if ((weeklyPlansU > monthlyPlan.Plan))
                {
                    ModelState.AddModelError("Error", Resource.DailyMarket_WeeklyErrorPlanValidation);
                }

                weekValidation.IsWeeklyTransferred = true;
                weekValidation.IsWeeklyApproved = true;
                db.WeeklyPlanValidations.Attach(weekValidation);
                db.Entry(weekValidation).State = EntityState.Modified;
                db.SaveChanges();
                response = new
                {
                    Success = true,
                    Message = Resource.DailyMarket_PlanYearApprovedSucceed
                };
                return Json(response);

            }
            response = new
            {
                Success = false,
                Message = Resource.DailyMarket_PlanYearApprovedSucceed
            };
            //ModelState.AddModelError("Error", "Plan not Found!");

            return Json(response);

        }

        public ActionResult WeeklyPlan_Reject(int monthlyPlanId, string userName, string remark)
        {
            var weekValidation = db.WeeklyPlanValidations.Where(q => q.MonthlyPlanId == monthlyPlanId && q.User == userName).FirstOrDefault();
            object response = null;
            if (weekValidation != null)
            {
                weekValidation.IsWeeklyTransferred = false;
                weekValidation.IsWeeklyRejected = true;
                weekValidation.IsWeeklyApproved = false;
                db.WeeklyPlanValidations.Attach(weekValidation);
                db.Entry(weekValidation).State = EntityState.Modified;
                var entity = new RejecetionRemark
                {

                    ReferenceId = monthlyPlanId,
                    Remark = remark,
                    RejectionPlanType = RejectionPlanType.Weekly,
                    DateCreated = DateTime.Now
                };
                var usersInRole = UserExtension.GetUsersInRole(DailyMarketRoles.DMPriceEntry).Select(x => x.UserName).ToList();

                // var users = UserExtension.GetUsersInSelectedWereda(profile.WeredaId).ToList(); 
                foreach (var user in usersInRole)
                {
                    NotifModels notif = new NotifModels
                    {
                        UserId = user,
                        SourceId = entity.RejecetionRemarkId,
                        Title = "Approval Request Rejected Notification",
                        TitleAm = "የማጽደቅ ጥያቄ ውድቅ የተደረገ ማስታወቂያ",
                        Message = "rejected approval requests notifications",
                        MessageAm = "ውድቅ የተደረገ የጥያቄዎች ማሳወቂያዎች",
                        NotificationType = NotifType.DailyMarketPlan,
                        URL = "/DailyMarket/WeeklyPlan"
                    };

                    Notify.NotifyUser(notif);
                }
                db.RejecetionRemarks.Add(entity);
                db.SaveChanges();
                response = new
                {
                    Success = true,
                    Message = Resource.DailyMarket_PlanYearRejectedSucceed
                };


            }
            else
            {
                response = new
                {
                    Success = false,
                    Message = Resource.DailyMarket_YearlyPlanNotFound
                };

                return Json(response);

            }


            return RedirectToAction("index","WeeklyPlanValidation");

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

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
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.Web.Models;

namespace ExceedERP.Web.Areas.HRM.Controllers
{
    public class HRMJobs : Controller
    {
        private static ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public static void DeActivateContractEmployees()
        {
            var employees = db.Person_Employee.Where(x => x.IsActive).ToList();

            foreach (var emp in employees)
            {
                if (emp.Status == EmployeeStatus.CONTRACT)
                {
                    var contracts = db.Contract.Where(x => x.EmployeeId == emp.EmployeeId).ToList();
                    if (contracts.Any())
                    {
                        DateTime endDate = contracts.Select(x => x.To).Max();

                        if (endDate <= DateTime.Today)
                        {
                            emp.IsActive = false;

                            db.Entry(emp).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                if (emp.Status == EmployeeStatus.FREELANCER)
                {
                    var freelancer = db.Freelancers.Where(x => x.EmployeeId == emp.EmployeeId).ToList();
                    if (freelancer.Any())
                    {
                        DateTime endDate = freelancer.Select(x => x.To).Max();

                        if (endDate <= DateTime.Today)
                        {
                            emp.IsActive = false;

                            db.Entry(emp).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
        }
        public static void CopyEducationFromCourseTrackingToProfile()
        {
            var trainees = db.Trainee.ToList();
            foreach (var trainee in trainees)
            {
                var courseTracking = db.CourseTrackings.FirstOrDefault(x => x.CourseTrackingId == trainee.CourseTrackingId && x.TrainingType == TrainingType.EDUCATION);
                if (courseTracking != null)
                {
                    var education = db.Education.FirstOrDefault(x => x.CourseTrackingId == courseTracking.CourseTrackingId);
                    if (education == null)
                    {
                        var edu = new Education();

                        edu.EmployeeId = trainee.EmployeeId;
                        edu.EduLevel = 0;
                        edu.EduField = 0;

                        var provider = db.Companies.FirstOrDefault(x => x.MainCompanyId == courseTracking.MainCompanyId);
                        if (provider != null)
                        {
                            edu.OrganizationName = provider.BrandName;
                        }

                        edu.From = courseTracking.From;
                        edu.To = courseTracking.To;
                        edu.FromEAmharic = courseTracking.FromAmharic;
                        edu.ToEAmharic = courseTracking.ToAmharic;
                        edu.CourseTrackingId = courseTracking.CourseTrackingId;

                        db.Education.Add(edu);
                        db.SaveChanges();
                    }
                }
            }
        }
        public static void CopyTrainingFromCourseTrackingToProfile()
        {
            var trainees = db.Trainee.ToList();
            foreach (var trainee in trainees)
            {
                var courseTracking = db.CourseTrackings.FirstOrDefault(x => x.CourseTrackingId == trainee.CourseTrackingId && x.TrainingType == TrainingType.TRAINING);
                if (courseTracking != null)
                {
                    var education = db.EmpTrainingEducations.FirstOrDefault(x => x.CourseTrackingId == courseTracking.CourseTrackingId);
                    if (education == null)
                    {
                        var tra = new Training();

                        tra.EmployeeId = trainee.EmployeeId;

                        var provider = db.Companies.FirstOrDefault(x => x.MainCompanyId == courseTracking.MainCompanyId);
                        if (provider != null)
                        {
                            tra.OrganizationName = provider.BrandName;
                        }
                        var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == courseTracking.LookupId);
                        if (lookup != null)
                        {
                            tra.TrainingName = lookup.Description;
                        }
                        var oneCourseTrainees = trainees.Where(x => x.CourseTrackingId == courseTracking.CourseTrackingId).ToList();
                        if (oneCourseTrainees.Any())
                        {
                            tra.TotalCost = Convert.ToDouble(courseTracking.Cost / oneCourseTrainees.Count());
                        }

                        tra.IsTraining = true;

                        tra.From = courseTracking.From;
                        tra.To = courseTracking.To;
                        tra.FromTrAm = courseTracking.FromAmharic;
                        tra.ToTraAm = courseTracking.ToAmharic;
                        tra.From = courseTracking.From;
                        tra.To = courseTracking.To;
                        tra.CourseTrackingId = courseTracking.CourseTrackingId;

                        db.EmpTrainingEducations.Add(tra);
                        db.SaveChanges();
                    }
                }
            }
        }
        public static void TerminateEmployees()
        {
            ApplicationDbContext adb = new ApplicationDbContext();

            var terminations = db.Terminations.Where(x => x.IsOnlineApproved).ToList();
            foreach (var term in terminations)
            {
                if (term.TerminationDate >= DateTime.Today)
                {
                    var employee = db.Person_Employee.Find(term.EmployeeId);
                    if (employee != null)
                    {
                        employee.IsActive = false;
                        db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == term.EmployeeId && x.isCurrent);
                    if (placement != null)
                    {
                        placement.isCurrent = false;
                        db.Entry(placement).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    var user = adb.Users.FirstOrDefault(x => x.EmployeeId == term.EmployeeId);
                    if (user != null)
                    {
                        user.IsEnabled = false;

                        adb.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        adb.SaveChanges();
                    }

                }

            }
        }
        public static void CheckDiscipline()
        {
            var disciplineCases = db.DisciplinaryCases.Where(x => x.IsActive).ToList();

            foreach (var disc in disciplineCases)
            {
                var disciplineMeasure = db.EmpDisciplineMeasure.FirstOrDefault(x => x.DisciplinaryCaseId == disc.DisciplinaryCaseId);
                if (disciplineMeasure != null)
                {
                    int totalPunishmentDays = disciplineMeasure.Month * 30;
                    int daysPunishedSoFar = (DateTime.Today - disciplineMeasure.DecisionDate).Days;

                    if (daysPunishedSoFar >= totalPunishmentDays)
                    {
                        disciplineMeasure.IsExpired = true;

                        db.Entry(disciplineMeasure).State = System.Data.Entity.EntityState.Modified;

                        disc.IsActive = false;

                        db.Entry(disc).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                    }
                }
            }
        }
        public static void CheckAndDeleteNotification()
        {
            var notifications = db.Notifications.Where(x => x.IsRead).ToList();

            foreach (var notif in notifications)
            {
                var dif = (DateTime.Now - notif.Date).Days;

                if (dif >= 3)
                {
                    // db.Notifications.Remove(notif);
                    db.Entry(notif).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

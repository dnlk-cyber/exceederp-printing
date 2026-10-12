using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class TrainingAndEducationOneByOneODS:IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TrainingAndEducationOneByeOneVM> GetTrainingAndEducation(string id, string year)
        {
            List<TrainingAndEducationOneByeOneVM> teLists = new List<TrainingAndEducationOneByeOneVM>();

            var courseTrakings = db.CourseTrackings.ToList();
            var trainees = db.Trainee.DistinctByField(x => x.EmployeeId).ToList();
            var trYears = courseTrakings.Select(x => x.GlFiscalYearId);
            var glYears = db.GLFiscalYears.Where(x => trYears.Contains(x.GlFiscalYearId)).ToList();

            if (year != null)
            {
                glYears = glYears.Where(x => x.GlFiscalYearId.ToString() == year).ToList();
            }
            if (id != null)
            {
                trainees = trainees.Where(x => x.EmployeeId.ToString() == id).ToList();
            }

            foreach (var traine in trainees)
            {
                var emp = new TrainingAndEducationOneByeOneVM();
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == traine.EmployeeId);
                if (person != null)
                {
                    emp.EmployeeName = person.FullNameaEnglish;
                }
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == traine.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                    if (mainCompany != null)
                    {
                        emp.WorkUnit = mainCompany.Name;
                    }
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
                    if (branch != null)
                    {
                        emp.Branch = branch.Name;
                    }
                }
                var oneEmp = trainees.DistinctByField(x => x.CourseTrackingId).Where(x => x.EmployeeId == traine.EmployeeId);
                var allOneEmp = db.Trainee.Where(x => x.EmployeeId == traine.EmployeeId);
                if (id != null)
                {
                    allOneEmp = allOneEmp.Where(x => x.EmployeeId.ToString() == id);
                }
                List<CourseTracking> ctList = new List<CourseTracking>();

                foreach (var t in allOneEmp)
                {
                    var tr = db.CourseTrackings.FirstOrDefault(x => x.CourseTrackingId == t.CourseTrackingId);
                    if (tr != null)
                    {
                        ctList.Add(tr);
                    }
                }
                emp.Years = new List<Year>();

                foreach (var yr in glYears)
                {

                    //emp.EducationVMs = new List<EducationVM>();
                    //emp.TrainingVMs = new List<TrainingVM>();
                    foreach (var eEmp in oneEmp)
                    {

                        //var traning = new TrainingVM();
                        var courseTs = ctList.Where(x => x.GlFiscalYearId == yr.GlFiscalYearId);
                        foreach (var courseT in courseTs)
                        {
                            var y = new Year();
                            y.Name = yr.Name;
                            if (courseT != null && courseT.TrainingType == Core.Domain.Common.TrainingType.TRAINING)
                            {
                                var look = db.Lookups.FirstOrDefault(x => x.LookupId == courseT.LookupId && x.LookupType == Core.Domain.Common.LookUpTypes.COURSE);
                                if (look != null)
                                {
                                    y.TrainingType = look.Description;
                                }

                            }
                            if (courseT != null && courseT.TrainingType == Core.Domain.Common.TrainingType.EDUCATION)
                            {
                                var look = db.Lookups.FirstOrDefault(x => x.LookupId == courseT.LookupId && x.LookupType == Core.Domain.Common.LookUpTypes.COURSE);
                                if (look != null)
                                {
                                    y.EduType = look.Description;
                                }

                            }
                            emp.Years.Add(y);
                        }


                    }
                    //foreach (var eEmp in oneEmp)
                    //{
                    //    var edu = new EducationVM();
                    //    var courseT = courseTrakings.FirstOrDefault(x => x.CourseTrackingId == eEmp.CourseTrackingId);

                    //    if (courseT != null && courseT.TrainingType == Core.Domain.Common.TrainingType.EDUCATION)
                    //    {
                    //        var look = db.Lookups.FirstOrDefault(x => x.LookupId == courseT.LookupId && x.LookupType == Core.Domain.Common.LookUpTypes.COURSE);
                    //        if (look != null)
                    //        {
                    //            edu.EduType = look.Description;
                    //        }

                    //    }
                    //    emp.EducationVMs.Add(edu);
                    //}
                    //emp.Years.Add(y);

                }
                teLists.Add(emp);

            }


            #region Commented
            //List<Trainee> traineeList = new List<Trainee>();
            //List<CourseTracking> trackingList = new List<CourseTracking>();
            //if (year != null)
            //{
            //    courseTrakings = courseTrakings.Where(x => x.GlFiscalYearId.ToString() == year).ToList();
            //    foreach (var course in courseTrakings)
            //    {
            //        var tr = trainees.Where(x => x.CourseTrackingId == course.CourseTrackingId).ToList();

            //        traineeList.AddRange(tr);
            //    }
            //    trainees = traineeList;
            //}
            //if (id != null)
            //{
            //    trainees = trainees.Where(x => x.EmployeeId.ToString() == id).ToList();

            //    foreach (var tra in trainees)
            //    {
            //        var tr = courseTrakings.FirstOrDefault(x => x.CourseTrackingId == tra.CourseTrackingId);
            //        trackingList.Add(tr);
            //    }
            //    courseTrakings = trackingList;
            //}

            //var traineesss = trainees.DistinctByFiled(x=>x.EmployeeId);
            //foreach(var train in traineesss)
            //{
            //    var courseTracking = new TrainingAndEducationOneByeOneVM();

            //    var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == train.EmployeeId);
            //    if(person != null)
            //    {
            //        courseTracking.EmployeeName = person.FullNameaEnglish;
            //    }
            //    courseTracking.Years = new List<Year>();
            //    foreach(var c in courseTrakings)
            //    {
            //        var yr = new Year();
            //        List<CourseTracking> trackList = new List<CourseTracking>();
            //        var cts = courseTrakings.Where(x=>x.GlFiscalYearId == c.GlFiscalYearId).ToList();
            //        foreach(var ct in cts)
            //        {
            //            trackList.Add(ct);
            //        }
            //        var fiscalYear = db.GLFiscalYears.FirstOrDefault(x=>x.GlFiscalYearId == c.GlFiscalYearId);
            //        if(fiscalYear != null)
            //        {
            //            yr.Name = fiscalYear.Name;
            //        }
            //        foreach(var t in trainees)
            //        {

            //        }
            //    }
            //}
            ////foreach(var course in courseTrakings)
            ////{

            ////    trainees = trainees.Where(x=>x.CourseTrackingId == course.CourseTrackingId).ToList();
            ////    if(trainees.Any())
            ////    {
            ////        foreach(var tra in trainees)
            ////        {
            ////            var courseTracking = new TrainingAndEducationOneByeOneVM();

            ////            var emp = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == tra.EmployeeId);
            ////            if(emp != null)
            ////            {
            ////                courseTracking.EmployeeName = emp.FullNameaEnglish;
            ////            }
            ////            var place = db.Placements.FirstOrDefault(x=>x.EmployeeId == tra.EmployeeId && x.isCurrent);
            ////            if(place != null)
            ////            {
            ////                var org = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId == place.OrgStructureId);
            ////                if(org != null)
            ////                {
            ////                    courseTracking.WorkUnit = org.Name;
            ////                }
            ////                var branch = db.Branch.FirstOrDefault(x=>x.BranchId == place.BranchId);
            ////                if(branch != null)
            ////                {
            ////                    courseTracking.Branch = branch.Name;
            ////                }
            ////            }
            ////        }
            ////    }

            ////}
            #endregion
            return teLists;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeEductaionObjectDataSource : List<EmployeeEductaion>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeEductaion> GetEmployeeTraining(string eduLevel, string eduField, string dept, DateTime from, DateTime to, string branchId, string id)
        {
            List<EmployeeEductaion> EmployeeInfoList = new List<EmployeeEductaion>();
            EmployeeEductaion EmployeeInfo = new EmployeeEductaion();
            db = new ExceedDbContext();
            var branches = db.Branch.ToList();
            db = new ExceedDbContext();
            var jobs = db.Lookups.ToList();
            db = new ExceedDbContext();
            var providers = db.Companies.Where(x => x.Category == "1").ToList();
            db = new ExceedDbContext();
            DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
            DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
            var placements = db.Placements.ToList();
            List<Person_Employee> employees;
            if (branchId != null && dept != null)
            {
                employees = EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(dept)).ToList();
            }
            else if (branchId != null && dept == null)
            {
                employees = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).ToList();
            }
            else if (branchId == null && dept != null)
            {
                employees = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(dept)).ToList();
            }
            
            else
            {

                employees = db.Person_Employee.Where(x => x.IsActive).ToList();

            }
            if(id != null)
            {
                employees = employees.Where(x => x.EmployeeId.ToString() == id).ToList();
            }
            #region new code - get report data

            var courses = db.Education.ToList();
            if (eduLevel != null)
            {
                courses = db.Education.Where(x => x.EduLevel.ToString() == eduLevel).ToList();
            }

            if (eduField != null)
            {
                courses = db.Education.Where(x => x.EduField.ToString() == eduField).ToList();
            }
            placements = db.Placements.Where(x => x.isCurrent).ToList();
            foreach (var pl in placements)
            {
                foreach (var e in employees.Where(x => x.IsActive && x.EmployeeId == pl.EmployeeId))
                {

                    EmployeeInfo = new EmployeeEductaion();
                    EmployeeInfo.EmployeeId = e.EmployeeCode;
                    if (e.Gender == 0)
                    {
                        EmployeeInfo.Sex = "ወንድ";
                    }
                    else
                    {
                        EmployeeInfo.Sex = "ሴት";
                    }
                    EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;

                    var place = placements.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
                    if (place != null)
                    {

                        var deptt =
                            db.MainCompanyStructures.Where(
                                    x => x.OrgStructureId == place.OrgStructureId)
                                .FirstOrDefault();
                        if (deptt != null)
                        {
                            EmployeeInfo.Departement = deptt.Name;
                        }
                    }


                    var empeducation = courses.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    foreach (var edu in empeducation)
                    {
                        var empedu = new EmployeeEductaionList();
                        var level = jobs.Where(x => x.LookupId == edu.EduLevel).FirstOrDefault();
                        if (level != null)
                        {
                            empedu.EduLevel = level.Description;
                        }
                        var field = jobs.Where(x => x.LookupId == edu.EduField).FirstOrDefault();
                        if (field != null)
                        {
                            empedu.EduField = field.Description;
                        }

                        if (edu.From != null)
                        {
                            DateTime eduform = new DateTime(edu.From.Value.Year, edu.From.Value.Month, edu.From.Value.Day);

                            var dateFrom = XAPI.EthiopicDateTime.GetEthiopicDate(eduform.Day, eduform.Month, eduform.Year);
                            empedu.From = dateFrom;// eduform.ToShortDateString();
                        }
                        DateTime eduto = new DateTime(edu.To.Year, edu.To.Month, edu.To.Day);
                        var dateTo = XAPI.EthiopicDateTime.GetEthiopicDate(eduto.Day, eduto.Month, eduto.Year);
                        empedu.To = dateTo;// eduto.ToShortDateString();

                        EmployeeInfo.EmployeeEductaionList.Add(empedu);
                    }
                    EmployeeInfoList.Add(EmployeeInfo);
                }
            }
            #endregion

            foreach (var emp in EmployeeInfoList)
            {
                if (emp.EmployeeEductaionList.Count > 0)
                    emp.Assigned = true;
                else
                    emp.Assigned = false;
            }

            return EmployeeInfoList;
            #region commented
            //if (eduLevel != null && eduField == null && dept != null)
            //{
            //    #region

            //    db = new ExceedDbContext();
            //    var courses = db.Education.Where(x => x.EduLevel.ToString() == eduLevel).ToList();
            //    if (courses.Any())
            //    {
            //        foreach (var l in courses)
            //        {
            //            db = new ExceedDbContext();
            //            var Assigned = placements.Where(x => x.OrgStructureId.ToString() == dept);
            //            foreach (var t in Assigned)
            //            {
            //                db = new ExceedDbContext();
            //                var emplist = employees.Where(x => x.EmployeeId == t.EmployeeId && x.IsActive).ToList();

            //                if (emplist.Any())
            //                {
            //                    foreach (var e in emplist)
            //                    {
            //                        if (l.From != null)
            //                        {
            //                            DateTime eduform = new DateTime(l.From.Value.Year, l.From.Value.Month,
            //                                l.From.Value.Day);
            //                            DateTime eduto = new DateTime(l.To.Year, l.To.Month,
            //                                l.To.Day);
            //                            if (eduform >= dateOne && eduto <= dateTow)
            //                            {
            //                                EmployeeInfo = new EmployeeEductaion();
            //                                EmployeeInfo.EmployeeId = e.EmployeeCode;
            //                                if (e.Gender == 0)
            //                                {
            //                                    EmployeeInfo.Sex = "MALE";
            //                                }
            //                                else
            //                                {
            //                                    EmployeeInfo.Sex = "FEMALE";
            //                                }
            //                                EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
            //                                EmployeeInfo.From = eduform.ToShortDateString();
            //                                EmployeeInfo.To = eduto.ToShortDateString();

            //                                var place =
            //                                    placements.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
            //                                if (place != null)
            //                                {

            //                                    var deptt =
            //                                        db.MainCompanyStructures.Where(
            //                                            x => x.OrgStructureId == place.OrgStructureId)
            //                                            .FirstOrDefault();
            //                                    if (deptt != null)
            //                                    {
            //                                        EmployeeInfo.Departement = deptt.Name;
            //                                    }
            //                                }
            //                                var field = jobs.Where(x => x.LookupId == l.EduField).FirstOrDefault();
            //                                if (field != null)
            //                                {
            //                                    EmployeeInfo.EduField = field.Description;
            //                                }



            //                                var level = jobs.Where(x => x.LookupId == l.EduLevel).FirstOrDefault();
            //                                if (level != null)
            //                                {
            //                                    EmployeeInfo.EduLevel = level.Description;
            //                                }
            //                                EmployeeInfo.From = eduform.ToShortDateString();
            //                                EmployeeInfo.To = eduto.ToShortDateString();
            //                                EmployeeInfoList.Add(EmployeeInfo);
            //                            }
            //                        }
            //                    }
            //                }

            //            }
            //        }
            //    }

            //    #endregion
            //}
            //else if (eduLevel != null && eduField != null && dept == null)
            //{
            //    #region

            //    db = new ExceedDbContext();
            //    var courses =
            //        db.Education.Where(x => x.EduField.ToString() == eduLevel && x.EduLevel.ToString() == eduLevel)
            //            .ToList();
            //    if (courses.Any())
            //    {
            //        foreach (var l in courses)
            //        {
            //            db = new ExceedDbContext();
            //            //  var Assigned = placements.Where(x => x.OrgStructureId.ToString() == dept);

            //            db = new ExceedDbContext();
            //            var emplist = employees.Where(x => x.EmployeeId == l.EmployeeId && x.IsActive).ToList();

            //            if (emplist.Any())
            //            {
            //                foreach (var e in emplist)
            //                {
            //                    if (l.From != null)
            //                    {
            //                        DateTime eduform = new DateTime(l.From.Value.Year, l.From.Value.Month,
            //                            l.From.Value.Day);
            //                        DateTime eduto = new DateTime(l.To.Year, l.To.Month,
            //                            l.To.Day);
            //                        if (eduform >= dateOne && eduto <= dateTow)
            //                        {
            //                            EmployeeInfo = new EmployeeEductaion();
            //                            EmployeeInfo.EmployeeId = e.EmployeeCode;
            //                            if (e.Gender == 0)
            //                            {
            //                                EmployeeInfo.Sex = "MALE";
            //                            }
            //                            else
            //                            {
            //                                EmployeeInfo.Sex = "FEMALE";
            //                            }
            //                            EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
            //                            EmployeeInfo.From = eduform.ToShortDateString();
            //                            EmployeeInfo.To = eduto.ToShortDateString();

            //                            var place = placements.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
            //                            if (place != null)
            //                            {

            //                                var deptt =
            //                                    db.MainCompanyStructures.Where(
            //                                        x => x.OrgStructureId == place.OrgStructureId)
            //                                        .FirstOrDefault();

            //                                if (deptt != null)
            //                                {
            //                                    EmployeeInfo.Departement = deptt.Name;
            //                                }
            //                            }
            //                            var field = jobs.Where(x => x.LookupId == l.EduField).FirstOrDefault();
            //                            if (field != null)
            //                            {
            //                                EmployeeInfo.EduField = field.Description;
            //                            }



            //                            var level = jobs.Where(x => x.LookupId == l.EduLevel).FirstOrDefault();
            //                            if (level != null)
            //                            {
            //                                EmployeeInfo.EduLevel = level.Description;
            //                            }
            //                            EmployeeInfo.From = eduform.ToShortDateString();
            //                            EmployeeInfo.To = eduto.ToShortDateString();
            //                            EmployeeInfoList.Add(EmployeeInfo);
            //                        }
            //                    }
            //                }
            //            }


            //        }
            //    }

            //    #endregion
            //}

            //else if (eduLevel == null && eduField != null && dept != null)
            //{
            //    #region

            //    db = new ExceedDbContext();
            //    var courses = db.Education.Where(x => x.EduField.ToString() == eduField).ToList();
            //    if (courses.Any())
            //    {
            //        foreach (var l in courses)
            //        {
            //            db = new ExceedDbContext();
            //            var Assigned = placements.Where(x => x.OrgStructureId.ToString() == dept);
            //            foreach (var t in Assigned)
            //            {
            //                db = new ExceedDbContext();
            //                var emplist = employees.Where(x => x.EmployeeId == t.EmployeeId && x.IsActive).ToList();

            //                if (emplist.Any())
            //                {
            //                    foreach (var e in emplist)
            //                    {
            //                        DateTime eduform = new DateTime(l.From.Value.Year, l.From.Value.Month,
            //                            l.From.Value.Day);
            //                        DateTime eduto = new DateTime(l.To.Year, l.To.Month,
            //                            l.To.Day);
            //                        if (eduform >= dateOne && eduto <= dateTow)
            //                        {
            //                            EmployeeInfo = new EmployeeEductaion();
            //                            EmployeeInfo.EmployeeId = e.EmployeeCode;
            //                            if (e.Gender == 0)
            //                            {
            //                                EmployeeInfo.Sex = "MALE";
            //                            }
            //                            else
            //                            {
            //                                EmployeeInfo.Sex = "FEMALE";
            //                            }
            //                            EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
            //                            EmployeeInfo.From = eduform.ToShortDateString();
            //                            EmployeeInfo.To = eduto.ToShortDateString();

            //                            var place =
            //                                placements.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
            //                            if (place != null)
            //                            {

            //                                var deptt =
            //                                    db.MainCompanyStructures.Where(
            //                                        x => x.OrgStructureId == place.OrgStructureId)
            //                                        .FirstOrDefault();
            //                                if (deptt != null)
            //                                {
            //                                    EmployeeInfo.Departement = deptt.Name;
            //                                }
            //                            }
            //                            EmployeeInfo.From = eduform.ToShortDateString();
            //                            var field = jobs.Where(x => x.LookupId == l.EduField).FirstOrDefault();
            //                            if (field != null)
            //                            {
            //                                EmployeeInfo.EduField = field.Description;
            //                            }



            //                            var level = jobs.Where(x => x.LookupId == l.EduLevel).FirstOrDefault();
            //                            if (level != null)
            //                            {
            //                                EmployeeInfo.EduLevel = level.Description;
            //                            }
            //                            EmployeeInfo.To = eduto.ToShortDateString();
            //                            EmployeeInfoList.Add(EmployeeInfo);
            //                        }
            //                    }
            //                }

            //            }
            //        }
            //    }

            //    #endregion
            //}
            //else if (eduLevel == null && eduField == null && dept != null)
            //{
            //    #region

            //    db = new ExceedDbContext();
            //    var courses = db.Education.ToList();
            //    if (courses.Any())
            //    {
            //        foreach (var l in courses)
            //        {
            //            db = new ExceedDbContext();
            //            var Assigned = placements.Where(x => x.OrgStructureId.ToString() == dept);
            //            foreach (var t in Assigned)
            //            {
            //                db = new ExceedDbContext();
            //                var emplist = employees.Where(x => x.EmployeeId == t.EmployeeId && x.IsActive).ToList();

            //                if (emplist.Any())
            //                {
            //                    foreach (var e in emplist)
            //                    {
            //                        DateTime eduform = new DateTime(l.From.Value.Year, l.From.Value.Month,
            //                            l.From.Value.Day);
            //                        DateTime eduto = new DateTime(l.To.Year, l.To.Month,
            //                            l.To.Day);
            //                        if (eduform >= dateOne && eduto <= dateTow)
            //                        {
            //                            EmployeeInfo = new EmployeeEductaion();
            //                            EmployeeInfo.EmployeeId = e.EmployeeCode;
            //                            if (e.Gender == 0)
            //                            {
            //                                EmployeeInfo.Sex = "MALE";
            //                            }
            //                            else
            //                            {
            //                                EmployeeInfo.Sex = "FEMALE";
            //                            }
            //                            EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
            //                            EmployeeInfo.From = eduform.ToShortDateString();
            //                            EmployeeInfo.To = eduto.ToShortDateString();

            //                            var place =
            //                                placements.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
            //                            if (place != null)
            //                            {

            //                                var deptt =
            //                                    db.MainCompanyStructures.Where(
            //                                        x => x.OrgStructureId == place.OrgStructureId)
            //                                        .FirstOrDefault();
            //                                if (deptt != null)
            //                                {
            //                                    EmployeeInfo.Departement = deptt.Name;
            //                                }
            //                            }
            //                            EmployeeInfo.From = eduform.ToShortDateString();
            //                            var field = jobs.Where(x => x.LookupId == l.EduField).FirstOrDefault();
            //                            if (field != null)
            //                            {
            //                                EmployeeInfo.EduField = field.Description;
            //                            }



            //                            var level = jobs.Where(x => x.LookupId == l.EduLevel).FirstOrDefault();
            //                            if (level != null)
            //                            {
            //                                EmployeeInfo.EduLevel = level.Description;
            //                            }
            //                            EmployeeInfo.To = eduto.ToShortDateString();
            //                            EmployeeInfoList.Add(EmployeeInfo);
            //                        }
            //                    }
            //                }

            //            }
            //        }
            //    }

            //    #endregion
            //}

            //else if (eduLevel != null && eduField == null && dept == null)
            //{
            //    #region

            //    db = new ExceedDbContext();
            //    var courses = db.Education.Where(x => x.EduLevel.ToString() == eduLevel).ToList();
            //    if (courses.Any())
            //    {
            //        foreach (var l in courses)
            //        {
            //            db = new ExceedDbContext();
            //            //  var Assigned = placements.Where(x => x.OrgStructureId.ToString() == dept);

            //            db = new ExceedDbContext();
            //            var emplist = employees.Where(x => x.EmployeeId == l.EmployeeId && x.IsActive).ToList();

            //            if (emplist.Any())
            //            {
            //                foreach (var e in emplist)
            //                {
            //                    DateTime eduform = new DateTime(l.From.Value.Year, l.From.Value.Month,
            //                        l.From.Value.Day);
            //                    DateTime eduto = new DateTime(l.To.Year, l.To.Month,
            //                        l.To.Day);
            //                    if (eduform >= dateOne && eduto <= dateTow)
            //                    {
            //                        EmployeeInfo = new EmployeeEductaion();
            //                        EmployeeInfo.EmployeeId = e.EmployeeCode;
            //                        if (e.Gender == 0)
            //                        {
            //                            EmployeeInfo.Sex = "MALE";
            //                        }
            //                        else
            //                        {
            //                            EmployeeInfo.Sex = "FEMALE";
            //                        }
            //                        EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
            //                        EmployeeInfo.From = eduform.ToShortDateString();
            //                        EmployeeInfo.To = eduto.ToShortDateString();

            //                        var place = placements.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
            //                        if (place != null)
            //                        {

            //                            var deptt =
            //                                db.MainCompanyStructures.Where(
            //                                    x => x.OrgStructureId == place.OrgStructureId)
            //                                    .FirstOrDefault();
            //                            if (deptt != null)
            //                            {
            //                                EmployeeInfo.Departement = deptt.Name;
            //                            }
            //                        }
            //                        EmployeeInfo.From = eduform.ToShortDateString();
            //                        var field = jobs.Where(x => x.LookupId == l.EduField).FirstOrDefault();
            //                        if (field != null)
            //                        {
            //                            EmployeeInfo.EduField = field.Description;
            //                        }



            //                        var level = jobs.Where(x => x.LookupId == l.EduLevel).FirstOrDefault();
            //                        if (level != null)
            //                        {
            //                            EmployeeInfo.EduLevel = level.Description;
            //                        }
            //                        EmployeeInfo.To = eduto.ToShortDateString();
            //                        EmployeeInfoList.Add(EmployeeInfo);
            //                    }
            //                }
            //            }


            //        }
            //    }

            //    #endregion
            //}
            //else if (eduLevel == null && eduField != null && dept == null)
            //{
            //    #region

            //    db = new ExceedDbContext();
            //    var courses = db.Education.Where(x => x.EduField.ToString() == eduField).ToList();
            //    if (courses.Any())
            //    {
            //        foreach (var l in courses)
            //        {
            //            db = new ExceedDbContext();
            //            //  var Assigned = placements.Where(x => x.OrgStructureId.ToString() == dept);

            //            db = new ExceedDbContext();
            //            var emplist = employees.Where(x => x.EmployeeId == l.EmployeeId && x.IsActive).ToList();

            //            if (emplist.Any())
            //            {
            //                foreach (var e in emplist)
            //                {
            //                    //DateTime eduform = new DateTime(l.From.Value.Year, l.From.Value.Month,
            //                    //    l.From.Value.Day);
            //                    //DateTime eduto = new DateTime(l.To.Year, l.To.Month,
            //                    //    l.To.Day);
            //                    //if (eduform >= dateOne && eduto <= dateTow)
            //                    //{
            //                    EmployeeInfo = new EmployeeEductaion();
            //                    EmployeeInfo.EmployeeId = e.EmployeeCode;
            //                    if (e.Gender == 0)
            //                    {
            //                        EmployeeInfo.Sex = "MALE";
            //                    }
            //                    else
            //                    {
            //                        EmployeeInfo.Sex = "FEMALE";
            //                    }
            //                    EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
            //                    if (l.From != null) EmployeeInfo.From = l.From.Value.ToShortDateString();
            //                    EmployeeInfo.To = l.To.ToShortDateString();

            //                    var place = placements.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
            //                    if (place != null)
            //                    {

            //                        var deptt =
            //                            db.MainCompanyStructures.Where(
            //                                x => x.OrgStructureId == place.OrgStructureId)
            //                                .FirstOrDefault();
            //                        if (deptt != null)
            //                        {
            //                            EmployeeInfo.Departement = deptt.Name;
            //                        }
            //                    }
            //                    var field = jobs.Where(x => x.LookupId == l.EduField).FirstOrDefault();
            //                    if (field != null)
            //                    {
            //                        EmployeeInfo.EduField = field.Description;
            //                    }



            //                    var level = jobs.Where(x => x.LookupId == l.EduLevel).FirstOrDefault();
            //                    if (level != null)
            //                    {
            //                        EmployeeInfo.EduLevel = level.Description;
            //                    }
            //                    if (l.From != null) EmployeeInfo.From = l.From.Value.ToShortDateString();
            //                    EmployeeInfo.To = l.To.ToShortDateString();
            //                    EmployeeInfoList.Add(EmployeeInfo);
            //                }
            //                //}
            //            }


            //        }
            //    }

            //    #endregion
            //}
            //else
            //{
            //    #region

            //    db = new ExceedDbContext();
            //    var courses = db.Education.ToList();
            //    if (courses.Any())
            //    {
            //        foreach (var l in courses)
            //        {
            //            db = new ExceedDbContext();

            //            db = new ExceedDbContext();
            //            var emplist = employees.Where(x => x.EmployeeId == l.EmployeeId && x.IsActive).ToList();

            //            if (emplist.Any())
            //            {
            //                foreach (var e in emplist)
            //                {

            //                    //DateTime eduform = new DateTime(l.From.Value.Year, l.From.Value.Month,
            //                    //        l.From.Value.Day);

            //                    //DateTime eduto = new DateTime(l.To.Year, l.To.Month,
            //                    //    l.To.Day);

            //                    EmployeeInfo = new EmployeeEductaion();
            //                    EmployeeInfo.EmployeeId = e.EmployeeCode;
            //                    if (e.Gender == 0)
            //                    {
            //                        EmployeeInfo.Sex = "MALE";
            //                    }
            //                    else
            //                    {
            //                        EmployeeInfo.Sex = "FEMALE";
            //                    }
            //                    EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
            //                    if (l.From != null) EmployeeInfo.From = l.From.Value.ToShortDateString();
            //                    EmployeeInfo.To = l.To.ToShortDateString();

            //                    var place = placements.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
            //                    if (place != null)
            //                    {

            //                        var deptt =
            //                            db.MainCompanyStructures.Where(
            //                                x => x.OrgStructureId == place.OrgStructureId)
            //                                .FirstOrDefault();
            //                        if (deptt != null)
            //                        {
            //                            EmployeeInfo.Departement = deptt.Name;
            //                        }
            //                    }
            //                    var field = jobs.Where(x => x.LookupId == l.EduField).FirstOrDefault();
            //                    if (field != null)
            //                    {
            //                        EmployeeInfo.EduField = field.Description;
            //                    }



            //                    var level = jobs.Where(x => x.LookupId == l.EduLevel).FirstOrDefault();
            //                    if (level != null)
            //                    {
            //                        EmployeeInfo.EduLevel = level.Description;
            //                    }
            //                    if (l.From != null) EmployeeInfo.From = l.From.Value.ToShortDateString();
            //                    EmployeeInfo.To = l.To.ToShortDateString();
            //                    EmployeeInfoList.Add(EmployeeInfo);
            //                }



            //            }
            //        }
            //    }

            //    #endregion
            //}
            //return EmployeeInfoList;
            #endregion
        }
    }
}
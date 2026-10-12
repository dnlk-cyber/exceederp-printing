using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeEductaionObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeEductaion> GetEmployeeEductaion(string eduLevel,string eduField,string dept,
                                                            DateTime? from,DateTime? to,string branchId,
                                                            string bussinessUnitId,string id, string status)
        {
            // Parse filter parameters once
            int? branchIdInt = int.TryParse(branchId, out int bId) ? bId : (int?)null;
            int? businessUnitIdInt = int.TryParse(bussinessUnitId, out int buId) ? buId : (int?)null;
            int? deptId = int.TryParse(dept, out int dId) ? dId : (int?)null;
            int? empIdInt = int.TryParse(id, out int eId) ? eId : (int?)null;
            int? eduLevelInt = int.TryParse(eduLevel, out int elId) ? elId : (int?)null;
            int? eduFieldInt = int.TryParse(eduField, out int efId) ? efId : (int?)null;

            // 0 = No Education Record
            bool noEducation = eduLevelInt.HasValue && eduLevelInt.Value == 0;

            // Base placement query
            var placementsQuery = db.Placements
                .Where(p => p.isCurrent)
                .AsQueryable();

            // Status filter
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Active")
                {
                    var activeIds = db.Person_Employee
                        .Where(x => x.IsActive)
                        .Select(x => x.EmployeeId);

                    placementsQuery = placementsQuery
                        .Where(x => activeIds.Contains(x.EmployeeId));
                }
                else if (status == "InActive")
                {
                    var inactiveIds = db.Person_Employee
                        .Where(x => !x.IsActive)
                        .Select(x => x.EmployeeId);

                    placementsQuery = placementsQuery
                        .Where(x => inactiveIds.Contains(x.EmployeeId));
                }
            }

            // Employee filters
            if (branchIdInt.HasValue)
                placementsQuery = placementsQuery
                    .Where(x => x.BranchId == branchIdInt.Value);

            if (businessUnitIdInt.HasValue)
                placementsQuery = placementsQuery
                    .Where(x => x.BusinessUnitId == businessUnitIdInt.Value);

            if (deptId.HasValue)
                placementsQuery = placementsQuery
                    .Where(x => x.OrgStructureId == deptId.Value);

            if (empIdInt.HasValue)
                placementsQuery = placementsQuery
                    .Where(x => x.EmployeeId == empIdInt.Value);


            // =========================================================
            // NO EDUCATION
            // =========================================================

            if (noEducation)
            {
                // Employees whose EmployeeId does NOT exist in Education
                var educationEmployeeIds = db.Education
                    .Select(x => x.EmployeeId);

                placementsQuery = placementsQuery
                    .Where(x => !educationEmployeeIds.Contains(x.EmployeeId));

                // No need to query Education because these employees
                // have no Education record.
                var noEducationQuery =
                    from p in placementsQuery
                    join e in db.Person_Employee
                        on p.EmployeeId equals e.EmployeeId

                    join r in db.Ranks
                        on p.RankId equals r.RankId into rankGroup
                    from r in rankGroup.DefaultIfEmpty()

                    join dpt in db.MainCompanyStructures
                        on p.OrgStructureId equals dpt.OrgStructureId into deptGroup
                    from dpt in deptGroup.DefaultIfEmpty()

                    select new
                    {
                        e.EmployeeCode,
                        FullNameAmharic =
                            e.FirstName + " " +
                            e.MiddleName + " " +
                            e.LastName,

                        Gender = e.Gender,
                        Grade = r != null ? r.RankName : null,
                        AssignmentDate = p.AssignmentDateAmharic,
                        AssignmentType = p.AssignmentType.ToString(),
                        Departement = dpt != null ? dpt.Name : null,

                        EduLevel = "No Education",
                        EduField = (string)null,
                        FromEAmharic = (string)null,
                        ToEAmharic = (string)null
                    };

                var noEducationResult = noEducationQuery
                    .ToList()
                    .GroupBy(x => new
                    {
                        x.EmployeeCode,
                        x.FullNameAmharic,
                        x.Gender,
                        x.Grade,
                        x.AssignmentDate,
                        x.AssignmentType,
                        x.Departement
                    })
                    .Select(g => new EmployeeEductaion
                    {
                        EmployeeId = g.Key.EmployeeCode,

                        FullName =g.Key.FullNameAmharic,

                        Sex = HrHelper.GetGender(g.Key.Gender),

                        Grade = g.Key.Grade,

                        AssignmentDate = g.Key.AssignmentDate,

                        AssignmentType = g.Key.AssignmentType,

                        Departement = g.Key.Departement,

                        EmployeeEductaionList = new List<EmployeeEductaionList>
                        {
                            new EmployeeEductaionList
                            {
                                EduLevel = "No Education",
                                EduField = null,
                                From = null,
                                To = null
                            }
                        }
                    }).ToList();

                return noEducationResult;
            }


            // =========================================================
            // RESTRICT TO HIGHEST EDUCATION LEVEL (reconcile with Educational Level summary report,
            // which buckets each employee once under their highest attained level)
            // =========================================================

            if (eduLevelInt.HasValue)
            {
                var highestLevelEmployeeIds = db.Education
                    .Join(db.Lookups,
                        edu => edu.EduLevel,
                        lookup => lookup.LookupId,
                        (edu, lookup) => new { edu.EmployeeId, edu.EduLevel, lookup.Index })
                    .GroupBy(x => x.EmployeeId)
                    .Select(g => new
                    {
                        EmployeeId = g.Key,
                        HighestEduLevelId = g.OrderByDescending(x => x.Index).FirstOrDefault().EduLevel
                    })
                    .Where(x => x.HighestEduLevelId == eduLevelInt.Value)
                    .Select(x => x.EmployeeId);

                placementsQuery = placementsQuery
                    .Where(x => highestLevelEmployeeIds.Contains(x.EmployeeId));
            }

            // =========================================================
            // NORMAL EDUCATION FILTER
            // =========================================================

            var educationQuery = db.Education.AsQueryable();

            if (from.HasValue)
                educationQuery = educationQuery
                    .Where(x => x.From >= from.Value);

            if (to.HasValue)
                educationQuery = educationQuery
                    .Where(x => x.To <= to.Value);

            if (eduLevelInt.HasValue)
                educationQuery = educationQuery
                    .Where(x => x.EduLevel == eduLevelInt.Value);

            if (eduFieldInt.HasValue)
                educationQuery = educationQuery
                    .Where(x => x.EduField == eduFieldInt.Value);


            // =========================================================
            // NORMAL QUERY
            // =========================================================

            var query =
                from p in placementsQuery

                join e in db.Person_Employee
                    on p.EmployeeId equals e.EmployeeId

                join edu in educationQuery
                    on e.EmployeeId equals edu.EmployeeId

                join r in db.Ranks
                    on p.RankId equals r.RankId into rankGroup
                from r in rankGroup.DefaultIfEmpty()

                join dpt in db.MainCompanyStructures
                    on p.OrgStructureId equals dpt.OrgStructureId into deptGroup
                from dpt in deptGroup.DefaultIfEmpty()

                join level in db.Lookups
                    on edu.EduLevel equals level.LookupId into levelGroup
                from level in levelGroup.DefaultIfEmpty()

                join field in db.Lookups
                    on edu.EduField equals field.LookupId into fieldGroup
                from field in fieldGroup.DefaultIfEmpty()

                select new
                {
                    e.EmployeeCode,

                    FullNameAmharic =
                        e.FirstName + " " +
                        e.MiddleName + " " +
                        e.LastName,

                    Gender = e.Gender,

                    Grade = r != null ? r.RankName : null,

                    AssignmentDate = p.AssignmentDateAmharic,

                    AssignmentType = p.AssignmentType.ToString(),

                    Departement = dpt != null ? dpt.Name : null,

                    EduLevel = level != null
                        ? level.Description
                        : null,

                    EduField = field != null
                        ? field.Description
                        : null,

                    edu.FromEAmharic,
                    edu.ToEAmharic
                };


            // =========================================================
            // RESULT
            // =========================================================

            var result = query
                .ToList()
                .GroupBy(x => new
                {
                    x.EmployeeCode,
                    x.FullNameAmharic,
                    x.Gender,
                    x.Grade,
                    x.AssignmentDate,
                    x.AssignmentType,
                    x.Departement
                })
                .Select(g => new EmployeeEductaion
                {
                    EmployeeId = g.Key.EmployeeCode,

                    FullName =
                        g.Key.FullNameAmharic,

                    Sex = HrHelper.GetGender(g.Key.Gender),

                    Grade = g.Key.Grade,

                    AssignmentDate = g.Key.AssignmentDate,

                    AssignmentType = g.Key.AssignmentType,

                    Departement = g.Key.Departement,

                    EmployeeEductaionList = g.Select(ed =>
                        new EmployeeEductaionList
                        {
                            EduLevel = ed.EduLevel,
                            EduField = ed.EduField,
                            From = ed.FromEAmharic,
                            To = ed.ToEAmharic
                        })
                        .ToList()
                })
                .ToList();

            return result;
        }

        
        //public List<EmployeeEductaion> GetEmployeeEductaion(string eduLevel, string eduField, 
        //                                                    string dept, DateTime? from, DateTime? to,
        //                                                     string branchId, string bussinessUnitId,
        //                                                     string id, string status)
        //{
        //    // Parse filter parameters once
        //    int? branchIdInt = int.TryParse(branchId, out int bId) ? bId : (int?)null;
        //    int? businessUnitIdInt = int.TryParse(bussinessUnitId, out int buId) ? buId : (int?)null;
        //    int? deptId = int.TryParse(dept, out int dId) ? dId : (int?)null;
        //    int? empIdInt = int.TryParse(id, out int eId) ? eId : (int?)null;
        //    int? eduLevelInt = int.TryParse(eduLevel, out int elId) ? elId : (int?)null;
        //    int? eduFieldInt = int.TryParse(eduField, out int efId) ? efId : (int?)null;

        //    // Base placement query
        //    var placementsQuery = db.Placements
        //        .Where(p => p.isCurrent)
        //        .AsQueryable();

        //    // Status filter
        //    if (!string.IsNullOrEmpty(status))
        //    {
        //        if (status == "Active")
        //        {
        //            var activeIds = db.Person_Employee.Where(x => x.IsActive).Select(x => x.EmployeeId);
        //            placementsQuery = placementsQuery.Where(x => activeIds.Contains(x.EmployeeId));
        //        }
        //        else if (status == "InActive")
        //        {
        //            var inactiveIds = db.Person_Employee.Where(x => !x.IsActive).Select(x => x.EmployeeId);
        //            placementsQuery = placementsQuery.Where(x => inactiveIds.Contains(x.EmployeeId));
        //        }
        //    }

        //    // Filters
        //    if (branchIdInt.HasValue)
        //        placementsQuery = placementsQuery.Where(x => x.BranchId == branchIdInt.Value);

        //    if (businessUnitIdInt.HasValue)
        //        placementsQuery = placementsQuery.Where(x => x.BusinessUnitId == businessUnitIdInt.Value);

        //    if (deptId.HasValue)
        //        placementsQuery = placementsQuery.Where(x => x.OrgStructureId == deptId.Value);

        //    if (empIdInt.HasValue)
        //        placementsQuery = placementsQuery.Where(x => x.EmployeeId == empIdInt.Value);

        //    // Education filter
        //    var educationQuery = db.Education.AsQueryable();
        //    if (from.HasValue)
        //        educationQuery = educationQuery.Where(x => x.From >= from.Value);
        //    if (to.HasValue)
        //        educationQuery = educationQuery.Where(x => x.To <= to.Value);
        //    if (eduLevelInt.HasValue)
        //        educationQuery = educationQuery.Where(x => x.EduLevel == eduLevelInt.Value);
        //    if (eduFieldInt.HasValue)
        //        educationQuery = educationQuery.Where(x => x.EduField == eduFieldInt.Value);

        //    // Only join employees who match both placement and education filters
        //    var query = from p in placementsQuery
        //                join e in db.Person_Employee on p.EmployeeId equals e.EmployeeId
        //                join edu in educationQuery on e.EmployeeId equals edu.EmployeeId
        //                join r in db.Ranks on p.RankId equals r.RankId into rankGroup
        //                from r in rankGroup.DefaultIfEmpty()
        //                join dpt in db.MainCompanyStructures on p.OrgStructureId equals dpt.OrgStructureId into deptGroup
        //                from dpt in deptGroup.DefaultIfEmpty()
        //                join level in db.Lookups on edu.EduLevel equals level.LookupId into levelGroup
        //                from level in levelGroup.DefaultIfEmpty()
        //                join field in db.Lookups on edu.EduField equals field.LookupId into fieldGroup
        //                from field in fieldGroup.DefaultIfEmpty()
        //                select new
        //                {
        //                    e.EmployeeCode,
        //                    FullNameAmharic = e.FirstName + " " + e.MiddleName + " " + e.LastName,
        //                    Gender = e.Gender,
        //                    Grade = r != null ? r.RankName : null,
        //                    AssignmentDate = p.AssignmentDateAmharic,
        //                    AssignmentType = p.AssignmentType.ToString(),
        //                    Departement = dpt != null ? dpt.Name : null,
        //                    EduLevel = level != null ? level.Description : null,
        //                    EduField = field != null ? field.Description : null,
        //                    edu.FromEAmharic,
        //                    edu.ToEAmharic
        //                };
        //    var result = query.ToList()
        //        .GroupBy(x => new { x.EmployeeCode, x.FullNameAmharic, x.Gender, x.Grade, x.AssignmentDate, x.AssignmentType, x.Departement })
        //        .Select(g => new EmployeeEductaion
        //        {
        //            EmployeeId = g.Key.EmployeeCode,
        //            FullName = g.Key.EmployeeCode + "-" + g.Key.FullNameAmharic,
        //            Sex = HrHelper.GetGender(g.Key.Gender),
        //            Grade = g.Key.Grade,
        //            AssignmentDate = g.Key.AssignmentDate,
        //            AssignmentType = g.Key.AssignmentType,
        //            Departement = g.Key.Departement,
        //            EmployeeEductaionList = g.Select(ed => new EmployeeEductaionList
        //            {
        //                EduLevel = ed.EduLevel,
        //                EduField = ed.EduField,
        //                From = ed.FromEAmharic,
        //                To = ed.ToEAmharic
        //            }).ToList()
        //        })
        //        .ToList();

        //    return result;
        //}
        public void Dispose()
        {
            db.Dispose();
        }

        //public List<EmployeeEductaion> GetEmployeeEductaion(string eduLevel, string eduField, string dept, DateTime? from, DateTime? to, string branchId,string bussinessUnitId, string id,string status)
        //{
        //    List<EmployeeEductaion> EmployeeInfoList = new List<EmployeeEductaion>();
        //    EmployeeEductaion EmployeeInfo = new EmployeeEductaion();
        //    var branches = db.Branch.ToList();
        //    var jobs = db.Lookups.ToList();
        //    var providers = db.Companies.Where(x => x.Category == "1").ToList();

        //    var placements = db.Placements.Where(x=>x.isCurrent).ToList();
        //    if (!string.IsNullOrEmpty(status))
        //    {
        //        IQueryable<int> empIdsQuery = null;

        //        if (status == "Active")
        //        {
        //            empIdsQuery = db.Person_Employee.Where(x => x.IsActive).Select(x => x.EmployeeId);
        //            placements = placements.Where(x => empIdsQuery.Contains(x.EmployeeId) && x.isCurrent).ToList();
        //        }
        //        else if (status == "InActive")
        //        {
        //            empIdsQuery = db.Person_Employee.Where(x => !x.IsActive).Select(x => x.EmployeeId);
        //            placements = placements.Where(x => empIdsQuery.Contains(x.EmployeeId) && !x.isCurrent).ToList();
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(branchId ))
        //    {
        //        int.TryParse(branchId, out int braId);
        //        placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
        //    }
        //    if (!string.IsNullOrEmpty(bussinessUnitId))
        //    {
        //        int.TryParse(bussinessUnitId, out int bussinesUnitId);
        //        placements = placements.Where(x=>x.BusinessUnitId==bussinesUnitId).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(dept ))
        //    {
        //        int.TryParse(dept, out int orgId);
        //        placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
        //    }

        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        int.TryParse(id, out int empId);
        //        placements = placements.Where(x => x.EmployeeId == empId).ToList();
        //    }
        //    var employeeEducations = db.Education.Where(x => x.From >= from.Value);

        //    if (from.HasValue&& employeeEducations.Any())
        //    {
        //         employeeEducations = employeeEducations.Where(x => x.From >= from.Value);

        //    }
        //    if (to.HasValue && employeeEducations.Any())
        //    {
        //        employeeEducations = employeeEducations.Where(x => x.To <= to.Value);

        //    }
        //    if(employeeEducations.Any())
        //    {
        //        var employeeIds = employeeEducations.Select(x => x.EmployeeId);
        //        if (employeeIds.Any())
        //        {
        //            placements = placements.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();

        //        }
        //    }
        //    #region new code - get report data

        //    foreach (var pl in placements)
        //    {
        //        var e = db.Person_Employee.Where(x=>x.IsActive).FirstOrDefault(x => x.EmployeeId == pl.EmployeeId);

        //        if (e != null)
        //        {
        //            EmployeeInfo = new EmployeeEductaion();

        //            EmployeeInfo.EmployeeId = e.EmployeeCode;
        //            EmployeeInfo.FullName = e.EmployeeCode+"-"+ e.FullNameAmharic;
        //            EmployeeInfo.Sex = HrHelper.GetGender(e.Gender);
        //            var rank = db.Ranks.Find(pl.RankId);
        //            if(rank != null)
        //            {
        //                EmployeeInfo.Grade = rank.RankName;

        //            }
        //            EmployeeInfo.AssignmentDate = pl.AssignmentDateAmharic;
        //            EmployeeInfo.AssignmentType = pl.AssignmentType.ToString();
        //            var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == pl.OrgStructureId);

        //            if (deptt != null)
        //            {
        //                EmployeeInfo.Departement = deptt.Name;
        //            }

        //            var empeducation = db.Education.Where(x => x.EmployeeId == e.EmployeeId).ToList();
        //            if (eduField != null)
        //            {
        //                int.TryParse(eduField, out int educationField);
        //                empeducation = empeducation.Where(x => x.EduField == educationField).ToList();
        //            }
        //            if (eduLevel != null)
        //            {
        //                int.TryParse(eduLevel, out int educationLevel);
        //                empeducation = empeducation.Where(x => x.EduLevel == educationLevel).ToList();
        //            }
        //            if (from.HasValue && empeducation.Any())
        //            {
        //                empeducation = empeducation.Where(x => x.From >= from.Value).ToList();

        //            }
        //            if (to.HasValue && empeducation.Any())
        //            {
        //                empeducation = empeducation.Where(x => x.To <= to.Value).ToList();

        //            }
        //            foreach (var edu in empeducation)
        //            {
        //                var empedu = new EmployeeEductaionList();
        //                var level = jobs.Where(x => x.LookupId == edu.EduLevel).OrderByDescending(x=>x.Index).FirstOrDefault();
        //                if (level != null)
        //                {
        //                    empedu.EduLevel = level.Description;
        //                }
        //                var field = jobs.Where(x => x.LookupId == edu.EduField).OrderByDescending(x => x.Index).FirstOrDefault();
        //                if (field != null)
        //                {
        //                    empedu.EduField = field.Description;
        //                }




        //                empedu.From = edu.FromEAmharic;
        //                empedu.To = edu.ToEAmharic;;

        //                EmployeeInfo.EmployeeEductaionList.Add(empedu);
        //            }


        //            EmployeeInfoList.Add(EmployeeInfo);
        //        }

        //    }
        //    return EmployeeInfoList;
        //    #endregion


        //}
    }
}
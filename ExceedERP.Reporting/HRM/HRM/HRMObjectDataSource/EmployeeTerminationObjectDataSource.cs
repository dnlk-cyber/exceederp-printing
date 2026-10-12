using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeTerminationObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        private string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeTermination> GetEmployeeTermination( string reason, DateTime? from, DateTime? to, string branchId, string orgStructureId, string jobtitle,string status)
        {
            var result = new List<EmployeeTermination>();

            var terminations = db.Terminations.Where(x => x.IsOnlineApproved);

            var employeeIds = db.Person_Employee
                .Where(x => !x.IsActive||x.IsReHired)
                .Select(x => x.EmployeeId)
                .ToList();

            var placements = db.Placements.Where(x => !x.isCurrent && employeeIds.Contains(x.EmployeeId))
                                             .GroupBy(x => x.EmployeeId)
                                             .Select(g => g
                                                 .OrderByDescending(x => x.AssignmentDate)
                                                 .FirstOrDefault()
                                             ).ToList();

            // ---------- Filters ----------
            if (int.TryParse(branchId, out int braId))
            {
                var ids = placements.Where(x => x.BranchId == braId)
                                    .Select(x => x.EmployeeId);
                terminations = terminations.Where(x => ids.Contains(x.EmployeeId));
            }

            if (int.TryParse(orgStructureId, out int orgId))
            {
                var ids = placements.Where(x => x.OrgStructureId == orgId)
                                    .Select(x => x.EmployeeId);
                terminations = terminations.Where(x => ids.Contains(x.EmployeeId));
            }

            if (int.TryParse(jobtitle, out int jobId))
            {
                var ids = placements.Where(x => x.LookupId == jobId)
                                    .Select(x => x.EmployeeId);
                terminations = terminations.Where(x => ids.Contains(x.EmployeeId));
            }

            if (int.TryParse(reason, out int reasonId))
            {
                terminations = terminations.Where(x => x.LookupId == reasonId);
            }
            if (status == "Active")
            {
               var ids = db.Person_Employee
                .Where(x => x.IsActive && x.IsReHired)
                .Select(x => x.EmployeeId)
                .ToList();
                terminations = terminations.Where(x => ids.Contains(x.EmployeeId));

            }

            if (from.HasValue)
                terminations = terminations.Where(x => x.TerminationDate >= from.Value);

            if (to.HasValue)
                terminations = terminations.Where(x => x.TerminationDate <= to.Value);

            var terminationList = terminations.ToList();

            // ---------- CACHING (important) ----------
            var employees = db.Person_Employee.ToDictionary(x => x.EmployeeId);
            var lookupDict = db.Lookups.ToDictionary(x => x.LookupId);
            var branchDict = db.Branch.ToDictionary(x => x.BranchId);
            var deptDict = db.MainCompanyStructures.ToDictionary(x => x.OrgStructureId);
            var education = db.Education.ToList()
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(x => x.Key, x => x.ToList());

            var placementDict = placements
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());

            // ---------- Mapping ----------
            foreach (var t in terminationList)
            {
                if (!employees.TryGetValue(t.EmployeeId, out var e))
                    continue;

                bool isAmharic = cultureName.Equals("am", StringComparison.OrdinalIgnoreCase) ||
                 cultureName.Equals("am-et", StringComparison.OrdinalIgnoreCase);

                var model = new EmployeeTermination
                {
                    EmployeeId = e.EmployeeCode,
                    Sex = HrHelper.GetLocalizedGender(e.Gender),
                    FullName = (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                        ? $"{e.FirstName} {e.MiddleName} {e.LastName}"
                        : $"{e.FirstNameEnglish} {e.MiddleNameEnglish} {e.LastNameEnglish}",

                    TerminationDate = isAmharic? (!string.IsNullOrWhiteSpace(t.TerminationDateAm) ? t.TerminationDateAm : t.TerminationDate?.ToString("dd.MM.yyyy") ?? string.Empty)
                                        : t.TerminationDate?.ToString("dd.MM.yyyy") ?? string.Empty
                };

                if (lookupDict.TryGetValue(t.LookupId, out var reasonLook))
                    model.Reason = reasonLook.Description;

                if (placementDict.TryGetValue(e.EmployeeId, out var place))
                {
                    if (place != null)
                    {
                        model.AssignmentDate = isAmharic ? (!string.IsNullOrWhiteSpace(place.AssignmentDateAmharic) ? place.AssignmentDateAmharic : place.AssignmentDate.ToString("dd.MM.yyyy") ?? string.Empty)
                                        : t.TerminationDate?.ToString("dd.MM.yyyy") ?? string.Empty;

                        if (branchDict.TryGetValue(place.BranchId, out var br))
                            model.Branch = br.Name ?? br.AmharicName;

                        if (lookupDict.TryGetValue(place.LookupId, out var job))
                            model.JobTitle = job.Description ?? job.DescriptionEng;

                        if (deptDict.TryGetValue(place.OrgStructureId, out var dept))
                            model.Departement = dept.Name;

                        if (education.TryGetValue(place.EmployeeId, out var eduList))
                        {
                            model.EducationField = string.Join(", ",
                                eduList.Select(x =>
                                    lookupDict.TryGetValue(x.EduField, out var lf)
                                        ? lf.Description
                                        : null).Where(x => x != null));

                            model.EducationLevel = string.Join(", ",
                                eduList.Select(x =>
                                    lookupDict.TryGetValue(x.EduLevel, out var ll)
                                        ? ll.Description
                                        : null).Where(x => x != null));
                        }
                    }
                }

                result.Add(model);
            }

            return result;
        }
        //public List<EmployeeTermination> GetEmployeeTermination(string reason, DateTime? from, DateTime? to, string branchId, string orgStructureId, string jobtitle)
        //{
        //    List<EmployeeTermination> EmployeeInfoList = new List<EmployeeTermination>();
        //    EmployeeTermination EmployeeInfo = new EmployeeTermination();
        //    var terminations = db.Terminations.Where(x=>x.IsOnlineApproved).ToList();
        //    var employeeIds = db.Person_Employee.Where(x => !x.IsActive&&x.IsPlaced).Select(x => x.EmployeeId).ToList();

        //    var placements = db.Placements.Where(x => !x.isCurrent&& employeeIds.Contains(x.EmployeeId)).OrderByDescending(x=>x.PlacementId).DistinctByField(x=>x.EmployeeId).ToList();

        //    if (!string.IsNullOrEmpty( branchId))
        //    {
        //        int.TryParse(branchId, out int braId);
        //        var placementEmployeeIDs = placements.Where(x => x.BranchId == braId).Select(x => x.EmployeeId).ToList();
        //        terminations = terminations.Where(x => placementEmployeeIDs.Contains(x.EmployeeId)).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(orgStructureId ))
        //    {
        //        int.TryParse(orgStructureId, out int orgId);
        //        var placementEmployeeIDs = placements.Where(x => x.OrgStructureId == orgId).Select(x => x.EmployeeId).ToList();
        //        terminations = terminations.Where(x => placementEmployeeIDs.Contains(x.EmployeeId)).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(jobtitle ))
        //    {
        //        int.TryParse(jobtitle, out int lookUpId);
        //        var placementEmployeeIDs = placements.Where(x => x.LookupId == lookUpId).Select(x => x.EmployeeId).ToList();
        //        terminations = terminations.Where(x => placementEmployeeIDs.Contains(x.EmployeeId)).ToList();
        //    }

        //    if (!string.IsNullOrEmpty(reason))
        //    {
        //        int.TryParse(reason, out int lookUpId);
        //        terminations = terminations.Where(x => x.LookupId == lookUpId).ToList();
        //    }
        //    if (from.HasValue)
        //    {
        //        terminations = terminations.Where(x => x.TerminationDate >= from.Value).ToList();
        //    }
        //    if (to.HasValue)
        //    {
        //        terminations = terminations.Where(x => x.TerminationDate <= to.Value).ToList();
        //    }


        //    foreach (var l in terminations)
        //    {

        //        db = new ExceedDbContext();

        //        var e = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == l.EmployeeId);

        //        if (e != null)
        //        {
        //            EmployeeInfo = new EmployeeTermination();

        //            EmployeeInfo.EmployeeId = e.EmployeeCode;

        //            EmployeeInfo.Sex = EmployeeInfo.Sex = HrHelper.GetLocalizedGender(e.Gender);
        //            EmployeeInfo.FullName = (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
        //                                ? $"{e.FirstName} {e.MiddleName} {e.LastName}"
        //                                : $"{e.FirstNameEnglish} {e.MiddleNameEnglish} {e.LastNameEnglish}"; ;

        //            var look = db.Lookups.FirstOrDefault(x => x.LookupId == l.LookupId);
        //            if (look != null)
        //            {
        //                EmployeeInfo.Reason = look.Description;
        //            }

        //            EmployeeInfo.TerminationDate = l.TerminationDateAm;// l.TerminationDate.ToShortDateString();
        //            var place = db.Placements.FirstOrDefault(x => x.EmployeeId == e.EmployeeId&&!x.isCurrent);

        //            if (place != null)
        //            {
        //                var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
        //                if (branch != null)
        //                {
        //                    EmployeeInfo.Branch = branch.Name??branch.AmharicName;
        //                }

        //                var jobTitle = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId);
        //                if (jobTitle != null)
        //                {
        //                    EmployeeInfo.JobTitle = jobTitle.Description??jobTitle.DescriptionEng;
        //                }

        //                var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
        //                if (deptt != null)
        //                {
        //                    EmployeeInfo.Departement = deptt.Name;
        //                }
        //                var educations = db.Education.Where(x => x.EmployeeId == place.EmployeeId).ToList();

        //                var educationFields = new List<string>();
        //                var educationLevels = new List<string>();
        //                foreach (var education in educations)
        //                {
        //                    var eduField = db.Lookups
        //                        .FirstOrDefault(x => x.LookupType == LookUpTypes.EDUCATION_FIELD && x.LookupId == education.EduField);
        //                    if (eduField != null)
        //                        educationFields.Add(eduField.Description);

        //                    var eduLevel = db.Lookups
        //                        .FirstOrDefault(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL && x.LookupId == education.EduLevel);
        //                    if (eduLevel != null)
        //                        educationLevels.Add(eduLevel.Description);
        //                }

        //                // Join into comma-separated strings (without trailing comma)
        //                EmployeeInfo.EducationField = string.Join(", ", educationFields);
        //                EmployeeInfo.EducationLevel = string.Join(", ", educationLevels);


        //            }

        //            EmployeeInfoList.Add(EmployeeInfo);

        //        }
        //    }

        //    return EmployeeInfoList;
        //}
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
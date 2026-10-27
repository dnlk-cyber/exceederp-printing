using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Add your project's actual using statements for these:
// using YourProject.Models;
// using YourProject.ViewModels;
// using YourProject.Helpers; // HrHelper, EmployeeHelper
// using YourProject.XAPI;    // EthiopicDateTime

namespace ExceedERP.ApplicationServices.IHRService
{
    public interface IHRService
    {
        List<EmployeeMasterViewModel> GetEmployeeMasterInfo(
            object[] branchId, string BussinessUnitId, string orgStructureId,
            string employeeType, string status, string employee,
            string from, string to, string hireFrom, string hireTo, string religion);
    }

    public class HRService : IHRService
    {
        private ExceedDbContext db = new ExceedDbContext();         // <-- replace with your actual DbContext type
        private readonly string cultureName;

        // Constructor injection (typical if you're using Unity/Autofac/etc.)
        public HRService(ExceedDbContext db)
        {
            this.db = db;
            this.cultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            // If your app stores culture differently (e.g. Session["Culture"], a cookie,
            // or a request-scoped service), swap the line above for that instead.
        }

        public List<EmployeeMasterViewModel> GetEmployeeMasterInfo(object[] branchId, string BussinessUnitId, string orgStructureId, string employeeType, string status, string employee, string from, string to, string hireFrom, string hireTo, string religion)
        {
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate??true;
            var isLocal = (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et") || (cultureName.ToLower() == "om");
            var EmployeeMasterViewModelList = new List<EmployeeMasterViewModel>();

            // Pre-load all lookup data into dictionaries for fast access
            var allLookups = db.Lookups.ToDictionary(x => x.LookupId);
            var allBranches = db.Branch.ToDictionary(x => x.BranchId);
            var allStructures = db.MainCompanyStructures.ToDictionary(x => x.OrgStructureId);
            var allSteps = db.Steps.ToDictionary(x => x.StepId);
            var allRanks = db.Ranks.ToDictionary(x => x.RankId);
            var allEmployeeTypes = db.EmployeeTypes.ToDictionary(x => x.EmployeeTypeId);
            var allLanguages = db.Languages.ToDictionary(x => x.LanguageId);
            var allExperiences = db.Experiences
                   .GroupBy(x => x.EmployeeId).ToDictionary(g => g.Key, g => g.ToList());

            // Start with base query
            var placements = db.Placements.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(status))
            {
                bool isActive = status.ToLower() == "active";
                placements = placements.Where(x => x.isCurrent == isActive);
            }

            if (branchId != null && branchId.Any())
            {
                var branchIds = branchId
                    .Where(b => b != null)
                    .Select(b => Convert.ToInt32(b))
                    .ToList();
                placements = placements.Where(x => branchIds.Contains(x.BranchId));
            }

            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                placements = placements.Where(x => x.OrgStructureId == orgId);
            }

            if (!string.IsNullOrEmpty(BussinessUnitId) && int.TryParse(BussinessUnitId, out int bussUnitId))
            {
                placements = placements.Where(x => x.BusinessUnitId == bussUnitId);
            }

            if (!string.IsNullOrEmpty(employee) && int.TryParse(employee, out int empId))
            {
                placements = placements.Where(x => x.EmployeeId == empId);
            }

            // Handle hire date range filter
            if ((!string.IsNullOrEmpty(hireFrom) || (!string.IsNullOrEmpty(hireTo))))
            {
                int glFiscalIdFrom = 0, glFiscalIdTo = 0;
                int.TryParse(hireFrom, out glFiscalIdFrom);
                int.TryParse(hireTo, out glFiscalIdTo);

                var glYears = db.GLFiscalYears
                    .Where(x => (glFiscalIdFrom == 0 || x.GlFiscalYearId == glFiscalIdFrom) ||
                                (glFiscalIdTo == 0 || x.GlFiscalYearId == glFiscalIdTo))
                    .ToList();

                var dateFrom = glYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalIdFrom)?.DateFrom ??
                              glYears.FirstOrDefault()?.DateFrom;
                var dateTo = glYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalIdTo)?.DateTo ??
                            glYears.FirstOrDefault()?.DateTo;

                if (dateFrom.HasValue || dateTo.HasValue)
                {
                    var empIds = db.Person_Employee
                        .Where(x => (dateFrom == null || x.HiredDate >= dateFrom) &&
                                   (dateTo == null || x.HiredDate <= dateTo))
                        .Select(x => x.EmployeeId)
                        .ToList();

                    placements = placements.Where(x => empIds.Contains(x.EmployeeId));
                }
            }

            // Handle employee type filter
            if (!string.IsNullOrEmpty(employeeType) && int.TryParse(employeeType, out int employeeTypeId))
            {
                var empIds = db.Person_Employee
                    .Where(x => x.EmployeeTypeId == employeeTypeId)
                    .Select(x => x.EmployeeId)
                    .ToList();

                placements = placements.Where(x => empIds.Contains(x.EmployeeId));
            }

            if (!string.IsNullOrEmpty(religion))
            {
                var empIds = db.Person_Employee
                    .Where(x => x.Religion.ToString() == religion)
                    .Select(x => x.EmployeeId)
                    .ToList();

                placements = placements.Where(x => empIds.Contains(x.EmployeeId));
            }

            // Handle experience range filter
            List<int> empColList = new List<int>();
            if (!string.IsNullOrEmpty(from) || !string.IsNullOrEmpty(to))
            {
                int fromYears = 0, toYears = 0;
                int.TryParse(from, out fromYears);
                int.TryParse(to, out toYears);

                var allEmployees = db.Person_Employee.ToDictionary(x => x.EmployeeId);
                var allTerminations = db.Terminations.GroupBy(x => x.EmployeeId).ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.TerminationDate).FirstOrDefault());

                foreach (var pl in placements)
                {
                    if (!allEmployees.TryGetValue(pl.EmployeeId, out var emp)) continue;

                    double totalExpDays = 0;

                    if (allExperiences.TryGetValue(emp.EmployeeId, out var experiences))
                    {
                        totalExpDays += experiences.Sum(ex => (ex.To.Subtract(ex.From)).Days);
                    }

                    if (emp.IsActive)
                    {
                        totalExpDays += (DateTime.Today - emp.HiredDate).TotalDays;
                    }
                    else if (allTerminations.TryGetValue(emp.EmployeeId, out var termination))
                    {
                        totalExpDays += termination.TerminationDate.Value.Subtract(emp.HiredDate).Days;
                    }

                    int totalYears = (int)(totalExpDays / 365);

                    if ((fromYears == 0 || totalYears >= fromYears) &&
                        (toYears == 0 || totalYears <= toYears))
                    {
                        empColList.Add(emp.EmployeeId);
                    }
                }

                placements = placements.Where(x => empColList.Contains(x.EmployeeId));
            }

            var placementDisticts = placements
                .GroupBy(x => x.EmployeeId)
                .Select(g => g.OrderByDescending(x => x.AssignmentDate).FirstOrDefault())
                .ToList();

            var employeeIds = placementDisticts.Select(x => x.EmployeeId).ToList();

            var employees = db.Person_Employee
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .ToDictionary(x => x.EmployeeId);

            var educations = db.Education
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var languageProficiencies = db.LanguageOfProficiencies
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var courseTrackings = db.Trainee
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var trainings = db.EmpTrainingEducations
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var performances = db.MeasurementDefinitions
                        .Where(x => x.EmployeeId.HasValue && employeeIds.Contains(x.EmployeeId.Value))
                        .GroupBy(x => x.EmployeeId.Value)
                        .ToDictionary(g => g.Key, g => g.ToList());

            var disciplineCases = db.DisciplinaryCases
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var placementHistories = db.PlacementHistories
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var allEmpDisciplineMeasures = db.EmpDisciplineMeasure
                .GroupBy(x => x.DisciplinaryCaseId)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());

            var allDisciplineMeasures = db.DisciplineMeasures
                .ToDictionary(x => x.MeasuresId);

            var allDisciplineMeasuresDetails = db.DisciplineMeasuresDetail
                .ToDictionary(x => x.DetailMeasuresId);

            var allDisciplineMeasureByMoney = db.DisciplineMeasuerByMoney
                .GroupBy(x => x.DisciplinaryCaseId)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());

            var pensionNumbers = db.Identifications
                             .Where(x => x.ReferenceType == "Employee" &&
                                         x.Type == IdentificationType.PENSION_NO &&
                                         x.Reference != null)
                             .ToList()
                             .Select(x => new
                             {
                                 EmployeeId = int.TryParse(x.Reference, out var employeeId) ? employeeId : (int?)null,
                                 x.IdNumber
                             })
                             .Where(x => x.EmployeeId.HasValue && employeeIds.Contains(x.EmployeeId.Value))
                             .GroupBy(x => x.EmployeeId.Value)
                             .ToDictionary(g => g.Key, g => g.First().IdNumber);

            foreach (var pl in placementDisticts)
            {
                if (!employees.TryGetValue(pl.EmployeeId, out var e)) continue;

                var viewModel = new EmployeeMasterViewModel
                {
                    DateOfBirth = datePreference ? e.DateOfBirthAm : e.DateOfBirth.ToString("dd.MM.yyyy"),
                    RetirementDate = datePreference ? XAPI.EthiopicDateTime.GetEthiopicDate(e.DateOfBirth.AddYears(60).Day,
                                    e.DateOfBirth.AddYears(60).Month, e.DateOfBirth.AddYears(60).Year) : e.DateOfBirth.AddYears(60).ToString("dd.MM.yyyy"),
                    EmployeeCode = e.EmployeeCode,
                    EmployeeId = e.EmployeeId,
                    FullName = isLocal ? $"{e.FirstName ?? ""} {e.MiddleName ?? ""} {e.LastName ?? ""}".Trim()
                                        : $"{e.FirstNameEnglish ?? ""} {e.MiddleNameEnglish ?? ""} {e.LastNameEnglish ?? ""}".Trim(),
                    Gender = e.Gender.ToString().ToLower(),
                    Religion = e.Religion.ToString().ToLower(),
                    Age = GetAge(e.DateOfBirth),
                    Education = new List<EmployeeEductaionList>(),
                    Training = new List<Training>(),
                    TrainingExt = new List<TrainingExt>(),
                    Languages = new List<Languages>(),
                    EmployeePerformances = new List<EmployeePerformances>(),
                    Discipline = new List<Discipline>(),
                    HireDate = datePreference ? e.HiredDateAmharic : e.HiredDate.ToString("dd.MM.yyyy"),
                    Salary = pl.Salary,
                    ResponsibilityLevel = pl.ResponsibilityLevel.ToString().ToLower(),
                    EmployementType = pl.AssignmentType.ToString().ToLower()
                };

                if (allEmployeeTypes.TryGetValue(e.EmployeeTypeId ?? 0, out var employeeTypeObj))
                {
                    viewModel.EmployeeType = employeeTypeObj?.Name;
                }

                if (!string.IsNullOrEmpty(e.Nation) && int.TryParse(e.Nation, out int nationId))
                {
                    if (allLookups.TryGetValue(nationId, out var nation))
                    {
                        viewModel.Nation = nation?.Description ?? null;
                    }
                }

                if (pensionNumbers.TryGetValue(e.EmployeeId, out var pensionNo))
                {
                    viewModel.PensionNo = pensionNo;
                }

                if (educations.TryGetValue(e.EmployeeId, out var employeeEducations))
                {
                    foreach (var educ in employeeEducations)
                    {
                        allLookups.TryGetValue(educ.EduField, out var field);
                        allLookups.TryGetValue(educ.EduLevel, out var level);

                        viewModel.Education.Add(new EmployeeEductaionList
                        {
                            EduField = field?.Description ?? null,
                            EduLevel = level?.Description ?? null,
                            From = datePreference ? educ?.FromEAmharic
                                                   : (educ?.From.HasValue == true ? educ.From.Value.ToString("dd.MM.yyyy") : null),
                            To = datePreference ? educ?.ToEAmharic
                                        : educ?.To.ToString("dd.MM.yyyy"),
                            Institute = educ?.OrganizationName ?? null
                        });
                    }
                }

                if (!viewModel.Education.Any())
                {
                    viewModel.Education.Add(new EmployeeEductaionList
                    {
                        EduField = " ",
                        EduLevel = " ",
                        From = " ",
                        To = " ",
                        Institute = " "
                    });
                }

                var expSum = 0d;
                double TotalDays = 0;
                var placeWorkedBuilder = new StringBuilder();
                var placeInternalWorkedBuilder = new StringBuilder();

                if (allExperiences.TryGetValue(e.EmployeeId, out var empExtExps))
                {
                    foreach (var ex in empExtExps)
                    {
                        allLookups.TryGetValue(ex.LookupId??0, out var jobdic);

                        expSum += ex.To.Subtract(ex.From).Days;
                        TotalDays += ex.To.Subtract(ex.From).Days;
                        if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                            placeWorkedBuilder.AppendLine($"+ከ: {(datePreference ? ex.FromExAmharic : ex.From.ToString("dd.MM.yyyy"))} እስከ: {(ex.To == null ? "እ.አ" : (datePreference ? ex.ToExAmharic : ex.To.ToString("dd.MM.yyyy")))} {jobdic?.Description ?? ex.Responsibility} {ex.Organozation} {ex.OrgFullAddress}");
                        else if (cultureName.ToLower() == "om")
                            placeWorkedBuilder.AppendLine($"+irraa: {(datePreference ? ex.FromExAmharic : ex.From.ToString("dd.MM.yyyy"))} gara: {(ex.To == null ? " " : (datePreference ? ex.ToExAmharic : ex.To.ToString("dd.MM.yyyy")))} {jobdic?.Description ?? ex.Responsibility} {ex.Organozation} {ex.OrgFullAddress}");
                        else
                            placeWorkedBuilder.AppendLine($"+From: {(datePreference ? ex.FromExAmharic : ex.From.ToString("dd.MM.yyyy"))} to: {(ex.To == null ? " " : (datePreference ? ex.ToExAmharic : ex.To.ToString("dd.MM.yyyy")))} {jobdic?.Description ?? ex.Responsibility} {ex.Organozation} {ex.OrgFullAddress}");
                    }
                }

                var lowerCulture = cultureName.ToLower();
                viewModel.Expiriance = lowerCulture == "am" || lowerCulture == "am-et"
                    ? XAPI.Common.getDaystoWordsamharic((float)expSum)
                    : XAPI.Common.getDaystoWordsamharic((float)expSum);

                viewModel.ExpirianceDetails = placeWorkedBuilder.ToString();
                viewModel.MartialStatus = e.MartialStatus.ToString().ToLower();
                var totalInternalDays = 0d;
                var today = DateTime.Now;
                var culture = cultureName?.ToLower();

                Func<DateTime?, string, string> formatDate = (date, amharicDate) =>
                {
                    if (!date.HasValue) return "-";
                    if (datePreference && !string.IsNullOrEmpty(amharicDate))
                        return amharicDate;
                    return date.Value.ToString("dd.MM.yyyy");
                };

                var placementss = db.Placements
                    .Where(x => x.EmployeeId == pl.EmployeeId)
                    .OrderByDescending(x => x.AssignmentDate)
                    .ToList();

                var termination = db.Terminations
                    .Where(x => x.IsOnlineApproved && x.EmployeeId == pl.EmployeeId)
                    .OrderByDescending(x => x.TerminationDate)
                    .FirstOrDefault();

                bool firstIteration = true;
                DateTime? previousEndDate = today;

                foreach (var perPl in placementss)
                {
                    allLookups.TryGetValue(perPl.LookupId, out var jobdic);
                    var jobTitlee = jobdic?.Description ?? perPl.JobTitleName;

                    DateTime startDate = perPl.AssignmentDate;
                    DateTime? endDate = null;

                    if (perPl.isCurrent)
                    {
                        endDate = today;
                    }
                    else if (termination != null && termination.TerminationDate > startDate)
                    {
                        endDate = firstIteration ? termination.TerminationDate : previousEndDate;
                    }
                    else
                    {
                        endDate = firstIteration ? (DateTime?)null : previousEndDate;
                    }

                    string fromStr = formatDate(startDate, perPl.AssignmentDateAmharic);
                    string toStr = endDate == null ? "-" : formatDate(endDate, termination?.TerminationDateAm);

                    if (culture == "am" || culture == "am-et")
                        placeInternalWorkedBuilder.AppendLine($"+  ከ: {fromStr}  እስከ: {toStr}   {jobTitlee}");
                    else if (culture == "om")
                        placeInternalWorkedBuilder.AppendLine($"+ Irraa: {fromStr}  Gara: {toStr}   {jobTitlee}");
                    else
                        placeInternalWorkedBuilder.AppendLine($"+ From: {fromStr}  to: {toStr}   {jobTitlee}");

                    if (endDate.HasValue)
                    {
                        var days = endDate.Value.Subtract(startDate).Days;
                        TotalDays += days;
                        totalInternalDays += days;
                    }

                    previousEndDate = startDate;
                    firstIteration = false;
                }

                var empExpsAfterSystems = db.PlacementHistories.Where(x => x.EmployeeId == pl.EmployeeId);
                foreach (var ex in empExpsAfterSystems)
                {
                    allLookups.TryGetValue(ex.LookupId, out var jobAft);
                    var jobTitleAft = jobAft?.Description ?? ex.JobTitle;

                    if (culture == "am" || culture == "am-et")
                    {
                        placeInternalWorkedBuilder.AppendLine(
                            datePreference
                                ? $"+  ከ: {ex.FromAmharic} እስከ: {ex.ToAmharic} {jobTitleAft}"
                                : $"+  ከ: {ex.From:dd.MM.yyyy} እስከ: {ex.To:dd.MM.yyyy} {jobTitleAft}"
                        );
                    }
                    else if (culture == "om")
                    {
                        placeInternalWorkedBuilder.AppendLine(
                            datePreference
                                ? $"+ Irraa: {ex.FromAmharic} Gara: {ex.ToAmharic} {jobTitleAft}"
                                : $"+ Irraa: {ex.From:dd.MM.yyyy} Gara: {ex.To:dd.MM.yyyy} {jobTitleAft}"
                        );
                    }
                    else if (culture == "en" || culture == "en-us")
                    {
                        placeInternalWorkedBuilder.AppendLine(
                            datePreference
                                ? $"+ From: {ex.FromAmharic} To: {ex.ToAmharic} {jobTitleAft}"
                                : $"+ From: {ex.From:dd.MM.yyyy} To: {ex.To:dd.MM.yyyy} {jobTitleAft}"
                        );
                    }

                    TotalDays += ex.To.Subtract(ex.From).Days;
                    totalInternalDays += ex.To.Subtract(ex.From).Days;
                }

                viewModel.InternalExpriance = lowerCulture == "am" || lowerCulture == "am-et"
                    ? XAPI.Common.getDaystoWordsamharic((float)totalInternalDays)
                    : XAPI.Common.getDaystoWords((float)totalInternalDays);

                viewModel.InternalExprianceDetail = placeInternalWorkedBuilder.ToString();
                viewModel.ExpTotalSum = cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et"
                    ? XAPI.Common.getDaystoWordsamharic((float)TotalDays)
                    : XAPI.Common.getDaystoWords((float)TotalDays);

                if (languageProficiencies.TryGetValue(e.EmployeeId, out var languages))
                {
                    foreach (var lang in languages)
                    {
                        if (allLanguages.TryGetValue(lang.LanguageId, out var language))
                        {
                            viewModel.Languages.Add(new Languages { Name = language?.Description });
                        }
                    }
                }

                if (!viewModel.Languages.Any())
                {
                    viewModel.Languages.Add(new Languages { Name = " " });
                }

                if (courseTrackings.TryGetValue(e.EmployeeId, out var traineeCourses))
                {
                    foreach (var course in traineeCourses)
                    {
                        var tracking = db.CourseTrackings.Find(course.CourseTrackingId);
                        if (tracking != null && allLookups.TryGetValue(tracking.LookupId, out var lookup))
                        {
                            viewModel.Training.Add(new Training { Name = lookup?.Description ?? null });
                        }
                    }
                }

                if (!viewModel.Training.Any())
                {
                    viewModel.Training.Add(new Training { Name = " " });
                }

                if (trainings.TryGetValue(e.EmployeeId, out var empTrainings))
                {
                    foreach (var train in empTrainings)
                    {
                        viewModel.TrainingExt.Add(new TrainingExt { Name = train?.TrainingName ?? null });
                    }
                }

                if (!viewModel.TrainingExt.Any())
                {
                    viewModel.TrainingExt.Add(new TrainingExt { Name = " " });
                }

                if (performances.TryGetValue(e.EmployeeId, out var empPerfs))
                {
                    foreach (var perf in empPerfs)
                    {
                        var perfModel = new EmployeePerformances
                        {
                            EvaluationCriteria = perf.EvaluationCriteria,
                            Result = perf.Result.ToString()
                        };

                        if (allLookups.TryGetValue(perf.LookupId, out var lookup))
                        {
                            perfModel.Type = lookup?.Description ?? null;
                        }

                        viewModel.EmployeePerformances.Add(perfModel);
                    }
                }

                if (!viewModel.EmployeePerformances.Any())
                {
                    viewModel.EmployeePerformances.Add(new EmployeePerformances { Type = " " });
                }

                if (disciplineCases.TryGetValue(e.EmployeeId, out var discCases))
                {
                    foreach (var disc in discCases)
                    {
                        if (allEmpDisciplineMeasures.TryGetValue(disc.DisciplinaryCaseId, out var empDisc))
                        {
                            var discModel = new Discipline
                            {
                                Month = empDisc?.Month ?? 0
                            };

                            if (allDisciplineMeasures.TryGetValue(empDisc.MeasuresId, out var discMeasure))
                            {
                                discModel.MeasureRank = discMeasure?.MeasuresType ?? null;
                            }

                            if (allDisciplineMeasuresDetails.TryGetValue(empDisc.DetailMeasuresId ?? 0, out var discDetail))
                            {
                                discModel.MeasureTaken = discDetail?.Description ?? null;
                            }

                            if (allDisciplineMeasureByMoney.TryGetValue(disc.DisciplinaryCaseId, out var moneyMeasure))
                            {
                                discModel.Amount = moneyMeasure?.Amount ?? 0;
                            }

                            viewModel.Discipline.Add(discModel);
                        }
                    }
                }

                if (!viewModel.Discipline.Any())
                {
                    viewModel.Discipline.Add(new Discipline
                    {
                        MeasureRank = " ",
                        MeasureTaken = " ",
                        Month = 0,
                        Amount = 0
                    });
                }

                if (e.Status == EmployeeStatus.CONTRACT)
                {
                    viewModel.Employement = lowerCulture == "am" || lowerCulture == "am-et" ? "ግዝያዊ"
                                          : lowerCulture == "om" ? "Waliigaltee"
                                          : "Contract";
                }
                else if (e.Status == EmployeeStatus.PERMANENT)
                {
                    viewModel.Employement = lowerCulture == "am" || lowerCulture == "am-et" ? "ቋሚ"
                                          : lowerCulture == "om" ? "Dhaabbataa"
                                          : "Permanent";
                }
                else
                {
                    viewModel.Employement = "";
                }

                if (allBranches.TryGetValue(pl.BranchId, out var branch))
                {
                    viewModel.BranchName = branch.AmharicName;
                }

                if (allStructures.TryGetValue(pl.OrgStructureId, out var structure))
                {
                    viewModel.StructureName = structure?.Name ?? null;
                }

                if (allLookups.TryGetValue(pl.LookupId, out var jobTitle))
                {
                    viewModel.JobTitle = jobTitle?.Description ?? null;
                }

                if (allSteps.TryGetValue(pl.StepId, out var step))
                {
                    viewModel.Step = step?.StepName ?? null;
                }

                if (allRanks.TryGetValue(pl.RankId, out var rank))
                {
                    viewModel.Rank = rank?.RankName ?? null;
                }

                var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= DateTime.Today && x.DateTo >= DateTime.Today);
                if (currentPeriod != null)
                {
                    var entitle = db.LeavesEntitle
                        .Where(x => x.EmployeeId == e.EmployeeId && !x.IsExpired)
                        .Select(x => (decimal?)x.Balance)
                        .DefaultIfEmpty(0)
                        .Sum();

                    viewModel.TotalLeave = entitle ?? 0;
                }

                EmployeeMasterViewModelList.Add(viewModel);
            }

            return EmployeeMasterViewModelList;
        }

        // Referenced by the method above — include if it isn't already defined elsewhere in your project
        private int GetAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    public class EmployeeMasterViewModel
    {
        public string EmployeeCode { get; set; }
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string EmployeeType { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        // public string EduLevel { get; set; }
        public string DateOfBirth { get; set; }
        public string RetirementDate { get; set; }
        public string HireDate { get; set; }
        public string Nation { get; set; }
        public string MartialStatus { get; set; }
        public string PensionDate { get; set; }
        public string Expiriance { get; set; }
        public string ExpirianceDetails { get; set; }
        public string PensionNo { get; set; }
        // public string EducationField { get; set; }
        public List<EmployeeEductaionList> Education { get; set; }
        public decimal Salary { get; set; }
        public string JobTitle { get; set; }
        public string IsDirect { get; set; }
        public string Step { get; set; }
        public string Rank { get; set; }
        public string Religion { get; set; }
        public string InternalExpriance { get; set; }
        public string InternalExprianceDetail { get; set; }
        public string ExpTotalSum { get; set; }
        public decimal TotalLeave { get; set; }
        public string BranchName { get; set; }
        public string StructureName { get; set; }
        public string ResponsibilityLevel { get; set; }
        public string EmployementType { get; set; }
        public string Employement { get; set; }
        public string Performance { get; set; }
        public List<Training> Training { get; set; }
        public List<TrainingExt> TrainingExt { get; set; }
        public List<Languages> Languages { get; set; }
        public List<EmployeePerformances> EmployeePerformances { get; set; }
        public List<Discipline> Discipline { get; set; }

    }
    public class Training
    {
        public string Name { get; set; }
    }
    public class TrainingExt
    {
        public string Name { get; set; }
    }
    public class Languages
    {
        public string Name { get; set; }
    }
    public class EmployeePerformances
    {
        public string Type { get; set; }
        public string Result { get; set; }
        public string EvaluationCriteria { get; set; }
    }
    public class Discipline
    {
        public string MeasureRank { get; set; }
        public string MeasureTaken { get; set; }
        public int Month { get; set; }
        public string DecisionDate { get; set; }
        public decimal Amount { get; set; }
    }
    public class EmployeeEductaionList
    {
        public string EduField { get; set; }
        public string EduLevel { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Institute { get; set; }

    }
}
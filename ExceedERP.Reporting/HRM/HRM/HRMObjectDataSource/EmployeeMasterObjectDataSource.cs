using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Localization;
using System.Text;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{

    [DataObject]
    public class EmployeeMasterObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();

        public EmployeeMasterObjectDataSource()
        {
            db.Database.CommandTimeout = 7200; // large bulk report query, default 30s SQL timeout is too short
        }

        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<EmployeeMasterViewModel> GetEmployeeMasterInfo(object[] branchId, string BussinessUnitId, string orgStructureId, string employeeType, string status, string employee, string from, string to, string hireFrom, string hireTo,string religion, DateTime? assignmentDateFrom = null, DateTime? assignmentDateTo = null)
        {
            bool datePreference = HrHelper.GetDatePreference();
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

            // Handle assignment date range filter
            if (assignmentDateFrom.HasValue || assignmentDateTo.HasValue)
            {
                placements = placements.Where(x => (!assignmentDateFrom.HasValue || x.AssignmentDate >= assignmentDateFrom.Value) &&
                                                    (!assignmentDateTo.HasValue || x.AssignmentDate <= assignmentDateTo.Value));
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

                // Preload all employee experiences
                var allEmployees = db.Person_Employee.ToDictionary(x => x.EmployeeId);
               
                var allTerminations = db.Terminations.GroupBy(x => x.EmployeeId).ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.TerminationDate).FirstOrDefault());



                //// Preload bank accounts / account numbers

                foreach (var pl in placements)
                {
                    if (!allEmployees.TryGetValue(pl.EmployeeId, out var emp)) continue;

                    double totalExpDays = 0;

                    // Add external experience
                    if (allExperiences.TryGetValue(emp.EmployeeId, out var experiences))
                    {
                        totalExpDays += experiences.Sum(ex => (ex.To.Subtract(ex.From)).Days);
                    }

                    // Add internal experience
                    if (emp.IsActive)
                    {
                        totalExpDays += (DateTime.Today - emp.HiredDate).TotalDays;
                    }
                    else if (allTerminations.TryGetValue(emp.EmployeeId, out var termination))
                    {
                        totalExpDays += termination.TerminationDate.Value.Subtract(emp.HiredDate).Days;
                    }

                    int totalYears = (int)(totalExpDays / 365);

                    // Check if within range
                    if ((fromYears == 0 || totalYears >= fromYears) &&
                        (toYears == 0 || totalYears <= toYears))
                    {
                        empColList.Add(emp.EmployeeId);
                    }
                }

                placements = placements.Where(x => empColList.Contains(x.EmployeeId));
            }
            // Get distinct employee placements
            var placementDisticts = placements
                .GroupBy(x => x.EmployeeId)
                .Select(g => g.OrderByDescending(x => x.AssignmentDate).FirstOrDefault())
                .ToList();

            // Preload additional data in bulk
            var employeIds = placementDisticts.Select(x => x.EmployeeId).ToList();

            var employees = db.Person_Employee
                .Where(x => employeIds.Contains(x.EmployeeId))
                .ToDictionary(x => x.EmployeeId);

            var educations = db.Education
                .Where(x => employeIds.Contains(x.EmployeeId))
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var languageProficiencies = db.LanguageOfProficiencies
                .Where(x => employeIds.Contains(x.EmployeeId))
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var courseTrackings = db.Trainee
                .Where(x => employeIds.Contains(x.EmployeeId))
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var trainings = db.EmpTrainingEducations
                .Where(x => employeIds.Contains(x.EmployeeId))
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var performances = db.MeasurementDefinitions
                        .Where(x => x.EmployeeId.HasValue && employeIds.Contains(x.EmployeeId.Value))
                        .GroupBy(x => x.EmployeeId.Value)
                        .ToDictionary(g => g.Key, g => g.ToList());


            var disciplineCases = db.DisciplinaryCases
                .Where(x => employeIds.Contains(x.EmployeeId))
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var placementHistories = db.PlacementHistories
                .Where(x => employeIds.Contains(x.EmployeeId))
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
                             .Where(x => x.EmployeeId.HasValue && employeIds.Contains(x.EmployeeId.Value))
                             .GroupBy(x => x.EmployeeId.Value)
                             .ToDictionary(g => g.Key, g => g.First().IdNumber);



            var bankAccounts = db.BankReferences
                    .Where(x => x.Type == BankAccountType.PAYROLL)
                    .AsEnumerable()
                    .Where(x => employeIds.Contains(Convert.ToInt32(x.Reference)))
                    .GroupBy(x => Convert.ToInt32(x.Reference))
                    .ToDictionary(g => g.Key, g => g.First().AccountNumber);
            // Preload phone addresses
            var phoneCategories = new[] { AddressType.MOBILE_PHONE_1, AddressType.HOME_PHONE, AddressType.OFFICE_PHONE };
            var phoneNumbers = db.Addresses
                .Where(x => phoneCategories.Contains(x.Category) && x.Reference != null)
                .ToList()
                .Where(x => int.TryParse(x.Reference, out int id) && employeIds.Contains(id))
                .GroupBy(x => int.Parse(x.Reference))
                .ToDictionary(g => g.Key, g => g.First().Value);

            // Preload payroll/allowance details if stored per employee / payroll definitions
            // Assuming you have an allowance table or payroll summary table linked by EmployeeId:
            var employeeAllowances = db.AllowancePerEmployees
                .Where(x => employeIds.Contains(x.EmployeeId) && !x.Pend)
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Preload course trackings (avoids a per-course DB round trip inside the loop)
            var allCourseTrackings = db.CourseTrackings.ToDictionary(x => x.CourseTrackingId);

            // Preload leave balances per employee (avoids a per-employee DB round trip inside the loop)
            var leaveBalances = db.LeavesEntitle
                .Where(x => employeIds.Contains(x.EmployeeId) && !x.IsExpired)
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.Sum(x => (decimal?)x.Balance) ?? 0);

            // Current fiscal period is the same for every row, so resolve it once outside the loop
            var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= DateTime.Today && x.DateTo >= DateTime.Today);

            // Process each employee
            foreach (var pl in placementDisticts)
            {
                //var currentPlacement = placements.FirstOrDefault(x => x.isCurrent&&x.EmployeeId==pl.EmployeeId);

                if (!employees.TryGetValue(pl.EmployeeId, out var e)) continue;

                var viewModel = new EmployeeMasterViewModel
                {
                    DateOfBirth = datePreference? e.DateOfBirthAm:e.DateOfBirth.ToString("dd.MM.yyyy"),
                    RetirementDate = datePreference? XAPI.EthiopicDateTime.GetEthiopicDate(e.DateOfBirth.AddYears(60).Day,
                                    e.DateOfBirth.AddYears(60).Month, e.DateOfBirth.AddYears(60).Year): e.DateOfBirth.AddYears(60).ToString("dd.MM.yyyy"),
                    EmployeeCode = e.EmployeeCode,
                    EmployeeId = e.EmployeeId,
                    FullName = isLocal? $"{e.FirstName ?? ""} {e.MiddleName ?? ""} {e.LastName ?? ""}".Trim()
                                        : $"{e.FirstNameEnglish ?? ""} {e.MiddleNameEnglish ?? ""} {e.LastNameEnglish ?? ""}".Trim(),
                    //FullName = isLocal?e.FirstName+" "+e.MiddleName+" "+e.LastNameEnglish:e.FirstNameEnglish+" "+e.MiddleNameEnglish+" "+e.LastNameEnglish,
                    Gender = HrHelper.GetGender(e.Gender),
                    Religion = HrHelper.GetReligion(e.Religion),
                    Age = GetAge(e.DateOfBirth),
                    Education = new List<EmployeeEductaionList>(),
                    Training = new List<Training>(),
                    TrainingExt = new List<TrainingExt>(),
                    Languages = new List<HRMViewModel.Languages>(),
                    EmployeePerformances = new List<EmployeePerformances>(),
                    Discipline = new List<Discipline>(),
                    HireDate = datePreference? e.HiredDateAmharic:e.HiredDate.ToString("dd.MM.yyyy"),
                    Salary = pl.Salary,
                    ResponsibilityLevel = HrHelper.GetResponsibilityLevel(pl.ResponsibilityLevel),
                    EmployementType = HrHelper.GetAssignmentType(pl.AssignmentType)
                };

                // Assign Phone Number
                if (phoneNumbers.TryGetValue(e.EmployeeId, out var phone))
                    viewModel.PhoneNo = phone;
                else
                    viewModel.PhoneNo = e.PhoneNo;

                // Assign Account Number
                if (bankAccounts.TryGetValue(e.EmployeeId, out var accNo))
                    viewModel.AccountNo = accNo;

                // Assign Allowances dynamically from preloaded data
                if (employeeAllowances.TryGetValue(e.EmployeeId, out var allowances))
                {
                    foreach (var al in allowances)
                    {
                        var amount = al.AllowanceDefination?.Amount ?? 0;
                        switch (al.AllowanceDefination?.AllowanceType)
                        {
                            case AllowanceType.RESPONSIBILITY:
                                viewModel.ResponsibilityAllowance = amount;
                                break;
                            case AllowanceType.PROFESSIONAL:
                                viewModel.ProfessionalAllowance = amount;
                                break;
                            case AllowanceType.PROJECT:
                                viewModel.ProjectAllowance = amount;
                                break;
                            case AllowanceType.DESERT:
                                viewModel.DesertAllowance = amount;
                                break;
                            case AllowanceType.TRANSPORT:
                                viewModel.TransportAllowance = amount;
                                break;
                        }
                    }
                }

                // Calculate Net Pay (Basic Salary + Sum of Allowances - Deductions if available)
                decimal totalAllowances = viewModel.ResponsibilityAllowance + viewModel.ProfessionalAllowance +
                                          viewModel.ProjectAllowance + viewModel.DesertAllowance + viewModel.TransportAllowance;
                viewModel.NetPay = viewModel.Salary + totalAllowances; // Adjust formula based on your payroll logic if deductions apply
                // Set employee type
                if (allEmployeeTypes.TryGetValue(e.EmployeeTypeId??0, out var employeeTypeObj))
                {
                    viewModel.EmployeeType = employeeTypeObj?.Name;
                }

                // Set nation
                if (!string.IsNullOrEmpty(e.Nation) && int.TryParse(e.Nation, out int nationId))
                {
                    if (allLookups.TryGetValue(nationId, out var nation))
                    {
                        viewModel.Nation = nation?.Description??null;
                    }
                }

                // Set pension number
                if (pensionNumbers.TryGetValue(e.EmployeeId, out var pensionNo))
                {
                    viewModel.PensionNo = pensionNo;
                }

                // Process education
                if (educations.TryGetValue(e.EmployeeId, out var employeeEducations))
                {
                    foreach (var educ in employeeEducations)
                    {
                        allLookups.TryGetValue(educ.EduField, out var field);
                        allLookups.TryGetValue(educ.EduLevel, out var level);

                        viewModel.Education.Add(new EmployeeEductaionList
                        {
                            EduField = field?.Description ??null,
                            EduLevel = level?.Description ?? null,
                            From = datePreference? educ?.FromEAmharic
                                                   : (educ?.From.HasValue == true ? educ.From.Value.ToString("dd.MM.yyyy") : null),

                            To = datePreference? educ?.ToEAmharic
                                        : educ?.To.ToString("dd.MM.yyyy"),

                            Institute = educ?.OrganizationName??null
                        });
                    }
                }

                // Add empty education if none exists
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

                // Process experience
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
                            placeWorkedBuilder.AppendLine($"+ከ: {(datePreference ? ex.FromExAmharic : ex.From.ToString("dd.MM.yyyy"))} እስከ: {(ex.To == null ? "እ.አ" : (datePreference ? ex.ToExAmharic : ex.To.ToString("dd.MM.yyyy")))} {jobdic?.Description??ex.Responsibility} {ex.Organozation} {ex.OrgFullAddress}");

                        //placeWorkedBuilder.AppendLine($"+ከ: {datePreference? ex.FromExAmharic:ex.From.ToString("dd.mm.yyy")} እስከ: {ex.ToExAmharic} {ex.Responsibility} {ex.Organozation} {ex.OrgFullAddress}");
                        else if (cultureName.ToLower() == "om")
                            placeWorkedBuilder.AppendLine($"+irraa: {(datePreference ? ex.FromExAmharic : ex.From.ToString("dd.MM.yyyy"))} gara: {(ex.To == null ? " " : (datePreference ? ex.ToExAmharic : ex.To.ToString("dd.MM.yyyy")))} {jobdic?.Description ?? ex.Responsibility} {ex.Organozation} {ex.OrgFullAddress}");

                        //placeWorkedBuilder.AppendLine($"+ irraa: {ex.FromExAmharic} gara: {ex.ToExAmharic}  {ex.Responsibility}  {ex.Organozation}  {ex.OrgFullAddress}");
                        else
                            placeWorkedBuilder.AppendLine($"+From: {(datePreference ? ex.FromExAmharic : ex.From.ToString("dd.MM.yyyy"))} to: {(ex.To == null ? " " : (datePreference ? ex.ToExAmharic : ex.To.ToString("dd.MM.yyyy")))} {jobdic?.Description ?? ex.Responsibility} {ex.Organozation} {ex.OrgFullAddress}");

                        //placeWorkedBuilder.AppendLine($"+ From: {ex.FromExAmharic} to: {ex.ToExAmharic}  {ex.Responsibility}  {ex.Organozation}  {ex.OrgFullAddress}");
                    }
                }

                // With this:
                var lowerCulture = cultureName.ToLower();
                if (lowerCulture == "am" || lowerCulture == "am-et")
                {
                    viewModel.Expiriance = EmployeeHelper.GetDaystoWordsAmharic(expSum);
                }
                else
                {
                    viewModel.Expiriance = EmployeeHelper.GetDaystoWords(expSum);
                }

                viewModel.ExpirianceDetails = placeWorkedBuilder.ToString();
                viewModel.MartialStatus = HrHelper.GetMartialStatus(e);
                var totalInternalDays = 0d;
                var today = DateTime.Now;

                // Normalize culture once
                var culture = cultureName?.ToLower();

                // Date formatter (handles Ethiopian / Gregorian fallback safely)
                Func<DateTime?, string, string> formatDate = (date, amharicDate) =>
                {
                    if (!date.HasValue) return "-";

                    if (datePreference && !string.IsNullOrEmpty(amharicDate))
                        return amharicDate;

                    return date.Value.ToString("dd.MM.yyyy");
                };

                // Preload data (NO DB inside loop)
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
                        endDate = firstIteration
                            ? termination.TerminationDate
                            : previousEndDate;
                    }
                    else
                    {
                        endDate = firstIteration ? (DateTime?)null : previousEndDate;
                    }

                    string fromStr = formatDate(startDate, perPl.AssignmentDateAmharic);
                    string toStr = endDate == null
                        ? "-": formatDate(endDate, termination?.TerminationDateAm);

                    // Culture-based formatting
                    if (culture == "am" || culture == "am-et")
                        placeInternalWorkedBuilder.AppendLine($"+  ከ: {fromStr}  እስከ: {toStr}   {jobTitlee}");
                    else if (culture == "om")
                        placeInternalWorkedBuilder.AppendLine($"+ Irraa: {fromStr}  Gara: {toStr}   {jobTitlee}");
                    else
                        placeInternalWorkedBuilder.AppendLine($"+ From: {fromStr}  to: {toStr}   {jobTitlee}");

                    // Days calculation (safe)
                    if (endDate.HasValue)
                    {
                        var days = endDate.Value.Subtract(startDate).Days;
                        TotalDays += days;
                        totalInternalDays += days;
                    }

                    previousEndDate = startDate;
                    firstIteration = false;
                }
              
                var empExpsAfterSystems = placementHistories.TryGetValue(pl.EmployeeId, out var empExpsAfterSystemsList)
                    ? empExpsAfterSystemsList
                    : new List<PlacementHistory>();
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
               
                // With this:
                if (lowerCulture == "am" || lowerCulture == "am-et")
                {
                    viewModel.InternalExpriance = EmployeeHelper.GetDaystoWordsAmharic(totalInternalDays);
                }
                else
                {
                    viewModel.InternalExpriance = EmployeeHelper.GetDaystoWords(totalInternalDays);

                }


                viewModel.InternalExprianceDetail = placeInternalWorkedBuilder.ToString();
                if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                {
                    viewModel.ExpTotalSum = EmployeeHelper.GetDaystoWordsAmharic(TotalDays);
                }
                else
                {
                    viewModel.ExpTotalSum = EmployeeHelper.GetDaystoWords(TotalDays);
                }

                // Process languages

                if (languageProficiencies.TryGetValue(e.EmployeeId, out var languages))
                {
                    foreach (var lang in languages)
                    {
                        if (allLanguages.TryGetValue(lang.LanguageId, out var language))
                        {
                            viewModel.Languages.Add(new HRMViewModel.Languages
                            {
                                Name = language?.Description

                            });
                        }
                    }
                }


                if (!viewModel.Languages.Any())
                {
                    viewModel.Languages.Add(new HRMViewModel.Languages { Name = " " });
                }

                // Process trainings
                if (courseTrackings.TryGetValue(e.EmployeeId, out var traineeCourses))
                {
                    foreach (var course in traineeCourses)
                    {
                        if (allCourseTrackings.TryGetValue(course.CourseTrackingId, out var tracking) && allLookups.TryGetValue(tracking.LookupId, out var lookup))
                        {
                            viewModel.Training.Add(new Training { Name = lookup?.Description??null });
                        }
                    }
                }

                if (!viewModel.Training.Any())
                {
                    viewModel.Training.Add(new Training { Name = " " });
                }

                // Process extended trainings
                if (trainings.TryGetValue(e.EmployeeId, out var empTrainings))
                {
                    foreach (var train in empTrainings)
                    {
                        viewModel.TrainingExt.Add(new TrainingExt { Name = train?.TrainingName??null });
                    }
                }

                if (!viewModel.TrainingExt.Any())
                {
                    viewModel.TrainingExt.Add(new TrainingExt { Name = " " });
                }

                // Process performances
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
                            perfModel.Type = lookup?.Description??null;
                        }

                        viewModel.EmployeePerformances.Add(perfModel);
                    }
                }

                if (!viewModel.EmployeePerformances.Any())
                {
                    viewModel.EmployeePerformances.Add(new EmployeePerformances { Type = " " });
                }

                // Process discipline cases
                if (disciplineCases.TryGetValue(e.EmployeeId, out var discCases))
                {
                    foreach (var disc in discCases)
                    {
                        if (allEmpDisciplineMeasures.TryGetValue(disc.DisciplinaryCaseId, out var empDisc))
                        {
                            var discModel = new Discipline
                            {
                                Month = empDisc?.Month??0
                            };

                            if (allDisciplineMeasures.TryGetValue(empDisc.MeasuresId, out var discMeasure))
                            {
                                discModel.MeasureRank = discMeasure?.MeasuresType??null;
                            }

                            if (allDisciplineMeasuresDetails.TryGetValue(empDisc.DetailMeasuresId??0, out var discDetail))
                            {
                                discModel.MeasureTaken = discDetail?.Description??null;
                            }

                            if (allDisciplineMeasureByMoney.TryGetValue(disc.DisciplinaryCaseId, out var moneyMeasure))
                            {
                                discModel.Amount = moneyMeasure?.Amount??0;
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

                    // Set employment status
                    // With this:
                    if (e.Status == EmployeeStatus.CONTRACT)
                    {
                        if (lowerCulture == "am" || lowerCulture == "am-et")
                        {
                            viewModel.Employement = "ግዝያዊ";
                        }
                        else if (lowerCulture == "om")
                        {
                            viewModel.Employement = "Waliigaltee";
                        }
                        else
                        {
                            viewModel.Employement = "Contract";
                        }
                    }
                    else if (e.Status == EmployeeStatus.PERMANENT)
                    {
                        if (lowerCulture == "am" || lowerCulture == "am-et")
                        {
                            viewModel.Employement = "ቋሚ";
                        }
                        else if (lowerCulture == "om")
                        {
                            viewModel.Employement = "Dhaabbataa";
                        }
                        else
                        {
                            viewModel.Employement = "Permanent";
                        }
                    }
                    else
                    {
                        viewModel.Employement = "";
                    }

                    // Set branch and structure names
                    if (allBranches.TryGetValue(pl.BranchId, out var branch))
                {
                    viewModel.BranchName = branch.AmharicName;
                }


                if (allStructures.TryGetValue(pl.OrgStructureId, out var structure))
                {
                    viewModel.StructureName = structure?.Name??null;
                }

                // Set job title
                if (allLookups.TryGetValue(pl.LookupId, out var jobTitle))
                {
                    viewModel.JobTitle = jobTitle?.Description??null;
                }

                // Set step and rank
                if (allSteps.TryGetValue(pl.StepId, out var step))
                {
                    viewModel.Step = step?.StepName??null;
                }

                if (allRanks.TryGetValue(pl.RankId, out var rank))
                {
                    viewModel.Rank = rank?.RankName??null;
                }

                // Calculate leave balance
                if (currentPeriod != null)
                {
                    viewModel.TotalLeave = leaveBalances.TryGetValue(e.EmployeeId, out var bal) ? bal : 0;
                }


                EmployeeMasterViewModelList.Add(viewModel);


            }


            return EmployeeMasterViewModelList;



        }

        public List<EmployeeMasterViewModel> GetEmployeeMasterInfoCCC(object[] branchId,string BussinessUnitId, string orgStructureId, string employeeType, string status, string employee, string from, string to, string hireFrom, string hireTo)
        {

            //   EmployeeMasterViewModel EmployeeMasterViewModel = new EmployeeMasterViewModel();
            List<EmployeeMasterViewModel> EmployeeMasterViewModelList = new List<EmployeeMasterViewModel>();
            var allLookups = db.Lookups.ToList();
            string GetResponsibility(int? lookupId) => allLookups.FirstOrDefault(x => x.LookupId == lookupId)?.Description;

            List<string> allBranches = new List<string>();

            var placements = db.Placements.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                if (status.ToLower() == "active")
                {
                    placements = placements.Where(x => x.isCurrent);
                }
                else if (status.ToLower() == "inactive")
                {
                    placements = placements.Where(x => !x.isCurrent);
                }
            }

            if (branchId != null && branchId.Any())
            {
                var branchIds = branchId
                    .Where(b => b != null)
                    .Select(b => Convert.ToInt32(b))
                    .ToList();

                placements = placements.Where(x => branchIds.Contains(x.BranchId));
            }


            if (!string.IsNullOrEmpty(orgStructureId))
            {
                int.TryParse(orgStructureId, out int orgId);
                placements = placements.Where(x => x.OrgStructureId == orgId);

            }
            if (!string.IsNullOrEmpty(BussinessUnitId))
            {
                int.TryParse(BussinessUnitId, out int bussUnitId);
                placements = placements.Where(x => x.BusinessUnitId == bussUnitId);

            }
            if (!string.IsNullOrEmpty(employee ))
            {
                int.TryParse(employee, out int empId);
                placements = placements.Where(x => x.EmployeeId == empId);
            }

            if (!string.IsNullOrEmpty(hireFrom) || !string.IsNullOrEmpty(hireTo))
            {
                int.TryParse(hireFrom, out int glFiscalIdFrom);
                int.TryParse(hireTo, out int glFiscalIdTo);

                var glYearFrom = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalIdFrom);
                var glYearTo = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalIdTo);

                if (glYearFrom != null || glYearTo != null)
                {
                    var dateFrom = glYearFrom?.DateFrom ?? glYearTo?.DateFrom;
                    var dateTo = glYearTo?.DateTo ?? glYearFrom?.DateTo;

                    if (dateFrom.HasValue && dateTo.HasValue)
                    {
                        var empIds = db.Person_Employee
                                       .Where(x => x.HiredDate >= dateFrom && x.HiredDate <= dateTo)
                                       .Select(x => x.EmployeeId)
                                       .ToList();

                        placements = placements.Where(x => empIds.Contains(x.EmployeeId));
                    }
                }
            }

            if (!string.IsNullOrEmpty(employeeType) && placements.Any())
            {
                int.TryParse(employeeType, out int employeeTypeId);

                var empIds = db.Person_Employee
                               .Where(x => x.EmployeeTypeId == employeeTypeId)
                               .Select(x => x.EmployeeId)
                               .ToList();

                placements = placements.Where(x => empIds.Contains(x.EmployeeId));
            }

            List<int> empColList = new List<int>();
            double checkTotalExp = 0;

            //if (from != null || to != null)
            if (!string.IsNullOrEmpty(from) 
                || !string.IsNullOrEmpty(to))
            {
                foreach (var pl in placements)
                {
                    var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == pl.EmployeeId);

                    if (emp != null)
                    {
                        var exps = db.Experiences.Where(x => x.EmployeeId == emp.EmployeeId).ToList();

                        foreach (var ex in exps)
                        {
                            checkTotalExp = checkTotalExp + (ex.To.Subtract(ex.From).Days);
                        }
                        if (emp.IsActive)
                        {
                            checkTotalExp = checkTotalExp + (DateTime.Today.Subtract(emp.HiredDate).Days);
                        }
                        else
                        {
                            var termination = db.Terminations.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (termination != null)
                            {
                                checkTotalExp = checkTotalExp + termination.TerminationDate.Value.Subtract(emp.HiredDate).Days;
                            }
                        }

                        int check = (int)checkTotalExp / 365;

                       

                        if (from != null && to != null)
                        {
                            int.TryParse(from, out int fr);
                            int.TryParse(to, out int tt);

                            if (check >= fr && check <= tt)
                            {
                                empColList.Add(emp.EmployeeId);
                            }
                        }
                        if (from != null && to == null)
                        {
                            int.TryParse(from, out int fr);

                            if (check >= fr)
                            {
                                empColList.Add(emp.EmployeeId);
                            }
                        }
                        if (from == null && to != null)
                        {
                            int.TryParse(to, out int tt);

                            if (check <= tt)
                            {
                                empColList.Add(emp.EmployeeId);
                            }
                        }

                        checkTotalExp = 0;
                    }

                }

                placements = placements.Where(x => empColList.Contains(x.EmployeeId));
            }

            var placementDisticts = placements.DistinctByField(x => x.EmployeeId).ToList();
            // After getting employeeIds and placementDisticts, batch load financial and allowance details:
            var employeeIds = placementDisticts.Select(x => x.EmployeeId).ToList();

            //// Preload bank accounts / account numbers
            
            var bankAccounts = db.BankReferences
                    .Where(x => x.Type == BankAccountType.PAYROLL)
                    .AsEnumerable()
                    .Where(x => employeeIds.Contains(Convert.ToInt32(x.Reference)))
                    .GroupBy(x => Convert.ToInt32(x.Reference))
                    .ToDictionary(g => g.Key, g => g.First().AccountNumber);
            // Preload phone addresses
            var phoneCategories = new[] { AddressType.MOBILE_PHONE_1, AddressType.HOME_PHONE, AddressType.OFFICE_PHONE };
            var phoneNumbers = db.Addresses
                .Where(x => phoneCategories.Contains(x.Category) && x.Reference != null)
                .ToList()
                .Where(x => int.TryParse(x.Reference, out int id) && employeeIds.Contains(id))
                .GroupBy(x => int.Parse(x.Reference))
                .ToDictionary(g => g.Key, g => g.First().Value);

            // Preload payroll/allowance details if stored per employee / payroll definitions
            // Assuming you have an allowance table or payroll summary table linked by EmployeeId:
            var employeeAllowances = db.AllowancePerEmployees
                .Where(x => employeeIds.Contains(x.EmployeeId) && !x.Pend)
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());
            //foreach (var pl in placements)
            foreach (var pl in placementDisticts)
            {
                var e = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == pl.EmployeeId);

                if (e != null)
                {
                    var retireGC = e.DateOfBirth.AddYears(60);
                    var retire = XAPI.EthiopicDateTime.GetEthiopicDate(retireGC.Day, retireGC.Month, retireGC.Year);
                    var EmployeeMasterViewModel = new EmployeeMasterViewModel();
                    EmployeeMasterViewModel.DateOfBirth = e.DateOfBirthAm;
                    EmployeeMasterViewModel.RetirementDate = retire;// e.DateOfBirth.AddYears(60).ToShortDateString();
                    EmployeeMasterViewModel.EmployeeCode = e.EmployeeCode;
                    EmployeeMasterViewModel.EmployeeId = e.EmployeeId;
                    EmployeeMasterViewModel.Salary = pl.Salary;
                    EmployeeMasterViewModel.FullName = e.FullNameAmharic; //HrHelper.GetEmployeeFullName(e.EmployeeId);
                    var employeeTypee = db.EmployeeTypes.Find(e.EmployeeTypeId);
                    if (employeeTypee !=null)
                    {
                        EmployeeMasterViewModel.EmployeeType = employeeTypee.Name;
                    }
                    // Assign Phone Number
                    if (phoneNumbers.TryGetValue(e.EmployeeId, out var phone))
                        EmployeeMasterViewModel.PhoneNo = phone;
                    else
                        EmployeeMasterViewModel.PhoneNo = e.PhoneNo;

                    // Assign Account Number
                    if (bankAccounts.TryGetValue(e.EmployeeId, out var accNo))
                        EmployeeMasterViewModel.AccountNo = accNo;

                    // Assign Allowances dynamically from preloaded data
                    if (employeeAllowances.TryGetValue(e.EmployeeId, out var allowances))
                    {
                        foreach (var al in allowances)
                        {
                            var amount = al.AllowanceDefination?.Amount ?? 0;
                            switch (al.AllowanceDefination?.AllowanceType)
                            {
                                case AllowanceType.RESPONSIBILITY:
                                    EmployeeMasterViewModel.ResponsibilityAllowance = amount;
                                    break;
                                case AllowanceType.PROFESSIONAL:
                                    EmployeeMasterViewModel.ProfessionalAllowance = amount;
                                    break;
                                case AllowanceType.PROJECT:
                                    EmployeeMasterViewModel.ProjectAllowance = amount;
                                    break;
                                case AllowanceType.DESERT:
                                    EmployeeMasterViewModel.DesertAllowance = amount;
                                    break;
                                case AllowanceType.TRANSPORT:
                                    EmployeeMasterViewModel.TransportAllowance = amount;
                                    break;
                            }
                        }
                    }

                    // Calculate Net Pay (Basic Salary + Sum of Allowances - Deductions if available)
                    decimal totalAllowances = EmployeeMasterViewModel.ResponsibilityAllowance + EmployeeMasterViewModel.ProfessionalAllowance +
                                              EmployeeMasterViewModel.ProjectAllowance + EmployeeMasterViewModel.DesertAllowance + EmployeeMasterViewModel.TransportAllowance;
                    EmployeeMasterViewModel.NetPay = EmployeeMasterViewModel.Salary + totalAllowances; // Adjust formula based on your payroll logic if deductions apply
                    EmployeeMasterViewModel.Gender = HrHelper.GetGender(e.Gender);
                    EmployeeMasterViewModel.Religion = HrHelper.GetReligion(e.Religion);
                    EmployeeMasterViewModel.Age = GetAge(e.DateOfBirth);
                    EmployeeMasterViewModel.Education = new List<EmployeeEductaionList>();
                    EmployeeMasterViewModel.Training = new List<Training>();
                    EmployeeMasterViewModel.TrainingExt = new List<TrainingExt>();
                    EmployeeMasterViewModel.Languages = new List<HRMViewModel.Languages>();
                    EmployeeMasterViewModel.EmployeePerformances = new List<EmployeePerformances>();
                    EmployeeMasterViewModel.Discipline = new List<Discipline>();
                    if (!string.IsNullOrEmpty(e.Nation))
                    {
                        int.TryParse(e.Nation, out int lookId);
                        var nation = db.Lookups.FirstOrDefault(x => x.LookupId == lookId);
                        if (nation != null)
                        {
                            EmployeeMasterViewModel.Nation = nation.Description;
                        }
                    }
                    var education = db.Education.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    foreach (var educ in education)
                    {
                        var filed = db.Lookups.Where(x => x.LookupId == educ.EduField).OrderByDescending(x => x.Index).FirstOrDefault();
                        var level = db.Lookups.Where(x => x.LookupId == educ.EduLevel).OrderByDescending(x=>x.Index).FirstOrDefault();
                        EmployeeEductaionList edu = new EmployeeEductaionList();
                        edu.EduField = filed != null ? filed.Description : "Unfilled";
                        edu.EduLevel = level != null ? level.Description : "Unfilled";
                        edu.From = educ.FromEAmharic;
                        edu.To = educ.ToEAmharic;
                        edu.Institute = educ.OrganizationName;
                        EmployeeMasterViewModel.Education.Add(edu);

                    }
                    //// i do this to connect the broke lines when the list is empty
                    if (!EmployeeMasterViewModel.Education.Any())
                    {
                        EmployeeEductaionList edu = new EmployeeEductaionList();
                        edu.EduField = " ";
                        edu.EduLevel = " ";
                        edu.From = " ";
                        edu.To = " ";
                        edu.Institute = " ";
                        EmployeeMasterViewModel.Education.Add(edu);
                    }

                    var value = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() 
                                                                       && x.ReferenceType == "Employee" 
                                                                       && x.Type == IdentificationType.PENSION_NO);
                    if (value != null)
                    {
                        EmployeeMasterViewModel.PensionNo = value.IdNumber;
                    }

                    var empExtExps = db.Experiences.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    double expSum = 0;
                    double TotalDays = 0;
                    double totalInternalDays = 0;
                    string PlaceWorked = "";
                    var placeWorkedBuilder = new StringBuilder();
                    var placeInternalWorkedBuilder = new StringBuilder();
                    var empExpsAfterSystems = db.PlacementHistories.Where(x => x.EmployeeId == pl.EmployeeId);
                    foreach (var ex in empExtExps)
                    {
                        expSum += (ex.To - ex.From).Days;
                        TotalDays += ex.To.Subtract(ex.From).Days;
                        if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                            placeWorkedBuilder.AppendLine($"+ከ: {ex.FromExAmharic} እስከ: {ex.ToExAmharic} {ex.Responsibility} {ex.Organozation} {ex.OrgFullAddress}");
                        if (cultureName.ToLower() == "om")
                            placeWorkedBuilder.AppendLine($"+ irraa: {ex.FromExAmharic} gara: {ex.ToExAmharic}  {ex.Responsibility}  {ex.Organozation}  {ex.OrgFullAddress}");
                        if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                            placeWorkedBuilder.AppendLine($"+ From: {ex.FromExAmharic} to: {ex.ToExAmharic}  {ex.Responsibility}  {ex.Organozation}  {ex.OrgFullAddress}");

                    }

                    PlaceWorked = placeWorkedBuilder.ToString();
                    if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                        EmployeeMasterViewModel.Expiriance = EmployeeHelper.GetDaystoWordsAmharic(expSum);
                    if (cultureName.ToLower() == "om")
                        EmployeeMasterViewModel.Expiriance = EmployeeHelper.GetDaystoWords(expSum);
                    if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                        EmployeeMasterViewModel.Expiriance = EmployeeHelper.GetDaystoWords(expSum);

                    EmployeeMasterViewModel.ExpirianceDetails = PlaceWorked;
                    EmployeeMasterViewModel.MartialStatus = HrHelper.GetMartialStatus(e);
                   
                    var individualPlacements = db.Placements.Where(x => x.EmployeeId == pl.EmployeeId).OrderByDescending(x=>x.AssignmentDate).ToList();
                    DateTime today = DateTime.Now;
                    var fromdatestr = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                    var fromc = today;
                    var toc = today;
                    int i = 0;
                    foreach (var perPl in individualPlacements)
                    {
                        var currentPlacement = db.Placements.Find(perPl.PlacementId);
                        if (currentPlacement !=null&&currentPlacement.isCurrent)
                        {
                        
                            if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                                placeInternalWorkedBuilder.AppendLine($"+  ከ:   {perPl.AssignmentDateAmharic }  እስከ:  {fromdatestr}   {perPl.JobTitleName}  ");
                            if (cultureName.ToLower() == "om")
                                placeInternalWorkedBuilder.AppendLine($"+ Irraa:   {perPl.AssignmentDateAmharic }  Gara:  {fromdatestr}   {perPl.JobTitleName}  ");
                            if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                                placeInternalWorkedBuilder.AppendLine($"+  From:   {perPl.AssignmentDateAmharic }  to:  {fromdatestr}   {perPl.JobTitleName}  ");
                                
                            //PlaceWorkedInternal += $"  ከ: " + perPl.AssignmentDateAmharic + " " + "እስከ:" + fromdatestr + " " + perPl.JobTitleName + "\n";
                            TotalDays += today.Subtract(perPl.AssignmentDate).Days;
                            totalInternalDays += today.Subtract(perPl.AssignmentDate).Days;
                            toc = perPl.AssignmentDate;
                            fromdatestr = perPl.AssignmentDateAmharic;

                        }
                        else
                        {
                            i++;
                            var termination = db.Terminations.Where(x => x.IsOnlineApproved && x.EmployeeId == perPl.EmployeeId).OrderByDescending(x => x.TerminationDate).FirstOrDefault();
                            if(termination !=null &&termination.TerminationDate > perPl.AssignmentDate)
                            {
                                if (i == 1)
                                {
                                    fromdatestr = termination.TerminationDateAm;
                                    toc = termination.TerminationDate.Value;
                                }
                                    
                                if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                                    placeInternalWorkedBuilder.AppendLine($"+  ከ:   {perPl.AssignmentDateAmharic }  እስከ:  {fromdatestr}   {perPl.JobTitleName}  ");
                                if (cultureName.ToLower() == "om")
                                    placeInternalWorkedBuilder.AppendLine($"+ Irraa:   {perPl.AssignmentDateAmharic }  Gara:  {fromdatestr}   {perPl.JobTitleName}  ");
                                if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                                    placeInternalWorkedBuilder.AppendLine($"+  From:   {perPl.AssignmentDateAmharic }  to:  {fromdatestr}   {perPl.JobTitleName}  ");

                                //PlaceWorkedInternal += "\n  ከ: " + perPl.AssignmentDateAmharic + " " + "እስከ:" + fromdatestr + " " + perPl.JobTitleName + "\n";
                                TotalDays += toc.Subtract(perPl.AssignmentDate).Days;
                                totalInternalDays += today.Subtract(perPl.AssignmentDate).Days;

                                fromdatestr = perPl.AssignmentDateAmharic;
                            }
                            else
                            {
                                if (i == 1)
                                {
                                    fromdatestr = "-";
                                    toc = perPl.AssignmentDate;
                                }
                                if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                                    placeInternalWorkedBuilder.AppendLine($"+  ከ:   {perPl.AssignmentDateAmharic }  እስከ:  {fromdatestr}   {perPl.JobTitleName}  ");
                                if (cultureName.ToLower() == "om")
                                    placeInternalWorkedBuilder.AppendLine($"+ Irraa:   {perPl.AssignmentDateAmharic }  Gara:  {fromdatestr}   {perPl.JobTitleName}  ");
                                if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                                    placeInternalWorkedBuilder.AppendLine($"+  From:   {perPl.AssignmentDateAmharic }  to:  {fromdatestr}   {perPl.JobTitleName}  ");

                                TotalDays += toc.Subtract(perPl.AssignmentDate).Days;
                                totalInternalDays += today.Subtract(perPl.AssignmentDate).Days;
                                fromdatestr = perPl.AssignmentDateAmharic;
                                toc = perPl.AssignmentDate;

                            }

                        }
                           
                    }
                    foreach (var ex in empExpsAfterSystems)
                    {
                        if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                            placeInternalWorkedBuilder.AppendLine($"+  ከ: {ex.FromAmharic} እስከ: {ex.ToAmharic} {db.Lookups.Find(ex.LookupId)?.Description ?? ex.JobTitle}");
                        if (cultureName.ToLower() == "om")
                            placeInternalWorkedBuilder.AppendLine($"+ Irraa: {ex.FromAmharic} Gara: {ex.ToAmharic} {db.Lookups.Find(ex.LookupId)?.Description ?? ex.JobTitle}");
                        if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                            placeInternalWorkedBuilder.AppendLine($"+ From: {ex.FromAmharic} to: {ex.ToAmharic} {db.Lookups.Find(ex.LookupId)?.Description ?? ex.JobTitle}");

                        TotalDays += ex.To.Subtract(ex.From).Days;
                        totalInternalDays += ex.To.Subtract(ex.From).Days;
                    }
                    if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                    {
                        EmployeeMasterViewModel.InternalExpriance = EmployeeHelper.GetDaystoWordsAmharic(totalInternalDays);

                    }
                    else
                    {
                        EmployeeMasterViewModel.InternalExpriance = EmployeeHelper.GetDaystoWords(totalInternalDays);

                    }
                    //EmployeeMasterViewModel.InternalExprianceDetail = PlaceWorkedInternal;
                    EmployeeMasterViewModel.InternalExprianceDetail = placeInternalWorkedBuilder.ToString();
                    if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                    {
                        EmployeeMasterViewModel.ExpTotalSum = EmployeeHelper.GetDaystoWordsAmharic(TotalDays);
                    }
                    else
                    {
                        EmployeeMasterViewModel.ExpTotalSum = EmployeeHelper.GetDaystoWords(TotalDays);
                    }
                    var languageProficiency = db.LanguageOfProficiencies.Where(x => x.EmployeeId == e.EmployeeId);
                    foreach (var lang in languageProficiency)
                    {
                        var language = db.Languages.FirstOrDefault(x => x.LanguageId == lang.LanguageId);
                        if (language != null)
                        {
                            HRMViewModel.Languages lan = new HRMViewModel.Languages();

                            lan.Name = language.Description;
                            EmployeeMasterViewModel.Languages.Add(lan);
                        }
                    }
                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.Languages.Any())
                    {
                        HRMViewModel.Languages lan = new HRMViewModel.Languages();

                        lan.Name = " ";

                        EmployeeMasterViewModel.Languages.Add(lan);
                    }
                    var courseTrankings = db.Trainee.Where(x => x.EmployeeId == e.EmployeeId);
                    foreach (var course in courseTrankings)
                    {
                        Training train = new Training();
                        var tracking = db.CourseTrackings.FirstOrDefault(x => x.CourseTrackingId == course.CourseTrackingId);
                        if (tracking != null)
                        {
                            var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == tracking.LookupId);
                            if (lookup != null)
                            {
                                train.Name = lookup.Description;

                                EmployeeMasterViewModel.Training.Add(train);
                            }
                        }
                    }
                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.Training.Any())
                    {
                        Training train = new Training();

                        train.Name = " ";

                        EmployeeMasterViewModel.Training.Add(train);
                    }

                    var trainings = db.EmpTrainingEducations.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    foreach (var train in trainings)
                    {
                        TrainingExt trainExt = new TrainingExt();
                        if (train != null)
                        {
                            try
                            {
                                trainExt.Name = train.TrainingName;

                                EmployeeMasterViewModel.TrainingExt.Add(trainExt);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.TrainingExt.Any())
                    {
                        TrainingExt trainExt = new TrainingExt();

                        trainExt.Name = " ";

                        EmployeeMasterViewModel.TrainingExt.Add(trainExt);
                    }

                    var empPerformances = db.MeasurementDefinitions.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    foreach (var empPerformance in empPerformances)
                    {
                        EmployeePerformances empPerform = new EmployeePerformances();
                        var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == empPerformance.LookupId);
                        if (lookup != null)
                        {
                            empPerform.Type = lookup.Description;
                        }
                        empPerform.EvaluationCriteria = empPerformance.EvaluationCriteria;
                        empPerform.Result = empPerformance.Result.ToString();

                        EmployeeMasterViewModel.EmployeePerformances.Add(empPerform);
                    }

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.EmployeePerformances.Any())
                    {
                        EmployeePerformances empPerform = new EmployeePerformances();

                        empPerform.Type = " ";

                        EmployeeMasterViewModel.EmployeePerformances.Add(empPerform);
                    }

                    var disciplineCases = db.DisciplinaryCases.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    foreach (var disc in disciplineCases)
                    {
                        var discipine = new Discipline();

                        var empDisc = db.EmpDisciplineMeasure.FirstOrDefault(x => x.DisciplinaryCaseId == disc.DisciplinaryCaseId);
                        if (empDisc != null)
                        {
                            var discMeasure = db.DisciplineMeasures.FirstOrDefault(x => x.MeasuresId == empDisc.MeasuresId);
                            if (discMeasure != null)
                            {
                                discipine.MeasureRank = discMeasure.MeasuresType;
                            }
                            var discMeasureDetail = db.DisciplineMeasuresDetail.FirstOrDefault(x => x.DetailMeasuresId == empDisc.DetailMeasuresId);
                            if (discMeasureDetail != null)
                            {
                                discipine.MeasureTaken = discMeasureDetail.Description;
                            }

                            discipine.Month = empDisc.Month;
                            var disMeasureByMoney = db.DisciplineMeasuerByMoney.FirstOrDefault(x => x.DisciplinaryCaseId == disc.DisciplinaryCaseId);
                            if (disMeasureByMoney != null)
                            {
                                discipine.Amount = disMeasureByMoney.Amount;
                            }

                            EmployeeMasterViewModel.Discipline.Add(discipine);
                        }
                    }

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.Discipline.Any())
                    {
                        var discipine = new Discipline();

                        discipine.MeasureRank = " ";
                        discipine.MeasureTaken = " ";
                        discipine.Month = 0;
                        discipine.Amount = 0;

                        EmployeeMasterViewModel.Discipline.Add(discipine);
                    }

                    //EmployeeMasterViewModel.InternalExpriance = DateTime.Now.Date.Subtract(new DateTime(e.HiredDate.Year, e.HiredDate.Month,
                    //    e.HiredDate.Day)).TotalDays / 365.25;

                    EmployeeMasterViewModel.HireDate = e.HiredDateAmharic;//placement.AssignmentDateAmharic;
                    EmployeeMasterViewModel.Salary = pl.Salary;
                    EmployeeMasterViewModel.ResponsibilityLevel = HrHelper.GetResponsibilityLevel(pl.ResponsibilityLevel);
                    EmployeeMasterViewModel.EmployementType = HrHelper.GetAssignmentType(pl.AssignmentType);
                    if (e.Status == EmployeeStatus.CONTRACT)
                    {

                        EmployeeMasterViewModel.Employement = Resources.Enum_EmployeeStatus_CONTRACT.ToLower();
                        if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                        {

                            EmployeeMasterViewModel.Employement = "ግዝያዊ";
                        }
                        if (cultureName.ToLower() == "om")
                        {

                            EmployeeMasterViewModel.Employement = "Waliigaltee";
                        }
                        if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                        {
                            EmployeeMasterViewModel.Employement = "Contract";

                        }
                    }
                    else if (e.Status == EmployeeStatus.PERMANENT)
                    {
                        EmployeeMasterViewModel.Employement = Resources.Enum_EmployeeStatus_PERMANENT.ToLower();
                        if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                        {

                            EmployeeMasterViewModel.Employement = "ቋሚ";
                        }
                        if (cultureName.ToLower() == "om")
                        {

                            EmployeeMasterViewModel.Employement = "Dhaabbataa";
                        }
                        if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                        {
                            EmployeeMasterViewModel.Employement = "Permanent";

                        }
                    }
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == pl.BranchId);
                    if (branch != null)
                    {
                        EmployeeMasterViewModel.BranchName = branch.AmharicName;
                    }
                    var structure = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == pl.OrgStructureId);
                    if (structure != null)
                    {
                        EmployeeMasterViewModel.StructureName = structure.Name; ;
                    }
                    var jobTitle = db.Lookups.FirstOrDefault(x => x.LookupId == pl.LookupId);
                    if (jobTitle != null)
                    {
                        EmployeeMasterViewModel.JobTitle = jobTitle.Description;
                    }
                    EmployeeMasterViewModel.Salary = pl.Salary;

                    var step = db.Steps.FirstOrDefault(x => x.StepId == pl.StepId);
                    if (step != null)
                    {
                        EmployeeMasterViewModel.Step = step.StepName;
                    }
                    var rank = db.Ranks.FirstOrDefault(x => x.RankId == pl.RankId);
                    if (rank != null)
                    {
                        EmployeeMasterViewModel.Rank = rank.RankName;
                    }
                    var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= DateTime.Today && x.DateTo >= DateTime.Today);
                    if (currentPeriod != null)
                    {
                        var entitle = db.LeavesEntitle.Where(x => x.EmployeeId == e.EmployeeId && !x.IsExpired);

                        if (entitle.Any())
                        {
                            EmployeeMasterViewModel.TotalLeave = entitle.Sum(x => x.Balance);
                        }
                    }
                    EmployeeMasterViewModelList.Add(EmployeeMasterViewModel);

                }

            }

            return EmployeeMasterViewModelList;
        }


        public List<EmployeeMasterViewModel> GetEmployeeMasterInfo_Old(/*string branchId,*/ object[] branchId, string orgStructureId, string employeeType, string status, string employee, string from, string to, string hireFrom, string hireTo)
        {

            EmployeeMasterViewModel EmployeeMasterViewModel = new EmployeeMasterViewModel();
            List<EmployeeMasterViewModel> EmployeeMasterViewModelList = new List<EmployeeMasterViewModel>();

            List<Placement> placements;
            List<string> allBranches = new List<string>();

            placements = FilterEmployees(branchId, orgStructureId, employeeType);

            if (status != null)
            {
                if (status.ToLower() == "active")
                {
                    var empIds = db.Person_Employee.Where(x => x.IsActive).Select(x => x.EmployeeId).ToList();
                    placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();
                }

                if (status.ToLower() == "inactive")
                {
                    var empIds = db.Person_Employee.Where(x => !x.IsActive).Select(x => x.EmployeeId).ToList();
                    placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();
                }
            }

            if (hireFrom != null && hireTo == null)
            {
                int.TryParse(hireFrom, out int glFiscalId);
                var glYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalId);
                if (glYear != null)
                {
                    var empIds = db.Person_Employee.Where(x => x.HiredDate >= glYear.DateFrom && x.HiredDate <= glYear.DateTo).Select(x => x.EmployeeId).ToList();
                    placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();
                    //  employees = employees.Where(x => x.HiredDate >= glYear.DateFrom && x.HiredDate <= glYear.DateTo).ToList();
                }

            }
            else if (hireTo != null && hireFrom == null)
            {
                int.TryParse(hireTo, out int glFiscalId);
                var glYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalId);
                if (glYear != null)
                {
                    var empIds = db.Person_Employee.Where(x => x.HiredDate >= glYear.DateFrom && x.HiredDate <= glYear.DateTo).Select(x => x.EmployeeId).ToList();
                    placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();

                    //  employees = employees.Where(x => x.HiredDate >= glYear.DateFrom && x.HiredDate <= glYear.DateTo).ToList();
                }
            }
            else if (hireFrom != null && hireTo != null)
            {
                int.TryParse(hireFrom, out int glFiscalId1);
                int.TryParse(hireTo, out int glFiscalId2);

                var glYearF = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalId1);
                var glYearT = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalId2);
                if (glYearF != null && glYearT != null)
                {
                    var empIds = db.Person_Employee.Where(x => x.HiredDate >= glYearF.DateFrom && x.HiredDate <= glYearT.DateTo).Select(x => x.EmployeeId).ToList();
                    placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();

                    // employees = employees.Where(x => x.HiredDate >= glYearF.DateFrom && x.HiredDate <= glYearT.DateTo).ToList();
                }
            }

            var expiriances = db.Experiences.ToList();
            DateTime today = DateTime.Today;
            var currentPeriod = db.GLFiscalYears.Where(x => x.DateFrom <= today).FirstOrDefault(x => x.DateTo >= today);



            if (employee != null)
            {
                int.TryParse(employee, out int empId);
                placements = placements.Where(x => x.EmployeeId == empId).ToList();
            }

            List<int> empColList = new List<int>();
            double checkTotalExp = 0;

            if (from != null || to != null)
            {
                foreach (var pl in placements)
                {
                    var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == pl.EmployeeId);

                    if (emp != null)
                    {
                        var exps = db.Experiences.Where(x => x.EmployeeId == emp.EmployeeId).ToList();

                        foreach (var ex in exps)
                        {
                            checkTotalExp = checkTotalExp + (ex.To.Subtract(ex.From).Days);
                        }
                        if (emp.IsActive)
                        {
                            checkTotalExp = checkTotalExp + (DateTime.Today.Subtract(emp.HiredDate).Days);
                        }
                        else
                        {
                            var termination = db.Terminations.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (termination != null)
                            {
                                checkTotalExp = checkTotalExp + termination.TerminationDate.Value.Subtract(emp.HiredDate).Days;
                            }
                        }

                        int check = (int)checkTotalExp / 365;

                        //// this is to check if the year entered by user is character or digit 

                        //int fr = -1;
                        //int tt = -1;

                        //if (from != null)
                        //{
                        //    foreach (char c in from)
                        //    {
                        //        if (char.IsDigit(c))
                        //        {
                        //            fr = Convert.ToInt32(from);
                        //        }                                
                        //    }
                        //}
                        //if (to != null)
                        //{
                        //    foreach (char c in to)
                        //    {
                        //        if (char.IsDigit(c))
                        //            tt = Convert.ToInt32(to);
                        //    }
                        //}

                        if (from != null && to != null)
                        {
                            int.TryParse(from, out int fr);
                            int.TryParse(to, out int tt);

                            if (check >= fr && check <= tt)
                            {
                                empColList.Add(emp.EmployeeId);
                            }
                        }
                        if (from != null && to == null)
                        {
                            int.TryParse(from, out int fr);

                            if (check >= fr)
                            {
                                empColList.Add(emp.EmployeeId);
                            }
                        }
                        if (from == null && to != null)
                        {
                            int.TryParse(to, out int tt);

                            if (check <= tt)
                            {
                                empColList.Add(emp.EmployeeId);
                            }
                        }

                        checkTotalExp = 0;
                    }

                }

                placements = placements.Where(x => empColList.Contains(x.EmployeeId)).ToList();
            }

            placements = placements.DistinctByField(x => x.EmployeeId).ToList();

            foreach (var pl in placements)
            {
                double expSum = 0;
                double totalExternalExp = 0;
                string PlaceWorked = "";
                string PlaceWorkedInternal = "";
                var e = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == pl.EmployeeId);

                if (e != null)
                {
                    var retireGC = e.DateOfBirth.AddYears(60);
                    var retire = XAPI.EthiopicDateTime.GetEthiopicDate(retireGC.Day, retireGC.Month, retireGC.Year);

                    EmployeeMasterViewModel = new EmployeeMasterViewModel();

                    EmployeeMasterViewModel.DateOfBirth = e.DateOfBirthAm;
                    EmployeeMasterViewModel.RetirementDate = retire;// e.DateOfBirth.AddYears(60).ToShortDateString();
                    EmployeeMasterViewModel.EmployeeCode = e.EmployeeCode;
                    EmployeeMasterViewModel.EmployeeId = e.EmployeeId;
                    EmployeeMasterViewModel.FullName = e.FullNameAmharic; //HrHelper.GetEmployeeFullName(e.EmployeeId);
                    var employeeTypee = db.EmployeeTypes.Find(e.EmployeeTypeId);
                    if (employeeTypee != null)
                    {
                        EmployeeMasterViewModel.EmployeeType = employeeTypee.Name;
                    }
                    EmployeeMasterViewModel.Gender = HrHelper.GetGender(e.Gender);
                    EmployeeMasterViewModel.Religion = HrHelper.GetReligion(e.Religion);
                    EmployeeMasterViewModel.Age = GetAge(e.DateOfBirth);
                    EmployeeMasterViewModel.Education = new List<EmployeeEductaionList>();
                    EmployeeMasterViewModel.Training = new List<Training>();
                    EmployeeMasterViewModel.TrainingExt = new List<TrainingExt>();
                    EmployeeMasterViewModel.Languages = new List<ExceedERP.Reporting.HRM.HRM.HRMViewModel.Languages>();
                    EmployeeMasterViewModel.EmployeePerformances = new List<EmployeePerformances>();
                    EmployeeMasterViewModel.Discipline = new List<Discipline>();
                    var individualPlacements = db.Placements.Where(x => x.EmployeeId == pl.EmployeeId).OrderByDescending(x => x.AssignmentDate).ToList();
                    var fromdatestr = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                    foreach (var perPl in individualPlacements)
                    {

                        PlaceWorkedInternal += "  From: " + perPl.AssignmentDateAmharic + " " + "to:" + fromdatestr + " " + perPl.JobTitleName + "<br /> ";
                        fromdatestr = perPl.AssignmentDateAmharic;
                    }
                    EmployeeMasterViewModel.InternalExprianceDetail = PlaceWorkedInternal;
                    var empExtExps = db.Experiences.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    foreach (var ex in empExtExps)
                    {
                        expSum = expSum + (ex.To - ex.From).Days;
                        PlaceWorked += "  From: " + ex.FromExAmharic + " " + "to:" + ex.ToExAmharic + " " + ex.Organozation + "  " + ex.OrgFullAddress + "<br /> ";
                    }
                    EmployeeMasterViewModel.Expiriance = EmployeeHelper.GetDaystoWords(expSum);
                    EmployeeMasterViewModel.ExpirianceDetails = PlaceWorked;
                    if (!string.IsNullOrEmpty(e.Nation))
                    {
                        int.TryParse(e.Nation, out int lookId);
                        var nation = db.Lookups.FirstOrDefault(x => x.LookupId == lookId);
                        if (nation != null)
                        {
                            EmployeeMasterViewModel.Nation = nation.Description;
                        }
                    }

                    var education = db.Education.Where(x => x.EmployeeId == e.EmployeeId).ToList();

                    foreach (var educ in education)
                    {
                        var filed = db.Lookups.FirstOrDefault(x => x.LookupId == educ.EduField);
                        var level = db.Lookups.FirstOrDefault(x => x.LookupId == educ.EduLevel);
                        EmployeeEductaionList edu = new EmployeeEductaionList();

                        edu.EduField = filed != null ? filed.Description : "Unfilled";

                        edu.EduLevel = level != null ? level.Description : "Unfilled";
                        edu.From = educ.FromEAmharic;
                        edu.To = educ.ToEAmharic;
                        edu.Institute = educ.OrganizationName;

                        EmployeeMasterViewModel.Education.Add(edu);
                    }
                    //// i do this to connect the broke lines when the list is empty
                    if (!EmployeeMasterViewModel.Education.Any())
                    {
                        EmployeeEductaionList edu = new EmployeeEductaionList();

                        edu.EduField = " ";

                        edu.EduLevel = " ";
                        edu.From = " ";
                        edu.To = " ";
                        edu.Institute = " ";

                        EmployeeMasterViewModel.Education.Add(edu);
                    }

                    var value = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.PENSION_NO);

                    if (value != null)
                    {
                        EmployeeMasterViewModel.PensionNo = value.IdNumber;
                    }

                    var empExtExpss = expiriances.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    foreach (var ex in empExtExpss)
                    {
                        expSum = expSum + (ex.To - ex.From).Days;
                        totalExternalExp = totalExternalExp + (ex.To - ex.From).Days;
                    }
                    if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                    {
                         EmployeeMasterViewModel.Expiriance = EmployeeHelper.GetDaystoWordsAmharic(expSum);

                    }
                    else
                    {
                        EmployeeMasterViewModel.Expiriance = EmployeeHelper.GetDaystoWords(expSum);

                    }
                    EmployeeMasterViewModel.MartialStatus = HrHelper.GetMartialStatus(e);

                    double totalInternalDays = DateTime.Now.Subtract(e.HiredDate).TotalDays;

                    EmployeeMasterViewModel.InternalExpriance = EmployeeHelper.GetDaystoWords(totalInternalDays);

                    var ExpTotalSum = totalInternalDays + totalExternalExp;

                    if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                    {
                        EmployeeMasterViewModel.ExpTotalSum = EmployeeHelper.GetDaystoWordsAmharic(ExpTotalSum);

                    }
                    else
                    {
                        EmployeeMasterViewModel.ExpTotalSum = EmployeeHelper.GetDaystoWords(ExpTotalSum);

                    }

                    var languageProficiency = db.LanguageOfProficiencies.Where(x => x.EmployeeId == e.EmployeeId);
                    foreach (var lang in languageProficiency)
                    {
                        var language = db.Languages.FirstOrDefault(x => x.LanguageId == lang.LanguageId);
                        if (language != null)
                        {
                            HRMViewModel.Languages lan = new HRMViewModel.Languages();

                            lan.Name = language.Description;
                            EmployeeMasterViewModel.Languages.Add(lan);
                        }
                    }

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.Languages.Any())
                    {
                        HRMViewModel.Languages lan = new HRMViewModel.Languages();

                        lan.Name = " ";

                        EmployeeMasterViewModel.Languages.Add(lan);
                    }

                    var courseTrankings = db.Trainee.Where(x => x.EmployeeId == e.EmployeeId);
                    foreach (var course in courseTrankings)
                    {
                        Training train = new Training();
                        var tracking = db.CourseTrackings.FirstOrDefault(x => x.CourseTrackingId == course.CourseTrackingId);
                        if (tracking != null)
                        {
                            var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == tracking.LookupId);
                            if (lookup != null)
                            {
                                train.Name = lookup.Description;

                                EmployeeMasterViewModel.Training.Add(train);
                            }
                        }
                    }

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.Training.Any())
                    {
                        Training train = new Training();

                        train.Name = " ";

                        EmployeeMasterViewModel.Training.Add(train);
                    }

                    var trainings = db.EmpTrainingEducations.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    foreach (var train in trainings)
                    {
                        TrainingExt trainExt = new TrainingExt();
                        if (train != null)
                        {
                            try
                            {
                                trainExt.Name = train.TrainingName;

                                EmployeeMasterViewModel.TrainingExt.Add(trainExt);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.TrainingExt.Any())
                    {
                        TrainingExt trainExt = new TrainingExt();

                        trainExt.Name = " ";

                        EmployeeMasterViewModel.TrainingExt.Add(trainExt);
                    }

                    var empPerformances = db.MeasurementDefinitions.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    foreach (var empPerformance in empPerformances)
                    {
                        EmployeePerformances empPerform = new EmployeePerformances();
                        var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == empPerformance.LookupId);
                        if (lookup != null)
                        {
                            empPerform.Type = lookup.Description;
                        }
                        empPerform.EvaluationCriteria = empPerformance.EvaluationCriteria;
                        empPerform.Result = empPerformance.Result.ToString();

                        EmployeeMasterViewModel.EmployeePerformances.Add(empPerform);
                    }

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.EmployeePerformances.Any())
                    {
                        EmployeePerformances empPerform = new EmployeePerformances();

                        empPerform.Type = " ";

                        EmployeeMasterViewModel.EmployeePerformances.Add(empPerform);
                    }

                    var disciplineCases = db.DisciplinaryCases.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    foreach (var disc in disciplineCases)
                    {
                        var discipine = new Discipline();

                        var empDisc = db.EmpDisciplineMeasure.FirstOrDefault(x => x.DisciplinaryCaseId == disc.DisciplinaryCaseId);
                        if (empDisc != null)
                        {
                            var discMeasure = db.DisciplineMeasures.FirstOrDefault(x => x.MeasuresId == empDisc.MeasuresId);
                            if (discMeasure != null)
                            {
                                discipine.MeasureRank = discMeasure.MeasuresType;
                            }
                            var discMeasureDetail = db.DisciplineMeasuresDetail.FirstOrDefault(x => x.DetailMeasuresId == empDisc.DetailMeasuresId);
                            if (discMeasureDetail != null)
                            {
                                discipine.MeasureTaken = discMeasureDetail.Description;
                            }

                            discipine.Month = empDisc.Month;
                            var disMeasureByMoney = db.DisciplineMeasuerByMoney.FirstOrDefault(x => x.DisciplinaryCaseId == disc.DisciplinaryCaseId);
                            if (disMeasureByMoney != null)
                            {
                                discipine.Amount = disMeasureByMoney.Amount;
                            }

                            EmployeeMasterViewModel.Discipline.Add(discipine);
                        }
                    }

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.Discipline.Any())
                    {
                        var discipine = new Discipline();

                        discipine.MeasureRank = " ";
                        discipine.MeasureTaken = " ";
                        discipine.Month = 0;
                        discipine.Amount = 0;

                        EmployeeMasterViewModel.Discipline.Add(discipine);
                    }

                    //EmployeeMasterViewModel.InternalExpriance = DateTime.Now.Date.Subtract(new DateTime(e.HiredDate.Year, e.HiredDate.Month,
                    //    e.HiredDate.Day)).TotalDays / 365.25;

                    EmployeeMasterViewModel.HireDate = e.HiredDateAmharic;//placement.AssignmentDateAmharic;
                    EmployeeMasterViewModel.Salary = pl.Salary;
                    EmployeeMasterViewModel.ResponsibilityLevel = HrHelper.GetResponsibilityLevel(pl.ResponsibilityLevel);
                    EmployeeMasterViewModel.EmployementType = HrHelper.GetAssignmentType(pl.AssignmentType);

                    if (e.Status == EmployeeStatus.CONTRACT)
                    {
                        EmployeeMasterViewModel.Employement = EmployeeStatus.CONTRACT.ToString();
                    }
                    else if (e.Status == EmployeeStatus.PERMANENT)
                    {
                        EmployeeMasterViewModel.Employement = EmployeeStatus.PERMANENT.ToString();
                    }

                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == pl.BranchId);
                    if (branch != null)
                    {
                        EmployeeMasterViewModel.BranchName = branch.AmharicName;
                    }
                    var structure = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == pl.OrgStructureId);
                    if (structure != null)
                    {
                        EmployeeMasterViewModel.StructureName = structure.Name; ;
                    }


                    var jobTitle = db.Lookups.FirstOrDefault(x => x.LookupId == pl.LookupId);

                    if (jobTitle != null)
                    {
                        EmployeeMasterViewModel.JobTitle = jobTitle.Description;
                    }
                    EmployeeMasterViewModel.Salary = pl.Salary;

                    var step = db.Steps.FirstOrDefault(x => x.StepId == pl.StepId);
                    if (step != null)
                    {
                        EmployeeMasterViewModel.Step = step.StepName;
                    }
                    var rank = db.Ranks.FirstOrDefault(x => x.RankId == pl.RankId);
                    if (rank != null)
                    {
                        EmployeeMasterViewModel.Rank = rank.RankName;
                    }

                    if (currentPeriod != null)
                    {
                        var entitle = db.LeavesEntitle.Where(x => x.EmployeeId == e.EmployeeId && !x.IsExpired);

                        if (entitle.Any())
                        {

                            EmployeeMasterViewModel.TotalLeave = entitle.Sum(x => x.Balance);
                        }
                    }

                    EmployeeMasterViewModelList.Add(EmployeeMasterViewModel);

                }

            }

            return EmployeeMasterViewModelList;
        }

        //private decimal GetTakenAmount(int periodId, int empId)
        //{
        //    var taken = db.Leaves.Where(x => x.GlFiscalYearId == periodId && x.EmployeeId == empId).ToList();
        //    if (taken.Any())
        //    {
        //        return taken.Sum(x => x.Amount);
        //    }
        //    return 0;
        //}

        /////////////////////////////////////////////////////////////////////////
        public List<Placement> FilterEmployees(object[] branchId, string orgStructureId, string employeeType)
        {

            List<Placement> emplist = new List<Placement>();

            var checkBr = branchId.ToList();
            //bool checkBra = object.ReferenceEquals(branchId, null);
            var placements = db.Placements.ToList();

            if (checkBr.Any())
            {
                foreach (var b in checkBr)
                {
                    var filt = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, Convert.ToInt32(b)).ToList();

                    emplist.AddRange(filt);
                }
            }
            if (orgStructureId != null)
            {
                emplist = EmployeeHelper.GetEmployees_ByDepartment_FromPlacement(placements, Convert.ToInt32(orgStructureId)).ToList();
            }
            if (!checkBr.Any() && orgStructureId == null)
            {
                emplist = placements;
            }
            return emplist;
        }

        ///////////////////////////////////////////

        //public List<Person_Employee> GetEmployeesByBranchAndWorkUnit(int branchId, int orgStructureId)
        //{

        //    var list = new List<Person_Employee>();
        //    var allEmployee = db.Person_Employee.ToList();
        //    var placements = db.Placements.Where(x => x.BranchId == branchId && x.OrgStructureId == orgStructureId).ToList();
        //    if (placements.Any())
        //    {
        //        var filterd = from f in placements
        //                      join e in allEmployee
        //                      on f.EmployeeId equals e.EmployeeId
        //                      where f.EmployeeId == e.EmployeeId
        //                      select (new { e });

        //        foreach (var p in filterd)
        //        {

        //            list.Add(p.e);

        //        }
        //    }
        //    return list;

        //}

        //////////////////////////////////////////////////////

        //public List<Person_Employee> GetEmployeesByBranch(int branchId)
        //{

        //    var list = new List<Person_Employee>();
        //    var allEmployee = db.Person_Employee.ToList();
        //    var placements = db.Placements.Where(x => x.BranchId == branchId).ToList();
        //    if (placements.Any())
        //    {
        //        foreach (var p in placements)
        //        {
        //            var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
        //            if (check != null)
        //            {
        //                list.Add(check);
        //            }
        //        }
        //    }
        //    return list;

        //}

        /////////////////////////////////////////////////////////////////

        //public List<Person_Employee> GetEmployeesByWorkUnit(int orgStructureId)
        //{

        //    var list = new List<Person_Employee>();
        //    var allEmployee = db.Person_Employee;
        //    var placements = db.Placements.Where(x => x.OrgStructureId == orgStructureId).ToList();
        //    if (placements.Any())
        //    {
        //        foreach (var p in placements)
        //        {
        //            var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
        //            if (check != null)
        //            {
        //                list.Add(check);
        //            }
        //        }
        //    }
        //    return list;
        //}
        public List<int> GetNumbersList()
        {
            List<int> numbers = new List<int>();
            for (var i = 0; i <= 100; i++)
            {
                numbers.Add(i);
            }
            return numbers;
        }
        private static int GetAge(DateTime dateOfBirth)
        {
            decimal diff = (decimal)(DateTime.Now - dateOfBirth).TotalDays / 365.25m;
            return (int)diff;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }





}
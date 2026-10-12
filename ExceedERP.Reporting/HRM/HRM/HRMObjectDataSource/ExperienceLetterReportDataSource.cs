using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class ExperienceLetterReportDataSource : IDisposable
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExperienceLetterReportDto> GetExperienceLetterData(string employeeId, string refNumber)
        {
            if (string.IsNullOrWhiteSpace(employeeId) || !int.TryParse(employeeId, out int empId))
                return new List<ExperienceLetterReportDto>();

            var personEmployee = db.Person_Employee
                .AsNoTracking()
                .FirstOrDefault(x => x.EmployeeId == empId);

            if (personEmployee == null)
                return new List<ExperienceLetterReportDto>();

            var expRequest = db.ExprianceRequests
                .AsNoTracking()
                .Where(x => x.EmployeeId == empId)
                .OrderByDescending(x => x.ApplicationDate)
                .FirstOrDefault();

            var placement = db.Placements
                .AsNoTracking()
                .Where(x => x.EmployeeId == empId && x.isCurrent)
                .OrderByDescending(x => x.PlacementId)
                .FirstOrDefault()
                ?? db.Placements
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == empId)
                    .OrderByDescending(x => x.PlacementId)
                    .FirstOrDefault();

            var termination = db.Terminations
                .AsNoTracking()
                .Where(x => x.EmployeeId == empId && x.IsOnlineApproved)
                .OrderByDescending(x => x.TerminationDate)
                .FirstOrDefault();

            var dto = new ExperienceLetterReportDto();

            // 1. Reference & Letter Header Dates
            dto.ReferenceNumber = !string.IsNullOrWhiteSpace(refNumber)
                ? refNumber
                : "____________________";

            dto.LetterDateAmharic = expRequest?.ApplicationDateAm ?? DateTime.Now.ToString("dd/MM/yyyy");
            dto.LetterDateGregorian = expRequest?.ApplicationDate.ToString("dd/MM/yyyy") ?? DateTime.Now.ToString("dd/MM/yyyy");

            // 2. Titles, Names & Grammatical Pronouns
            bool isFemale = personEmployee.Gender == Gender.FEMALE;
            dto.EmployeeCode = personEmployee.EmployeeCode;
            dto.EmployeeTitleEnglish = isFemale ? "Mrs." : "Mr.";
            dto.EmployeeTitleAmharic = isFemale ? "ወ/ሮ" : "አቶ";
            dto.FullNameEnglish = personEmployee.FullNameaEnglish
                ?? $"{personEmployee.FirstNameEnglish} {personEmployee.MiddleNameEnglish} {personEmployee.LastNameEnglish}".Trim();
            dto.FullNameAmharic = personEmployee.FullNameAmharic
                ?? $"{personEmployee.FirstName} {personEmployee.MiddleName} {personEmployee.LastName}".Trim();

            dto.PronounSubject = isFemale ? "she" : "he";
            dto.PronounPossessive = isFemale ? "her" : "his";
            dto.PronounObject = isFemale ? "her" : "him";

            // 3. Status & Terminations
            dto.IsCurrentEmployee = (termination == null);

            var internalPlacements = db.Placements
                .AsNoTracking()
                .Where(x => x.EmployeeId == empId)
                .OrderByDescending(x => x.AssignmentDate)
                .ToList();

            var placementHistories = db.PlacementHistories
                .AsNoTracking()
                .Where(x => x.EmployeeId == empId)
                .OrderByDescending(x => x.From)
                .ToList();

            var experienceLookupIds = internalPlacements.Select(x => x.LookupId)
                .Concat(placementHistories.Select(x => x.LookupId))
                .Distinct()
                .ToList();
            var experienceJobTitles = db.Lookups
                .AsNoTracking()
                .Where(x => experienceLookupIds.Contains(x.LookupId))
                .ToDictionary(x => x.LookupId);

            var internalExperienceEnglish = new StringBuilder();
            var internalExperienceAmharic = new StringBuilder();
            var today = DateTime.Today;
            var todayAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(today.Day, today.Month, today.Year);

            for (int i = 0; i < internalPlacements.Count; i++)
            {
                var internalPlacement = internalPlacements[i];
                DateTime? endDate = null;
                string endDateAmharic = null;

                if (internalPlacement.isCurrent)
                {
                    endDate = today;
                    endDateAmharic = todayAmharic;
                }
                else if (i == 0 && termination?.TerminationDate.HasValue == true &&
                         termination.TerminationDate.Value > internalPlacement.AssignmentDate)
                {
                    endDate = termination.TerminationDate.Value;
                    endDateAmharic = termination.TerminationDateAm;
                }
                else if (i > 0)
                {
                    endDate = internalPlacements[i - 1].AssignmentDate;
                    endDateAmharic = internalPlacements[i - 1].AssignmentDateAmharic;
                }

                experienceJobTitles.TryGetValue(internalPlacement.LookupId, out var placementJob);
                var englishJobTitle = placementJob?.DescriptionEng ?? placementJob?.Description ?? internalPlacement.JobTitleName ?? "";
                var amharicJobTitle = placementJob?.Description ?? internalPlacement.JobTitleName ?? "";
                var startDateEnglish = internalPlacement.AssignmentDate.ToString("dd.MM.yyyy");
                var startDateAmharic = string.IsNullOrWhiteSpace(internalPlacement.AssignmentDateAmharic)
                    ? XAPI.EthiopicDateTime.GetEthiopicDate(internalPlacement.AssignmentDate.Day, internalPlacement.AssignmentDate.Month, internalPlacement.AssignmentDate.Year)
                    : internalPlacement.AssignmentDateAmharic;
                var endDateEnglish = endDate.HasValue ? endDate.Value.ToString("dd.MM.yyyy") : "-";
                var formattedEndDateAmharic = !endDate.HasValue
                    ? "-"
                    : string.IsNullOrWhiteSpace(endDateAmharic)
                        ? XAPI.EthiopicDateTime.GetEthiopicDate(endDate.Value.Day, endDate.Value.Month, endDate.Value.Year)
                        : endDateAmharic;

                internalExperienceEnglish.Append("+ From: ").Append(startDateEnglish)
                    .Append(" to: ").Append(endDateEnglish).Append(" — ")
                    .Append(System.Web.HttpUtility.HtmlEncode(englishJobTitle)).Append("<br />");
                internalExperienceAmharic.Append("+ ከ: ").Append(System.Web.HttpUtility.HtmlEncode(startDateAmharic))
                    .Append(" እስከ: ").Append(System.Web.HttpUtility.HtmlEncode(formattedEndDateAmharic)).Append(" — ")
                    .Append(System.Web.HttpUtility.HtmlEncode(amharicJobTitle)).Append("<br />");
            }

            foreach (var history in placementHistories)
            {
                experienceJobTitles.TryGetValue(history.LookupId, out var historyJob);
                var englishJobTitle = historyJob?.DescriptionEng ?? historyJob?.Description ?? history.JobTitle ?? "";
                var amharicJobTitle = historyJob?.Description ?? history.JobTitle ?? "";
                var startDateEnglish = history.From.ToString("dd.MM.yyyy");
                var endDateEnglish = history.To.ToString("dd.MM.yyyy");
                var startDateAmharic = string.IsNullOrWhiteSpace(history.FromAmharic)
                    ? XAPI.EthiopicDateTime.GetEthiopicDate(history.From.Day, history.From.Month, history.From.Year)
                    : history.FromAmharic;
                var endDateAmharic = string.IsNullOrWhiteSpace(history.ToAmharic)
                    ? XAPI.EthiopicDateTime.GetEthiopicDate(history.To.Day, history.To.Month, history.To.Year)
                    : history.ToAmharic;

                internalExperienceEnglish.Append("+ From: ").Append(startDateEnglish)
                    .Append(" to: ").Append(endDateEnglish).Append(" — ")
                    .Append(System.Web.HttpUtility.HtmlEncode(englishJobTitle)).Append("<br />");
                internalExperienceAmharic.Append("+ ከ: ").Append(System.Web.HttpUtility.HtmlEncode(startDateAmharic))
                    .Append(" እስከ: ").Append(System.Web.HttpUtility.HtmlEncode(endDateAmharic)).Append(" — ")
                    .Append(System.Web.HttpUtility.HtmlEncode(amharicJobTitle)).Append("<br />");
            }

            dto.InternalExperienceEnglish = internalExperienceEnglish.ToString();
            dto.InternalExperienceAmharic = internalExperienceAmharic.ToString();

            if (placement != null)
            {
                // Extract primitive IDs before executing LINQ to prevent EF constant conversion errors
                int orgStructureId = placement.OrgStructureId;
                int? businessUnitId = placement.BusinessUnitId;
                int lookupId = placement.LookupId;

                // Department / Project Structure
                var workUnit = db.MainCompanyStructures
                    .AsNoTracking()
                    .FirstOrDefault(x => x.OrgStructureId == orgStructureId);

                var businessUnit = businessUnitId.HasValue
                    ? db.BusinessUnits.AsNoTracking().FirstOrDefault(x => x.BusinessUnitId == businessUnitId.Value)
                    : null;

                string projectOrDeptEng = businessUnit?.Name ?? workUnit?.Name ?? "Head Office";
                string projectOrDeptAm = workUnit?.Name ?? businessUnit?.Name ?? workUnit?.Name ?? "ዋና መ/ቤት";

                dto.DepartmentEnglish = projectOrDeptEng.Replace("&", "&amp;");
                dto.DepartmentAmharic = projectOrDeptAm.Replace("&", "&amp;");

                // Job Title
                var job = db.Lookups
                    .AsNoTracking()
                    .FirstOrDefault(x => x.LookupId == lookupId);

                // Job Title Sanitization Example
                dto.JobTitleEnglish = (job?.DescriptionEng ?? job?.Description ?? placement.JobTitleName ?? "Staff").Replace("&", "&amp;");
                dto.JobTitleAmharic = (job?.Description ?? placement.JobTitleName ?? "ሠራተኛ").Replace("&", "&amp;");
                
                // Service Dates
                dto.StartDateGregorian = $"{placement.AssignmentDate:MMM.dd/yyyy} G.C";
                dto.StartDateAmharic = placement.AssignmentDateAmharic;
                dto.MonthlyGrossSalary = placement.Salary;

                if (!dto.IsCurrentEmployee && termination != null)
                {
                    dto.EndDateGregorian = $"{termination.TerminationDate:MMM.dd/yyyy} G.C";
                    dto.EndDateAmharic = termination.TerminationDateAm;
                    dto.ResignationDateGregorian = termination.TerminationDate.HasValue
                        ? $"{termination.TerminationDate.Value.AddDays(1):MMM.d/yyyy} G.C"
                        : string.Empty;
                    dto.ResignationDateAmharic = termination.TerminationDateAm;
                }
                else
                {
                    dto.EndDateGregorian = "date";
                    dto.EndDateAmharic = "ይህ ደብዳቤ ወጪ እስከ ተደረገበት ቀን ድረስ";
                }
            }

            // 4. Allowances Breakdown
            var allowances = db.AllowancePerEmployees
                .AsNoTracking()
                .Where(x => x.EmployeeId == empId && !x.Pend)
                .ToList();

            if (allowances.Any())
            {
                var defIds = allowances.Select(a => a.AllowanceDefinationId).Distinct().ToList();
                var definitions = db.AllowanceDefinations
                    .AsNoTracking()
                    .Where(d => defIds.Contains(d.AllowanceDefinationId))
                    .ToDictionary(k => k.AllowanceDefinationId);

                foreach (var a in allowances)
                {
                    if (!definitions.TryGetValue(a.AllowanceDefinationId, out var def)) continue;

                    switch (def.AllowanceType)
                    {
                        case AllowanceType.RESPONSIBILITY:
                            dto.ResponsibilityAllowance += def.Amount;
                            break;
                        case AllowanceType.PROFESSIONAL:
                            dto.ProfessionalAllowance += def.Amount;
                            break;
                        case AllowanceType.HOME:
                            dto.HouseAllowance += def.Amount;
                            break;
                        case AllowanceType.TRANSPORT:
                            dto.TransportAllowance += def.Amount;
                            break;
                        case AllowanceType.POSITION:
                            dto.PositionAllowance += def.Amount;
                            break;
                    }
                }
            }

            // 5. Approver / Department Head Resolution (Primitive types only in LINQ)
            int hId = placement?.HeadId ?? 0;
            if (hId > 0)
            {
                var head = db.Person_Employee
                    .AsNoTracking()
                    .FirstOrDefault(x => x.EmployeeId == hId);

                if (head != null)
                {
                    dto.SignerNameEnglish = $"{head.FirstNameEnglish} {head.MiddleNameEnglish} {head.LastNameEnglish}".Trim();
                    dto.SignerNameAmharic = $"{head.FirstName} {head.MiddleName} {head.LastName}".Trim();

                    int headEmpId = head.EmployeeId;
                    var headPlacement = db.Placements
                        .AsNoTracking()
                        .FirstOrDefault(x => x.EmployeeId == headEmpId && x.isCurrent);

                    // Extract primitive lookup ID before querying db.Lookups
                    int headLookupId = headPlacement?.LookupId ?? 0;

                    var headJob = headLookupId > 0
                        ? db.Lookups.AsNoTracking().FirstOrDefault(x => x.LookupId == headLookupId)
                        : null;

                    // In ExperienceLetterReportDataSource.cs:

                    dto.SignerJobTitleEnglish = (headJob?.DescriptionEng ?? "Human Resource Dev't & Administration Dep't Manager")
                        .Replace("&", "&amp;");

                    dto.SignerJobTitleAmharic = (headJob?.Description ?? "የሰው ኃይል ልማትና አስተዳደር መምሪያ ሥራ አስኪያጅ")
                        .Replace("&", "&amp;");
                    var approverUserName = UserHelper.GetUserNameByEmployeeId(head.EmployeeId);
                    dto.SignatureImage = SignatureHelper.GetUserSignature(approverUserName);
                }
            }

            // 6. Company Stamp (Safe Memory Clone)
            var stampAttachment = db.Attachments
                .AsNoTracking()
                .FirstOrDefault(a => a.ReferenceType == AttachmentType.ISO_LOGO || a.ReferenceType == AttachmentType.STAMP);

            if (stampAttachment?.BinaryFile != null)
            {
                using (var ms = new MemoryStream(stampAttachment.BinaryFile))
                {
                    using (var tempImg = Image.FromStream(ms))
                    {
                        dto.CompanyStamp = new Bitmap(tempImg);
                    }
                }
            }

            return new List<ExperienceLetterReportDto> { dto };
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}
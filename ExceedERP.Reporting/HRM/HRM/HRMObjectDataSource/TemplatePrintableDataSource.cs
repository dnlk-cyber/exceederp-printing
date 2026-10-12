using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Templates;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class TemplatePrintableDataSource : IDisposable
    {
        private readonly ExceedDbContext _db;
        private readonly bool _ownsContext;

        public TemplatePrintableDataSource()
        {
            _db = new ExceedDbContext();
            _ownsContext = true;
        }

        public TemplatePrintableDataSource(ExceedDbContext context)
        {
            _db = context;
            _ownsContext = false;
        }

        /// <summary>
        /// Main DataObject select method exposed to Telerik Reporting or Direct Print Services.
        /// </summary>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PrintableLetter> GetPrintableExperienceLetter(string employeeId, bool includeHeader, string language)
        {
            var results = new List<PrintableLetter>();

            // 1. Fetch Employee and Company Info
            var emp = _db.Person_Employee.FirstOrDefault(e => e.EmployeeId.ToString() == employeeId);
            var company = _db.Companies.FirstOrDefault();

            if (emp == null)
            {
                return results;
            }

            // 2. Instantiate base printable letter structure
            var letter = new PrintableLetter
            {
                IssueDate = DateTime.Now.ToString("yyyy-MM-dd"),
                DocumentNumber = $"EXP-{DateTime.Now:yyyyMM}-{emp.EmployeeId}"
            };

            // Map Letterhead if required
            if (includeHeader && company != null)
            {
                MapCompanyHeader(letter, company);
            }

            // 3. Resolve Template based on Language & Type
            string langKey = (language ?? "en").ToLower();
            var template = ResolveTemplate("ExperienceLetter", langKey);

            if (template == null || string.IsNullOrWhiteSpace(template.Document))
            {
                letter.Title = GetDefaultTitle(langKey);
                letter.DocumentBody = "<p>Template content not configured.</p>";
                results.Add(letter);
                return results;
            }

            // 4. Build and Replace Placeholders
            letter.Title = template.Title ?? GetDefaultTitle(langKey);
            letter.DocumentBody = ProcessTemplateBody(template.Document, emp, langKey);

            results.Add(letter);
            return results;
        }

        #region Helper Methods

        private void MapCompanyHeader(PrintableLetter letter, MainCompany company)
        {
            //letter.CompanyName = company.BrandName;
            //letter.LogoFileName = company.lo;
            //letter.Address = company.Address;
            //letter.City = company.City;
            //letter.Country = company.Country;
            //letter.Email = company.Email;
            //letter.Phone = company.Phone;
            //letter.WebSite = company.WebSite;
            //letter.Pobox = company.Pobox;
        }

        private Template ResolveTemplate(string category, string language)
        {
            return _db.Templates
                .FirstOrDefault(t => t.TemplateId.ToString()==category);
        }

        private string GetDefaultTitle(string language)
        {
            switch (language)
            {
                case "am":
                case "am-et":
                    return "ለሚመለከተው ሁሉ";
                case "om":
                    return "KAN IRRATTI YAADACHUU DANDA'A";
                default:
                    return "TO WHOM IT MAY CONCERN";
            }
        }

        private string ProcessTemplateBody(string rawContent, Person_Employee emp, string language)
        {
            if (string.IsNullOrEmpty(rawContent)) return string.Empty;

            var sb = new StringBuilder(rawContent);

            // Basic replacements
            sb.Replace("-FullName-", $"{emp.FirstName} {emp.MiddleName} {emp.LastName}".Trim());
            sb.Replace("-EmployeeId-", emp.EmployeeCode ?? "");
            //sb.Replace("-JobTitle-", emp.CurrentJobTitle ?? "");
            //sb.Replace("-Department-", emp.DepartmentName ?? "");
            //sb.Replace("-HireDate-", emp.HireDate?.ToString("yyyy-MM-dd") ?? "");
            //sb.Replace("-Salary-", emp.Salary.ToString("N2"));

            // Dynamic Status Clauses (e.g., active vs terminated)
            string currentStatusClause = GetStatusClause(!emp.IsActive, language);
            sb.Replace("-EmploymentStatusClause-", currentStatusClause);

            return sb.ToString();
        }

        private string GetStatusClause(bool isTerminated, string language)
        {
            if (isTerminated)
            {
                switch (language)
                {
                    case "am": return "ይከፈላቸው የነበረው";
                    case "om": return "kan kaffalamu ture";
                    default: return "was formerly paid a salary of";
                }
            }
            else
            {
                switch (language)
                {
                    case "am": return "አሁን እየተከፈላቸው ያለ";
                    case "om": return "amma kaffalama jira";
                    default: return "is currently paid a salary of";
                }
            }
        }

        #endregion

        public void Dispose()
        {
            if (_ownsContext)
            {
                _db?.Dispose();
            }
        }
    }
    //Output DTO expected by Telerik / Report Viewer / PDF Exporter
    public class PrintableLetter
    {
        // Company / Letterhead Information
        public string CompanyName { get; set; }
        public string LogoFileName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string WebSite { get; set; }
        public string Pobox { get; set; }

        // Document Specifics
        public string DocumentNumber { get; set; }
        public string Title { get; set; }
        public string DocumentBody { get; set; }
        public string IssueDate { get; set; }
        public string SignatoryName { get; set; }
        public string SignatoryTitle { get; set; }
    }

    // Context DTO passed into the template builder
    public class PrintableContext
    {
        public string EmployeeId { get; set; }
        public string RecipientName { get; set; }
        public string Language { get; set; } // "en", "am", "om"
        public bool IncludeHeader { get; set; }
        public bool IsTerminated { get; set; }
        public Dictionary<string, string> CustomPlaceholders { get; set; } = new Dictionary<string, string>();
    }
}

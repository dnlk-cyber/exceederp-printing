using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExceedERP.Core.Domain.Common.HRM;
using Kendo.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    internal sealed class EmployeeProfileNameFilters
    {
        private readonly IQueryable<Person_Employee> employees;
        private readonly Lazy<List<EmployeeNames>> names;

        internal EmployeeProfileNameFilters(IQueryable<Person_Employee> employees)
        {
            this.employees = employees;
            // Fetch only names and IDs already within the profile's access scope.
            names = new Lazy<List<EmployeeNames>>(LoadNames);
        }

        internal IQueryable<Person_Employee> Search(string employeeName)
        {
            if (string.IsNullOrWhiteSpace(employeeName))
                return employees;

            var search = employeeName.Trim().Normalize(NormalizationForm.FormC);
            if (!ContainsEthiopic(search))
            {
                return employees.Where(employee =>
                    ((employee.FirstName ?? "") + " " + (employee.MiddleName ?? "") + " " + (employee.LastName ?? "")).Contains(search)
                    || ((employee.FirstNameEnglish ?? "") + " " + (employee.MiddleNameEnglish ?? "") + " " + (employee.LastNameEnglish ?? "")).Contains(search));
            }

            var normalized = NormalizeName(search);
            var matchingIds = names.Value.Where(name =>
                FullNameContains(normalized, name.FirstName, name.MiddleName, name.LastName) ||
                FullNameContains(normalized, name.FirstNameEnglish, name.MiddleNameEnglish, name.LastNameEnglish))
                .Select(name => name.EmployeeId).ToArray();
            return employees.Where(employee => matchingIds.Contains(employee.EmployeeId));
        }

        internal void ApplyColumnFilters(IList<IFilterDescriptor> filters)
        {
            for (int index = 0; index < filters.Count; index++)
            {
                if (filters[index] is CompositeFilterDescriptor group)
                {
                    ApplyColumnFilters(group.FilterDescriptors);
                }
                else if (filters[index] is FilterDescriptor filter && filter.Value is string value && IsLocalNameColumn(filter.Member))
                {
                    filter.Value = value.Trim().Normalize(NormalizationForm.FormC);
                    if (!ContainsEthiopic(value))
                        continue;

                    // Apply the same Kendo operator to the Unicode names before SQL paging.
                    filter.Value = NormalizeName(value);
                    var name = Expression.Parameter(typeof(EmployeeNames), "name");
                    var matches = Expression.Lambda<Func<EmployeeNames, bool>>(filter.CreateFilterExpression(name), name).Compile();
                    var matchingIds = names.Value.Where(matches).Select(employee => employee.EmployeeId).ToArray();
                    filters[index] = new EmployeeIdsFilter(matchingIds);
                }
            }
        }

        private List<EmployeeNames> LoadNames()
        {
            var rows = employees.Select(employee => new EmployeeNames
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                MiddleName = employee.MiddleName,
                LastName = employee.LastName,
                FirstNameEnglish = employee.FirstNameEnglish,
                MiddleNameEnglish = employee.MiddleNameEnglish,
                LastNameEnglish = employee.LastNameEnglish
            }).ToList();

            foreach (var name in rows)
            {
                name.FirstName = NormalizeName(name.FirstName);
                name.MiddleName = NormalizeName(name.MiddleName);
                name.LastName = NormalizeName(name.LastName);
                name.FirstNameEnglish = NormalizeName(name.FirstNameEnglish);
                name.MiddleNameEnglish = NormalizeName(name.MiddleNameEnglish);
                name.LastNameEnglish = NormalizeName(name.LastNameEnglish);
            }
            return rows;
        }

        private static bool IsLocalNameColumn(string member)
        {
            return member == nameof(EmployeeNames.FirstName) || member == nameof(EmployeeNames.MiddleName) || member == nameof(EmployeeNames.LastName);
        }

        private static string NormalizeName(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // Ignore invisible formatting marks that may be copied along with a name.
            return string.Concat(value.Where(character => char.GetUnicodeCategory(character) != UnicodeCategory.Format))
                .Trim().Normalize(NormalizationForm.FormC);
        }

        private static bool ContainsEthiopic(string value)
        {
            for (int index = 0; index < value.Length; index++)
            {
                int codePoint = value[index];
                if (char.IsSurrogatePair(value, index))
                {
                    codePoint = char.ConvertToUtf32(value, index);
                    index++;
                }
                if ((codePoint >= 0x1200 && codePoint <= 0x139F) || (codePoint >= 0x2D80 && codePoint <= 0x2DDF)
                    || (codePoint >= 0xAB00 && codePoint <= 0xAB2F) || (codePoint >= 0x1E7E0 && codePoint <= 0x1E7FF))
                    return true;
            }
            return false;
        }

        private static bool FullNameContains(string search, params string[] names)
        {
            var fullName = string.Join(" ", names.Where(name => name.Length > 0));
            return fullName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private sealed class EmployeeNames
        {
            public int EmployeeId { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string FirstNameEnglish { get; set; }
            public string MiddleNameEnglish { get; set; }
            public string LastNameEnglish { get; set; }
        }

        private sealed class EmployeeIdsFilter : IFilterDescriptor
        {
            private readonly int[] employeeIds;

            internal EmployeeIdsFilter(int[] employeeIds)
            {
                this.employeeIds = employeeIds;
            }

            public Expression CreateFilterExpression(Expression instance)
            {
                if (employeeIds.Length == 0)
                    return Expression.Constant(false);

                return Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] { typeof(int) },
                    Expression.Constant(employeeIds), Expression.Property(instance, nameof(Person_Employee.EmployeeId)));
            }
        }
    }
}

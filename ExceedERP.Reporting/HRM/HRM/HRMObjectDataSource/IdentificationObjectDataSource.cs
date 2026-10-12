using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class IdentificationObjectDataSource : IDisposable
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<IdentificationReportViewModel> GetIdentificationsByDepartmentAndBranch(int? branchId, int? orgStructureId,string type)
        {

            // Check if type is provided once outside or handle it via standard string comparison
            bool hasType = !string.IsNullOrEmpty(type);

            var query = from iden in _db.Identifications
                        join emp in _db.Person_Employee on iden.Reference equals emp.EmployeeId.ToString()
                        join pl in _db.Placements.Where(p => p.isCurrent) on emp.EmployeeId equals pl.EmployeeId
                        join branch in _db.Branch on pl.BranchId equals branch.BranchId
                        join dept in _db.MainCompanyStructures on pl.OrgStructureId equals dept.OrgStructureId
                        where iden.ReferenceType == "Employee"
                              && (!hasType || iden.Type.ToString() == type)
                              && (!branchId.HasValue || pl.BranchId == branchId.Value)
                              && (!orgStructureId.HasValue || pl.OrgStructureId == orgStructureId.Value)
                        select new IdentificationReportViewModel
                        {
                            IdentificationId = iden.IdentificationId,
                            BranchName = branch.Name,
                            DepartmentName = dept.Name,
                            EmployeeName = emp.FirstNameEnglish+" "+ emp.MiddleNameEnglish+" "+ emp.LastNameEnglish,
                            EmployeeCode = emp.EmployeeCode,
                            Type = iden.Type.ToString(),
                            IdNumber = iden.IdNumber,
                            Description = iden.Description,
                            IssueDate = iden.IssueDate,
                            IssueDateIdAm = iden.IssueDateIdAm,
                            ExpiryDate = iden.ExpiryDate,
                            ExpiryDateAm = iden.ExpiryDateAm,
                            Issuer = iden.Issuer,
                            Remark = iden.Remark
                        };

            return query.ToList();

        }

        public void Dispose()
        {
            _db.Dispose();
        }

    }

    [DataObject]
    public class IdentificationTypeODS
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EnumViewModel> GetIdentificationType()
        {
            return Enum.GetValues(typeof(IdentificationType))
                .Cast<IdentificationType>()
                .Select(e => new EnumViewModel
                {
                    Value = e.ToString(), // Raw enum name
                    DisplayName = GetDisplayName(e) // Localized or default display name

                }).ToList();
        }

        private string GetDisplayName(IdentificationType religion)
        {
            FieldInfo fieldInfo = religion.GetType().GetField(religion.ToString());
            if (fieldInfo != null)
            {
                DisplayAttribute displayAttribute = fieldInfo
                    .GetCustomAttribute<DisplayAttribute>();

                if (displayAttribute != null)
                {
                    return displayAttribute.GetName();
                }
            }

            return religion.ToString(); // Fallback to raw enum name
        }

    }
}

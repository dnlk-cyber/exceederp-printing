using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeCourtInsuranceObjectDataSource : List<EmployeeCourtInsurance>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeCourtInsurance> GetEmployeeCourtInsurance()
        {
            List<EmployeeCourtInsurance> EmployeeInfoList = new List<EmployeeCourtInsurance>();
            EmployeeCourtInsurance EmployeeInfo = new EmployeeCourtInsurance();
            Children Children = new Children();
            var employees = db.Person_Employee.Where(x => x.IsActive && x.IsInsured).ToList();
            if (employees.Any())
            {
                foreach (var e in employees)
                {
                    EmployeeInfo = new EmployeeCourtInsurance();
                    EmployeeInfo.Children = new List<Children>();
                    EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                    EmployeeInfo.Dob = e.DateOfBirthAm;
                    EmployeeInfo.Sex = e.Gender;
                    var assoacation = db.EmployeeCourtAssociation.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    if (assoacation.Any())
                    {
                        foreach (var a in assoacation)
                        {
                            //added
                            var exist = db.CourtInsurance.Where(x => x.AssociationId == a.AssociationId)
                                .FirstOrDefault();
                            if (exist != null)
                            {
                                if (a.AssociationType == AssociationType.WIFE)
                                {
                                    EmployeeInfo.WifHusName = a.Name;
                                    EmployeeInfo.WifeHusBob = a.DateOfBirthAmharic;
                                    EmployeeInfo.WifeHusSex = a.Gender;
                                }
                                else if (a.AssociationType == AssociationType.HUSBAND)
                                {
                                    EmployeeInfo.WifHusName = a.Name;
                                    EmployeeInfo.WifeHusBob = a.DateOfBirthAmharic;
                                    EmployeeInfo.WifeHusSex = a.Gender;
                                }
                                else if (a.AssociationType == AssociationType.CHILD)
                                {
                                    Children = new Children();
                                    Children.ChildName = a.Name;
                                    Children.ChildDOB = a.DateOfBirthAmharic;
                                    Children.ChildSex = a.Gender;
                                    EmployeeInfo.Children.Add(Children);
                                }
                            }
                        }
                    }
                    EmployeeInfoList.Add(EmployeeInfo);
                }

            }
            return EmployeeInfoList;
        }
    }
}
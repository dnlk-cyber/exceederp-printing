using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EvaluationTypeObjectDataSource : PerformanceSummary
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public PerformanceSummary GetEvaluationType(string id, string period)
        {

            PerformanceSummary EmployeeInfo = new PerformanceSummary();
            var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();

            if (Organization != null)
            {
                EmployeeInfo.Name = Organization.TradeName;
                EmployeeInfo.AmharicName = Organization.AmharicName;
                EmployeeInfo.Abbreviation = Organization.Abbreviation;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == Organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {
                        if (a.Category == AddressType.KIFLEKETEMA)
                        {
                            EmployeeInfo.Address = "Subcity: " + a.Value + " ";
                        }
                        if (a.Category == AddressType.KEBELE)
                        {
                            EmployeeInfo.Address += "Kebele: " + a.Value + " ";
                        }
                        if (a.Category == AddressType.CITYPROVINCE)
                        {
                            if (EmployeeInfo.Address == null)
                            {
                                EmployeeInfo.Address += a.Value + " -Ethiopia";
                            }
                        }
                        else if (a.Category == AddressType.POBOX)
                        {

                            EmployeeInfo.Pobox = "P.O.Box: " + a.Value;

                        }
                        else if (a.Category == AddressType.OFFICE_EMAIL)
                        {

                            EmployeeInfo.Email = "Email: " + a.Value;

                        }
                        else if (a.Category == AddressType.WEBSITE)
                        {

                            EmployeeInfo.WebSite = "Website:" + a.Value;

                        }
                        else if (a.Category == AddressType.FAX)
                        {

                            EmployeeInfo.Fax = "Fax:" + a.Value;

                        }
                        else if (a.Category == AddressType.OFFICE_PHONE)
                        {

                            EmployeeInfo.Tel = "Tel:" + a.Value;

                        }


                    }

                }
                var logo =
                    db.Attachments.Where(
                        a =>
                            a.Reference == Organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO).FirstOrDefault();
                if (logo != null)
                {
                    EmployeeInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            var periods = db.Quarters.FirstOrDefault(x => x.QuarterId.ToString() == period);

            int GLPeriodId = 0;
            if (periods != null)
            {
                GLPeriodId = periods.QuarterId;
            }
            if (id != null)
            {
                var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId.ToString() == id);

                if (personEmployee != null)
                {
                    EmployeeInfo.FullName = personEmployee.FullNameAmharic;

                    var responsibilityLevel = ResponsibilityLevel.STAFF;
                    var place = db.Placements.FirstOrDefault(x => x.EmployeeId == personEmployee.EmployeeId && x.isCurrent);
                    if (place != null)
                    {
                        responsibilityLevel = place.ResponsibilityLevel;

                        var look = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                        if (look != null)
                        {
                            EmployeeInfo.JobTitle = look.Description;
                        }

                        var mainCompanyStructure = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                        if (mainCompanyStructure != null)
                        {
                            EmployeeInfo.Department = mainCompanyStructure.Name;
                        }
                    }

                    var empid = personEmployee.EmployeeId;
                    var evaluationTypes = db.MeasurementDefinitions.Where(x => x.Role == responsibilityLevel);//db.Lookups.Where(x => x.LookupType == LookUpTypes.EVALUATION_TYPE).ToList();

                    EvaluationTypes type = new EvaluationTypes();
                    decimal result = 0;

                    if (evaluationTypes.Any())
                    {
                        EmployeeInfo.Types = new List<EvaluationTypes>();

                        var empResult = db.EmployeePerformanceScores.Where(x => x.EmployeeId == empid).ToList();
                        if (GLPeriodId != 0)
                        {
                            empResult = empResult.Where(x => x.QuarterId == GLPeriodId).ToList();
                        }

                        foreach (var d in evaluationTypes)
                        {
                            type = new EvaluationTypes();
                            result = 0;
                            type.AssignedAmount = evaluationTypes.Where(x => x.LookupId == d.LookupId).Sum(x => x.DefnPercentageofWeight);
                            type.Type = d.EvaluationCriteria;

                            var measurments = db.Measurements.Where(x => x.MeasurementDefinitionId == d.MeasurementDefinitionId).ToList();
                            foreach (var m in measurments)
                            {
                                result = result + empResult.Where(x => x.MeasurementId == m.MeasurementId).Sum(x => x.Result);
                            }
                            type.ActualAmount = result;

                            type.ActualFrom100 = result * 100 / type.AssignedAmount;

                            EmployeeInfo.Types.Add(type);

                        }
                    }

                   

                }
            }
            return EmployeeInfo;
        }


    }
}
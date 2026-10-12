using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class TrainingEvaluationEvaluaterInfoODS:IDisposable
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EvaluatorInfoVM> GetEvaluatorInfo(string courseTrackingId, string empId)
        {
            List<EvaluatorInfoVM> evaluatorInfoList = new List<EvaluatorInfoVM>();

            EvaluatorInfoVM evaluator = new EvaluatorInfoVM();

            var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (Organization != null)
            {
                evaluator.Name = Organization.TradeName;
                evaluator.AmharicName = Organization.AmharicName;
                evaluator.Abbreviation = Organization.Abbreviation;
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
                            evaluator.Address = "Subcity: " + a.Value + " ";
                        }

                        if (a.Category == AddressType.KEBELE)
                        {
                            evaluator.Address += "Kebele: " + a.Value + " ";
                        }

                        if (a.Category == AddressType.CITYPROVINCE)
                        {
                            if (evaluator.Address == null)
                            {
                                evaluator.Address += a.Value + ", Ethiopia";
                            }
                        }
                        else if (a.Category == AddressType.POBOX)
                        {

                            evaluator.Pobox = "P.O.Box: " + a.Value;

                        }
                        else if (a.Category == AddressType.OFFICE_EMAIL)
                        {

                            evaluator.Email = "Email: " + a.Value;

                        }
                        else if (a.Category == AddressType.WEBSITE)
                        {

                            evaluator.WebSite = "Website:" + a.Value;

                        }
                        else if (a.Category == AddressType.FAX)
                        {

                            evaluator.Fax = "Fax:  " + a.Value;

                        }
                        else if (a.Category == AddressType.OFFICE_PHONE)
                        {

                            evaluator.Tel = "Tel:  " + a.Value;

                        }

                    }

                }

                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == Organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    evaluator.LogoFileName =
                        OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }

                evaluator.Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year);
            }


            if (courseTrackingId != null && empId != null)
            {
                int.TryParse(courseTrackingId, out int courseTraId);
                int.TryParse(empId, out int employeeId);

                var courseTracking = db.CourseTrackings.FirstOrDefault(x=>x.CourseTrackingId == courseTraId);

                if (courseTracking != null)
                {
                    evaluator.From = courseTracking.FromAmharic;
                    evaluator.To = courseTracking.ToAmharic;

                    var course = db.Lookups.FirstOrDefault(x=>x.LookupId == courseTracking.LookupId);
                    if(course != null)
                    {
                        evaluator.TrainingName = course.Description;
                    }

                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId);

                    if(person != null)
                    {
                        evaluator.FullName = person.FullNameAmharic;
                    }

                    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeId && x.isCurrent);

                    if(placement != null)
                    {
                        var mainCompany = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId == placement.OrgStructureId);

                        if (mainCompany != null)
                        {
                            evaluator.Department = mainCompany.Name;
                        }

                        var jobtitle = db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);

                        if(jobtitle != null)
                        {
                            evaluator.Jobtitle = jobtitle.Description;
                        }
                    }
                }
            }

            evaluatorInfoList.Add(evaluator);

            return evaluatorInfoList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }

    public class EvaluatorInfoVM
    {
        public string FullName { get; set; }
        public string TrainingName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Department { get; set; }
        public string Jobtitle { get; set; }
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string EnglishName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
        public string Fax { get; set; }
        public string Pobox { get; set; }
        public string Tel { get; set; }
        public string HimHer { get; set; }
        public string HisHer { get; set; }
        public string Date { get; set; }
    }
}

using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{


    [DataObject]
    public class ClinicObjectDataSource : IDisposable
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ClinicViewModel> GetClinicReport(
            string branch,
            string empId,
            DateTime? from,
            DateTime? to)
        {
            try
            {
                // Prescription Query
                var prescriptionQuery = db.Prescription.AsQueryable();

                if (from.HasValue)
                    prescriptionQuery = prescriptionQuery.Where(x => x.Date >= from.Value);

                if (to.HasValue)
                    prescriptionQuery = prescriptionQuery.Where(x => x.Date <= to.Value);

                var prescriptions = prescriptionQuery.ToList();

                // Placements Query
                var placementQuery = db.Placements.Where(x => x.isCurrent);

                if (!string.IsNullOrWhiteSpace(branch) &&
                    int.TryParse(branch, out int branchId))
                {
                    placementQuery = placementQuery.Where(x => x.BranchId == branchId);
                }

                var placements = placementQuery.ToList();

                // Employees
                var employees = db.Person_Employee
                    .Where(x => x.IsActive)
                    .ToDictionary(x => x.EmployeeId);

                // Patient Cards
                var patientCardQuery = db.PatientCard.AsQueryable();

                if (!string.IsNullOrWhiteSpace(empId))
                {
                    patientCardQuery = patientCardQuery
                        .Where(x => x.EmployeeId.ToString() == empId);
                }

                var patientCards = patientCardQuery.ToList();

                // Dictionaries for fast lookup
                var patientCardDict = patientCards
                    .GroupBy(x => x.PatientId)
                    .ToDictionary(x => x.Key, x => x.First());

                var placementDict = placements
                    .GroupBy(x => x.EmployeeId)
                    .ToDictionary(x => x.Key, x => x.First());

                var departments = db.MainCompanyStructures
                    .ToDictionary(x => x.OrgStructureId);

                var branches = db.Branch
                    .ToDictionary(x => x.BranchId);

                var patientDetails = db.PatientCardDetail
                    .ToList();

                var referrals = db.ReferralSheet
                    .GroupBy(x => x.PatientId)
                    .ToDictionary(x => x.Key, x => x.First());

                var clinicList = new List<ClinicViewModel>();

                foreach (var prescription in prescriptions)
                {
                    if (!patientCardDict.TryGetValue(
                            prescription.PatientId,
                            out var card))
                        continue;

                    if (!placementDict.TryGetValue(
                            card.EmployeeId,
                            out var place))
                        continue;

                    var clinic = new ClinicViewModel();

                    if (employees.TryGetValue(place.EmployeeId, out var employee))
                    {
                        clinic.EmployeeName = employee.FullNameaEnglish;
                        clinic.EmployeeCode = employee.EmployeeCode;

                        if (employee.DateOfBirth != default(DateTime))
                        {
                            clinic.Age = DateTime.Today.Year - employee.DateOfBirth.Year;

                            if (employee.DateOfBirth.Date >
                                DateTime.Today.AddYears(-clinic.Age))
                            {
                                clinic.Age--;
                            }
                        }

                        clinic.Gender = employee?.Gender.ToString().ToLower();
                    }

                    if (departments.TryGetValue(place.OrgStructureId, out var dept))
                    {
                        clinic.Workunit = dept.Name;
                    }

                    if (branches.TryGetValue(place.BranchId, out var branchInfo))
                    {
                        clinic.Branch = branchInfo.Name;
                    }

                    clinic.TreatedDate = prescription.DateAmharic;

                    var detail = patientDetails.FirstOrDefault(x =>
                        x.PatientId == card.PatientId &&
                        x.Date == prescription.Date);

                    if (detail != null)
                    {
                        clinic.Diagnosis = detail.DiagnosisTratment;
                    }

                    if (referrals.TryGetValue(
                            prescription.PatientId,
                            out var referral))
                    {
                        clinic.ReferalDate = referral.DateAmharic;
                    }

                    clinicList.Add(clinic);
                }

                return clinicList;
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return new List<ClinicViewModel>();
            }
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
    //public class ClinicObjectDataSource :IDisposable
    //{
    //    private ExceedDbContext db = new ExceedDbContext();

    //    [DataObjectMethod(DataObjectMethodType.Select)]
    //    public List<ClinicViewModel> GetClinicReport(string branch, string empId, DateTime? from, DateTime ?to)
    //    {
    //        List<ClinicViewModel> clinicList = new List<ClinicViewModel>();

    //        var prescripitons = db.Prescription.ToList();
    //        var placements = db.Placements.Where(x => x.isCurrent).ToList();
    //        var employees = db.Person_Employee.Where(x => x.IsActive).ToList();
    //        var patientCard = db.PatientCard.ToList();



    //        if (from.HasValue)
    //        {

    //            prescripitons = prescripitons.Where(x => x.Date >= from.Value).ToList();

    //        }
    //        if (to.HasValue)
    //        {
    //            prescripitons = prescripitons.Where(x => x.Date <= to.Value).ToList();
    //        }
    //        if (!string.IsNullOrWhiteSpace(branch) && int.TryParse(branch,out int branchId))
    //        {
    //            placements = placements.Where(x => x.BranchId == branchId).ToList();
    //        }


    //        if (!string.IsNullOrWhiteSpace(empId))
    //        {
    //            patientCard = patientCard.Where(x => x.EmployeeId.ToString() == empId).ToList();
    //        }

    //        foreach (var prescription in prescripitons)
    //        {
    //            ClinicViewModel clinic = new ClinicViewModel();
    //            var card = patientCard.FirstOrDefault(x => x.PatientId == prescription.PatientId);
    //            if (card != null)
    //            {
    //                var place = placements.FirstOrDefault(x => x.EmployeeId == card.EmployeeId);
    //                if (place != null)
    //                {

    //                    var employee = employees.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
    //                    if (employee != null)
    //                    {
    //                        clinic.EmployeeName = employee.FullNameaEnglish;
    //                        clinic.EmployeeCode = employee.EmployeeCode;
    //                        clinic.Age = DateTime.Today.Year - employee.DateOfBirth.Year;
    //                        clinic.Gender = employee.Gender.ToString();
    //                    }


    //                    var dept = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
    //                    if (dept != null)
    //                    {
    //                        clinic.Workunit = dept.Name;
    //                    }

    //                    var bra = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
    //                    if (bra != null)
    //                    {
    //                        clinic.Branch = bra.Name;
    //                    }
    //                    clinic.TreatedDate = prescription?.DateAmharic;

    //                    var patientCardDetail = db.PatientCardDetail.FirstOrDefault(x=> x.PatientId == card.PatientId && x.Date == prescription.Date);
    //                    if(patientCardDetail != null)
    //                    {
    //                        clinic.Diagnosis = patientCardDetail.DiagnosisTratment;
    //                    }

    //                    var referalReferal = db.ReferralSheet.FirstOrDefault(x => x.PatientId == prescription.PatientId);
    //                    if (referalReferal != null)
    //                    {
    //                        clinic.ReferalDate = referalReferal.DateAmharic;
    //                    }

    //                    clinicList.Add(clinic);
    //                }


    //            }



    //        }


    //        return clinicList;
    //    }
    //    public void Dispose()
    //    {
    //        db.Dispose();
    //    }
    //}
}

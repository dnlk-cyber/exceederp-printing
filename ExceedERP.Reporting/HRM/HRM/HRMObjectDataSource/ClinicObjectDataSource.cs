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
    public class ClinicObjectDataSource : List<ClinicViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ClinicViewModel> GetClinicReport(string branch, string empId, DateTime from, DateTime to)
        {
            List<ClinicViewModel> clinicList = new List<ClinicViewModel>();

            var prescripitons = db.Prescription.ToList();
            var placements = db.Placements.Where(x => x.isCurrent).ToList();
            var employees = db.Person_Employee.Where(x => x.IsActive).ToList();
            var patientCard = db.PatientCard.ToList();



            if (from != null && to != null)
            {
                if (from <= to)
                {
                    prescripitons = prescripitons.Where(x => x.Date >= from && x.Date <= to).ToList();
                }
            }
            if (branch != null)
            {
                int branchId = int.Parse(branch);
                placements = placements.Where(x => x.BranchId == branchId).ToList();
            }


            if (empId != null)
            {
                patientCard = patientCard.Where(x => x.EmployeeId.ToString() == empId).ToList();
            }

            foreach (var prescription in prescripitons)
            {
                ClinicViewModel clinic = new ClinicViewModel();
                var card = patientCard.FirstOrDefault(x => x.PatientId == prescription.PatientId);
                if (card != null)
                {
                    var place = placements.FirstOrDefault(x => x.EmployeeId == card.EmployeeId);
                    if (place != null)
                    {
                        ///////////////////////////

                        var employee = employees.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
                        if (employee != null)
                        {
                            clinic.EmployeeName = employee.FullNameaEnglish;
                            clinic.EmployeeCode = employee.EmployeeCode;
                            clinic.Age = DateTime.Today.Year - employee.DateOfBirth.Year;
                            clinic.Gender = employee.Gender.ToString();
                        }


                        var dept = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                        if (dept != null)
                        {
                            clinic.Workunit = dept.Name;
                        }

                        var bra = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                        if (bra != null)
                        {
                            clinic.Branch = bra.Name;
                        }
                        clinic.TreatedDate = prescription?.DateAmharic;

                        var patientCardDetail = db.PatientCardDetail.FirstOrDefault(x=> x.PatientId == card.PatientId && x.Date == prescription.Date);
                        if(patientCardDetail != null)
                        {
                            clinic.Diagnosis = patientCardDetail.DiagnosisTratment;
                        }

                        var referalReferal = db.ReferralSheet.FirstOrDefault(x => x.PatientId == prescription.PatientId);
                        if (referalReferal != null)
                        {
                            clinic.ReferalDate = referalReferal.DateAmharic;
                        }

                        clinicList.Add(clinic);
                    }


                }



            }


            #region commented
            //if (branch == null)
            //{

            //    foreach (var prescription in prescripitons)
            //    {
            //        ClinicViewModel clinic = new ClinicViewModel();

            //        var card = patientCard.FirstOrDefault(x => x.PatientId == prescription.PatientId);
            //        if (card != null)
            //        {
            //            var employee = employees.FirstOrDefault(x => x.EmployeeId == card.EmployeeId);
            //            if (employee != null)
            //            {
            //                clinic.EmployeeName = employee.FullNameaEnglish;
            //                clinic.EmployeeCode = employee.EmployeeCode;
            //            }

            //            var placement = placements.FirstOrDefault(x => x.EmployeeId == card.EmployeeId && x.isCurrent);
            //            if (placement != null)
            //            {


            //                var dept = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
            //                if (dept != null)
            //                {
            //                    clinic.Workunit = dept.Name;
            //                }

            //                var bra = db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
            //                if (bra != null)
            //                {
            //                    clinic.Branch = bra.Name;
            //                }

            //            }
            //            clinic.TreatedDate = prescription?.DateAmharic;
            //            var referalReferal = db.ReferralSheet.FirstOrDefault(x => x.PatientId == prescription.PatientId);
            //            if (referalReferal != null)
            //            {
            //                clinic.ReferalDate = referalReferal.DateAmharic;
            //            }

            //            clinicList.Add(clinic);
            //        }




            //    }

            //}
            #endregion
            return clinicList;
        }
    }
}

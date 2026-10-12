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
    public class LaboratoryRequestFormObjectDataSource:IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LaboratoryRequestCatagoryVM> GetComplain(string patientCardDetailId)
        {
            var result = new List<LaboratoryRequestCatagoryVM>();

            if (string.IsNullOrWhiteSpace(patientCardDetailId))
                return result;
            int.TryParse(patientCardDetailId, out int id);
            var patientCardDetail = db.PatientCardDetail
                .FirstOrDefault(x => x.PatientCardDetailId == id);

            if (patientCardDetail == null)
                return result;

            var categories = db.LaboratoryCatagoryPatientCards
                .Where(x =>
                    x.PatientId == patientCardDetail.PatientId ||
                    x.PatientCardDetailId == patientCardDetail.PatientCardDetailId)
                .ToList();

            foreach (var category in categories)
            {
                var categoryVm = new LaboratoryRequestCatagoryVM();

                var categoryEntity = db.LaboratoryCatagories
                    .FirstOrDefault(x => x.LaboratoryCatagoryId.ToString() == category.Catagory);

                categoryVm.Catagory = categoryEntity?.Catagory;

                var tests = db.LaboratoryTestPatientCards
                    .Where(x =>
                        x.PatientId == category.PatientId &&
                        x.PatientCardDetailId == category.PatientCardDetailId &&
                        x.LaboratoryCatagoryPatientCardId == category.LaboratoryCatagoryPatientCardId)
                    .ToList();

                foreach (var testItem in tests)
                {
                    var testEntity = db.LaboratoryTests
                        .FirstOrDefault(x => x.LaboratoryTestsId.ToString() == testItem.Test);

                    categoryVm.LaboratoryRequestTests.Add(new LaboratoryRequestTestsVM
                    {
                        Test = testEntity?.Test,
                        Reference = testItem.Reference,
                        Result = testItem.Result
                    });
                }

                result.Add(categoryVm);
            }

            return result;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}

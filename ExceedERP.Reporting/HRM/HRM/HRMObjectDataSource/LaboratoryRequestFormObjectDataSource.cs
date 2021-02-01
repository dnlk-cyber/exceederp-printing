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
    public class LaboratoryRequestFormObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LaboratoryRequestCatagoryVM> GetComplain(string id)
        {
            List<LaboratoryRequestCatagoryVM> requestLists = new List<LaboratoryRequestCatagoryVM>();

            //var patientCard = db.PatientCard.FirstOrDefault(x => x.PatientId.ToString() == id);

           // DateTime dateRequest = new DateTime(date.Year, date.Month, date.Day);

            LaboratoryRequestCatagoryVM labCat = new LaboratoryRequestCatagoryVM();
            LaboratoryRequestTestsVM test = new LaboratoryRequestTestsVM();

            if (id != null)
            {

                var patientCardDetail = db.PatientCardDetail.FirstOrDefault(x => x.PatientCardDetailId.ToString() == id);

                if(patientCardDetail != null)
                {
                    var catagories = db.LaboratoryCatagoryPatientCards.Where(x=>x.PatientId == patientCardDetail.PatientId && x.PatientCardDetailId == patientCardDetail.PatientCardDetailId).ToList();
                    foreach(var cat in catagories)
                    {
                        labCat = new LaboratoryRequestCatagoryVM();
                        labCat.LaboratoryRequestTests = new List<LaboratoryRequestTestsVM>();

                        var catag = db.LaboratoryCatagories.FirstOrDefault(x=>x.LaboratoryCatagoryId.ToString() == cat.Catagory);
                        if(catag != null)
                        {
                            labCat.Catagory = catag.Catagory;
                        }                                               

                        if (cat != null)
                        {
                            var tests = db.LaboratoryTestPatientCards.Where(x => x.PatientId == cat.PatientId && x.PatientCardDetailId == cat.PatientCardDetailId && x.LaboratoryCatagoryPatientCardId == cat.LaboratoryCatagoryPatientCardId).ToList();

                            foreach (var tt in tests)
                            {
                                test = new LaboratoryRequestTestsVM();

                                var tes = db.LaboratoryTests.FirstOrDefault(x=>x.LaboratoryTestsId.ToString() == tt.Test);
                                if(tes != null)
                                {
                                    test.Test = tes.Test;
                                }
                              
                                test.Reference = tt.Reference;
                                test.Result = tt.Result;

                                labCat.LaboratoryRequestTests.Add(test);
                            }
                        }

                        requestLists.Add(labCat);
                    }

                }
            }

            return requestLists;
        }
    }
}

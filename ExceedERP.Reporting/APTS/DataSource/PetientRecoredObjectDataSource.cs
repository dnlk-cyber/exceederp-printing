using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.APTS.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.APTS.DataSource
{
   
    [DataObject]
    public class PetientRecoredObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PatientRecotedViewModel> GetDrugStore(DateTime from, DateTime to)
        {
            List<PatientRecotedViewModel> patientLists = new List<PatientRecotedViewModel>();

            var patients = db.PatientRegisters.ToList();

            foreach(var patient in patients)
            {
              var  patientRecord = new PatientRecotedViewModel();

                //var drugList = db.DrugLists.FirstOrDefault(x=>x.DrugListsId == patient.DrugListsId);
                //if(drugList != null)
                //{
                //    patientRecord.PatientName = patient.PatientName;
                //    patientRecord.DrugCode = drugList.DrugCode;
                //    patientRecord.DrugName = drugList.Description;
                //    patientRecord.CardNo = patient.CardNumber;
                //    patientRecord.Age = patient.Age;
                //    patientRecord.Gender = patient.Sex.ToString();

                //    patientLists.Add(patientRecord);
                //}
       
            }
            return patientLists;

        }
    }
}
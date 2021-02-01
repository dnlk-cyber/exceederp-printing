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
    public class DrugSaleWithDrugistObjectDatasource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DrugSaleWithDrugistViewModel> GetDrugStore(bool isCredit, DateTime from, DateTime to)
        {
            List<DrugSaleWithDrugistViewModel> drugSaleLists = new List<DrugSaleWithDrugistViewModel>();

            var drugSales = db.PatientRegisters.ToList();

            if (isCredit)
            {
                drugSales = drugSales.Where(x => x.IsCredit).ToList();
            }
            if (from != null && to != null)
            {
                drugSales = drugSales.Where(x => x.RegisterDate >= from && x.RegisterDate <= to).ToList();
            }

            decimal totalPrice =0;
            decimal totalQuantity =0;

            var employees = db.Person_Employee.Where(x=>x.IsActive).ToList();
            foreach (var emp in employees)
            {
             
                drugSales = drugSales.Where(x=>x.DrugistId == emp.EmployeeId).ToList();

                if (drugSales.Any())
                {
                    var drug = new DrugSaleWithDrugistViewModel();

                    drug.DrugistName = emp.FullNameaEnglish;
                    foreach(var drugSale in drugSales)
                    {
                      
                        var patientDetails = db.PatientRegisterDetails.Where(x => x.PatientRegisterId == drugSale.PatientRegisterId).ToList();
                        if(patientDetails.Any())
                        {
                            totalPrice = totalPrice + patientDetails.Sum(x => x.TotalRetailPrice);
                            totalQuantity = totalQuantity + patientDetails.Sum(x => x.TotalQty);
                        }                        
                      

                    }

                    drug.NoOfPatient = drugSales.Count();
                    drug.Price = totalPrice;
                    drug.Quantity = totalQuantity;

                    drugSaleLists.Add(drug);
                }
            }
            

            return drugSaleLists;
        }
    }
}

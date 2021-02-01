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
    public class DrugSaleObjectDatasource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DrugSaleViewModel> GetDrugStore(bool isCredit, DateTime from, DateTime to)
        {
            List<DrugSaleViewModel> drugSaleLists = new List<DrugSaleViewModel>();

            var drugSales = db.PatientRegisters.ToList();

            if(isCredit)
            {
                drugSales = drugSales.Where(x=>x.IsCredit).ToList();
            }
            if(from != null && to != null)
            {
                drugSales = drugSales.Where(x => x.RegisterDate >= from && x.RegisterDate <= to).ToList();
            }

            foreach(var drugSale in drugSales)
            {
                var drug = new DrugSaleViewModel();
                drug.DrugSaleDetailViewModel = new List<DrugSaleDetailViewModel>();

                var employee = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == drugSale.EmployeeId);
                if(employee != null)
                {
                    drug.ClientName = employee.FullNameaEnglish;
                }

              
                drug.HospitalName = drugSale.NameOfOrg;
                drug.CardNo = drugSale.CardNumber;
                drug.Sex = drugSale.Sex.ToString();
                drug.Age = drugSale.Age;

                var soldDrugs = db.PatientRegisterDetails.Where(x=>x.PatientRegisterId == drugSale.PatientRegisterId);

                foreach(var soldDrug in soldDrugs)
                {
                    var sold = new DrugSaleDetailViewModel();

                    sold.DrugCode = soldDrug.InternalCode;

                    var drugName = db.DrugLists.FirstOrDefault(x => x.DrugListsId == soldDrug.DrugListsId);
                    if (drugName != null)
                    {
                        sold.Description = drugName.Description;
                    }
                    var unitName = db.DrugUnits.FirstOrDefault(x => x.DrugUnitId == soldDrug.DrugUnitId);
                    if (unitName != null)
                    {
                        sold.Unit = unitName.UnitName;
                    }
                    sold.TotalQty = soldDrug.TotalQty;
                    sold.UnitPrice = soldDrug.RetailPrice;
                    sold.TotalPrice = soldDrug.TotalRetailPrice;

                    drug.DrugSaleDetailViewModel.Add(sold);
                }
                

                drugSaleLists.Add(drug);
            }
            return drugSaleLists;
        }
    }
}

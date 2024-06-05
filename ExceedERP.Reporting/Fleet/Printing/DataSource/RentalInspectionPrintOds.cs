using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.Printing.DataSource
{
    [DataObject]
    public class RentalInspectionPrintOds
    {
        private ExceedDbContext db = new ExceedDbContext();
        
        [DataObjectMethod(DataObjectMethodType.Select)]

        public RentalInspectionViewModel GetRentalInspection(string Id)
        {
            var rentalInspection = db.RentalInspections.Find(Id);
            RentalInspectionViewModel rentalInspectionViewModel = new RentalInspectionViewModel();
            rentalInspectionViewModel = new RentalInspectionViewModel
            {
                Capacity = rentalInspection.Capacity,
                //BranchId = rentalInspection.BranchId,
                ChasisSerialNo = rentalInspection.ChasisSerialNo,
                EngineModel = rentalInspection.EngineModel,
                EngineSerialNo = rentalInspection.EngineSerialNo,
                //EquipmentNameID = rentalInspection.EquipmentNameID,
                //EquipmentTypeID = rentalInspection.EquipmentTypeID,
                Hp = rentalInspection.Hp,
                Lessor = rentalInspection.Lessor,
                Manufacturer = rentalInspection.Manufacturer,
                ManufacturingYear = rentalInspection.ManufacturingYear,
                MeterReading = rentalInspection.MeterReading,
                Model = rentalInspection.Model,
                OperatorLicenceNo = rentalInspection.OperatorLicenceNo,
                PlateNo = rentalInspection.PlateNo,
                
            };
            return rentalInspectionViewModel;

        }
        public class RentalInspectionViewModel
        {
            [DisplayName("Equipment Type")]
            
            public string EquipmentType { get; set; }
           
            [DisplayName("Equipment Name")]
            public string EquipmentName { get; set; }
           
            [DisplayName("Model")]
            public string Model { get; set; }
            
            [DisplayName("Chasis/Serial No.")]
            public string ChasisSerialNo { get; set; }
            
            [DisplayName("Engine Model")]
            public string EngineModel { get; set; }
            
            [DisplayName("Engine Serial No.")]
            public string EngineSerialNo { get; set; }
           
            [DisplayName("Year of Manufacture")]
            public string ManufacturingYear { get; set; }
           
            [DisplayName("Capacity")]
            public string Capacity { get; set; }
            public string Manufacturer { get; set; }
           
            [DisplayName("HP")]
            public decimal Hp { get; set; }
       
            [DisplayName("Operator's License No.")]
            public string OperatorLicenceNo { get; set; }
           
            [DisplayName("Lessor")]
            public string Lessor { get; set; }
           
            [DisplayName("Plate No.")]
            public string PlateNo { get; set; }
          
            [DisplayName("Branch")]

            public string Branch { get; set; }
           
            [DisplayName("Hr./Km Meter Reading")]
            public string MeterReading { get; set; }
        }
    }
}

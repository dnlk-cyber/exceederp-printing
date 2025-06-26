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
            RentalInspectionViewModel rentalInspectionViewModel = new RentalInspectionViewModel();
            if (int.TryParse(Id, out int id))
            {
                var rentalInspection = db.RentalInspections.FirstOrDefault(x => x.RentalInspectionId == id);

                rentalInspectionViewModel = new RentalInspectionViewModel
                {
                    Capacity = rentalInspection.Capacity,
                    ChasisSerialNo = rentalInspection.ChasisSerialNo,
                    EngineModel = rentalInspection.EngineModel,
                    EngineSerialNo = rentalInspection.EngineSerialNo,
                   
                    Hp = rentalInspection.Hp,
                    Lessor = rentalInspection.Lessor,
                    Manufacturer = rentalInspection.Manufacturer,
                    ManufacturingYear = rentalInspection.ManufacturingYear,
                    MeterReading = rentalInspection.MeterReading,
                    Model = rentalInspection.Model,
                    OperatorLicenceNo = rentalInspection.OperatorLicenceNo,
                    PlateNo = rentalInspection.PlateNo,
                    OperatorName = rentalInspection.OperatorName,
                    EquipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == rentalInspection.EquipmentTypeID)?.EquipmentTypeName,
                    EquipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == rentalInspection.EquipmentNameID)?.Name,
                    Branch = db.Branch.FirstOrDefault(x => x.BranchId == rentalInspection.BranchId)?.Name,
                    CCKW = rentalInspection.CCKW
                };
            }
           
            return rentalInspectionViewModel;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RentalInspectionViewModel> GetEquipments(string rentId)
        {
            List<RentalInspectionViewModel> list = new List<RentalInspectionViewModel>();
            if (int.TryParse(rentId, out int id))
            {
                var rentalInspection = db.RentalInspections.FirstOrDefault(x => x.RentalInspectionId == id);
                
                if (rentalInspection != null)
                {
                    var equips = db.RentalMVInspections.Where(x => x.RentalInspectionId == rentalInspection.RentalInspectionId);
                    foreach (var item in equips)
                    {

                        list.Add(new RentalInspectionViewModel
                        {
                            CriteriaDetail = db.RentalInspectionCriteriaDetails.Find(item.RentalInspectionCriteriaDetailId)?.CriteriaName,
                            BranchOfficeGoodCondtion = item.BranchOfficeGoodCondtion.ToString(),
                            BranchOfficePoorCondtion = item.BranchOfficePoorCondtion.ToString(),
                            HeadOfficeGoodCondtion = item.HeadOfficeGoodCondtion.ToString(),
                            HeadOfficePoorCondtion = item.HeadOfficePoorCondtion.ToString(),
                        });
                    }
                }
            }
            return list;

        }
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
            public string OperatorName { get; set; }
            public string CCKW { get; set; }
        [DisplayName("Technical Paramenter")]
        public string TechnicalParameter { get; set; }
        [DisplayName("Criterea Detail")]
        public string CriteriaDetail { get; set; }
        [DisplayName("Branch Office Good Condition")]
        public string BranchOfficeGoodCondtion { get; set; }
        [DisplayName("Branch Office Poor Condition")]
        public string BranchOfficePoorCondtion { get; set; }
        [DisplayName("Head Office Good Condition")]
        public string HeadOfficeGoodCondtion { get; set; }
        [DisplayName("Head Office Poor Condition")]
        public string HeadOfficePoorCondtion { get; set; }
       
        
       
        }
    }


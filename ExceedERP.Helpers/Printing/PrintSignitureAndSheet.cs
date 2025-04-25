using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Printing
{
    public static class PrintSignitureAndSheet
    {
        private static readonly ExceedDbContext db = new ExceedDbContext();

        public static decimal PrintSigniture(PrintingCostEstimation estimation)
        {            
            var machineType = db.PrintingMachineTypes
                .FirstOrDefault(x => x.PrintingMachineTypeId == estimation.TextMachineTypeId); // text machine
            var machineDetail = db.PrintingMachineDetails
                .FirstOrDefault(x => x.PrintingMachineTypeId == machineType.PrintingMachineTypeId &&
                x.PaperSizeId == estimation.TextPaperSizeId); // text paper
           
            return (estimation.ProductQuantity * estimation.Pages * estimation.TextNoOfColors) /
                (machineDetail.SectionUp * machineType.NumberOfColors * estimation.TextPrintSide);            
        }

        public static decimal PrintSheet(PrintingCostEstimation estimation)
        {
            var machineType = db.PrintingMachineTypes
                .FirstOrDefault(x => x.PrintingMachineTypeId == estimation.CoverMachineTypeId); // cover machine
            var machineDetail = db.PrintingMachineDetails
                .FirstOrDefault(x => x.PrintingMachineTypeId == machineType.PrintingMachineTypeId &&
                x.PaperSizeId == estimation.CoverPaperSizeId); // cover paper
            
            return (estimation.ProductQuantity * estimation.CoverNoOfColor) /
                (machineDetail.SectionUp * machineType.NumberOfColors);           
        }
    }
}

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
            if (machineType == null)
                return 0;
            var machineDetail = db.PrintingMachineDetails
                .FirstOrDefault(x => x.PrintingMachineTypeId == machineType.PrintingMachineTypeId &&
                x.PaperSizeId == estimation.TextPaperSizeId); // text paper
            if (machineDetail == null)
                return 0;
            return (estimation.ProductQuantity * estimation.Pages * estimation.TextNoOfColors) /
                (machineDetail.SectionUp * machineType.NumberOfColors * estimation.TextPrintSide);            
        }

        public static decimal PrintSheetCover(PrintingCostEstimation estimation)
        {
            var machineType = db.PrintingMachineTypes
                .FirstOrDefault(x => x.PrintingMachineTypeId == estimation.CoverMachineTypeId); // cover machine
            if (machineType == null)
                return 0;
            var machineDetail = db.PrintingMachineDetails
                .FirstOrDefault(x => x.PrintingMachineTypeId == machineType.PrintingMachineTypeId &&
                x.PaperSizeId == estimation.CoverPaperSizeId); // cover paper
            if (machineDetail == null)
                return 0;
            return (estimation.ProductQuantity * (int)estimation.CoverNoOfColor) /
                (machineDetail.SectionUp * machineType.NumberOfColors);           
        }
        public static decimal PrintSheetText(PrintingCostEstimation estimation)
        {
            var machineType = db.PrintingMachineTypes
                .FirstOrDefault(x => x.PrintingMachineTypeId == estimation.TextMachineTypeId); // text machine
            if (machineType == null)
                return 0;
            var machineDetail = db.PrintingMachineDetails
                .FirstOrDefault(x => x.PrintingMachineTypeId == machineType.PrintingMachineTypeId &&
                x.PaperSizeId == estimation.TextPaperSizeId); // text paper
            if (machineDetail == null)
                return 0;
            return (estimation.ProductQuantity * (int)estimation.CoverNoOfColor) /
                (machineDetail.SectionUp * machineType.NumberOfColors);
        }
    }
}

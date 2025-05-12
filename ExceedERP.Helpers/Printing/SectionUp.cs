using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Printing
{
    public static class SectionUp
    {
        readonly static private ExceedDbContext db = new ExceedDbContext();

        public static void A(PrintingPaperSize paperSize, int machineId)
        {
            var entity = new PrintingMachineDetail
            {
                PaperSizeId = paperSize.PrintingPaperSizeId,
                PrintingMachineTypeId = machineId,
                SectionUp = 0
            };
            db.PrintingMachineDetails.Add(entity);
            db.SaveChanges();
        }

        public static int B(PrintPaperSize printPaperSize)
        {
            switch (printPaperSize)
            {
                case PrintPaperSize.A1:
                    return 1;                    
                case PrintPaperSize.A2:
                    return 2;
                case PrintPaperSize.A3:
                    return 4;
                case PrintPaperSize.A4:
                    return 8;
                case PrintPaperSize.A5:
                    return 16;
                case PrintPaperSize.A6:
                    return 32;
                case PrintPaperSize.B1:
                    
                case PrintPaperSize.B2:
                    break;
                case PrintPaperSize.B3:
                    break;
                case PrintPaperSize.B4:
                    break;
                case PrintPaperSize.B5:
                    break;
                case PrintPaperSize.B6:
                    break;
                default:
                    break;
            }
            return 1;
        }
    }
}

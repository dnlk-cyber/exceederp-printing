using ExceedERP.Core.Domain.Printing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
namespace ExceedERP.Helpers.Printing
{
    public static class PaperSizeHelper
    {

        private static readonly Dictionary<PrintPaperSize, int> PaperRanks = new Dictionary<PrintPaperSize, int>()
        {

            { PrintPaperSize.A1, 1 },
            { PrintPaperSize.A2, 2 },
            { PrintPaperSize.A3, 3 },
            { PrintPaperSize.A4, 4 },
            { PrintPaperSize.A5, 5 },
            { PrintPaperSize.A6, 6 },
            { PrintPaperSize.B1, 1 },
            { PrintPaperSize.B2, 2 },
            { PrintPaperSize.B3, 3 },
            { PrintPaperSize.B4, 4 },
            { PrintPaperSize.B5, 5 },
            { PrintPaperSize.B6, 6 },
        };

        public static double? GetPaperUpRatio(PrintPaperSize fromSize, PrintPaperSize toSize)
        {
            // Only compare within same series (e.g., A to A, B to B)
            if (fromSize.ToString()[0] != toSize.ToString()[0])
                return null;

            if (!PaperRanks.ContainsKey(fromSize) || !PaperRanks.ContainsKey(toSize))
                return null;

            int fromRank = PaperRanks[fromSize];
            int toRank = PaperRanks[toSize];
            int diff = toRank - fromRank;

            return Math.Pow(2, diff);
        }

    }

}

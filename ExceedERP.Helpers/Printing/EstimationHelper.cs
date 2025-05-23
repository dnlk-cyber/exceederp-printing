using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Printing
{
    public static class EstimationHelper
    {
        public static decimal TextPlate(decimal page, decimal quantity, decimal color, decimal sectionUp, decimal fade, decimal wastage, out decimal result)
        {
            int[] allowedSizes = { 8, 4, 2 };
            var x = Math.Ceiling(quantity / (fade));
            result = ((page * color) / sectionUp);
            int wholePlates = (int)Math.Floor(result);
            var a = wholePlates * sectionUp;
            var remainingPage = page - a;
            List<int> list = BreakDownToValidSizes((int)remainingPage, allowedSizes);
            int no = list.Count() * 2;


            result = wholePlates + no;
            if(result % 2 != 0)
            {
                result++;
            }
            result = result * x;
            result = Math.Ceiling(result * (1 + (wastage / 100)));
            return result;
        }
        private static List<int> BreakDownToValidSizes(int remainingPages, int[] allowedSizes)
        {
            var breakdown = new List<int>();
            Array.Sort(allowedSizes);
            Array.Reverse(allowedSizes); // Descending order

            while (remainingPages > 0)//12
            {
                foreach (var size in allowedSizes)//[2,4,8]
                {
                    if (remainingPages >= size)
                    {
                        breakdown.Add(size);//[2,4]
                        remainingPages -= size;
                        break;
                    }
                }
            }

            return breakdown;
        }
        public static int RoundUpToNextMultipleOf4(int pages)
        {
            return (int)(Math.Ceiling(pages / 4.0) * 4);
        }

    }
}

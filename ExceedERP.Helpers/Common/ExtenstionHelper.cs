using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Common
{
    public static class ExtenstionHelper
    {

        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> list)
        {
            return Task.Run(() => list.ToList());
        }
        public static decimal DivideByZeroHelper(this decimal Value)
        {
            if (Value == decimal.Zero)
            {
                return decimal.One;
            }
            return Value;
        }

    }
}

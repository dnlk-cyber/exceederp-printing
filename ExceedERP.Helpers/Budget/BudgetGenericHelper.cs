using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Budget
{
 
    public static  class BudgetGenericHelper
    {

        
        public static decimal GetItemByIndexOfRange(this List<RatioModel> list,  int index)
        {
            
                try
                {

                    return list[index].Actual;
                }
                catch (Exception e)
                {
                    return decimal.Zero;
                }
            

        }
        public static RatioModel GetDefault(this RatioModel ratio)
        {

            try
            {

                if (ratio == null)
                    return new RatioModel() { Actual = decimal.Zero, Budget = decimal.Zero, Name = string.Empty, Type = string.Empty };
                else
                    return ratio;
            }
            catch (Exception e)
            {
                return new RatioModel() { Actual = decimal.Zero, Budget = decimal.Zero, Name = string.Empty, Type = string.Empty };
            }


        }



         
        public static string GetYearByIndexOfRange(this List<RatioModel> list, int index)
        {

            try
            {

                return list[index].FiscalYear;
            }
            catch (Exception e)
            {
                return string.Empty;
            }


        }
        public static string GetItemByIndexOfRange(this List<string> list, int index)
        {

           
                try
                {

                    return list[index];
                }
                catch (Exception e)
                {
                    return string.Empty;
                }
            
        }
        public static int GetDynamicSegment(DynamicSegmentTypes dynamicSegmentTypes, List<GLSegmentSetup> glSegmentSetups)
        {

            var glSegmentSetup = glSegmentSetups.Where(x => x.Enabled).OrderBy(x => x.Sequence).FirstOrDefault(x => x.SegmentType == dynamicSegmentTypes);
            if (glSegmentSetup != null)
            {

                return glSegmentSetup.Sequence;
            }

            return 0;


        }

        public static string GetSegment(this string data, int index)
        {
            try
            {
                return data.Split('-')[index].ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static decimal DivideByZero(decimal Value)
        {
            if(Value == decimal.Zero)
            {
                return decimal.One;
            }
            return Value;
        }

       


    }

}

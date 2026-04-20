using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAPI
{
    public static class IncomeTax
    {
        public static double GetIncomeTax(float sal)
        {
            double tax = 0.0;

            if (sal <= 2000)
            {
                tax = 0;
            }
            else if (sal <= 4000)
            {
                tax = sal * 0.15 - 300;
            }
            else if (sal <= 7000)
            {
                tax = sal * 0.20 - 500;
            }
            else if (sal <= 10000)
            {
                tax = sal * 0.25 - 850;
            }
            else if (sal <= 14000)
            {
                tax = sal * 0.30 - 1350;
            }
            else // sal > 14000
            {
                tax = sal * 0.35 - 2050;
            }

            return tax;
        }
        public static double GetGrossAmount(float totaltaxable, float net)
        {
            double grossAmount = 0;

            double tax = GetIncomeTax(totaltaxable);
            double net1 = totaltaxable - tax;
            double totalNet = net1 + net;

            if (totalNet <= 2000)
            {
                grossAmount = totalNet;
            }
            else if (totalNet <= 3700) // (4000 * 0.85 + 300)
            {
                grossAmount = (totalNet - 300) / 0.85;
            }
            else if (totalNet <= 6100) // (7000 * 0.80 + 500)
            {
                grossAmount = (totalNet - 500) / 0.80;
            }
            else if (totalNet <= 8650) // (10,000 * 0.75 + 850)
            {
                grossAmount = (totalNet - 850) / 0.75;
            }
            else if (totalNet <= 11150) // (14,000 * 0.70 + 1350)
            {
                grossAmount = (totalNet - 1350) / 0.70;
            }
            else // > 11150
            {
                grossAmount = (totalNet - 2050) / 0.65;
            }

            return grossAmount;
        }


        //public static double GetIncomeTax_recentold(float sal)
        //{
        //    double tax = 0.0f;
        //    if (sal <= 600)
        //    {
        //        tax = 0;
        //    }
        //    else if (sal > 600 && sal <= 1650)
        //    {
        //        tax = sal * 0.1 - 60;
        //    }
        //    else if (sal > 1650 && sal <= 3200)
        //    {
        //        tax = sal * 0.15 - 142.50;
        //    }
        //    else if (sal > 3200 && sal <= 5250)
        //    {
        //        tax = sal * 0.2 - 302.50;
        //    }
        //    else if (sal > 5250 && sal <= 7800)
        //    {
        //        tax = sal * 0.25 - 565;
        //    }
        //    else if (sal > 7800 && sal <= 10900)
        //    {
        //        tax = sal * 0.30 - 955;
        //    }
        //    else
        //    {
        //        tax = sal * 0.35 - 1500;

        //    }
        //    return tax;
        //}

        //public static double GetGrossAmount_recentold(float totaltaxable, float net)
        //{
        //    double grossAmount = 0;
           
        //    double tax= GetIncomeTax(totaltaxable);
        //    double net1 = totaltaxable-tax;
        //    double totalNet = net1 + net;

        //    if (totalNet <= 600)
        //        grossAmount = totalNet;
        //    else if(totalNet<= 1545)
        //    {
        //        grossAmount = (totalNet-60)/0.9;
        //    }
        //    else if (totalNet <= 2862)
        //    {
        //        grossAmount = (totalNet - 142.5) / 0.85;
        //    }
        //    else if (totalNet <= 4502)
        //    {
        //        grossAmount = (totalNet - 302.5) / 0.8;
        //    }
        //    else if (totalNet <= 6415)
        //    {
        //        grossAmount = (totalNet - 565) / 0.75;
        //    }
        //    else if (totalNet <= 8585)
        //    {
        //        grossAmount = (totalNet - 955) / 0.70;
        //    }

        //    else if (totalNet > 8585)
        //    {
        //        grossAmount = (totalNet - 1500) / 0.65;
        //    }

        //    return grossAmount;
        //}
        //public static double GetGrossAmount_old(float totaltaxable,float net)
        //{
        //    double grossAmount = 0;
        //    double taxDiff = 0;
        //    double taxDiffBeforeTax = 0;
        //    taxDiff = GetIncomeTax(totaltaxable + net) - GetIncomeTax(totaltaxable);
        //    grossAmount = net + taxDiff;

        //    //determing tax of the difference
        //    double total = totaltaxable + net + taxDiff;
        //    if(total>10900)
        //    {
        //        taxDiffBeforeTax = taxDiff / 0.65;
        //    }
        //    else if(total > 7800 && total <= 10900)
        //    {
        //        taxDiffBeforeTax = taxDiff / 0.7;
        //    }
        //    else if (total > 5250 && total <= 7800)
        //    {
        //        taxDiffBeforeTax = taxDiff / 0.75;
        //    }

        //    else if (total > 3200 && total <= 5250)
        //    {
        //        taxDiffBeforeTax = taxDiff / 0.8;
        //    }

        //    else if (total > 1650 && total <= 3200)
        //    {
        //        taxDiffBeforeTax = taxDiff / 0.85;
        //    }
        //    else if (total > 600 && total <= 1650)
        //    {
        //        taxDiffBeforeTax = taxDiff / 0.9;
        //    }
        //    else
        //    {
        //        taxDiffBeforeTax = taxDiff;
        //    }

        //    grossAmount = net + taxDiffBeforeTax;

        //    return grossAmount;
        //}

        //public static double GetIncomeTax_old(float sal)
        //{
        //    double tax = 0.0f;
        //    if (sal <= 150)
        //    {
        //        tax = 0;

        //    }
        //    else if (sal >= 151 && sal <= 650.99)
        //    {

        //        tax = sal * 0.1 - 15;

        //    }
        //    else if (sal >= 651 && sal <= 1400.99)
        //    {

        //        tax = sal * 0.15 - 47.50;
        //    }
        //    else if (sal >= 1401 && sal <= 2350.99)
        //    {

        //        tax = sal * 0.2 - 117.50;
        //    }
        //    else if (sal >= 2351 && sal <= 3550.99)
        //    {

        //        tax = sal * 0.25 - 235;
        //    }
        //    else if (sal >= 3551 && sal <= 5000.99)
        //    {

        //        tax = sal * 0.30 - 412.50;
        //    }
        //    else
        //    {

        //        tax = sal * 0.35 - 662.50;
        //    }
        //    return tax;
        //}
    }
}

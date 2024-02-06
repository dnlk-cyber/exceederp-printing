 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using System.Web.Mvc;

namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
    public enum Furniture_Category
    {
        Printing = 1 ,
        Finishing = 2,
    }
    public enum FurnitureSub_Category
    {
        Offset = 1,
        LeterPress = 2,
    }
    public enum FurnitureJob_Type
    {
        Book = 1,
        Magazin = 2,
        Label = 3,
        Commerical = 4,
        Secure = 5
    }

    public enum FurnitureProductionDepartment
    {
        JobFollowup = 0,
        Graphic = 1,
        Printing = 2,
        Finishing = 3,
        FinishingDelivery = 4,
        SalesDelivery = 5
    }

    public enum FurnitureCheckType
    {
         Start = 1,
        Finish = 2
    }
    public enum FurnitureCompleted_Job_Type
    {
        ColaMekebat = 1,
        Duges = 2,
        Mabeter = 3,
        Magatem = 4,
        Makebele = 5,
        Maketatel = 6,
        Malebese = 7,
        Masere = 8,
        Mashet = 9,
        Matefe = 10,
        Mebeten = 11,
        Mebolek = 12,
        Mebsate = 13,
        Medereder = 14,
        Mekfele = 15,
        Mekurete = 16,
        Mekuter = 17,
        Meleyete = 18,
        Memekise = 19,
        Memeles = 20,
        Menetele = 21,
        Mesbere = 22,
        Mesebseb = 23,
        Meshenshine = 24,
        Mesifate = 25,
        Metegen = 26,
        Metektek = 27,
        Meterege = 28,
        Metereze = 29,
        Mezerzere = 30,
        Miremera = 31,
        Pak = 32,
        ShiboMasgebat = 33,
        ShiboMekurete = 34,
        ShiboMetemtem = 35,
        Sibeter = 36,
        Strich = 37,
        TelaMeletefe = 38
    }

    public enum FurnitureCodeOfTime
    {
        Normal = 1,
        OverTime = 2
    }

    public enum Cost_Center
    {
        Binding = 1,
        Contract = 2 
    }
    public enum FurnitureCost_Center2
    {
        Manual = 3,
        Mechanical = 4,
        LetterPress = 5,
        Offset = 6,
        Web = 7
    }
    public enum FurnitureCost_Center3
    {
        Camera = 8,
        Computer = 9
    }
    public enum FurnitureCompletedJobType
    {
        Typing = 1,
        Scanning = 2,
        PlateMaking = 3,
        Layout = 4,
        Stripping = 5,
        Dane = 6,
        Designing = 7,
        ColorPrint = 8
    }
    public enum FurnitureProductionFollowup
    {
        PrePrinting,
        Printing,
        Finishing
    }

    public static class Extensions
    {
        public static List<SelectListItem> EnumToSelectList(Type enumType)
        {
            return Enum
              .GetValues(enumType)
              .Cast<int>()
              .Select(i => new SelectListItem
              {
                  Value = i.ToString(),
                  Text = Enum.GetName(enumType, i),
              }
              )
              .ToList();
        }
    }
}

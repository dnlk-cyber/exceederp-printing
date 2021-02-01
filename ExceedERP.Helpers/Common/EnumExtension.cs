using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Helpers.Common
{
    public class EnumExtension
    {
        
        public static List<SelectListItem> EnumToSelectList(Type enumType)
        {
            return Enum.GetValues(enumType).Cast<int>()
                                          .Select(i => new SelectListItem
                                              {
                                                  Value = i.ToString(),
                                                  Text = Enum.GetName(enumType, i),
                                              }
                                            ).ToList();
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

    }
}

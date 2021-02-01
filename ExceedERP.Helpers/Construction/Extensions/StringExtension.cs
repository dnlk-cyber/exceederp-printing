using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Construction.Extensions
{
    public static class StringExtension
    {
        public static string ReplaceLineBreaksWithSpaces(this String str)
        {
            return str.Replace(System.Environment.NewLine, " ");
        }
        public static string RemoveUnnecessaryCharactors(this String str)
        {
            str = str.Replace(System.Environment.NewLine, " ");
            return str.Replace('"', ' ').Replace('#', ' ').Trim();
        }
    }
}

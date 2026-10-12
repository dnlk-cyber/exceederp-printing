using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class LanguageODS
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetLanguages()
        {
            List<string> languages = new List<string>();
            languages.Add("English");
            languages.Add("Local");
            //var languages = Enum.GetValues(typeof(LetterLang))
            //               .Cast<LetterLang>()
            //               .Select(v => v.ToString())
            //               .ToList();

            return languages;
        }
    }
}

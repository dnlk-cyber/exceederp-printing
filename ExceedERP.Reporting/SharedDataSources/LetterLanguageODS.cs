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
    public class LetterLanguageODS
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetLetterLanguages()
        {
            var languages = Enum.GetValues(typeof(LetterLang))
                           .Cast<LetterLang>()
                           .Select(v => v.ToString())
                           .ToList();

            return languages;
        }
    }
}

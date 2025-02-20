using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Core.Localization;

namespace ExceedERP.Helpers.InfoDesk
{
    public static class InfoDeskHelper
    {
        
        public static string CapitalizeEacgWordFirstLetter(string word)
        {
            string capitalizedWord = "";
            var spliteWords = word.ToLower().Split(' ');

            for (int i = 0; i < spliteWords.Count(); i++)
            {
                var wordArray = spliteWords[i].ToCharArray();
                var firstLetter = wordArray[0];
                var capital = firstLetter.ToString().ToUpper();
                for (int j = 1; j < wordArray.Count(); j++)
                {
                    capital = capital + wordArray[j];
                }

                capitalizedWord = capitalizedWord + " " + capital;
            }
            return capitalizedWord;
        }

        /// New

        


    }
}

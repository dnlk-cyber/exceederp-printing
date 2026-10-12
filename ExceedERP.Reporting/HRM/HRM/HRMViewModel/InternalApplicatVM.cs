using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class InternalApplicatVM
    {
        public string VacancyCode { get; set; }
        public string VacancyType { get; set; }
        public string Gender { get; set; }
        public decimal ExpeienceNeeded { get; set; }
        public string ApplicantFullName { get; set; }
        public string RelatedJobTitles { get; set; }
        public decimal TotalRelatedExperience { get; set; }
        public decimal RelatedRate { get; set; }
        public string RelavantJobTitles { get; set; }
        public decimal RelavantRate { get; set; }
        public decimal TotalRelavanceExperience { get; set; }
        public string NonRelaetedJobTitle { get; set; }
        public string NonRelaetedRates { get; set; }
        public decimal TotalNonRelaetedExperience { get; set; }
        public decimal TotalExperience { get; set; }
        public IList<InternalApplicatDetailVM> internalApplicatDetails   { get; set; }

    }
    public class InternalApplicatDetailVM
    {
        public string ApplicantFullName { get; set; }
        public string RelatedJobTitles { get; set; }
        public decimal TotalRelatedExperience { get; set; }
        public decimal RelatedRate { get; set; }
        public string RelavantJobTitles { get; set; }
        public decimal RelavantRate { get; set; }
        public decimal TotalRelavanceExperience { get; set; }
        public string NonRelaetedJobTitle { get; set; }
        public string NonRelaetedRates { get; set; }
        public decimal TotalNonRelaetedExperience { get; set; }
        public decimal TotalExperience { get; set; }

    }
}

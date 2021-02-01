using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    class GeleralViewModels
    {
    }
    /// <summary>
    /// this view model is used to populate the single selector
    /// dropdown list in the report viewers ...
    /// </summary>
    public class ProjectViewModel
    {
        public string Name { get; set; }
        public int ProjectInformationId { get; set; }
    }
    public class ProjectTypeViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
    public class SubProjectTypeViewModel
    {
        public SubProjectTypeViewModel()
        {
            SubSubProjectList = new List<SubSubProjectViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SubSubProjectViewModel> SubSubProjectList { get; set; }
    }
    /// <summary>
    /// Blocks, or Roads ....
    /// </summary>
    public class SubSubProjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class SubPeriodViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    /// <summary>
    /// view model for project headers
    /// </summary>

    public class ProjectHeaderVM : Organization
    {
        public string ProjectName { get; set; }
        public string Client { get; set; }
        public string Consultant { get; set; }
        public string Location { get; set; }
    }
    // 
    public class StructureVM
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
    public class WorkCategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
    public class EvaluationSummaryHeaderVM : Organization
    {
        public string EvaluationPeriod { get; set; }
    }
    public class ProjectTypeHeaderVM : Organization
    {
        public string TypeName { get; set; }
    }
    public class YearlyGraphicalReportHederVM : Organization
    {
        public string ProjectName { get; set; }
        public string Year { get; set; }

    }

    public class ReportYearViewModel
    {
        public string Title { get; set; }
    }
    public class ProjectBlockVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BranchViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

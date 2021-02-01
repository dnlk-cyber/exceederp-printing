using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptRecruitmentSummary : Telerik.Reporting.Report
    {
        public rptRecruitmentSummary()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = HeaderObjectDataSource;
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            var cultures = new CultureInfo(culture);

            this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
          
            //this.Criteria.Value = Resources.ResourceManager.GetString("Rpt_Criteria", cultures);
          
            //this.EducLevel.Value = Resources.ResourceManager.GetString("Rpt_LevelofEducation", cultures);
            //this.EdutaionTyp.Value = Resources.ResourceManager.GetString("Rpt_FieldofEducation", cultures);
            //this.EmploymentType.Value = Resources.ResourceManager.GetString("Rpt_EmployementType", cultures);
           
            //this.JobTitleName.Value = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
            //this.Location.Value = Resources.ResourceManager.GetString("Rpt_Location", cultures);
            //this.MinSalary.Value = Resources.ResourceManager.GetString("Rpt_MinSalary", cultures);
            //this.MembName.Value = Resources.ResourceManager.GetString("Rpt_MemberName", cultures);
         
            //this.Quantity.Value = Resources.ResourceManager.GetString("Rpt_Quantity", cultures);
            //this.RankLevel.Value = Resources.ResourceManager.GetString("Rpt_RankLevel", cultures);
          
            this.Signuture.Value = Resources.ResourceManager.GetString("Rpt_Signuture", cultures);
            this.ApplicantName.Value = Resources.ResourceManager.GetString("Rpt_ApplicantName", cultures);
            this.Gender.Value = Resources.ResourceManager.GetString("Rpt_From", cultures);
            this.RelatedExp.Value = Resources.ResourceManager.GetString("Rpt_RelatedExperience", cultures);
            this.Relevant.Value = Resources.ResourceManager.GetString("Rpt_RelevantExperience", cultures);
            this.Discipline.Value = Resources.ResourceManager.GetString("Rpt_Discipline", cultures);
            this.Interview.Value = Resources.ResourceManager.GetString("Rpt_Interview", cultures);
            this.Disability.Value = Resources.ResourceManager.GetString("Rpt_Disability", cultures);
            this.Profile.Value = Resources.ResourceManager.GetString("Rpt_Profile", cultures);
            this.Training.Value = Resources.ResourceManager.GetString("Rpt_Training", cultures);
            this.TheoryExam.Value = Resources.ResourceManager.GetString("Rpt_TheoryExam", cultures);
            this.PracticalExam.Value = Resources.ResourceManager.GetString("Rpt_PracticalExam", cultures);
            this.Total.Value = Resources.ResourceManager.GetString("Rpt_Total", cultures);
            this.Result.Value = Resources.ResourceManager.GetString("Rpt_Result", cultures);
            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "orgStructureId");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            var reportParameter = this.ReportParameters.FirstOrDefault(x => x.Name == "from");
            if (reportParameter != null)
                reportParameter.Text = Resources.ResourceManager.GetString("Rpt_From", cultures);
            var orDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "to");
            if (orDefault != null)
                orDefault.Text = Resources.ResourceManager.GetString("Rpt_To", cultures);
        }
    }
}
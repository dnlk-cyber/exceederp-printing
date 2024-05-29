using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptExternalApplicantList : Telerik.Reporting.Report
    {
        public rptExternalApplicantList()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = HeaderObjectDataSource;
            var culture = "am-ET";//System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            var cultures = new CultureInfo(culture);

            this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.No.Value = Resources.ResourceManager.GetString("Rpt_No", cultures);
            //this.Criteria.Value = Resources.ResourceManager.GetString("Rpt_Criteria", cultures);
            this.EJobTitle.Value = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
            this.Eduaction.Value = Resources.ResourceManager.GetString("Rpt_EduactionBack", cultures);
            this.EducLevel.Value = Resources.ResourceManager.GetString("Rpt_LevelofEducation", cultures);
            this.EdutaionTyp.Value = Resources.ResourceManager.GetString("Rpt_FieldofEducation", cultures);
            //this.EmploymentType.Value = Resources.ResourceManager.GetString("Rpt_EmployementType", cultures);
            this.EndDate.Value = Resources.ResourceManager.GetString("Rpt_EndDate", cultures);
            this.Expriance.Value = Resources.ResourceManager.GetString("Rpt_Expriance", cultures);
            this.From.Value = Resources.ResourceManager.GetString("Rpt_From", cultures);
            //this.JobTitleName.Value = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
            //this.Location.Value = Resources.ResourceManager.GetString("Rpt_Location", cultures);
            //this.MinSalary.Value = Resources.ResourceManager.GetString("Rpt_MinSalary", cultures);
            this.MembName.Value = Resources.ResourceManager.GetString("Rpt_MemberName", cultures);
            this.OrgName.Value = Resources.ResourceManager.GetString("Rpt_InstitutionName", cultures);
            //this.Quantity.Value = Resources.ResourceManager.GetString("Rpt_Quantity", cultures);
            //this.RankLevel.Value = Resources.ResourceManager.GetString("Rpt_RankLevel", cultures);
            this.Remark.Value = Resources.ResourceManager.GetString("Rpt_Remark", cultures);
            this.Signuture.Value = Resources.ResourceManager.GetString("Rpt_Signuture", cultures);
            this.TEducLevel.Value = Resources.ResourceManager.GetString("Rpt_LevelofEducation", cultures);
            this.TEducType.Value = Resources.ResourceManager.GetString("Rpt_FieldofEducation", cultures);
            this.TotalExpriance.Value = Resources.ResourceManager.GetString("Rpt_TotalExpriance", cultures);
            this.To.Value = Resources.ResourceManager.GetString("Rpt_To", cultures);
            this.TExpriance.Value = Resources.ResourceManager.GetString("Rpt_TotalExpriance", cultures);
            this.ApplicantName.Value = Resources.ResourceManager.GetString("Rpt_ApplicantName", cultures);
           // this.TEOrgName.Value = Resources.ResourceManager.GetString("Rpt_InstitutionName", cultures);
            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "orgStructureId");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            var reportParameter = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            if (reportParameter != null)
                reportParameter.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);
            var orDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "jobTitle");
            if (orDefault != null)
                orDefault.Text = Resources.ResourceManager.GetString("Rpt_JobTitle", cultures);
            var orDefaul = this.ReportParameters.FirstOrDefault(x => x.Name == "vacancyCod");
            if (orDefaul != null)
                orDefaul.Text = Resources.ResourceManager.GetString("Rpt_VacancyMediaCode", cultures);
            var mediaCode = this.ReportParameters.FirstOrDefault(x=>x.Name=="vacancyId");
            if(mediaCode!=null)
                mediaCode.Text= Resources.ResourceManager.GetString("Rpt_VacancyCode", cultures);
        }
    }
}
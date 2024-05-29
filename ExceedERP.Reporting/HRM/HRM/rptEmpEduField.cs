using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptEmpEduField : Telerik.Reporting.Report
    {
        public rptEmpEduField()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            var cultures = new CultureInfo(culture);
            this.Date.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.NoOfFemaleEmployee.Value = Resources.ResourceManager.GetString("Rpt_NoOfFemaleEmployee", cultures);
            this.NoOfMaleEmployee.Value = Resources.ResourceManager.GetString("Rpt_NoOfMaleEmployee", cultures);
            this.Total.Value = Resources.ResourceManager.GetString("Rpt_Total", cultures);
            this.TotalEmployee.Value = Resources.ResourceManager.GetString("Rpt_TotalEmployee", cultures);
            this.ByEducationalField.Value = Resources.ResourceManager.GetString("Rpt_ByEducationalField", cultures);
            this.FieldofEducation.Value = Resources.ResourceManager.GetString("Rpt_FieldofEducation", cultures);
            var parameter = this.ReportParameters.FirstOrDefault(x => x.Name == "eduField");
            if (parameter != null)
                parameter.Text = Resources.ResourceManager.GetString("Rpt_FieldofEducation", cultures);
            var param = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            if (param != null)
                param.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);
            var para = this.ReportParameters.FirstOrDefault(x => x.Name == "orgStructureId");
            if (para != null)
                para.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);
        }
    }
}
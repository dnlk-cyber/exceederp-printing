using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for EmployeeBirthDate.
    /// </summary>
    public partial class EmployeeBirthDate : Telerik.Reporting.Report
    {
        public EmployeeBirthDate()
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
         this.IdNumber.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
         this.FullName .Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
         this.Sex .Value = Resources.ResourceManager.GetString("Rpt_Sex", cultures);
         this.Age.Value = Resources.ResourceManager.GetString("Rpt_Age", cultures);
         this.Department.Value = Resources.ResourceManager.GetString("Rpt_Department", cultures);
         this.EmpByAge.Value = Resources.ResourceManager.GetString("Rpt_EmpByAge", cultures);
            var firstOrDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "orgStructureId");
            if (firstOrDefault != null)
                firstOrDefault.Text = Resources.ResourceManager.GetString("Rpt_Department", cultures);
            var reportParameter = this.ReportParameters.FirstOrDefault(x => x.Name == "branchId");
            if (reportParameter != null)
                reportParameter.Text = Resources.ResourceManager.GetString("Rpt_Branch", cultures);
            var orDefault = this.ReportParameters.FirstOrDefault(x => x.Name == "agemin");
            if (orDefault != null)
                orDefault.Text = Resources.ResourceManager.GetString("Rpt_Minumum", cultures);
            var parameter = this.ReportParameters.FirstOrDefault(x => x.Name == "agemax");
            if (parameter != null)
                parameter.Text = Resources.ResourceManager.GetString("Rpt_Maximum", cultures);
            this.Page .Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.Date.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
        }
       
    }
}
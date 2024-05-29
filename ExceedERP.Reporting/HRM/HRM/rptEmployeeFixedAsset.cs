using System.Globalization;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptEmployeeFixedAsset : Telerik.Reporting.Report
    {
        public rptEmployeeFixedAsset()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = EmployeeFixedAssetObjectDataSource;//Organization;
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            var cultures = new CultureInfo(culture);
            this.Date.Value = Resources.ResourceManager.GetString("Rpt_Date", cultures);
            this.Page.Value = Resources.ResourceManager.GetString("Rpt_Page", cultures);
            this.FixedAssTitle.Value = Resources.ResourceManager.GetString("Rpt_FixedAssTitle", cultures);
            //this.IdNumber.Value = Resources.ResourceManager.GetString("Rpt_IDNumber", cultures);
            //this.FullName.Value = Resources.ResourceManager.GetString("Rpt_Name", cultures);
            //this.objectDetail.Value = Resources.ResourceManager.GetString("Rpt_objectDetail", cultures);
            //this.objectName.Value = Resources.ResourceManager.GetString("Rpt_objectName", cultures);
            //this.Quan.Value = Resources.ResourceManager.GetString("Rpt_Quantity", cultures);
            //this.UnitP.Value = Resources.ResourceManager.GetString("Rpt_UnitPrice", cultures);
            //this.Code.Value = Resources.ResourceManager.GetString("Rpt_Code", cultures);
            //this.GivenDate.Value = Resources.ResourceManager.GetString("Rpt_GivenDate", cultures);
        }
    }
}
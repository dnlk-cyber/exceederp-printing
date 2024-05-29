using System.Globalization;
using System.Linq;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class TrainingRequest : Telerik.Reporting.Report
    {
        public TrainingRequest()
        {
           // var cultureInfo = new System.Globalization.CultureInfo("am");

           
            //
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
          
          
           
        }
       
    }
}
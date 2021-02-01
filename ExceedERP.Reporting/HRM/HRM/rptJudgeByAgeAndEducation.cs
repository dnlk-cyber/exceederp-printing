using System.Globalization;
namespace ExceedERP.Reporting.HRM.HRM
{
    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class rptJudgeByAgeAndEducation : Telerik.Reporting.Report
    {
        public rptJudgeByAgeAndEducation()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
    
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.DataSource = Organization;
        }
       
    }
}
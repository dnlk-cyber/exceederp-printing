using System.Linq;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Inventoryy
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for BinCard.
    /// </summary>
    public partial class BinCard : Telerik.Reporting.Report
    {
        ExceedDbContext  db = new ExceedDbContext();

        public BinCard()
        {
            //
            // Required for telerik Reporting designer support
            //
            
            InitializeComponent();
            this.DataSource = BinCardHeader;
            table1.DataSource = binCardDs;

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }
    }
}
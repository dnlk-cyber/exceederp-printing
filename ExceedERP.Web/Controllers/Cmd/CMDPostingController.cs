using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Web.Controllers.Cmd
{
    public partial class CmdController : Controller
    {
        private AccountHelper accountHelper = new AccountHelper();
        // GET: CMD

        public ActionResult xyy()
        {
           
            return View("Success");
        }

        public ActionResult AccDesc()
        {
            
            var lines = db.GLRecordJournalDetails.Where(x => x.AccountDesc == "").ToList();
            foreach (var line in lines)
            {
                line.AccountDesc = accountHelper.GetAccountDesc(line.AccountCode);
                db.Entry(line).State = EntityState.Modified;
                db.SaveChanges();
            }
            
            return View("Success");
        }
    }
}
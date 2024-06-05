//using ExceedERP.DataAccess.Context;
//using ExceedERP.Reporting.Fleet.ViewModel;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ExceedERP.Reporting.Fleet.DataSource
//{
//    [DataObject]
//    public class EquipmentReportTestDataSource
//    {

//        ExceedDbContext db = new ExceedDbContext();

//        [DataObjectMethod(DataObjectMethodType.Select)]
//        public List<EquipmentReportTestViewModel> GetBook(string Category, string auditor, string equipmentName)
//        {
//            var bookList = new List<EquipmentReportTestViewModel>();
//            var books = db.Books.ToList();
//            if(Category != null)
//            {
//                int.TryParse(Category, out int categoryId);
//                books = books.Where(x => x.CategoryId == categoryId).ToList();
//            }
//            foreach( var book in books)
//            {
//                var bookItem = new EquipmentReportTestViewModel();
//                bookItem.equipmentName = book.Title;
//                var bookCategory = db.Categories.Where(x => x.CategoryId == book.CategoryId).FirstOrDefault();
//                if(bookCategory != null)
//                {
//                    bookItem.equipmentCategory = bookCategory.CatName;

//                }
//                bookItem.equipmentType = book.ISBN;
//                bookList.Add(bookItem);
//            }
//            return bookList;
//        }

//    }
//}


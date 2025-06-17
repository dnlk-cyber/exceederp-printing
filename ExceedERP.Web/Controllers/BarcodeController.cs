using ExceedERP.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Controllers
{
    public class BarcodeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string isbn)
        {
            ViewBag.ISBN = isbn;

            if (string.IsNullOrWhiteSpace(isbn))
            {
                ViewBag.Error = "ISBN cannot be empty.";
                return View();
            }

            if (!BarcodeGenerator.IsValidISBN13(isbn))
            {
                ViewBag.Error = "Invalid ISBN-13 format.";
                return View();
            }

            ViewBag.BarcodeImage = BarcodeGenerator.GenerateISBNBarcodeBase64(isbn);
            return View();
        }

        [HttpGet]
        public ActionResult Download(string isbn)
        {
            if (!BarcodeGenerator.IsValidISBN13(isbn))
            {
                ViewBag.Error = "Invalid ISBN.";
                return View("Error");
            }

            using (var bitmap = BarcodeGenerator.GenerateISBNBarcode(isbn))
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return File(ms.ToArray(), "image/png", $"ISBN_{isbn}.png");
            }
        }
    }
}
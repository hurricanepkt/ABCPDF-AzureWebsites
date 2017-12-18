using System;
using System.Configuration;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebSupergoo.ABCpdf11;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var screen = System.Windows.Forms.Screen.PrimaryScreen;
            ViewBag.Height = screen.Bounds.Height;
            ViewBag.Width = screen.Bounds.Width;
            ViewBag.Control = "/Home/Control";
            ViewBag.Test = "/Home/Test";
            ViewBag.Razor = "/Home/Razor";
            ViewBag.Gecko = "/Home/Gecko";
            ViewBag.Chrome = "/Home/Chrome";
            ViewBag.Description = "Live Test";
            ViewBag.LinkDest = "/Home/Reference";
            ViewBag.Link = "See what it should look like";
            return View();

        }

     

        

        public ActionResult Reference()
        {
            ViewBag.Control = "/Content/Reference/Control.pdf";
            ViewBag.Test = "/Content/Reference/IE.pdf";
            ViewBag.Razor = "/Content/Reference/RazorRender.pdf";
            ViewBag.Gecko = "/Content/Reference/GeckoFetch.pdf";
            ViewBag.Chrome = "/Content/Reference/Chrome.pdf";
            ViewBag.Description = "Reference PDFs";
            ViewBag.LinkDest = "/Home/Index";
            ViewBag.Link = "See what it does look like";
            return View("Index");
        }


        public void Control()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["LicenseKey"].ConnectionString;
            XSettings.InstallLicense(connectionString);
            var theDoc = new Doc { FontSize = 96 };

            theDoc.AddText("Control");
            Response.ContentType = "application/pdf";
            theDoc.Save(Response.OutputStream);
            theDoc.Clear();
            theDoc.Dispose();
        }

        public void Test()
        {
            CreatePDFFetched(EngineType.MSHtml);
        }
        public void Gecko()
        {
            
            CreatePDFFetched(EngineType.Gecko);
        }

        public void Chrome()
        {
            CreatePDFFetched(EngineType.Chrome);
        }

        public ActionResult Printers()
        {
            ViewBag.Printer = Printerinfo();
            return View();
        }
        public ActionResult RazorPDF()
        {
            return View();
        }
        public ActionResult Razor()
        {
            // Return view if there is an error

            // If user has clicked the export button

            // Render view output to string
            var report = RenderViewToString(this, "RazorPDF", new { Name = "Bill Gates" }, "_Print");

            // Convert string to PDF using ABCpdf
            var pdfbytes = PDFForHtml(report);

            // Return file result
            Response.AddHeader("Content-Disposition", "inline; filename=\"Razor.pdf\"");
            return File(pdfbytes, "application/pdf");
            //Response.ContentType = "application/pdf";
            //Response.BinaryWrite(pdfbytes);

        }

        private string Printerinfo()
        {
            var retval = "Printers -<br /><table>";
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                var printer = new PrinterSettings { PrinterName = printerName };
                retval += String.Format("<tr><td>{0}</td><td>{1}</td><td>", printerName, printer.IsDefaultPrinter);
                foreach (var printerResolution in printer.PrinterResolutions)
                {
                    retval += String.Format("{0}<br />", printerResolution.ToString());
                }
                retval += "</td></tr>";
            }
            retval += "</table>Thats all printers <br />";
            return retval;
        }
        public static string RenderViewToString(Controller controller, string viewName, object model, string masterName)
        {
            controller.ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(controller.ControllerContext, viewName, masterName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public async Task<ActionResult> Info()
        {
            //const string url = "http://www.mybrowserinfo.com/detail.asp?bhcp=1";
            //var client = new HttpClient();
            //var get = await client.GetAsync(url);
            //var str = await get.Content.ReadAsStringAsync();
            //return Content(str, "text/html");
            var sb = new StringBuilder();
            sb.Append("[");
            OperatingSystem os = Environment.OSVersion;
            sb.Append(JsonConvert.SerializeObject(os));
            sb.Append(",{\"s\" : \"" + os.VersionString + "\"}");
            sb.Append("]");
            return Content(sb.ToString(),"application/json");
        }
        public static byte[] PDFForHtml(string html)
        {
            // Create ABCpdf Doc object
            var doc = new Doc();
            doc.HtmlOptions.Engine = EngineType.Gecko;
            doc.HtmlOptions.ForGecko.ProcessOptions.LoadUserProfile = true;
            doc.HtmlOptions.HostWebBrowser = true;
            doc.HtmlOptions.BrowserWidth = 800;
            doc.HtmlOptions.ForGecko.InitialWidth = 800;
            // Add html to Doc
            int theID = doc.AddImageHtml(html);

            // Loop through document to create multi-page PDF
            while (true)
            {
                if (!doc.Chainable(theID))
                    break;
                doc.Page = doc.AddPage();
                theID = doc.AddImageToChain(theID);

            }

            // Flatten the PDF
            for (int i = 1; i <= doc.PageCount; i++)
            {
                doc.PageNumber = i;
                doc.Flatten();
            }

            // Get PDF as byte array. Couls also use .Save() to save to disk
            var pdfbytes = doc.GetData();

            doc.Clear();

            return pdfbytes;
        }
        private void CreatePDFFetched(EngineType engine)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["LicenseKey"].ConnectionString;
            XSettings.InstallLicense(connectionString);
            var theDoc = new Doc();
            theDoc.HtmlOptions.Engine = engine;
            theDoc.HtmlOptions.UseScript = false;
            theDoc.HtmlOptions.BrowserWidth = 800;
            theDoc.HtmlOptions.ForGecko.UseScript = false;
            theDoc.Rendering.DotsPerInch = 300;
            theDoc.HtmlOptions.ForGecko.InitialWidth = 800;
            theDoc.Rect.Inset(18, 18);
            theDoc.Page = theDoc.AddPage();
            int theID = theDoc.AddImageUrl("http://woot.com/", true, 800, true);
            while (true)
            {
                theDoc.FrameRect(); // add a black border
                if (!theDoc.Chainable(theID))
                    break;
                theDoc.Page = theDoc.AddPage();
                theID = theDoc.AddImageToChain(theID);
            }
            for (int i = 1; i <= theDoc.PageCount; i++)
            {
                theDoc.PageNumber = i;
                theDoc.Flatten();
            }
            Response.Buffer = false;
            Response.AddHeader("Content-Disposition", "inline; filename=\"rept.pdf\"");
            Response.ContentType = "application/pdf";
            theDoc.Save(Response.OutputStream);
            Response.Flush();
        }
    }
}

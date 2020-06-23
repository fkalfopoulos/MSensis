using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts; 
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSensis.Models;
using MSensis.Services;
using MSensis.ViewModels;
 
namespace MSensis.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MSensisController : ControllerBase
    {
         
        private readonly MSensisContext _db;
        private readonly UserManager<User> _manager;
        private IConverter _converter;
      
        private readonly ILogger<MSensisController> _logger;
        private readonly IHostingEnvironment _env;
        



        public MSensisController(MSensisContext _db, UserManager<User> manager, IConverter converter,  
            ILogger<MSensisController> logger, IHostingEnvironment env )
        {
            this._db = _db;
            _manager = manager;
            _converter = converter; 
            _logger =logger;
            _env = env;
           
        }

       

        [HttpPost]
            public bool Delete_Invoice(string id)
            {
            Invoice i = _db.Invoices.SingleOrDefault(u => u.Id == id);

            Pdf p = _db.Pdfs.SingleOrDefault(u => u.Invoice.Id == id);
            try
            {
                if (p != null && i != null)
                {
                    _db.Remove(p);
                    _db.Remove(i);
                    _db.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            }

            return false;
            }

            [HttpPost]
            public bool Delete_Client(string id)
            {
            Client i = _db.Clients.SingleOrDefault(u => u.Id == id);
            if( i != null)
            {
                _db.Remove(i);
                _db.SaveChangesAsync();
                return true;
            }
             return false;

             }

        [HttpPost]
        public bool Delete_Company(string id)
        {
            Company i = _db.Companies.SingleOrDefault(u => u.Id == id);
            try
            {
                if (i != null)
                {
                    _db.Remove(i);
                    _db.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            }
            return false;
        }


        public async Task<IActionResult> Issue_Invoice(string id)
        {
            var invoice = _db.Invoices.SingleOrDefault(u => u.Id == id);

            try
            {
                using (var db = new MSensisContext())
                {
                    invoice.Invoice_Type = "Issued";
                    await _db.SaveChangesAsync();
                    return RedirectToAction("IssuedPdfs", "Home");
                }
            }
             
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            } 
            return BadRequest("badboy");
       }


        public async Task<IActionResult> Ιnvoice_del(string id)
        {
            var invoice = _db.Invoices.SingleOrDefault(u => u.Id == id);

            try
            {
                using (var db = new MSensisContext())
                {
                    invoice.Invoice_Type = "Deleted";
                    await _db.SaveChangesAsync();
                    return RedirectToAction("IssuedPdfs", "Home");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            }
            return BadRequest("badboy");
        }



        public async Task<IActionResult> Download(string id)
        {
            Generator pdf = new Generator(_db, _env);
            return await pdf.CreatePDF(id);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePDF(string id)
        { 
             
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = "PDF Report",
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    Page =  $"https://localhost:44389/Home/Pdf/{id}",
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "Services", "styles222.css") },
                    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                    FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" },

                };
             
            _logger.LogWarning($"{objectSettings.Page}"); 
                var pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                }; 
              
            string url = HttpContext.Request.Path;
            _logger.LogWarning($"{url}");

            var date = DateTime.Now;

            var name = await _db.Pdfs.Where(u => u.Id == id).Select(u => u.Client.CompanyName).SingleOrDefaultAsync();
            if (name == null)
            {
                name = $"PDF{DateTime.Now}";
            }

            var contentType = "application/pdf";
            var fileName = $"{name}-{date.Month}-{date.Year}-{date.Second}.pdf";

            byte[] file = _converter.Convert(pdf);
               //this writes pdf to disk
                using (FileStream stream = new FileStream(@"Files\" + DateTime.UtcNow.Ticks.ToString() + ".pdf", FileMode.Create))
                {
                        stream.Write(file, 0, file.Length);
                 }
	        return new FileContentResult(file, contentType);
        }

       

      
         

        public PDFViewModel SearchByMonth(int months)
        {

            var DateList2 = _db.Pdfs.Where(p => p.Timestamp >= DateTime.Today.AddMonths(-months)).Include(u => u.Invoice)
                .ToList();

            string name = _db.Pdfs.Where(p => p.Timestamp >= DateTime.Today.AddMonths(-months)).Include(u => u.Client).Select(u => u.Client.CompanyName).SingleOrDefault();

            PDFViewModel model = new PDFViewModel()
            {
                DataPdf = DateList2,
                CompanyName = name

            };

            return model;
        } 

        public async Task<IActionResult> LoadCompany([FromBody]UserViewModel model)
        {
            User user = await _manager.GetUserAsync(HttpContext.User);
            Company company = _db.Companies.Where(c => c.Id == model.CompanyId).SingleOrDefault();
            return Ok(company);
        }

         
    }
        public class TestData
        {
            public string Text { get; set; }
            public int Number { get; set; }
        }
    }
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSensis.Models;
using MSensis.Services;
using MSensis.ViewModels;
using WkWrap.Core;

namespace MSensis.Controllers
{
    public class AccountController : Controller
    {

        private readonly MSensisContext _db;
        private readonly UserManager<User> _manager;
        private readonly FileManager _filemanager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IHostingEnvironment _env;


        public AccountController(MSensisContext _db, UserManager<User> manager, FileManager filemanager, SignInManager<User> signInManager,
           ILogger<AccountController> logger,    IHostingEnvironment  env )
        {
            this._db = _db;
            _manager = manager;
            _filemanager = filemanager;
            _signInManager = signInManager;
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }



        [ViewLayout("_Panel")]
        public async Task<IActionResult> Panel()
        {

            ViewBag.Current = "Home"; 
            GetClientPage(); 

            return View();
        }

        [ViewLayout("Empty")]
        public async Task<IActionResult> Profile()
        {
             
            return View();
        }

        public  void GetClientPage()
        { 

            List<Invoice> invoiceList = null;
            int paidCount = 0;
            int unpaidCount = 0;
            int conceptCount = 0;
            int finalCount = 0;

            try {
                //Get all invoices
                invoiceList = _db.Invoices.ToList();

                paidCount = invoiceList.Where(inv => inv.Payment_Status == "Paid" && inv.Invoice_Type == "Issued").Count();
                unpaidCount = invoiceList.Where(inv => inv.Payment_Status == "Unpaid" && inv.Invoice_Type == "Issued").Count();
                conceptCount = invoiceList.Where(inv => inv.Invoice_Type == "Draft").Count();
                finalCount = invoiceList.Where(inv => inv.Invoice_Type == "Issued").Count();

                //Variables for chart 1
                ViewBag.conceptCount = conceptCount;
                ViewBag.finalCount = finalCount;

                //Variables for chart 2
                ViewBag.adminPaidCount = paidCount;
                ViewBag.adminNotPaidCount = unpaidCount;

                ViewBag.total = CalculateTotal(invoiceList);
                ViewBag.totalPaid = CalculatePaid(invoiceList);
                ViewBag.totalNotPaid = CalculateToBePaid(invoiceList);
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }
     

        //Calculate the amount to be paid
        private decimal CalculateToBePaid(List<Invoice> invoices)
{
                decimal total = 0;
    
                foreach (var invoice in invoices)
                {
                    if (invoice.Invoice_Type == "Issued" && invoice.Payment_Status == "Paid")
                    {
                         
                    }
                }
                return total;
            }

            //Calculate the amount of already paid invoices
            private decimal CalculatePaid(List<Invoice> invoices)
            {
                            decimal total = 0;

                            foreach (var invoice in invoices)
                            {
                                if (invoice.Invoice_Type == "Issued" && invoice.Payment_Status == "Paid")
                                {
                                    
                                }
                            }
                            return total;
                        }

            //Calculate the total amount of all invoices
                                private decimal CalculateTotal(List<Invoice> invoices)
                                {
                                    decimal total = 0;

                                    foreach (var invoice in invoices)
                                    {
                                        if (invoice.Invoice_Type == "Issued")
                                        {
                                            
                                        }
                                    }
                                    return total;
                                }



            [ViewLayout("_Administrator")]
                    public ActionResult Create_Company()
                    {
                        return View();
                    }




        [HttpPost]
        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Create_Company(CompanyViewModel model)
        {
            User user = await _manager.GetUserAsync(HttpContext.User);
            string FilePath = await _filemanager.SaveImage(model.Company_Logo);

            Guid identifier = Guid.NewGuid();

            Company newcompany = new Company
            {
                Name = model.Name,
                Address = model.Address,
                City = model.City,
                TelephoneNumber = model.TelephoneNumber,
                Code = model.Code,
                User = user,
                ImageSrc = FilePath,
                Timestamp = DateTime.Now,
                IdentifierId = identifier
            };
            _db.Companies.Add(newcompany);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");

        }       


        [ViewLayout("_Administrator")]
        public ActionResult Edit_Product(string id)
        {
            return View(_db.Products.SingleOrDefault(c => c.Id == id));
        }

        [HttpPost]
        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Edit_Product(InvoiceViewModel model)
        {
            try
            {
                Product c = await _db.Products.SingleOrDefaultAsync(u => u.Id == model.Id);

                c.Code = model.Invoice_Code;
                c.Name = model.Name;
                c.Quantity = model.Invoice_Quantity; 
                c.PricePerUnit = model.PricePerUnit;
                c.Vat = model.Invoice_VAT;
                c.Discount = model.Discount; 
                c.Absolute_value = model.PricePerUnit;

                _db.Entry(await _db.Products.FirstOrDefaultAsync(x => x.Id == c.Id)).CurrentValues.SetValues(c);
                await _db.SaveChangesAsync();

                return RedirectToPage($"/Home/ViewProducts/{model.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"the reeson is {ex}");
                return BadRequest();
            }
        }


        public async  Task<IActionResult> Download(string id)
        {
            Generator pdf = new Generator(_db, _env);
            return await pdf.CreatePDF(id);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePDF(string id)
        {


            Pdf pdf = _db.Pdfs.Include(u => u.Invoice)
               .Include(u => u.Client)
               .Include(u => u.Company)
               .Where(c => c.Id == id).SingleOrDefault();

            List<Product> productList = await _db.Products.Where(a => a.Pdf.Id == pdf.Id).ToListAsync();
            var cssString = @"<style>
                .invoice-box {
                    max-width: 2480px;
                    max-height: 3495px:
                    width: 100%;
                    height: 90%;
                    margin: auto 0;
                    padding: 30px 7px;
                    font-size: 15px;
                    line-height: 24px;
                    font-family: 'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;
                    color: #555;
                    letter-spacing: 0px;
                }
                .invoice-box table {
                    width: 100%;
                    line-height: inherit;
                    text-align: left;
                }
                .invoice-box table td {
                    padding: 5px 0px;
                    vertical-align: top;
                }
                .invoice-box table tr.top table td {
                    padding-bottom: 50px;
                }
                .invoice-box table .top td .title {
                    font-size: 32px;
                    line-height: 32px;
                    padding-left: 65px !important;
                    color: #333;
                    float: left;
                    width: 80%;
                    text-align: left;
                }
                .invoice-box table .top td .logo {
                    padding-left: 0px !important;
                    float: left;
                    max-width: 65%;
                    height: auto;
                    text-align: left;
                }
                .invoice-box table .top .company {
                    float: right;
                    font-size: 13.5px;
                    text-align: left !important;
                    width: 30%;
                    align: right !important;
                    margin-right: 0;
                    padding-right: 10px;
                    margin-left: 10px;
                }
                .invoice-box table .top .company hr{
                    margin-left: auto;
                    margin-right: auto;
                    width: 100%;
                    height: 2px;
                    margin-top: 2px;
                    margin-bottom: 0;
                    padding-bottom: 1px;
                }
                .invoice-box .debtor-table{
                    margin-left: 55px !important;
                    margin-bottom: 45px !important;
                }
                .invoice-box .debtor-table .debtor-info{
                    border-collapse: collapse;
                    border-spacing; 0;
                    padding: 0 0 0 0;
                    margin: 0 0 0 15px;
                }
                .invoice-box .debtor-table .debtor-info tr{
                    padding: 0 0 0 0;
                    margin: 0 0 0 0;
                }
                .invoice-box .debtor-table .debtor-name,
                .invoice-box .debtor-table .debtor-address,
                .invoice-box .debtor-table .debtor-city{
                    font-size: 16px;
                    text-align: left !important;
                    padding-bottom: 0px;
                    line-height: normal;
                }
                .invoice-box .invoice-info{
                    border-collapse: collapse;
                    border-spacing; 0;
                    padding: 0 0 0 0;
                    margin: 8px 0 65px 55px !important;
                    width: 30%;
                }
                .invoice-box .invoice-info tr{
                    margin: 0 0 0 0 !important;
                    padding: 0 0 0 0;
                    line-height: 64%;
                    font-size: 16px;
                    border-collapse: collapse;
                    border-spacing; 0;
                }
                .invoice-box .invoice-info tr td:nth-child(1){
                    width: 40%;
                }
                .invoice-box .invoice-info tr td:nth-child(2){
                    width: 60%
                    padding-left: 2px;
                    text-align: left;
                    align: left;
                }
                .invoice-box .item-table tr.heading td {
                    background: #eee;
                    border-bottom: 1px solid #ddd;
                    font-weight: bold;
                    font-size: 15px;
                }
                .invoice-box .item-table tr.details td {
                    padding-bottom: 20px;
                }
                .invoice-box .item-table{
                    margin-bottom: 25px !important;
                }
                .invoice-box .item-table tr.item td {
                    border-bottom: 1px solid #eee;
                    font-size: 14px;
                }
                .invoice-box .item-table tr.item.last td {
                    border-bottom: none;
                }
                .invoice-box .item-table tr td:nth-child(1){
                    border-top: 2px solid #eee;
                    width: 26%;
                    padding-left: 4px;
                }
                .invoice-box .item-table tr td:nth-child(2){
                    width: 34%;
                }
                .invoice-box .item-table tr td:nth-child(4),
                .invoice-box .item-table tr td:nth-child(5),
                .invoice-box .item-table tr td:nth-child(6){
                    text-align: center;
                }
                .invoice-box .item-table tr td:nth-child(3){
                    width: 12%;
                    text-align: center;
                }
                .invoice-box .item-table tr td:nth-child(4),
                .invoice-box .item-table tr td:nth-child(5){
                    width: 8%;
                }
                .invoice-box .item-table tr td:nth-child(6){
                    width: 12%
                }
                .invoice-box .total-table{
                    border-collapse: collapse;
                    border-spacing; 0;
                    width: 18%;
                    float: right;
                    margin: 5px 35px 0 0 !important;
                    padding: 0 0 0 0 !important;
                }
                .invoice-box .total-table .total{
                    border-top: 1px solid #888;
                }
                .invoice-box .total-table tr td:nth-child(1){
                    font-weight: 600;
                    text-align: right;
                    font-size: 15px;
                    padding-bottom: 1px;
                    width: 40%;
                }
                .invoice-box .total-table tr td:nth-child(2){
                    font-weight: 800;
                    text-align: left;
                    font-size: 15px;
                    padding-bottom: 1px;
                    padding-left: 6px;
                    width: 60%;
                }
                .invoice-box .disclaimer-table{
                    width: 100%;
                    border-top: 2px solid #DDD;
                    margin: 0 0 0 0 !important;
                    padding: 0 0 0 0 !important;
                    position: fixed !important;
                    bottom: 4%;
                    left: 0%;
                }
                .invoice-box .disclaimer-table .message .message-text{
                    font-size: 16px;
                    text-align: center;
                    padding: 5px 25px !important;
                }
                @media only screen and (max-width: 600px) {
                    .invoice-box table tr.top table td {
                        width: 100%;
                        display: block;
                        text-align: center;
                    }
                    .invoice-box table tr.information table td {
                        width: 100%;
                        display: block;
                        text-align: center;
                    }
                }
            </style>";

            //Company string
            string companyString = @"<div class=invoice-box>
                <table cellpadding=0 cellspacing=0>
                <tr class=top>
                <td colspan=2>
                <table>
                <tr>
                <td class=title>";


            companyString += "<img class=logo src=" + pdf.Company.ImageSrc + " />";




            companyString +=
                "</td>"
                + "<td class=company>"
                + "<b>" + pdf.Company.CompanyName + "</b>"
                + "<hr />"
                + pdf.Company.Address
                + "<br />" + pdf.Company.City

                + "<b>Tel: </b>" + pdf.Company.TelephoneNumber
                + "<br />"

                + "</tr> "
                + "</table> "
                + "</td>"
                + "</tr>"
                + "</table>";

            string debtorString = "";

            //Debtor string
            if (pdf.Client != null)
            {
                debtorString = @"<table class=debtor-table cellpadding=0 cellspacing=0>"
                    + "<tr>"
                        + "<td class=debtor-name>"
                            + pdf.Client.CompanyName
                        + "</td>"
                    + "</tr>"
                    + "<tr>"
                        + "<td class=debtor-address>"
                            + pdf.Client.Address
                        + "</td>"
                    + "</tr>"
                    + "<tr>"
                        + "<td class=debtor-city>"
                            + pdf.Client.Email + " " + pdf.Client.City
                        + "</td>"
                    + "</tr>"
                + "</table>";
            }

            //Spacer string
            string spacerString = @"<br />";

            //Invoice info
            string invoiceString = @"<table class=invoice-info cellpadding=0 cellspacing=0>"
                + "<tr>"
                + "<td><b>Date: </b></td>"
                + "<td>" + pdf.DateTimeString + "</td>"
                + "</tr>"
                + "<tr>"
                + "<td><b>Invoice No: </b></td>"
                //+ "<td>" + invoiceNumber + "</td>"
                + "</tr>"
                + "</table>";

            //Product string
            string productString = @"<table class=item-table cellpadding=0 cellspacing=0>"
                + "<tr class=heading> "
                + "<td>#</td>"
                + "<td>Description</td>"
                + "<td>Price</td>"
                + "<td>Qnt</td>"
                + "<td>VAT</td>"
                + "<td>Total</td>"
                + "</tr>";

            //Table string
            string tableString = "";
            decimal totalBeforeDiscount = 0;

            decimal subTotalAmount = 0;
            decimal vatTotalAmount = 0;
            decimal discount = 0;
            int discountPercentage = pdf.Invoice.Discount;

            for (int i = 0; i < productList.Count; i++)
            {
                Product product = productList[i];
                decimal total = (decimal)(product.PricePerUnit * product.Quantity);
                decimal subTotal = (decimal)(product.PricePerUnit * 100) / product.Vat;
                totalBeforeDiscount += total;
                subTotalAmount += subTotal;

                vatTotalAmount = totalBeforeDiscount;
                discount = (vatTotalAmount * discountPercentage) / 100;


                tableString += @"<tr class=item>"
                        + "<td>" + product.Code + "</td>"
                        + "<td>" + product.Name + "</td>"
                        + "<td>" + product.Quantity + "</td>"
                        + "<td>&euro; " + String.Format("{0:N2}", product.PricePerUnit) + "</td>"
                        + "<td>" + String.Format("{0}%", product.Vat) + "</td>"
                        + "</tr>";
            }

            var sumvat = vatTotalAmount / 100 * 24;
            var sum = sumvat + vatTotalAmount;
            //Total string
            string totalString = @"<table class=total-table cellspacing=0 cellpadding=0>"
                + "<tr class=vat>"
                    + "<td>Price without taxes:</td>"
                    + "<td>&euro; " + String.Format("{0:N2}", vatTotalAmount.ToString("#.##")) + "</td>"
                + "</tr>"
                + "<tr class=subtotal>"
                    + "<td>Total Discount:</td>"
                    + "<td>&euro; " + String.Format("{0:N2}", discount) + "</td>"
                + "</tr>"
                + "<tr class=discount>"
                    + "<td>Total Vat:</td>"
                    + "<td>&euro; " + String.Format("{0:N2}", sumvat) + "</td>"
                + "</tr>"
                + "<tr class=total>"
                    + "<td>Grant Total:</td>"
                    + "<td>&euro; " + String.Format("{0:N2}", sum) + "</td>"
                + "</tr>"
                + "</table>";



            //Full HTML string
            string htmlContent = cssString + companyString + debtorString + spacerString
                + invoiceString + productString + tableString + totalString;

            string save_path2 = _env.WebRootPath + "/wkhtmltopdf/bin/wkhtmltopdf";
            _logger.LogWarning($"{save_path2}");


            //var wkhtmltopdf = new FileInfo(save_path);
            //var converter = new HtmlToPdfConverter(wkhtmltopdf);
            //var file = converter.ConvertToPdf(htmlContent);
            //var contentType = "application/pdf";

            var wkhtmltopdf = new FileInfo(save_path2);
            var converter = new HtmlToPdfConverter(wkhtmltopdf);
            var pdfBytes = converter.ConvertToPdf(htmlContent);

            FileResult fileResult = new FileContentResult(pdfBytes, "application/pdf");
            fileResult.FileDownloadName = "invoice-" + id + ".pdf";
            return fileResult;

        }

      
 

[HttpPost]
        public async Task<IActionResult>Upload(Company model) 
        {
            Company company = _db.Companies.SingleOrDefault(u => u.Id == model.Id);
            string Filepath = await _filemanager.SaveImage(model.Logo);
            try
            {
                company.ImageSrc = Filepath;
               

                _db.Entry(await _db.Companies.FirstOrDefaultAsync(x => x.Id == company.Id)).CurrentValues.SetValues(company);
                await _db.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            } 
            return RedirectToAction("Manage", "Home"); 
        }



        public async Task<IActionResult> TotalVatThreeM()
        {
            var invoices = await _db.Pdfs.Where(p => p.Timestamp >= DateTime.Today.AddMonths(-3))
            .Include(u => u.Invoice)
            .Include(u => u.Client)
            .Include(u => u.Company).ToListAsync();


            var totalvat = _db.Pdfs.Where(p => p.Timestamp >= DateTime.Today.AddMonths(-3)).Select(u => u.TotalVat).Sum();
            var totalValue = _db.Pdfs.Where(p => p.Timestamp >= DateTime.Today.AddMonths(-3)).Select(u => u.TotalVat).Sum();

            PDFViewModel model = new PDFViewModel()
            {
                DataPdf = invoices,
                TotalVat = totalvat.ToString("#.##"),
                GrantTotal = totalValue

            };

            return View(model);
        }

        public async Task<IActionResult> TotalVatTSixM()
        {
            var invoices = await _db.Pdfs.Where(p => p.Timestamp >= DateTime.Today.AddMonths(-6))
            .Include(u => u.Invoice)
            .Include(u => u.Client)
            .Include(u => u.Company).ToListAsync();


            var totalvatThree = _db.Pdfs.Where(p => p.Timestamp >= DateTime.Today.AddMonths(-3)).Select(u => u.TotalVat).Sum();
            var totalvatSix = _db.Pdfs.Where(p => p.Timestamp >= DateTime.Today.AddMonths(-6)).Select(u => u.TotalVat).Sum();
            var totalValueSix = _db.Pdfs.Where(p => p.Timestamp >= DateTime.Today.AddMonths(-6)).Select(u => u.TotalVat).Sum();
            var totalValueThree = _db.Pdfs.Where(p => p.Timestamp >= DateTime.Today.AddMonths(-3)).Select(u => u.TotalValue).Sum();

            PDFViewModel model = new PDFViewModel()
            {
                DataPdf = invoices 
            };

            return View(model);
        }

        public IActionResult OnGetSetCultureCookie(string cltr, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cltr)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        } 




        [ViewLayout("Empty")]
        public async Task<IActionResult> Pdf()

        {
            List<Company> newCompanyList = new List<Company>();
            List<Client> all_clients = _db.Clients.ToList();

            var subquery = (from cl in all_clients
                            join c in _db.Companies.ToList() on cl.IdentifierId equals c.IdentifierId

                            select new Company
                            {
                                Id = c.Id,
                                IdentifierId = c.IdentifierId
                            });

            newCompanyList = subquery.ToList();

            var query = await _db.Companies.OrderByDescending(c => c.Timestamp).FirstOrDefaultAsync();

            Company company = _db.Companies.SingleOrDefault(c => c.IdentifierId == query.IdentifierId);
            Client client = _db.Clients.SingleOrDefault(c => c.IdentifierId == query.IdentifierId);
            Invoice invoice = _db.Invoices.OrderByDescending(inv => inv.Timestamp).FirstOrDefault(inv => inv.Company.Id == company.Id);

            PDFViewModel pf = new PDFViewModel
            {
                Company = company,
                Client = client,
                Invoice = invoice
            };

            return Ok(pf);
        }


     


        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");


            return RedirectToAction("Index", "Home");
        }
    }
}
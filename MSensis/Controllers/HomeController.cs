using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSensis.Email;
using MSensis.Models;
using MSensis.Services;
using MSensis.ViewModels;
using Newtonsoft.Json;

namespace MSensis.Controllers
{
    public class HomeController : Controller
    {

        private readonly MSensisContext _db;
        private readonly UserManager<User> _manager;
        private readonly FileManager _filemanager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IMailer _mailer;
        private readonly IConverter _converter;


        public HomeController(MSensisContext _db, UserManager<User> manager, FileManager filemanager, SignInManager<User> signInManager,
           ILogger<HomeController> logger, IMailer mailer, IConverter converter)
        {
            this._db = _db;
            _manager = manager;
            _filemanager = filemanager;
            _signInManager = signInManager;
            _logger = logger;
            _mailer = mailer;
            _converter = converter;
           
        }

        [HttpGet]
        [Authorize]
        [ViewLayout("_Administrator")]
        public async Task<IActionResult> UpdateProfile()
        {
            User user = await _manager.GetUserAsync(HttpContext.User);
            List<Company> user_companies = _db.Companies.Where(c => c.User.Id == user.Id).ToList();
            List<Client> Clients = _db.Clients.Where(c => c.User.Id == user.Id).ToList();

            UserForProfileViewModel model = new UserForProfileViewModel
            {
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Clients = Clients,
                Companies = user_companies
            };


            return View(model);
        }
        [HttpPost]
        [Authorize]
        [ViewLayout("_Administrator")]
        public async Task<IActionResult> UpdateProfile(UserForProfileViewModel model)
        {
            User user = await _manager.GetUserAsync(HttpContext.User);
            if (model.PhoneNumber != null && model.Name != null)
            {
                user.PhoneNumber = model.PhoneNumber;
                user.Name = model.Name;

                await _manager.UpdateAsync(user);
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Index()
        {
            
                User user = await _manager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    _logger.LogWarning("no user found {0}", DateTime.Now);
                     
                }
                else
                {
                    _logger.LogWarning("user is in {0}", user); 
                }
                List<Company> user_companies = _db.Companies.Where(c => c.User.Id == user.Id).ToList();
            

                List<Pdf> user_pdfs = _db.Pdfs.Where(c => c.User.Id == user.Id).ToList();
                List<Client> Clients = _db.Clients.Where(c => c.User.Id == user.Id).ToList();
                var totalvatThree = user_pdfs.Where(p => p.Timestamp <= DateTime.Today.AddMonths(-3)).Select(u => u.TotalVat).Sum();
                var totalvatSix = user_pdfs.Where(p => p.Timestamp <= DateTime.Today.AddMonths(-6)).Select(u => u.TotalVat).Sum();
                var totalValueSix = user_pdfs.Where(p => p.Timestamp <= DateTime.Today.AddMonths(-6)).Select(u => u.TotalValue).Sum();
                var totalValueThree = user_pdfs.Where(p => p.Timestamp <= DateTime.Today.AddMonths(-3)).Select(u => u.TotalValue).Sum();

                UserViewModel model = new UserViewModel
                {
                    Companies = user_companies,
                    Companys_Clients = user_companies.Count(),
                    User_Invoices = user_pdfs.Count(),
                    pdfs = user_pdfs,
                    User_pdfs = user_pdfs.Count(),
                    Clients = Clients,
                    TotalValueSix = totalValueSix,
                    TotalValueThree = totalValueThree,
                    TotalVatSix = totalvatSix,
                    TotalVatThree = totalvatThree ,
                    User = user 
                };

                return View(model);  
            
        }

        [ViewLayout("_Administrator")]
        public async Task<IActionResult> AllProducts()
        {
            var list = await _db.Products.ToListAsync();

            InvoiceViewModel model = new InvoiceViewModel
            {
                products2 = list
            };
            return View(model);
        }

        [ViewLayout("_Administrator")]
        public async Task<IActionResult> AllProd()
        {
            var list = await _db.Products.ToListAsync();

            InvoiceViewModel model = new InvoiceViewModel
            {
                Products = list
            };
            return View(model);
        }



        [ViewLayout("Empty")]
        public async Task<IActionResult> Administrator()
        {
            User user = await _manager.GetUserAsync(HttpContext.User);
            List<Company> user_companies = _db.Companies.Where(c => c.User.Id == user.Id).ToList();
            List<Pdf> user_pdfs = _db.Pdfs.Where(c => c.User.Id == user.Id).ToList();
            List<Client> Clients = _db.Clients.Where(c => c.User.Id == user.Id).ToList();

            UserViewModel model = new UserViewModel
            {
                Companies = user_companies,
                Companys_Clients = user_companies.Count(),
                pdfs = user_pdfs,
                User_pdfs = user_pdfs.Count(),
                Clients = Clients

            };

            return View(model);
        }

        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Administrator2()
        {
            User user = await _manager.GetUserAsync(HttpContext.User);
            List<Company> user_companies = _db.Companies.Where(c => c.User.Id == user.Id).ToList();
            List<Pdf> user_pdfs = _db.Pdfs.Where(c => c.User.Id == user.Id).ToList();
            List<Client> Clients = _db.Clients.Where(c => c.User.Id == user.Id).ToList();

            UserViewModel model = new UserViewModel
            {
                Companies = user_companies,
                Companys_Clients = user_companies.Count(),
                pdfs = user_pdfs,
                User_pdfs = user_pdfs.Count(),
                Clients = Clients

            };

            return View(model);
        }

        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Manage(string CompanyId)
        {
            if (CompanyId == null)
            {
                User user = await _manager.GetUserAsync(HttpContext.User);
                List<Company> user_companies = _db.Companies.Where(c => c.User.Id == user.Id).ToList(); 

                UserViewModel model = new UserViewModel
                {
                    Companies = user_companies,
                    Companys_Clients = user_companies.Count(),
                    
                };

                return View(model);
            }
            else
            {
                Company company = _db.Companies.Where(c => c.Id == CompanyId).SingleOrDefault();
                return View(company);
            }

        }

        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Create_Invoice()
        {
            User user = await _manager.GetUserAsync(HttpContext.User);
            List<Company> user_companies = await _db.Companies.Where(c => c.User.Id == user.Id).ToListAsync();
            List<Client> user_clients = await _db.Clients.Where(c => c.User.Id == user.Id).ToListAsync();
            

            InvoiceViewModel model = new InvoiceViewModel()
            {
                Companies = user_companies,
                Clients = user_clients
                 
            };

            return View(model);
        }




        [ViewLayout("_Administrator")]
        public async Task<IActionResult> CreateProduct()
        {
            User user = await _manager.GetUserAsync(HttpContext.User);
            Company user_company = await _db.Companies.Where(c => c.User.Id == user.Id).SingleOrDefaultAsync();
            List<Client> user_clients = await _db.Clients.Where(c => c.User.Id == user.Id).ToListAsync();
            List<Invoice> user_invoices = await _db.Invoices.Where(c => c.Company.Id == user_company.Id).ToListAsync();


            InvoiceViewModel model = new InvoiceViewModel()
            {
                Company = user_company,
                Clients = user_clients,
                Invoices = user_invoices

            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm]InvoiceViewModel model)
        {
            User user = await _manager.GetUserAsync(HttpContext.User);

            Company company = await _db.Companies.Where(c => c.User.Id == c.User.Id).FirstOrDefaultAsync();
            Client client = await _db.Clients.Where(c => c.Company.Id == company.Id).FirstOrDefaultAsync();
            Invoice invoice = await _db.Invoices.Where(c => c.Company.Id == company.Id).FirstOrDefaultAsync();


            
            await _db.SaveChangesAsync();
            Product item = new Product()
            {
                Comments = model.Invoice_Comments,
                Code = model.Invoice_Code,
                Quantity = model.Invoice_Quantity,
                Vat = 24,
                Description = model.Invoice_Description,
                Timestamp = DateTime.Now,
                Absolute_value = model.Price   ,
                Invoice = invoice,
                DateTimeString = DateTime.Now.ToString("MM/hh/yyyy")

            };
            _db.Products.Add(item);
            await _db.SaveChangesAsync();

            Invoice_Product invoice_products = new Invoice_Product
            {
                Invoice = invoice,
                User = user,
                Company = company
            };
            _db.Add(invoice_products);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            }

             
                Pdf pdf = new Pdf
                {
                    Client = client,
                    Company = company,
                    Invoice = invoice,
                    User = user,
                    Timestamp = DateTime.Now

                };
                _db.Pdfs.Add(pdf);
                await _db.SaveChangesAsync();

            return RedirectToAction("AllProd", "Home");

            }
              


        public int CalculateVat(int value)
        {
            return (value / 100) * 24;
        }

      

        [HttpPost]
        public async Task<IActionResult> Create_Action([FromBody]InvoiceViewModel model)
        { 
            User user = await _manager.GetUserAsync(HttpContext.User);


            Company company = await _db.Companies.SingleOrDefaultAsync(u => u.Id == model.CompanyId);
            Client client = await _db.Clients.SingleOrDefaultAsync(u => u.Id == model.ClientId);

            try
            {
                Invoice invoice = new Invoice()
                {
                    Comments = model.Invoice_Comments,
                    Code = model.Invoice_Code,
                    Quantity = model.Invoice_Quantity,
                    Vat = model.Invoice_VAT,
                    Description = model.Invoice_Description,
                    Discount = model.Discount,
                    PricePerUnit = model.PricePerUnit,
                    Invoice_Type = "Draft",
                    Client = client,
                    Company = company,
                    Timestamp = DateTime.UtcNow.Date,
                    DateTimeString = DateTime.Now.ToString("dd/MM/yyyy"),
                    IdentifierId = Guid.NewGuid()
                };
                _db.Invoices.Add(invoice);
                await _db.SaveChangesAsync(); 

                Invoice_Product invoice_products = new Invoice_Product
                {
                    Invoice = invoice,
                    User = user,
                    Company = company
                };
                _db.Add(invoice_products);

                await _db.SaveChangesAsync(); 

                Pdf pdf = new Pdf
                {
                    Client = client,
                    Company = company,
                    Invoice = invoice,
                    User = user,
                    DateTimeString = DateTime.Now.ToString("dd/MM/yyyy")

                };
                _db.Pdfs.Add(pdf);
                await _db.SaveChangesAsync();

                List<Product> list = new List<Product>();
                foreach (var c in model.Ids)
                {
                    Product product = new Product();
                    product.Description = c.Name;
                    product.Name = c.Name;
                    product.PricePerUnit = c.Absolute_value;
                    product.Vat = c.Vat;
                    product.Discount = c.Discount; 
                    product.Invoice = invoice;
                    product.PdfId = pdf.Id;
                    product.Quantity = c.Quantity;
                    product.Code = c.Code;
                    product.DateTimeString = DateTime.Now.ToString("dd/MM/yyyy");
                    invoice_products.Product = c;
                    product.Timestamp = DateTime.Now;
                    list.Add(product);
                }
                await _db.Products.AddRangeAsync(list); 
                await _db.SaveChangesAsync(); 

                Product item = new Product()
                {
                    Comments = model.Invoice_Comments,
                    Code = model.Invoice_Code,
                    Quantity = model.Invoice_Quantity,
                    Discount = model.Discount,
                    Vat = model.Invoice_VAT,
                    Description = model.Invoice_Description,
                    Timestamp = DateTime.Now,
                    Invoice = invoice,
                    PdfId = pdf.Id,
                    DateTimeString = DateTime.Now.ToString("dd/MM/yyyy"),
                    PricePerUnit = model.PricePerUnit 
                   

                };
                _db.Products.Add(item);
                await _db.SaveChangesAsync(); 

                List<Product> products = await _db.Products.Where(u => u.PdfId == pdf.Id)
                       .ToListAsync();

                decimal totalBeforeDiscount = 0;
               
                decimal subTotalAmount = 0;
                decimal vatTotalAmount = 0;
                decimal discount = 0;
                int discountPercentage = pdf.Invoice.Discount;

                for (int i = 0; i < products.Count; i++)
                {
                    Product product = products[i];
                    decimal total = (decimal)(product.PricePerUnit * product.Quantity);
                    decimal subTotal = (decimal)(product.PricePerUnit * 100) / product.Vat;
                    totalBeforeDiscount += total;
                    subTotalAmount += subTotal;
                }

                vatTotalAmount = totalBeforeDiscount;
                discount = (vatTotalAmount * discountPercentage) / 100;
                var sumvat = vatTotalAmount / 100 * 24;

                pdf.TotalValue = vatTotalAmount;
                pdf.TotalVat = sumvat;
                await  _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            }

            return Ok();
    }

          
        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Create_Client()
        {
            User user = await _manager.GetUserAsync(HttpContext.User); 
            return View();
        }

        [HttpPost]
        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Create_Client(ClientViewModel model)
        {

            User user = await _manager.GetUserAsync(HttpContext.User);
            try
            {
                Client client = new Client()
                {
                    Address = model.Address,
                    City = model.City,
                    AFM = model.AFM,
                    DOY = model.DOY,
                    Code = model.Code,
                    Client_Occupation = model.Client_Occupation,
                    CompanyName = model.CompanyName,
                    TelephoneNumber = model.TelephoneNumber,
                    Email = model.Email,
                    ZipCode = model.ZipCode,
                    Timestamp = DateTime.Now,
                    User = user
                };

                _db.Clients.Add(client);
                await _db.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            }

            return RedirectToAction("CClients", "Home");
        }

        

        [ViewLayout("Empty")]
        public async Task<ActionResult> ViewInvoice(string id)
        {
            string url = HttpContext.Request.Path;
            _logger.LogWarning($"{url}");

            Pdf pdf = _db.Pdfs.Include(u => u.Invoice)
               .Include(u => u.Client)
               .Include(u => u.Company)
               .Where(c => c.Id == id).SingleOrDefault();


            decimal totalBeforeDiscount = 0;
            
            decimal subTotalAmount = 0;
            decimal vatTotalAmount = 0;
            decimal discount = 0;
            int discountPercentage = pdf.Invoice.Discount;

            List<Product> products = await _db.Products.Where(u => u.PdfId == id)
                   .ToListAsync();

            for (int i = 0; i < products.Count; i++)
            {
                Product product = products[i];
                decimal total = (decimal)(product.PricePerUnit * product.Quantity);
                decimal subTotal = (decimal)(product.PricePerUnit * 100) / product.Vat; 
                totalBeforeDiscount += total;
                subTotalAmount += subTotal;
            }
            vatTotalAmount = totalBeforeDiscount;
            discount = (vatTotalAmount * discountPercentage) / 100;
            var sumvat = vatTotalAmount / 100 * 24;

            PDFViewModel model = new PDFViewModel
            {
                Company = pdf.Company,
                Client = pdf.Client,
                Invoice = pdf.Invoice,
                ImageSrc = pdf.Company.ImageSrc,
                products2 = products, 
                Id = pdf.Id,
                SubTotal = subTotalAmount.ToString("#.##"),
                TotalVat = sumvat.ToString("#.##"),
                Discount = discount.ToString("#.##"),
                PriceWithoutVat = vatTotalAmount.ToString("#.##"),
                GrantTotal = vatTotalAmount  + sumvat,
                DateTimeString =pdf.DateTimeString

        };

            return View(model);
        }

        [ViewLayout("Empty")]
        public async Task<ActionResult> ViewPdf(string id)
        {
            string url = HttpContext.Request.Path;
            _logger.LogWarning($"{url}");

            Pdf pdf = _db.Pdfs.Include(u => u.Invoice)
               .Include(u => u.Client)
               .Include(u => u.Company)
               .Where(c => c.Id == id).SingleOrDefault();


            decimal totalBeforeDiscount = 0;

            decimal subTotalAmount = 0;
            decimal vatTotalAmount = 0;
            decimal discount = 0;
            int discountPercentage = pdf.Invoice.Discount;

            List<Product> products = await _db.Products.Where(u => u.PdfId == id)
                   .ToListAsync();

            for (int i = 0; i < products.Count; i++)
            {
                Product product = products[i];
                decimal total = (decimal)(product.PricePerUnit * product.Quantity);
                decimal subTotal = (decimal)(product.PricePerUnit * 100) / product.Vat;
                totalBeforeDiscount += total;
                subTotalAmount += subTotal;
            }
            vatTotalAmount = totalBeforeDiscount;
            discount = (vatTotalAmount * discountPercentage) / 100;
            var sumvat = vatTotalAmount / 100 * 24;

            PDFViewModel model = new PDFViewModel
            {
                Company = pdf.Company,
                Client = pdf.Client,
                Invoice = pdf.Invoice,
                ImageSrc = pdf.Company.ImageSrc,
                products2 = products,
                Id = pdf.Id,
                SubTotal = subTotalAmount.ToString("#.##"),
                TotalVat = sumvat.ToString("#.##"),
                Discount = discount.ToString("#.##"),
                PriceWithoutVat = vatTotalAmount.ToString("#.##"),
                GrantTotal = vatTotalAmount + sumvat,
                DateTimeString = pdf.DateTimeString

            };

            return View(model);
        }
        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Update_Client(string id)
        {

            return View(await _db.Clients.SingleOrDefaultAsync(c => c.Id == id));
        }

        [HttpPost]
        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Update_Client(Client model)
        {
            try
            {
                Client c = await _db.Clients.SingleOrDefaultAsync(u => u.Id == model.Id);

                c.Address = model.Address;
                c.City = model.City;
                c.AFM = model.AFM;
                c.DOY = model.DOY;
                c.CompanyName = model.CompanyName;
                c.Code = model.Code;
                c.Client_Occupation = model.Client_Occupation;
                c.CompanyName = model.CompanyName;
                c.TelephoneNumber = model.TelephoneNumber;
                c.Email = model.Email;
                c.ZipCode = model.ZipCode;
                c.Timestamp = DateTime.Now;

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            }
            return RedirectToAction("CClients", "Home");
        }

        [ViewLayout("_Administrator")]
        public ActionResult Update_Invoice(string id)
        {
            return View(_db.Invoices.SingleOrDefault(c => c.Id == id));
        }

        [HttpPost]
        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Update_Invoice(InvoiceViewModel model)
        {
            try
            {
                Invoice c = await _db.Invoices.SingleOrDefaultAsync(u => u.Id == model.Id);

                c.Code = model.Invoice_Code;
                c.Description = model.Invoice_Description;
                c.Quantity = model.Invoice_Quantity; 
                c.PricePerUnit = model.PricePerUnit;
                c.Vat = model.Invoice_VAT;
                c.Discount = model.Discount;
                c.Timestamp = DateTime.Now;

                _db.Entry(await _db.Invoices.FirstOrDefaultAsync(x => x.Id == c.Id)).CurrentValues.SetValues(c);
                await _db.SaveChangesAsync();
                return RedirectToAction("ManagePdfs", "Home");
                    }
            catch (Exception ex)
            {
                _logger.LogError($"the reeson is {ex}");
                return BadRequest("Error");
            } 
        }


        public async Task<IActionResult> ManageC(string id)
        {
            Company company = _db.Companies.SingleOrDefault(c => c.Id == id);

            List<Invoice>  company_invoices = await _db.Invoices.Where(i => i.Company.Id == id).ToListAsync();
            List<Client> company_clients = _db.Clients.Where(c => c.Company.Id == id).ToList();

            UserViewModel model = new UserViewModel
            {
                Company = company,
                Invoices = company_invoices,
                Clients = company_clients
            };

            return View(model);
        }


        [ViewLayout("_Administrator")]
        public async Task<IActionResult> CClients()
        {
            User user = await _manager.GetUserAsync(HttpContext.User);

            List<Client> company_clients = _db.Clients.Where(c => c.User.Id == user.Id).ToList();
            ClientViewModel model = new ClientViewModel
            {
                Clients = company_clients
            };

            return View(model);
        }

        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Client(string id)
        {
            Client client = await _db.Clients.SingleOrDefaultAsync(c => c.Id == id);
            ClientViewModel model = new ClientViewModel
            {
                Address = client.Address,
                City = client.City,
                AFM = client.AFM,
                DOY = client.DOY,
                Code = client.Code,
                CompanyName = client.CompanyName,
                Email = client.Email,
                ZipCode = client.ZipCode  
            };
            return RedirectToAction("Manage","Home");
        }


        [ViewLayout("_Administrator")]
        public async Task<IActionResult> ManagePdfs()
        {
            User user = await _manager.GetUserAsync(HttpContext.User);

            List<Pdf> pdfs = await _db.Pdfs.Where(c => c.User.Id == user.Id && c.Invoice.Invoice_Type == "Draft" || c.Invoice.Invoice_Type == "Issued")
                .Include(u => u.Invoice)
                .Include(u => u.Client)
                .Include(u => u.Company)
                 .OrderByDescending(u => u.Id)
                .ToListAsync();
            ClientViewModel model = new ClientViewModel
            {
                Pdfs = pdfs,

            };
            return View(model);
        }
        [HttpPost]
        [ViewLayout("_Administrator")]
        public async Task<IActionResult> GetPdfs(int period)
        {
            User user = await _manager.GetUserAsync(HttpContext.User);
            

            List<Pdf> pdfs = await _db.Pdfs.Where(c => c.User.Id == user.Id && (c.Invoice.Invoice_Type == "Draft" || c.Invoice.Invoice_Type == "Issued") && c.Invoice.Timestamp> DateTime.Now.AddMonths(-period))
                .Include(u => u.Invoice)
                .Include(u => u.Client)
                .Include(u => u.Company)
                 .OrderByDescending(u => u.Id)
                .ToListAsync();
            ClientViewModel model = new ClientViewModel
            {
                Pdfs = pdfs,

            };

            return PartialView("_DataTableInvoice", model);
        }



        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Invoices()
        {

            User user = await _manager.GetUserAsync(HttpContext.User);
            Company company = _db.Companies.SingleOrDefault(c => c.User.Id == user.Id);
            List<Invoice> company_invoices = _db.Invoices.Where(c => c.Company.Id == company.Id).ToList();
            UserViewModel model = new UserViewModel
            {

                Invoices = company_invoices
            };

            return View(model);
        }

        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Invoice(string id)
        {
            Invoice invoice = await _db.Invoices.SingleOrDefaultAsync(c => c.Id == id);
            InvoiceViewModel model = new InvoiceViewModel
            {
                Invoice_Code = invoice.Code,
                Discount = invoice.Discount,
                Invoice_Description = invoice.Description,
                Invoice_VAT = invoice.Vat
            };

            return View(model);
        }

        
        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Edit_Product(string id)
        {
            var product = await _db.Products.Where(u => u.Id == id)
                .SingleOrDefaultAsync();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit_Product(Product model)
        {
            Product product = _db.Products.SingleOrDefault(u => u.Id == model.Id); 
            try
            {
                product.Code = model.Code;
                product.Absolute_value = model.Absolute_value;
                product.Code = model.Code;
                product.PriceAfterDiscount = model.PriceAfterDiscount;
                product.PriceBeforeDiscount = model.PriceBeforeDiscount;
                product.Vat = model.Vat;
                product.Discount = model.Discount;

                _db.Entry(await _db.Products.FirstOrDefaultAsync(x => x.Id == product.Id)).CurrentValues.SetValues(product);
                await _db.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            }
            return RedirectToAction("AllProd", "Home");
        }



        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Update_Company(string id)
        {
            var company = await _db.Companies.Where(u => u.Id == id)                 
                .SingleOrDefaultAsync();

            return View(company);
        }

        [HttpPost]
        [ViewLayout("_Administrator")]
        public async Task<IActionResult> Update_Company(Company model)
        {
              Company company = _db.Companies.SingleOrDefault(u => u.Id == model.Id);
              if(model.Logo != null)
            {
                string Filepath = await _filemanager.SaveImage(model.Logo);

                try
                {
                    company.ImageSrc = Filepath;
                    company.Name = model.Name;
                    company.Code = model.Code;
                    company.City = model.City;
                    company.Address = model.Address;
                    company.TelephoneNumber = model.TelephoneNumber;

                    _db.Entry(await _db.Companies.FirstOrDefaultAsync(x => x.Id == company.Id)).CurrentValues.SetValues(company);
                    await _db.SaveChangesAsync();
                }

                catch (Exception ex)
                {
                    _logger.LogError($"{ex}");
                }
            }             

            if (model.Logo == null)
            {
                company.ImageSrc = await _db.Companies.Where(u => u.Id == model.Id).Select(u => u.ImageSrc).SingleOrDefaultAsync();
                company.Name = model.Name;
                company.Code = model.Code;
                company.City = model.City;
                company.Address = model.Address;
                company.TelephoneNumber = model.TelephoneNumber;
                _db.Entry(await _db.Companies.FirstOrDefaultAsync(x => x.Id == company.Id)).CurrentValues.SetValues(company);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Manage","Home");             
        }      

         
        

        [ViewLayout("_Administrator")]
        public async Task<IActionResult> DraftPdfs()
        {
            User user = await _manager.GetUserAsync(HttpContext.User);

            List<Pdf> pdfs = await _db.Pdfs.Where(c => c.User.Id == user.Id && c.Invoice.Invoice_Type == "Draft")
                .Include(u => u.Invoice)
                .Include(u => u.Client)
                .Include(u => u.Company)
                 .OrderByDescending(u => u.Id)
                .ToListAsync();

            ClientViewModel model = new ClientViewModel
            {
                Pdfs = pdfs
                
            };

           
            return View(model);
        }


        [HttpPost]
        [ViewLayout("_Administrator")]
        public async Task<IActionResult> GetDraftPdfs(int period)
        {


            User user = await _manager.GetUserAsync(HttpContext.User);
            List<Pdf> pdfs = await _db.Pdfs.Where(c => c.User.Id == user.Id && c.Invoice.Invoice_Type == "Draft" && c.Invoice.Timestamp > DateTime.Now.AddMonths(-period))
                .Include(u => u.Invoice)
                .Include(u => u.Client)
                .Include(u => u.Company)
                 .OrderByDescending(u => u.Id)
                .ToListAsync();
            //ClientViewModel model = new ClientViewModel
            //{
            //    Pdfs = pdfs
            //};
            List<ClientViewModel> toSend = new List<ClientViewModel>();
            for (var i = 0; i < pdfs.Count; i++)
            {
                Pdf pdf = pdfs[i];
                toSend.Add(new ClientViewModel()
                {
                    CompanyName = pdf.Company.Name,
                    ClientName = pdf.Client.CompanyName,
                    DatetimeString = pdf.DateTimeString,
                    pdfId = pdf.Id,
                    invoiceId = pdf.InvoiceId

                });
            }
            return Json(toSend);
        }

        [ViewLayout("_Administrator")]
        public async Task<IActionResult> ViewProducts(string id)
        { 
            
            List<Product> products = await _db.Products.Where(u => u.PdfId == id).ToListAsync();
            PDFViewModel model = new PDFViewModel
            {
                products2 = products
            };
            return View(model);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }


        [ViewLayout("_Administrator")]
        public async Task<IActionResult> IssuedPdfs()
        {
            User user = await _manager.GetUserAsync(HttpContext.User);

            List<Pdf> pdfs = await _db.Pdfs.Where(c => c.User.Id == user.Id && c.Invoice.Invoice_Type == "Issued")
                .Include(u => u.Invoice)
                .Include(u => u.Client)
                .Include(u => u.Company)
                 .OrderByDescending(u => u.Id)
                .ToListAsync();
            ClientViewModel model = new ClientViewModel
            {
                Pdfs = pdfs
            };
            return View(model);
        }



        [HttpPost]
        public bool Delete(string id)
        {
            try
            {
                Company company = _db.Companies.Where(s => s.Id == id).SingleOrDefault();
                _db.Companies.Remove(company);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return false;
            }
        }

        

        [HttpPost]
        public bool Delete_Product(string id)
        {
            try
            {
                Product product = _db.Products.Where(s => s.Id == id).SingleOrDefault();
                _db.Products.Remove(product);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return false;
            }
        }

        [HttpPost]
        public bool Delete_Client(string id)
        {
            try
            {
                Client c =  _db.Clients.SingleOrDefault(s => s.Id == id);
                _db.Remove(c);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return false;
            }
        }

        public bool Delete_Invoice(string id)
        {
            if (id == null)
            {
                return false;
            }
            try
            {
          var invoice = _db.Invoices.Where(u => u.Id == id).SingleOrDefault();
            var products = _db.Products.Where(u => u.Invoice.Id == id).ToList();
            _db.RemoveRange(products);
            _db.Remove(invoice);
            _db.SaveChanges();
            return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return false;
            }
        }



        public async Task<List<Company>> GetCompanies()
        {
            User user =  await _manager.GetUserAsync(HttpContext.User);
            var query =  _db.Companies.OrderByDescending(s => s.Timestamp)
                          .Where(s => s.User.Id == user.Id).ToList();
            return  query;
        }
 

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ViewLayoutAttribute : ResultFilterAttribute
    {
        private readonly string layout;
        public ViewLayoutAttribute(string layout)
        {
            this.layout = layout;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var viewResult = context.Result as ViewResult;
            if (viewResult != null)
            {
                viewResult.ViewData["Layout"] = layout;
            }
        }
    }
}
  


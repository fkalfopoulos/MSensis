using Microsoft.AspNetCore.Http;
using MSensis.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.ViewModels
{
    public class UserForProfileViewModel
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }
        public IEnumerable<Company> Companies { get; set; }
        public IEnumerable<Client> Clients { get; set; }

        public static void UpdateProfile(User user, UserForProfileViewModel model)
        {
            user.PhoneNumber = model.PhoneNumber;
            user.Name = model.Name;
        }


    }
    public class LogoViewModel
    {
        public string PostId { get; set; }
        public IFormFile PostImage { get; set; }
    }

    public class UserViewModel
    {

        public IEnumerable<Company> Companies { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<Invoice> Invoices { get; set; }
        public IEnumerable<Pdf> pdfs { get; set; }
        public Company Company { get; set; }
        public User User { get; set; }

        public string Id { get; set; }
        public int Companys_Clients { get; set; }
        public int UserCompanies { get; set; }

        public string Company_Name { get; set; }

        public int Company_Invoices { get; set; }
        public int User_pdfs { get; set; }
        public string CompanyId { get; set; }
        public int User_Companies { get; set; }
        public int User_Invoices { get; set; }
        public int ChoiceId { get; set; }
        public decimal TotalVatSix { get; set; }
        public decimal TotalValueSix { get; set; }
        public decimal TotalVatThree { get; set; }
        public decimal TotalValueThree { get; set; }
    }

    public class CompanyViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CompanyName { get; set; }
        public int TelephoneNumber { get; set; }
        public int Code { get; set; }

        public IFormFile Company_Logo { get; set; }

        public string ImageSrc { get; set; }

        public IEnumerable<Client> Cients { get; set; }
        public IEnumerable<Invoice> Invoices { get; set; }
    }

    public class ClientViewModel
    {
        public string Id { get; set; }
        public string pdfId { get; set; }
        public string invoiceId { get; set; }
        public string CompanyName { get; set; }
        public string ClientName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public int CategoryId { get; set; }

        public List<string> pdfss { get; set; }

        public List<int> ChooseList { get; set; }

        public List<string> Ids { get; set; }

        public string DatetimeString { get; set; }

        public int choice { get; set; }
        public List<int> Choose { get; set; }
        public int TelephoneNumber { get; set; }
        public int Code { get; set; }
        public int ZipCode { get; set; }
        public string Email { get; set; }
        public int AFM { get; set; }
        public string Client_Occupation { get; set; }
        public string DOY { get; set; }

        public IEnumerable<Pdf> Pdfs { get; set; }
        public DateTime? Timestamp { get; set; }
        public Company Company { get; set; }
        public Invoice Invoice { get; set; }
        public Client Client { get; set; }


    }

    public class InvoiceViewModel
    {

        public IEnumerable<Company> Companies { get; set; }
        public Company Company { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Invoice> Invoices { get; set; }
        public int Invoice_Code { get; set; }
        public string Invoice_Description { get; set; }
        public string CompanyId { get; set; }
        public string ClientId { get; set; }
        public string DateTime { get; set; }
        public string Name { get; set; }

        public List<Product> Ids { get; set; }
        public List<Product> products2 { get; set; }


        public string Id { get; set; }
        public int Invoice_VAT { get; set; }

        public int Invoice_Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal PricePerUnit { get; set; }

        public int Discount { get; set; }
        public string Invoice_Comments { get; set; }


        public string message { get; set; }
    }
}

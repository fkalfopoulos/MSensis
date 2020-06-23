using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.ViewModels
{
    public class SiteViewModel
    {
        //Company

        public string Company_Name { get; set; }
        public string Company_Address { get; set; }
        public string Company_City { get; set; }
        public string Company_CompanyName { get; set; }
        public int Company_TelephoneNumber { get; set; }
        public int Company_Code { get; set; }

         
        public IFormFile Company_Logo { get; set; }
        public string ImageSrc { get; set; }

        //End of company

        //Client

        public string Client_Id { get; set; }
        public string Client_CompanyName { get; set; }
        public string Client_Occupation { get; set; }
        public string Client_Address { get; set; }
        public string Client_City { get; set; }
        public string Client_Email { get; set; }

        public int Client_TelephoneNumber { get; set; }
        public int Client_Code { get; set; }
        public int Client_ZipCode { get; set; }
        public int Client_AFM { get; set; }
        public string Client_DOY { get; set; }

        // End of client


        //Invoice
        public string Invoice_Id { get; set; }
        public int Invoice_Code { get; set; }
        public string Invoice_Description { get; set; }
        public string Invoice_Comments { get; set; }
 
        public int Invoice_Quantity { get; set; }
        public int Invoice_PricePerUnit { get; set; }
        public int Invoice_Vat { get; set; }
        public int Invoice_PriceBeforeDiscount { get; set; }
        public int Invoice_PriceAfterDiscount { get; set; }
        public int Invoice_Discount { get; set; }

    }
}

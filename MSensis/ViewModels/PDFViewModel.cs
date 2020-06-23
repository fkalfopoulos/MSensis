using MSensis.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.ViewModels
{
    public class PDFViewModel
    {
        public Invoice Invoice { get; set; }
        public User User { get; set; }

        public string Id { get; set; }

        public Company Company { get; set; }
        public string CompanyName { get; set; }
        public DateTime Timestamp { get; set; }
        public string ClientCompanyName { get; set; }

         
        public string TotalVat { get; set; }

      
        public string SubTotal { get; set; }

       
        public string PriceWithoutVat { get; set; }

         
        public decimal GrantTotal { get; set; }
        public string Discount { get; set; }

        public string DateTimeString { get; set; } 

        [JsonIgnore]
        public Client Client { get; set; }

        public string ImageSrc { get; set; }

        public List<Pdf> DataPdf { get; set; }

        public List<Invoice_Product> Products { get; set; }
        public List<Product> products2 { get; set; }
    }
}

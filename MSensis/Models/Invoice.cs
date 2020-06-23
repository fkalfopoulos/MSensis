using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.Models
{
    public class Invoice
    {
         
        public string Id { get; set; }
        public string Description { get; set; }

        public string Payment_Status { get; set; }
        public int Code { get; set; }
         
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public int Vat { get; set; }
        public decimal PriceBeforeDiscount { get; set;}
        public decimal PriceAfterDiscount { get; set;}
         
        public int Discount { get; set; } 
        

        public Guid? IdentifierId { get; set; }
        public IList<Product> Products { get; set; }

        [JsonIgnore]
        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        [JsonIgnore]
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }


        public IList<Invoice_Product> Invoice_Products { get; set; }

        public string Comments { get; set; }

        public string Invoice_Type { get; set; }
        public DateTime? Timestamp { get; set;}

        
        public string DateTimeString { get; set;}
        public int Number { get; set; }


    }
}

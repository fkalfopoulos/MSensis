using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }


        public string Description { get; set; }
        public int Code { get; set; } 
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public int Vat { get; set; }
        public decimal PriceBeforeDiscount { get; set; }
        public decimal PriceAfterDiscount { get; set; }
       
        public decimal Absolute_value { get; set; }
        public int Discount { get; set; }


        public string DateTimeString { get; set; }

        public string Comments { get; set; } 
        public DateTime? Timestamp { get; set; }


        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }

        
        public Pdf Pdf { get; set; }
        public string PdfId { get; set; }
        

        public IList<Invoice_Product> Ιnvoice_Products { get; set; }
    }
}

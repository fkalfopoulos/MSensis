using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.Models
{
    public class Pdf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
         

        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }
        public string InvoiceId { get; set; }
          
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string UserId { get; set; }

         
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        public string CompanyId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public string ProductId { get; set; }

        public Client Client { get; set; }
        public string ClientId { get; set; }

        public string Type { get; set; }

        public decimal TotalVat { get; set; }
        public decimal TotalValue { get; set; }


     
        public string DateTimeString { get; set; }

        public Guid? IdentifierId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

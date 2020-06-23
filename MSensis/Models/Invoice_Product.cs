using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.Models
{
    public class Invoice_Product
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }


        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }
        public string InvoiceId { get; set; }


      
        public Company Company { get; set; }
        public string CompanyId { get; set; }

        public Product Product { get; set; }
        public string ProductId { get; set; }




        public User User { get; set; }
        public string UserId { get; set; }





    }
}

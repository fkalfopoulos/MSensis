using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.Models
{
    public class Company
    {
         
        public string  Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CompanyName { get; set; }
        public int TelephoneNumber { get; set; }
        public int Code { get; set; }
        public DateTime? Timestamp { get; set; }


        public Guid? IdentifierId { get; set; }

        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        [NotMapped]
        public IFormFile Logo { get; set; }

        public string ImageSrc { get; set; }

       
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.Models
{
    public class Client
    {
         
        public string Id { get; set; }

        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        
        public int TelephoneNumber { get; set; }
        public int Code { get; set; }
        public int ZipCode { get; set; }
        public int AFM { get; set; }
        public string Client_Occupation { get; set; }
        public string DOY { get; set; }
        public Guid? IdentifierId { get; set; }

        public DateTime? Timestamp { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }



        public ICollection<Invoice> Invoices { get; set; }

        [NotMapped]
        public ICollection<Client> Clients { get; set; }
    }
}

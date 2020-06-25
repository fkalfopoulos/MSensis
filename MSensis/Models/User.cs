using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public ICollection<Company> Companies { get; set; }
        public ICollection<Client> Clients { get; set; } 

    }
}

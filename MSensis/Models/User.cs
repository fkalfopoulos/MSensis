using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.Models
{
    public class User : IdentityUser
    {
        
        public ICollection<Company> Companies { get; set; }
         
        public ICollection<Client> Clients { get; set; } 

    }
}

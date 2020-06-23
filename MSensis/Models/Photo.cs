using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.Models
{
    public class Photo
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        

        [ForeignKey("CompanyId")]
        public Company Company { get; set; } 

        public Photo()
        {
            DateAdded = DateTime.Now;
        }
    }
}

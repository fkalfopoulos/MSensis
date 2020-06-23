using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSensis.Models
{
    public class Culture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Resource> Resources { get; set; }
    }
}

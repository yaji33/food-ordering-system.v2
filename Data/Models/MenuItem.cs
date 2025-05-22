using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace food_ordering_system.v2.Data.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        // Foreign key
        public int CategoryId { get; set; }

        // Navigation property
        public virtual Category Category { get; set; }
        public int Tag { get; internal set; }
    }
}
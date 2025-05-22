using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace food_ordering_system.v2.Data.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        // Navigation property for related menu items
        public virtual ICollection<MenuItem> MenuItems { get; set; }

        public Category()
        {
            MenuItems = new HashSet<MenuItem>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShopIn_API.Models
{
    public class ProductRate
    {
        public int Id { get; set; }
        public int Rate { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Products Product { get; set; }
    }
}

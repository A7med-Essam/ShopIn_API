using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShopIn_API.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Describtion { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Discount { get; set; }
        [Required]
        public bool OnSale { get; set; }
        [Required]
        public bool OutOfStock { get; set; }
        [Required]
        public bool NewItem { get; set; }
        public string publicId { get; set; }
        public Products()
        {
            ProductRates = new List<ProductRate>();
        }

        public virtual List<ProductRate> ProductRates { get; set; }
    }
}

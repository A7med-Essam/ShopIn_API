using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using ShopIn_API.Models;
using Microsoft.AspNetCore.Authorization;
using ShopIn_API.Interfaces;
using System;
using ShopIn_API.ViewModel;

namespace ShopIn_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopInContext db;
        private readonly IphotoServices _photoService;

        #region constructor
        public ProductsController(IphotoServices photoService, ShopInContext db)
        {
            _photoService = photoService;
            this.db = db;
        }
        #endregion

        #region Get Products in different ways
        [HttpGet("getAllProducts")]
        public ActionResult getAllProducts([FromQuery] PagingParameter_VM pagingparametermodel)
        {
            var products = db.Products.Select(x => new
            {
                x.Category,
                x.Date,
                x.Describtion,
                x.Discount,
                x.Id,
                x.ImageUrl,
                x.Name,
                x.NewItem,
                x.OnSale,
                x.OutOfStock,
                x.Price,
                RateCount = x.ProductRates.Count(),
                RateSum = x.ProductRates.Select(x => x.Rate).Sum()
            }).ToList();
            int count = products.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = products.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previousPage = CurrentPage > 1 ? "Yes" : "No";
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
            var paginationMetadata = new
            {
                data = items,
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };

            return Ok(paginationMetadata);
        }

        [HttpGet("getNewProducts")]
        public ActionResult getNewProducts(int NumberOfProducts = 10)
        {
            var products = db.Products.Where(x => x.NewItem == true).Select(x => new
            {
                x.Category,
                x.Date,
                x.Describtion,
                x.Discount,
                x.Id,
                x.ImageUrl,
                x.Name,
                x.NewItem,
                x.OnSale,
                x.OutOfStock,
                x.Price,
                RateCount = x.ProductRates.Count(),
                RateSum = x.ProductRates.Select(x => x.Rate).Sum()
            }).ToList().Take(NumberOfProducts);
            return Ok(products);
        }

        [HttpGet("getProductsOnSale")]
        public ActionResult getProductsOnSale(int NumberOfProducts = 10)
        {
            var products = db.Products.Where(x => x.OnSale == true).Select(x => new
            {
                x.Category,
                x.Date,
                x.Describtion,
                x.Discount,
                x.Id,
                x.ImageUrl,
                x.Name,
                x.NewItem,
                x.OnSale,
                x.OutOfStock,
                x.Price,
                RateCount = x.ProductRates.Count(),
                RateSum = x.ProductRates.Select(x => x.Rate).Sum()
            }).ToList().Take(NumberOfProducts);
            return Ok(products);
        }

        [HttpGet("getProductsByCategory")]
        public ActionResult getProductsByCategory([FromQuery] string category, int NumberOfProducts = 10)
        {
            var products = db.Products.Where(x => x.Category == category).Select(x => new
            {
                x.Category,
                x.Date,
                x.Describtion,
                x.Discount,
                x.Id,
                x.ImageUrl,
                x.Name,
                x.NewItem,
                x.OnSale,
                x.OutOfStock,
                x.Price,
                RateCount = x.ProductRates.Count(),
                RateSum = x.ProductRates.Select(x => x.Rate).Sum()
            }).ToList().Take(NumberOfProducts);
            return Ok(products);
        }

        [HttpGet("getProductsById")]
        public ActionResult getProductsById([FromQuery] int id, int NumberOfProducts = 1)
        {
            var products = db.Products.Where(x => x.Id == id).Select(x => new
            {
                x.Category,
                x.Date,
                x.Describtion,
                x.Discount,
                x.Id,
                x.ImageUrl,
                x.Name,
                x.NewItem,
                x.OnSale,
                x.OutOfStock,
                x.Price,
                RateCount = x.ProductRates.Count(),
                RateSum = x.ProductRates.Select(x => x.Rate).Sum()
            }).ToList().Take(NumberOfProducts);
            return Ok(products);
        }

        #endregion

        #region rating products
        [HttpPost("AddProductRate")]
        public ActionResult AddProductRate([FromBody] ProductRate rate)
        {
            if (rate == null)
            {
                return BadRequest();
            }

            db.ProductRates.Add(rate);
            db.SaveChanges();
            return Ok();
        }
        #endregion

        #region Admin Products Controller
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("AddProduct")]
        public async Task<ActionResult> AddProduct([FromQuery] Products product, [FromForm] IFormFile file)
        {
            if (product == null)
            {
                return BadRequest();
            }
            product.Date = DateTime.Now;

            if (file == null)
            {
                return BadRequest("image is required!");
            }

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            product.ImageUrl = result.SecureUrl.AbsoluteUri;
            product.publicId = result.PublicId;

            db.Products.Add(product);
            db.SaveChanges();
            return Ok();
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("UpdateProduct")]
        public async Task<ActionResult> UpdateProduct([FromQuery] Products product, [FromForm] IFormFile file)
        {
            if (product == null)
            {
                return BadRequest();
            }

            Products CurrentProduct = db.Products.Where(x => x.Id == product.Id).FirstOrDefault();

            if (file != null)
            {
                // delete old image & pubic id
                var DeletionResult = await _photoService.DeletePhotoAsync(CurrentProduct.publicId);
                if (DeletionResult.Error != null)
                {
                    return BadRequest(DeletionResult.Error.Message);
                }

                // add new image
                var result = await _photoService.AddPhotoAsync(file);
                if (result.Error != null)
                {
                    return BadRequest(result.Error.Message);
                }
                product.ImageUrl = result.SecureUrl.AbsoluteUri;
                product.publicId = result.PublicId;

            }

            if (file == null)
            {
                product.ImageUrl = CurrentProduct.ImageUrl;
                product.publicId = CurrentProduct.publicId;
            }

            product.Date = DateTime.Now;

            //db.Entry(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            CurrentProduct.Id = product.Id;
            CurrentProduct.Name = product.Name;
            CurrentProduct.ImageUrl = product.ImageUrl;
            CurrentProduct.NewItem = product.NewItem;
            CurrentProduct.OnSale = product.OnSale;
            CurrentProduct.OutOfStock = product.OutOfStock;
            CurrentProduct.Price = product.Price;
            CurrentProduct.publicId = product.publicId;
            CurrentProduct.Date = product.Date;
            CurrentProduct.Describtion = product.Describtion;
            CurrentProduct.Discount = product.Discount;
            CurrentProduct.Category = product.Category;
            db.SaveChanges();
            return Ok();
        }
       
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("DeleteProduct")]
        public async Task<ActionResult> DeleteProduct([FromQuery] int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var product = await db.Products.FindAsync(id);
            var result = await _photoService.DeletePhotoAsync(product.publicId);
            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
            db.Remove(product);
            db.SaveChanges();
            return Ok();
        }
        #endregion
   
    }
}

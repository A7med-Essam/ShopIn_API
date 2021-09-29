using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopIn_API.Models;
using ShopIn_API.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopIn_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromoCodes : ControllerBase
    {
        private readonly ShopInContext db;

        public PromoCodes(ShopInContext db)
        {
            this.db = db;
        }

        #region PromoCode Crud

        //[Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("AddPromoCode")]
        public ActionResult AddPromoCode([FromBody] PromoCode model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var promo = db.PromoCodes.Where(x => x.promoCode == model.promoCode).FirstOrDefault();

            if (promo == null)
            {
                db.PromoCodes.Add(model);
                db.SaveChanges();
                return Ok();
            }
            return BadRequest("PromoCode Already exist!");
        }

        //[Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("GetPromoCode")]
        public ActionResult GetPromoCode()
        {
            return Ok(db.PromoCodes.ToList());
        }

        //[Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("UpdatePromoCode")]
        public ActionResult UpdatePromoCode([FromBody] PromoCode model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var promo = db.PromoCodes.Where(x => x.id == model.id).FirstOrDefault();

            if (promo != null)
            {
                if (promo.promoCode == model.promoCode)
                {
                    return BadRequest("PromoCode Already exist!");
                }
                promo.promoCode = model.promoCode;
                promo.DiscountValue = model.DiscountValue;
                db.SaveChanges();
                return Ok();
            }
            return BadRequest("Invalid PromoCode!");
        }

        //[Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("DeletePromoCode")]
        public ActionResult DeletePromoCode(int id)
        {
            if (id == 0)
            {
                return BadRequest("Invalid PromoCode!");
            }
            var promo = db.PromoCodes.Where(x => x.id == id).FirstOrDefault();

            if (promo != null)
            {
                db.PromoCodes.Remove(promo);
                db.SaveChanges();
                return Ok();
            }
            return BadRequest("PromoCode does not exist!");
        }
      
        [HttpPost("UsePromoCode")]
        public ActionResult UsePromoCode(ApplicationUserPromoCodes_VM model)
        {
            if (model.PromoCode == null || model.UserId == null)
            {
              return BadRequest("PromoCode is required!");
            }

            var promo = db.PromoCodes.Where(x => x.promoCode == model.PromoCode).FirstOrDefault();


            if (promo != null )
            {
                var UsedPromo = db.ApplicationUserPromoCodes.Where(x => x.PromoCodeId == promo.id && x.ApplicationUserId == model.UserId).FirstOrDefault();
                if (UsedPromo == null)
                {
                    db.ApplicationUserPromoCodes.Add(new ApplicationUserPromoCode { ApplicationUserId = model.UserId, PromoCodeId = promo.id, isUsedPromoCode = true });
                    db.SaveChanges();
                    return Ok(promo);
                }
                if (UsedPromo != null)
                {
                    return BadRequest("This PromoCode is expired");
                }
            }


            return BadRequest("Invalid PromoCode!");
        }
      
        [HttpGet("getPromoCodeById")]
        public ActionResult getPromoCodeById(int id)
        {
            return Ok(db.PromoCodes.Where(x=>x.id == id).FirstOrDefault());
        }

        #endregion
    }
}

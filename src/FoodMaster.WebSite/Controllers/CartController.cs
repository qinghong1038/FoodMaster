﻿using FoodMaster.WebSite.Abstraction.Services;
using FoodMaster.WebSite.Filters;
using FoodMaster.WebSite.Helpers;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodMaster.WebSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User, Guest", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(WriteToDiskFilterAttribute))]
    public class CartController : ControllerBase
    {
        private readonly IMealsService mealsService;
        private readonly ICartService cartService;
        private readonly IDiscountProvider discountProvider;

        public CartController(IMealsService mealsService, ICartService cartService, IDiscountProvider discountProvider)
        {
            this.mealsService = mealsService;
            this.cartService = cartService;
            this.discountProvider = discountProvider;
        }

        [HttpPost("{id:int}", Name = nameof(Create))]
        public IActionResult Create([FromRoute] int id)
        {
            if (!mealsService.HasItemWithId(id))
            {
                return NotFound();
            }

            cartService.CreateFromItemId(id);

            return Created("/cart", null);
        }

        [HttpPut("{id:int}", Name = nameof(Edit))]
        [Produces("application/json")]
        public IActionResult Edit([FromRoute] int id, [FromQuery] int quantity)
        {
            if (quantity is default(int))
            {
                return BadRequest();
            }

            if (!cartService.HasItemWithId(id))
            {
                return NotFound();
            }

            cartService.GetByItemId(id).Quantity = quantity;

            var total = cartService.GetCartTotal();
            var discount = total * (decimal)discountProvider.GetDiscount();

            return Ok(new { Total = Currency.AsRubles(total), TotalDiscount = Currency.AsRubles(discount) });
        }

        [HttpDelete("{id:int}", Name = nameof(Delete))]
        public IActionResult Delete([FromRoute] int id)
        {
            if (!mealsService.HasItemWithId(id))
            {
                return NotFound();
            }

            cartService.DeleteByItemId(id);

            return NoContent();
        }
    }
}
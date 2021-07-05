using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cinema.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Cinema.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        /* private readonly ApplicationDbContext _context;
 */
        private readonly IShoppingCartService _shoppingCartService;
        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }
        public IActionResult Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Select * from Users Where Id LIKE userId

            return View(this._shoppingCartService.getShoppingCartInfo(userId));
        }
        public IActionResult DeleteFromShoppingCart(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._shoppingCartService.deleteTicketFromShoppingCart(userId, id);
            if (result)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }
            else
            {
                return RedirectToAction("Index", "ShoppingCart");
            }
        }

        private Boolean Order()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._shoppingCartService.orderNow(userId);
            return result;
        }
        public IActionResult PayOrder(string stripeEmail, string stripeToken)
        {
            var customerService = new CustomerService();
            var chargeService = new ChargeService();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = this._shoppingCartService.getShoppingCartInfo(userId);

            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var charge = chargeService.Create(new ChargeCreateOptions
            {
                Amount = (Convert.ToInt32(order.TotalPrice) * 100),
                Description = "Cinema Application Payment",
                Currency = "usd",
                Customer = customer.Id
            });

            if (charge.Status == "succeeded")
            {
                var result = this.Order();/* da se kreira vo baza order, i ke se iscisti soping kartickata
                - bez da uplatime nema da pomine ova*/

                if (result)
                {
                    return RedirectToAction("Index", "ShoppingCart");
                }
                else
                {
                    return RedirectToAction("Index", "ShoppingCart");
                }
            }

            return RedirectToAction("Index", "ShoppingCart");
        }

    }
}
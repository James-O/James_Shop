using Shop_Models;
using Shop_Models.ViewModels;
using Shop_Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Shop_DataA.Data;
using Shop_DataA.Repository.IRepository;

namespace JamesShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _prodRepo;
        private readonly ICategoryRepository _catRepo;

        public HomeController(ILogger<HomeController> logger,IProductRepository prodRepo,ICategoryRepository catRepo)
        {
            _logger = logger;
            _prodRepo= prodRepo;
            _catRepo = catRepo;
        }

        public IActionResult Index()
        {
            HomeVM home = new HomeVM
            {
                Products = _prodRepo.GetAll(includeProperties:"Category,Application"),
                Categories = _catRepo.GetAll()
            };
            return View(home);
        }

        public IActionResult Details(int id)
        {
            //session
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            var cart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            if (cart != null && cart.Count() > 0)
            {
                shoppingCarts = cart.ToList();
            }

            DetailsVM detail = new DetailsVM
            {
                Product = _prodRepo.FirstOrDefault(u=>u.Id==id,includeProperties: "Category,Application"),
                ExistInCart = false
            };

            foreach(var item in shoppingCarts)
            {
                if (item.ProductId==id)
                {
                    detail.ExistInCart = true;
                }
            }
            return View(detail);
        }
        [HttpPost,ActionName("Details")]
        public IActionResult DetailsPost(int id, DetailsVM detailsVM)
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            var cart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            if(cart != null && cart.Count() > 0)
            {
                shoppingCarts = cart.ToList();
            }
            shoppingCarts.Add(new ShoppingCart { ProductId = id,Quantity = detailsVM.Product.Quantity });
            HttpContext.Session.Set(WC.SessionCart, shoppingCarts);
            TempData[WC.Success] = "Item added to cart successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            var cart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            if (cart != null && cart.Count() > 0)
            {
                shoppingCarts = cart.ToList();
            }
            var itemToRemove = shoppingCarts.SingleOrDefault(r=>r.ProductId== id);
            if (itemToRemove != null)
            {
                shoppingCarts.Remove(itemToRemove);
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCarts);
            TempData[WC.Success] = "Item removed from cart successfully";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using Shop_Models;
using Shop_DataA.Data;
using Shop_Models.ViewModels;
using Shop_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Shop_DataA.Repository.IRepository;
using Shop_Utility.BrainTree;
using Braintree;

namespace JamesShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductRepository _prodRepo;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IInquiryHeaderRepository _inquiryHRepo;
        private readonly IInquiryDetailsRepository _inquiryDRepo;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;//inject wwwroot
        private readonly IOrderHeaderRepository _orderHRepo;
        private readonly IOrderDetailsRepository _orderDRepo;
        private readonly IBrainTreeGate _brainTree;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }
        public CartController(IProductRepository prodRepo, IApplicationUserRepository userRepo,
            IInquiryHeaderRepository inquiryHRepo, IInquiryDetailsRepository inquiryDRepo,
            IEmailSender emailSender,IWebHostEnvironment webHostEnvironment, 
            IOrderHeaderRepository orderHRepo, IOrderDetailsRepository orderDRepo,
            IBrainTreeGate brainTree)
        {
            _prodRepo=prodRepo;
            _userRepo=userRepo;
            _inquiryHRepo=inquiryHRepo;
            _inquiryDRepo=inquiryDRepo;
            _emailSender=emailSender;
            _webHostEnvironment=webHostEnvironment;
            _orderHRepo=orderHRepo;
            _orderDRepo=orderDRepo;
            _brainTree=brainTree;
        }
        public IActionResult Index()
        {
            //list all product in shopping cart
            List<ShoppingCart> CartList = new List<ShoppingCart>();
            var cart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            if (cart != null && cart.Count() > 0) 
            {
                //retrieve the session
                CartList = cart.ToList();
            }
            List<int> prodinCart = CartList.Select(c=>c.ProductId).ToList();
            IEnumerable<Product> prodListTemp = _prodRepo.GetAll(p => prodinCart.Contains(p.Id));
            IList<Product> prodList = new List<Product>(); 
            
            foreach(var cartObj in CartList)
            {
                Product prodTemp = prodListTemp.FirstOrDefault(p=>p.Id==cartObj.ProductId);
                prodTemp.Quantity = cartObj.Quantity;
                prodList.Add(prodTemp);
            }
            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Index))]
        public IActionResult IndexPost(IEnumerable<Product> prodList)
        {
            List<ShoppingCart> CartList = new List<ShoppingCart>();
            foreach (Product product in prodList)
            {
                CartList.Add(
                    new ShoppingCart { 
                        ProductId = product.Id, 
                        Quantity=product.Quantity 
                    });
            }
            HttpContext.Session.Set(WC.SessionCart, CartList);
            
            return RedirectToAction(nameof(Summary));
        }
        
        public IActionResult Summary()
        {
            ApplicationUser applicationUser;
            if (User.IsInRole(WC.AdminRole))
            {
                var sesion = HttpContext.Session.Get<int>(WC.SessionInquiryId);
                if (sesion !=0)
                {
                    //cart has been loaded using inquiry
                    InquiryHeader header = _inquiryHRepo.FirstOrDefault(h => h.Id==sesion);
                    applicationUser = new ApplicationUser()
                    {
                        Email = header.Email,
                        FullName = header.FullName,
                        PhoneNumber = header.PhoneNumber
                    };
                }
                else
                {
                    applicationUser = new ApplicationUser();
                }

                //transaction token
                var gateway = _brainTree.GetGateway();
                var clientToken = gateway.ClientToken.Generate();
                ViewBag.ClientToken = clientToken;
            }
            else
            {
                //retrieve user info
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                //var userId = User.FindFirstValue(ClaimTypes.Name);
                applicationUser = _userRepo.FirstOrDefault(u => u.Id ==claim.Value);


            }



            List<ShoppingCart> CartList = new List<ShoppingCart>();
            var cart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            if (cart != null && cart.Count() > 0)
            {
                //retrieve the session
                CartList = cart.ToList();
            }
            List<int> prodinCart = CartList.Select(c => c.ProductId).ToList();
            IEnumerable<Product> prodList = _prodRepo.GetAll(p => prodinCart.Contains(p.Id));

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = applicationUser,
            };
            foreach(var prod in CartList)
            {
                Product prodTemp = _prodRepo.FirstOrDefault(u => u.Id==prod.ProductId);
                prodTemp.Quantity = prod.Quantity;
                ProductUserVM.ProductList.Add(prodTemp);
            }
            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Summary))]
        public async Task<IActionResult> SummaryPost(IFormCollection collection, ProductUserVM ProductUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (User.IsInRole(WC.AdminRole))
            {
                //var orderTotal = 0.0;
                //foreach(Product prod in ProductUserVM.ProductList)
                //{
                //    orderTotal += prod.Quantity * prod.Price;
                //}

                //create an order
                OrderHeader orderHeader = new OrderHeader()
                {
                    CreatedByUserId=claim.Value,
                    FinalOrderTotal=ProductUserVM.ProductList.Sum(x=>x.Quantity*x.Price),//shorten what is above
                    City=ProductUserVM.ApplicationUser.City,
                    StreetAddress=ProductUserVM.ApplicationUser.Address,
                    State=ProductUserVM.ApplicationUser.State,
                    PostalCode = ProductUserVM.ApplicationUser.PostalCode,
                    FullName = ProductUserVM.ApplicationUser.FullName,
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                    OrderDate = DateTime.Now,
                    OrderStatus = WC.StatusPending
                };
                _orderHRepo.Add(orderHeader);
                _orderHRepo.Save();

                foreach (var product in ProductUserVM.ProductList)
                {
                    OrderDetails orderDetails = new OrderDetails()
                    {
                        OrderHeaderId = orderHeader.Id,
                        ProductId = product.Id,
                        PricePerQuantity=product.Price,
                        Quantity = product.Quantity
                    };
                    _orderDRepo.Add(orderDetails);

                }
                _orderDRepo.Save();

                string nonceFromTheClient = collection["payment_method_nonce"];

                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),
                    PaymentMethodNonce = nonceFromTheClient,
                    OrderId=orderHeader.Id.ToString(),
                    
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                var gateway = _brainTree.GetGateway();
                Result<Transaction> result = gateway.Transaction.Sale(request);

                if (result.Target.ProcessorResponseText == "Approved")
                {
                    orderHeader.TransactionId = result.Target.Id;
                    orderHeader.OrderStatus = WC.StatusApproved;
                }
                else
                {
                    orderHeader.OrderStatus = WC.StatusCancelled;
                }
                _orderHRepo.Save();
                return RedirectToAction(nameof(InquiryConfirmation), new {id=orderHeader.Id });
            }
            else
            {
                //create an inquiry
                var TempletePath = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templetes" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";

                var subject = "New Inquiry";
                string HtmlBody = "";
                using (StreamReader sr = System.IO.File.OpenText(TempletePath))
                {
                    HtmlBody = sr.ReadToEnd();
                }

                StringBuilder productListSB = new StringBuilder();
                foreach (var prod in ProductUserVM.ProductList)
                {
                    productListSB.Append($" - Name: {prod.Name} <span style = 'font-size:14px;'> (ID: {prod.Id}) </span><br />");
                }

                string messageBody = string.Format(HtmlBody,
                    ProductUserVM.ApplicationUser.FullName,
                    ProductUserVM.ApplicationUser.Email,
                    ProductUserVM.ApplicationUser.PhoneNumber,
                    productListSB.ToString());

                await _emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBody);

                InquiryHeader inquiryHeader = new InquiryHeader()
                {
                    ApplicaionUserId = claim.Value,
                    FullName = ProductUserVM.ApplicationUser.FullName,
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                    InquiryDate = DateTime.Now
                };
                _inquiryHRepo.Add(inquiryHeader);
                _inquiryHRepo.Save();

                foreach (var product in ProductUserVM.ProductList)
                {
                    InquiryDetails inquiryDetails = new InquiryDetails()
                    {
                        InquiryHeaderId = inquiryHeader.Id,
                        ProductId = product.Id
                    };
                    _inquiryDRepo.Add(inquiryDetails);

                }
                _inquiryDRepo.Save();
                TempData[WC.Success] = "Inquiry submitted successfully";
            }
            

            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation(int id=0)
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(h => h.Id==id);
            HttpContext.Session.Clear();
            return View(orderHeader);
        }
        public IActionResult Remove(int id)
        {
            //retrieve all product
            List<ShoppingCart> CartList = new List<ShoppingCart>();
            var cart = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart);
            if (cart != null && cart.Count() > 0)
            {
                //retrieve the session
                CartList = cart.ToList();
            }
            //remove certain product with id
            CartList.Remove(CartList.FirstOrDefault(r=>r.ProductId == id));
            // set session again
            HttpContext.Session.Set(WC.SessionCart, CartList);
            TempData[WC.Success] = "Item removed from cart successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult UpdateCart(IEnumerable<Product> prodList)
        {
            List<ShoppingCart> CartList = new List<ShoppingCart>();
            foreach(Product product in prodList)
            {
                CartList.Add(
                    new ShoppingCart {
                        ProductId = product.Id,
                        Quantity=product.Quantity
                        
                    });
            }
            HttpContext.Session.Set(WC.SessionCart, CartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Clear()
        {

            HttpContext.Session.Clear();
            TempData[WC.Success] = "Cart cleared successfully";
            return RedirectToAction("Index","Home");
        }
    }
}

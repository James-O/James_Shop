using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop_DataA.Repository.IRepository;
using Shop_Models;
using Shop_Models.ViewModels;
using Shop_Utility;

namespace JamesShop.Controllers
{
    [Authorize(Roles=WC.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inquiryHeader;
        private readonly IInquiryDetailsRepository _inquiryDetails;
        
        [BindProperty]
        public InquiryVM InquiryVM { get; set; }
        public InquiryController(IInquiryHeaderRepository inquiryHeader,
            IInquiryDetailsRepository inquiryDetails)
        {
            _inquiryHeader=inquiryHeader;
            _inquiryDetails=inquiryDetails;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            InquiryVM inquiryVM = new InquiryVM()
            {
                InquiryHeader = _inquiryHeader.FirstOrDefault(x => x.Id == id),
                InquiryDetails = _inquiryDetails.GetAll(d => d.InquiryHeaderId == id, includeProperties: "Product")
            };
            //InquiryVM = new InquiryVM()
            //{
            //    InquiryHeader = _inquiryHeader.FirstOrDefault(h => h.Id == id),
            //    InquiryDetails = _inquiryDetails.GetAll(u => u.InquiryHeaderId == id, includeProperties: "Product")
            //};
            
            return View(inquiryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            InquiryVM.InquiryDetails = _inquiryDetails.GetAll(d => d.InquiryHeaderId==InquiryVM.InquiryHeader.Id);
            
            foreach(var detail in InquiryVM.InquiryDetails)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId,
                    Quantity = 1
                };
                shoppingCartList.Add(shoppingCart);
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WC.SessionInquiryId, InquiryVM.InquiryHeader.Id);

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult Delete()
        {
            InquiryHeader inquiryHeader = _inquiryHeader.FirstOrDefault(h => h.Id==InquiryVM.InquiryHeader.Id);
            IEnumerable<InquiryDetails> inquiryDetails = _inquiryDetails.GetAll(d => d.InquiryHeaderId==InquiryVM.InquiryHeader.Id);

            _inquiryDetails.RemoveRange(inquiryDetails);
            _inquiryHeader.Remove(inquiryHeader);
            _inquiryHeader.Save();
            TempData[WC.Success] = "Action completed successfully";
            return RedirectToAction(nameof(Index));
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inquiryHeader.GetAll() });
        }
        #endregion
    }
}

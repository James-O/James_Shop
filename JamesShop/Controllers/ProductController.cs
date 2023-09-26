using Shop_DataA.Data;
using Shop_Models;
using Shop_Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop_Utility;
using Shop_DataA.Repository.IRepository;
//using JamesShop.Models.ViewModels;

namespace JamesShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _prodRepository;
        private readonly IWebHostEnvironment _env;

        public ProductController(IProductRepository prodRepository, IWebHostEnvironment env)
        {
            _prodRepository=prodRepository;
            _env=env;//have access to our img
        }
        public IActionResult Index()
        {
            //IEnumerable<Product> products = _appDbContext.Products.Include(c=>c.Category).Include(a=>a.Application);//eager loading
            IEnumerable<Product> products = _prodRepository.GetAll(includeProperties:"Category,Application");
            return View(products);
        }

        public IActionResult CreOrdit(int? id) 
        {
            //IEnumerable<SelectListItem> CatDropDown =
            //    _appDbContext.Categories.Select(c => new SelectListItem
            //    {
            //        Text = c.Name,
            //        Value=c.Id.ToString()
            //    });
            //ViewBag.AllCategory = CatDropDown;

            //var product = new Product();

            ProductVM product = new ProductVM
            {
                Product = new Product(),
                CatSelectList = _prodRepository.GetAllDropdownList(WC.CategoryName),
                AppSelectList = _prodRepository.GetAllDropdownList(WC.ApplicationName)
            };
            
            if(id == null)
            {
                //create
                return View(product);
            }
            else
            {
                product.Product = _prodRepository.Find(id.GetValueOrDefault());
                if (product.Product == null)
                {
                    NotFound();
                }
                return View(product);
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreOrdit(ProductVM products)
        {
            //if (ModelState.IsValid)
            //{
                var files = HttpContext.Request.Form.Files;//retrive and save new uploaded img
                string webRootpath = _env.WebRootPath;//path to our wwwroot folder

                if(products.Product.Id == 0)
                  {
                //create
                //string upload = webRootpath + @"~/images/product/"; //WC.ImagePath;//upload the img
                    string upload = webRootpath + @WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();//name of file coming in. since we don't know the name we can generate a random guid
                    string extension = Path.GetExtension(files[0].FileName);//extension of the uploaded file 
                
                    //copy the file into upload (new location)
                    using(var fileStream = new FileStream(Path.Combine(
                        upload, fileName+extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    products.Product.Image = fileName +  extension;

                    _prodRepository.Add(products.Product);
                }
                else
                {
                //update
                //var proFromDb = _appDbContext.Products.AsNoTracking().FirstOrDefault(p => p.Id==products.Product.Id);//retrieve products from db
                var proFromDb = _prodRepository.FirstOrDefault(p => p.Id==products.Product.Id,isTracking:false);//retrieve products from db
                
                if(files.Count > 0)
                {
                    //string upload = webRootpath + @"~/images/product/"; // WC.ImagePath;
                    string upload = webRootpath + @WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    //remove the old img
                    var old = Path.Combine(upload, proFromDb.Image);
                    if (System.IO.File.Exists(old))
                    {
                        System.IO.File.Delete(old);  
                    }
                    //add new
                    using (var fileStream = new FileStream(Path.Combine(
                        upload, fileName+extension), FileMode.Create))//the filestream
                    {
                        files[0].CopyTo(fileStream);
                    }
                    products.Product.Image = fileName + extension;//save here
                }
                else
                {
                    products.Product.Image = proFromDb.Image;//keep img same if not modified
                }
                _prodRepository.Update(products.Product);
                }
                _prodRepository.Save();
            TempData[WC.Success] = "Action completed successfully";
            return RedirectToAction("Index");
            //}
            //products.CatSelectList = _prodRepository.GetAllDropdownList(WC.CategoryName);
            //products.AppSelectList = _prodRepository.GetAllDropdownList(WC.ApplicationName);
            //return View(products);
        }

        public IActionResult Delete(int? id)
        {
            if (id==0||id==null)
            {
                return NotFound();
            }
            //var productFromDb =  _appDbContext.Products.Find(id);
            //productFromDb.Category=_appDbContext.Categories.Find(productFromDb.CategoryId);
            //var proFromDb = _appDbContext.Products.Include(c=>c.Category).Include(a=>a.Application).Where(p=>p.Id==id).FirstOrDefault();
            var proFromDb = _prodRepository.FirstOrDefault(p => p.Id==id, includeProperties: "Category,Application");
            
            if(proFromDb == null)
            {
                return NotFound();
            }
            else
            {
                return View(proFromDb);
            }
        }

        [HttpPost]
        public IActionResult DeletePost(int? id)
        {
            //var obj = _appDbContext.Products.Include(c => c.Category).Include(a=>a.Application).FirstOrDefault(p => p.Id==id);
            var obj = _prodRepository.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            string upload = _env.WebRootPath + @WC.ImagePath;
            //remove the old img
            var old = Path.Combine(upload, obj.Image);
            if (System.IO.File.Exists(old))
            {
                System.IO.File.Delete(old);
            }
            _prodRepository.Remove(obj);
            _prodRepository.Save();
            TempData[WC.Success]="Product deleted successfully";
            return RedirectToAction("Index");
        }
    }
}

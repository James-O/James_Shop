using Shop_DataA.Data;
using Shop_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop_Utility;
using Shop_DataA.Repository.IRepository;
using Shop_DataA.Repository;

namespace JamesShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationController : Controller
    {
        private readonly IApplicationRepository _appRepository;
        public ApplicationController(IApplicationRepository appRepository)
        {
            _appRepository = appRepository;
        }
        public IActionResult Index()
        {
            //var appType = _applicationDbContext.ApplicationTypes.ToList();
            IEnumerable<Application> appType = _appRepository.GetAll();
            return View(appType);
        }

        public IActionResult Create()
        {
            return View();  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Application applicationType)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(applicationType==null)
                return NotFound();

            _appRepository.Add(applicationType);
            _appRepository.Save();
            TempData[WC.Success]="Application created successfully";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if(id==null || id == 0)
            {
                return NotFound();
            }
            var appType = _appRepository.Find(id.GetValueOrDefault());
            if (appType == null)
            {
                return NotFound();
            }
            return View(appType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Application appType)
        {
            if (ModelState.IsValid)
            {
                _appRepository.Update(appType);
                _appRepository.Save();
                TempData[WC.Success]="Record edited successfully";
                return RedirectToAction("Index");
            }
            return View(appType);
        }

        public IActionResult Delete(int? id)
        {
            var appType = _appRepository.Find(id.GetValueOrDefault());
            if (appType == null)
            {
                return NotFound();
            }
            return View(appType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var appType = _appRepository.Find(id.GetValueOrDefault());
            if(appType == null)
                return NotFound();

            _appRepository.Remove(appType);
            _appRepository.Save();
            TempData[WC.Success]="Record deleted successfully";
            return RedirectToAction("Index");
        }
    }
}

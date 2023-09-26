using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shop_Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CatSelectList { get; set; }
        public IEnumerable<SelectListItem> AppSelectList { get; set; }
    }
}

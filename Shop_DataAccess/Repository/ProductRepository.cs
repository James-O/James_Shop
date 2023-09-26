using Microsoft.AspNetCore.Mvc.Rendering;
using Shop_DataA.Data;
using Shop_DataA.Repository.IRepository;
using Shop_Models;
using Shop_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop_DataA.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            _dbContext=dbContext;
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string objlist)
        {
            if(objlist == WC.CategoryName)
            {
                return _dbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }
            if(objlist == WC.ApplicationName)
            {
               return _dbContext.ApplicationTypes.Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.Id.ToString()
                });
            }
            return null;
        }

        public void Update(Product product)
        {
            _dbContext.Products.Update(product);//updates all ppties of prod
            
            
            
            //var prodFromDb = base.FirstOrDefault(c=>c.Id ==product.Id);
            //if(prodFromDb != null)
            //{
            //    prodFromDb.Name = product.Name;
            //    prodFromDb.ShortDesc = product.ShortDesc;
            //    prodFromDb.Description = product.Description;
            //    prodFromDb.Price = product.Price;
                
            //}
        }
    }
}

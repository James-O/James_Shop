using Shop_DataA.Data;
using Shop_DataA.Repository.IRepository;
using Shop_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop_DataA.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            _dbContext=dbContext;
        }
        public void Update(Category category)
        {
            //var catFromDb = _dbContext.Categories.FirstOrDefault(c=>c.Id ==category.Id);
            var catFromDb = base.FirstOrDefault(c=>c.Id ==category.Id);
            if(catFromDb != null)
            {
                catFromDb.Name = category.Name;
                catFromDb.DisplayOrder = category.DisplayOrder;
            }
        }
    }
}

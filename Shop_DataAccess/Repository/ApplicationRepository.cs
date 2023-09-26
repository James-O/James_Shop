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
    public class ApplicationRepository : Repository<Application>, IApplicationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ApplicationRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            _dbContext=dbContext;
        }
        public void Update(Application application)
        {
            //var catFromDb = _dbContext.Categories.FirstOrDefault(c=>c.Id ==category.Id);
            var catFromDb = base.FirstOrDefault(c=>c.Id ==application.Id);
            if(catFromDb != null)
            {
                catFromDb.Name = application.Name;
                catFromDb.Description = application.Description;
            }
        }
    }
}

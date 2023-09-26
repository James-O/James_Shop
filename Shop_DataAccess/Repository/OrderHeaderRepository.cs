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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderHeaderRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            _dbContext=dbContext;
        }
        public void Update(OrderHeader orderHeader)
        {
            _dbContext.OrderHeader.Update(orderHeader);
        }
    }
}

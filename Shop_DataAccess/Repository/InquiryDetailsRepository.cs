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
    public class InquiryDetailsRepository : Repository<InquiryDetails>, IInquiryDetailsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public InquiryDetailsRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            _dbContext=dbContext;
        }
        public void Update(InquiryDetails inquiryDetails)
        {
            _dbContext.InquiryDetails.Update(inquiryDetails);
        }
    }
}

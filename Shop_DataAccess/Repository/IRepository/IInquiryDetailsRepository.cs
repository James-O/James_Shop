using Shop_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop_DataA.Repository.IRepository
{
    public interface IInquiryDetailsRepository: IRepository<InquiryDetails>
    {
        void Update(InquiryDetails inquiryDetails);
    }
}

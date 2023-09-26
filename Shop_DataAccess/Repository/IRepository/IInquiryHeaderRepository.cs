using Shop_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop_DataA.Repository.IRepository
{
    public interface IInquiryHeaderRepository: IRepository<InquiryHeader>
    {
        void Update(InquiryHeader inquiryHeader);
    }
}

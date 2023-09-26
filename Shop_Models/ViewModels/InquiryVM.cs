using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop_Models.ViewModels
{
    public class InquiryVM
    {
        public InquiryHeader InquiryHeader { get; set; }
        public IEnumerable<InquiryDetails> InquiryDetails { get; set; }
        //public IList<InquiryDetails> InquiryDetails { get; set; }
    }
}

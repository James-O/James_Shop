using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop_Models
{
    public class InquiryDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int InquiryHeaderId { get; set; }
        [ForeignKey(nameof(InquiryHeaderId))]
        public InquiryHeader InquiryHeader { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }


    }
}
